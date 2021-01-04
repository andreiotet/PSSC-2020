using CSharp.Choices;
using StackUnderflow.EF.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace StackUnderflow.Domain.Core.Contexts.Question.SendNotifyReply
{
    [AsChoice]
    public static partial class NotifyReplyResult
    {
        public interface INotifyReplyResult { }

        public class ReplyConfirmed : INotifyReplyResult
        {
            public User QuestionUser { get; }

            public string InvitationAcknowlwedgement { get; set; }

            public ReplyConfirmed(User adminUser, string invitationAcknowledgement)
            {
                QuestionUser = adminUser;
                InvitationAcknowlwedgement = invitationAcknowledgement;
            }
        }

        public class ReplyNotConfirmed : INotifyReplyResult
        {
            ///TODO
        }

        public class InvalidRequest : INotifyReplyResult
        {
            public string Message { get; }

            public InvalidRequest(string message)
            {
                Message = message;
            }

        }
    }
}
