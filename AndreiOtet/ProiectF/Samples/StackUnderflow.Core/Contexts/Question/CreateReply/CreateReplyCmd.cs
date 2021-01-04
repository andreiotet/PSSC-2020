using EarlyPay.Primitives.ValidationAttributes;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace StackUnderflow.Domain.Core.Contexts.Question.CreateReply
{
    public struct CreateReplyCmd
    {
        public CreateReplyCmd(int questionId,Guid userId, string body)
        {
            QuestionId = questionId;
            UserId = userId;
            Body = body;
        }

        [Required]
        public int QuestionId { get; set; }

        [GuidNotEmpty]
        public Guid UserId { get; set; }

        [Required]
        public string Body { get; set; }
    }
}
