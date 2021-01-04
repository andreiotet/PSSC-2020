using LanguageExt;
using StackUnderflow.Domain.Core.Contexts.Question.SendConfirmation;
using StackUnderflow.Domain.Core.Contexts.Question.SendNotifyReply;
using System;
using System.Collections.Generic;
using System.Text;

namespace StackUnderflow.Domain.Core.Contexts.Question
{
    public class QuestionDependencies
    {
        public Func<string> GenerateConfirmationToken { get; set; }
        public Func<ConfirmationLetter, TryAsync<ConfirmationAcknowledgement>> SendConfirmationEmail { get; set; }

        public Func<NotifyLetter, TryAsync<NotifyAcknowledgement>> SendNotifyEmail { get; set; }
    }
}
