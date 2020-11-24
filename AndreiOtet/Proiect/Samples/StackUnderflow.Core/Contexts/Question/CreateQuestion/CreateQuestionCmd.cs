using EarlyPay.Primitives.ValidationAttributes;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace StackUnderflow.Domain.Core.Contexts.Question.CreateQuestion
{
    public struct CreateQuestionCmd
    {
            
        public CreateQuestionCmd(Guid userId, string title, string body, string tags)            
        {
            UserId = userId;
            Title = title;              
            Body = body;              
            Tags = tags;            
        }

        [GuidNotEmpty]
        public Guid UserId { get; set; }

        [Required]
        public string Title { get; set; }

            
        [Required]
        public string Body { get; set; }

            
        [Required]            
        public string Tags { get; set; }
    }
    
}
