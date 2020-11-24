using Access.Primitives.Extensions.Cloning;
using CSharp.Choices;
using StackUnderflow.DatabaseModel.Models;
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
            public QuestionBody Question { get; }
            public QuestionCreated(QuestionBody question)
            {
                Question = question;
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
