using Question.Domain.CreateQuestionWorkflow;
using System;
using System.Collections.Generic;
using System.Net;
using LanguageExt;
using static Question.Domain.CreateQuestionWorkflow.CreateQuestionResult;

namespace Test.App
{
    class Program_Question
    {
        static void Main(string[] args)
        {
            var cmd = new CreateQuestionCmd("Title", "Body", "C#");
            var result = CreateNewQuestion(cmd);

            var createQuestionEvent = result.Match(ProcessQuetionPosted,ProcessQuestionNotPosted,ProcessInvalidQuestion);

            Console.ReadLine();
        }

        private static ICreateQuestionResult ProcessInvalidQuestion(QuestionValidationFailed validationErrors)
        {
            Console.WriteLine("Question validation failed: ");
            foreach (var error in validationErrors.ValidationErrors)
            {
                Console.WriteLine(error);
            }
            return validationErrors;
        }

        private static ICreateQuestionResult ProcessQuestionNotPosted(QuestionNotPosted questionNotPosted)
        {
            Console.WriteLine($"Question not posted: {questionNotPosted.Reason}");
            return questionNotPosted;
        }

        private static ICreateQuestionResult ProcessQuetionPosted(QuestionPosted new_question)
        {
            Console.WriteLine($"Question: {new_question.QuestionId}");
            Console.WriteLine($"Body: {new_question.Body}");
            return new_question;
        }

        public static ICreateQuestionResult CreateNewQuestion(CreateQuestionCmd createQuestion)
        {
            if (string.IsNullOrWhiteSpace(createQuestion.Body))
            {
                var errors = new List<string>() { "Describe the question" };
                return new QuestionValidationFailed(errors);
            }

            if(string.IsNullOrEmpty(createQuestion.Title))
            {
                return new QuestionNotPosted("Choose a title");
            }

            var questionId = Guid.NewGuid();
            var result = new QuestionPosted(questionId, createQuestion.Body); 

            return result;
        }
    }
}
