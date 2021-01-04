using Access.Primitives.Extensions.Cloning;
using CSharp.Choices;
using StackUnderflow.Domain.Schema.Models;
using StackUnderflow.EF.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace StackUnderflow.Domain.Core.Contexts.Question.CreateQuestion
{
    [AsChoice]
    public static partial class CreateQuestionResult
    {
        public interface ICreateQuestionResult : IDynClonable { }

        public class QuestionCreated : ICreateQuestionResult
        {
            public User Author { get; }
            public Post Question { get; }
            public QuestionCreated(Post post, User author)
            {
                Question = post;
                Author = author;
            }

            public object Clone() => this.ShallowClone();
        }

        public class QuestionNotCreated : ICreateQuestionResult
        {
            public string Reason { get; private set; }

            ///TODO
            public object Clone() => this.ShallowClone();
        }

        public class InvalidQuestion: ICreateQuestionResult
        {
            public string Message { get; }

            public InvalidQuestion(string message)
            {
                Message = message;
            }

            ///TODO
            public object Clone() => this.ShallowClone();

        }
    }
}
