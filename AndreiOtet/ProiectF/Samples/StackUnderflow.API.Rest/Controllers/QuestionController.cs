using Access.Primitives.EFCore;
using Access.Primitives.IO;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Access.Primitives.Extensions.ObjectExtensions;
using StackUnderflow.Domain.Core.Contexts.Question;
using StackUnderflow.Domain.Core.Contexts.Question.CreateQuestion;
using StackUnderflow.Domain.Core.Contexts.Question.SendConfirmation;
using StackUnderflow.EF.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LanguageExt;
using Orleans;
using Microsoft.AspNetCore.Http;
using GrainInterfaces;
using StackUnderflow.Domain.Core.Contexts.Question.CreateReply;
using StackUnderflow.Domain.Core.Contexts.Question.SendNotifyReply;

namespace StackUnderflow.API.AspNetCore.Controllers
{
    [ApiController]
    [Route("question")]
    public class QuestionController : ControllerBase
    {
        private readonly IInterpreterAsync _interpreter;
        private readonly StackUnderflowContext _dbContext;
        private readonly IClusterClient _client;

        public QuestionController(IInterpreterAsync interpreter, StackUnderflowContext dbContext, IClusterClient client)
        {
            _interpreter = interpreter;
            _dbContext = dbContext;
            _client = client;
        }
        

        [HttpPost("question")]
        public async Task<IActionResult> CreateAndConfirmationQuestion([FromBody] CreateQuestionCmd createQuestionCmd)
        {
            QuestionWriteContext ctx = new QuestionWriteContext(
               new EFList<Post>(_dbContext.Post),
               new EFList<User>(_dbContext.User));

            var dependencies = new QuestionDependencies();
            dependencies.GenerateConfirmationToken = () => Guid.NewGuid().ToString();
            dependencies.SendConfirmationEmail = SendConfirmationEmail;

            var expr = from createQuestionResult in QuestionDomain.CreateQuestion(createQuestionCmd)
                       let user = createQuestionResult.SafeCast<CreateQuestionResult.QuestionCreated>().Select(p => p.Author)
                       let confirmationQuestionCmd = new ConfirmationQuestionCmd(user)
                       from ConfirmationQuestionResult in QuestionDomain.ConfirmQuestion(confirmationQuestionCmd)
                       select new { createQuestionResult, ConfirmationQuestionResult };
            var r = await _interpreter.Interpret(expr, ctx, dependencies);

            // _dbContext.Post.Add(new Post { PostTypeId=1,Title=createQuestionCmd.Title, PostText=createQuestionCmd.Body});
            await _dbContext.SaveChangesAsync();

            return r.createQuestionResult.Match(
                created => (IActionResult)Ok(created.Question.PostId),
                notCreated => StatusCode(StatusCodes.Status500InternalServerError, "Question could not be created."),//todo return 500 (),
            invalidRequest => BadRequest("Invalid request."));

        }
        private TryAsync<ConfirmationAcknowledgement> SendConfirmationEmail(ConfirmationLetter letter)
       => async () =>
       {
           var emialSender = _client.GetGrain<IEmailSender>(0);
           await emialSender.SendEmailAsync(letter.Letter);
           return new ConfirmationAcknowledgement(Guid.NewGuid().ToString());
       };


        [HttpPost("question")]
        public async Task<IActionResult> CreateAndNotifyReply([FromBody] CreateReplyCmd createNotifyCmd)
        {
            QuestionWriteContext ctx = new QuestionWriteContext(
               new EFList<Post>(_dbContext.Post),
               new EFList<User>(_dbContext.User));

            var dependencies = new QuestionDependencies();
            dependencies.GenerateConfirmationToken = () => Guid.NewGuid().ToString();
            dependencies.SendNotifyEmail = SendNotifyEmail;

            var expr = from createReplyResult in QuestionDomain.CreateReply(createNotifyCmd)
                       let user = createReplyResult.SafeCast<CreateReplyResult.ReplyCreated>().Select(p => p.Author)
                       let notifyReplyCmd = new NotifyReplyCmd(user)
                       from NotifyReplyResult in QuestionDomain.NotifyReply(notifyReplyCmd)
                       select new { createReplyResult };
            var r = await _interpreter.Interpret(expr, ctx, dependencies);

            _dbContext.Post.Add(new Post { PostTypeId = 2, PostText = createNotifyCmd.Body, PostedBy = new Guid("f505c32f-3573-4459-8112-af8276d3e919"), PostId = createNotifyCmd.QuestionId });
            await _dbContext.SaveChangesAsync();

            return r.createReplyResult.Match(
                created => (IActionResult)Ok(created.Answer.PostId),
                notCreated => StatusCode(StatusCodes.Status500InternalServerError, "Reply could not be created."),//todo return 500 (),
            invalidRequest => BadRequest("Invalid request."));

        }

        private TryAsync<NotifyAcknowledgement> SendNotifyEmail(NotifyLetter letter)
      => async () =>
      {
          var emialSender = _client.GetGrain<IEmailSender>(0);
          await emialSender.SendEmailAsync(letter.Letter);
          return new NotifyAcknowledgement(Guid.NewGuid().ToString());
      };

        [HttpGet("all")]
        public async Task<IActionResult> GetAllQuestions()
        {
            //var questions= GetQuestionsFromDb();
            var questionsGrain = this._client.GetGrain<IQuestionGain>("Id");
            var questions = await questionsGrain.GetQuestionsAsync();
            List<Object> all = (from x in questions select (Object)x).ToList();
            all.AddRange(questions);

            return Ok(all);
        }

    }
    }
