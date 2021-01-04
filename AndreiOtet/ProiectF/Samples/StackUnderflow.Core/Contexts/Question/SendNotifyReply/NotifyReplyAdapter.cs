using Access.Primitives.Extensions.ObjectExtensions;
using Access.Primitives.IO;
using Access.Primitives.IO.Mocking;
using StackUnderflow.Domain.Core.Contexts.Question.CreateReply;
using StackUnderflow.Domain.Core.Contexts.Question.SendConfirmation;
using StackUnderflow.EF.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using static StackUnderflow.Domain.Core.Contexts.Question.CreateReply.CreateReplyResult;
using static StackUnderflow.Domain.Core.Contexts.Question.SendNotifyReply.NotifyReplyResult;

namespace StackUnderflow.Domain.Core.Contexts.Question.SendNotifyReply
{
    public partial class NotifyReplyAdapter: Adapter<NotifyReplyCmd, INotifyReplyResult, QuestionWriteContext, QuestionDependencies>
    {
        private readonly IExecutionContext _ex;

        public NotifyReplyAdapter(IExecutionContext ex)
        {
            _ex = ex;
        }           

        public override async Task<INotifyReplyResult> Work(NotifyReplyCmd command, QuestionWriteContext state, QuestionDependencies dependencies)
        {
            var wf = from isValid in command.TryValidate()
                     from user in command.QuestionUser.ToTryAsync()
                     let letter = GenerateConfirmationLetter(user)
                     from confirmationAck in dependencies.SendConfirmationEmail(letter)
                     select (user, confirmationAck);

            return await wf.Match(
                Succ: r => new ReplyConfirmed(r.user, r.confirmationAck.Receipt),
                Fail: ex => (INotifyReplyResult)new InvalidRequest(ex.ToString()));
        }

        private ConfirmationLetter GenerateConfirmationLetter(User user)
        {
            var link = $"https://stackunderflow/Question986";
            var letter = $@"Dear {user.DisplayName} your reply is posted. For details please click on {link}";
            return new ConfirmationLetter(user.Email, letter, new Uri(link));
        }

        public override Task PostConditions(NotifyReplyCmd cmd, INotifyReplyResult result, QuestionWriteContext state)
        {
            return Task.CompletedTask;
        }
    }
}
