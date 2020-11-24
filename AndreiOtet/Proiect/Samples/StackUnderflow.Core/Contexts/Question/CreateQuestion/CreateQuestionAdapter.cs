using Access.Primitives.Extensions.ObjectExtensions;
using Access.Primitives.IO;
using Access.Primitives.IO.Mocking;
using StackUnderflow.DatabaseModel.Models;
using StackUnderflow.Domain.Core.Contexts.Question.CreateQuestion;
using StackUnderflow.Domain.Schema.Models;
using StackUnderflow.EF.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static StackUnderflow.Domain.Core.Contexts.Question.CreateQuestion.CreateQuestionResult;

namespace StackUnderflow.Domain.Core.Contexts.Question
{
    public partial class CreateQuestionAdapter: Adapter<CreateQuestionCmd, ICreateQuestionResult, QuestionWriteContext,QuestionDependencies>
    {
        private readonly IExecutionContext _ex;

        public CreateQuestionAdapter(IExecutionContext ex)
        {
            _ex = ex;
        }

        public override async Task<ICreateQuestionResult> Work(CreateQuestionCmd command, QuestionWriteContext state, QuestionDependencies dependencies)
        {
            var workflow = from valid in command.TryValidate()
                           let t = AddQuestionIfMissing(state, CreateQuestionFromCommand(command))
                           select t;
            var result = await workflow.Match(
                Succ: r => r,
                Fail: ex => new InvalidQuestion(ex.ToString()));
            return result;
        }

        public ICreateQuestionResult AddQuestionIfMissing(QuestionWriteContext state, QuestionBody question)
        {
            if (state.Questions.Any(p => p.Title.Equals(question.Title)))
                return new QuestionNotCreated();
            if (state.Questions.All(p => p.QuestionId != question.QuestionId))
                state.Questions.Add(question);
            return new QuestionCreated( question);
        }

        private QuestionBody CreateQuestionFromCommand(CreateQuestionCmd cmd)
        {
            var question = new QuestionBody()
            {
                Title = cmd.Title,
                Body = cmd.Body,
                Tags = cmd.Tags
            };
            return question;
        }

        public override Task PostConditions(CreateQuestionCmd op, CreateQuestionResult.ICreateQuestionResult result, QuestionWriteContext state)
        {
            return Task.CompletedTask;
        }

    }
}
