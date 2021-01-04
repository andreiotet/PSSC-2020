
using System;
using System.Collections.Generic;
using System.Text;

namespace StackUnderflow.Domain.Core.Contexts.Question.SendNotifyReply
{
    public class NotifyLetter
    {

        public string Email { get; private set; }
        public string Letter { get; private set; }
        public Uri NotifyLink { get; private set; }

        public NotifyLetter(string email, string letter, Uri link)
        {
            Email = email;
            Letter = letter;
            NotifyLink = link;
        }
    }
}
