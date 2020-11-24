using CSharp.Choices;
using StackUnderflow.EF.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace StackUnderflow.Domain.Core.Contexts.Question.SendConfirmation
{
    [AsChoice]
    public static partial class ConfirmationQuestionResult
    {
        public interface IConfirmationQuestionResult { }

        public class QuestionConfirmed : IConfirmationQuestionResult
        {
            public User QuestionUser { get; }

            public string InvitationAcknowlwedgement { get; set; }

            public QuestionConfirmed(User adminUser, string invitationAcknowledgement)
            {
                QuestionUser = adminUser;
                InvitationAcknowlwedgement = invitationAcknowledgement;
            }
        }
        public class QuestionNotConfirmed : IConfirmationQuestionResult
        {
            ///TODO
        }

        public class InvalidRequest : IConfirmationQuestionResult
        {
            public string Message { get; }

            public InvalidRequest(string message)
            {
                Message = message;
            }

        }
    }
}
