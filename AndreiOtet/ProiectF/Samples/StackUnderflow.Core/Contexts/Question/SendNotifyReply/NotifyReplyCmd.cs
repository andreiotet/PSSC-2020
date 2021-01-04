using Access.Primitives.IO;
using EarlyPay.Primitives.ValidationAttributes;
using LanguageExt;
using StackUnderflow.EF.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace StackUnderflow.Domain.Core.Contexts.Question.SendNotifyReply
{
    public struct NotifyReplyCmd
    {
        [OptionValidator(typeof(RequiredAttribute))]
        public Option<User> QuestionUser { get; }
        public NotifyReplyCmd(Option<User> questionUser)
        {
            QuestionUser = questionUser;
        }
    }

    public enum NotifyReplyCmdInput
    {
        Valid,
        Invalid
    }
    public class NotifyReplyCmdInputGen : InputGenerator<NotifyReplyCmd, NotifyReplyCmdInput>
    {
        public NotifyReplyCmdInputGen()
        {
            mappings.Add(NotifyReplyCmdInput.Valid, () =>
             new NotifyReplyCmd(
                 Option<User>.Some(new User()
                 {
                     DisplayName = Guid.NewGuid().ToString(),
                     Email = $"{Guid.NewGuid()}@mailinator.com"
                 }))
            );

            mappings.Add(NotifyReplyCmdInput.Invalid, () =>
                new NotifyReplyCmd(
                    Option<User>.None
                    )
            );
        }
    }
}
