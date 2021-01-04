using Access.Primitives.Extensions.ObjectExtensions;
using Access.Primitives.IO;
using Access.Primitives.IO.Mocking;
using StackUnderflow.EF.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static StackUnderflow.Domain.Core.Contexts.Question.CreateReply.CreateReplyResult;

namespace StackUnderflow.Domain.Core.Contexts.Question.CreateReply
{
    public partial class CreateReplyAdapter: Adapter<CreateReplyCmd, ICreateReplyResult, QuestionWriteContext, QuestionDependencies>
    {
        private readonly IExecutionContext _ex;

        public CreateReplyAdapter(IExecutionContext ex)
        {
            _ex = ex;
        }

        public override async Task<ICreateReplyResult> Work(CreateReplyCmd command, QuestionWriteContext state, QuestionDependencies dependencies)
        {
            var workflow = from valid in command.TryValidate()
                           let t = AddReplyIfMissing(state, CreateReplyFromCommand(command))
                           select t;
            var result = await workflow.Match(
                Succ: r => r,
                Fail: ex => new InvalidReply(ex.ToString()));
            return result;
        }

        public ICreateReplyResult AddReplyIfMissing(QuestionWriteContext state, Post post)
        {
            if (state.Questions.Any(p => p.PostId.Equals(post.PostId)))
                return new ReplyNotCreated();
            if (state.Questions.All(p => p.PostId != post.PostId))
                state.Questions.Add(post);
            return new ReplyCreated(post, post.TenantUser.User);
        }

        private Post CreateReplyFromCommand(CreateReplyCmd cmd)
        {
            var reply = new Post()
            {
                PostId = cmd.QuestionId,
                PostedBy=cmd.UserId,
                PostText = cmd.Body
            };
            return reply;
        }

        public override Task PostConditions(CreateReplyCmd cmd, ICreateReplyResult result, QuestionWriteContext state)
        {
            return Task.CompletedTask;
        }
    }
}
