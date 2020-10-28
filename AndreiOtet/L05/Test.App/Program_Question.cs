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
            String[] tags = new string[3] { "C#", "PHP", "Python" };
            int[] vote = new int[5] { -1, 1, -1, 1, 1 }; 
            var cmd = new CreateQuestionCmd("Title", "Body", tags, vote );
            var result = CreateQuestion(cmd);

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

        private static ICreateQuestionResult ProcessQuetionPosted(QuestionPosted question)
        {
            Console.WriteLine($"Question: {question.QuestionId}");
            Console.WriteLine($"Title: {question.Title}");
            Console.WriteLine($"Body: {question.Body}");
            Console.Write($"Tag: ");
            foreach (var item in question.Tags)
            {
                Console.Write(item.ToString() + " " );
            }
            Console.WriteLine();
            var yes = 0;
            var no = 0;
            foreach (var item in question.Vote)
            {
                if (item == 1)
                    yes++;
                else no++;
            }
            Console.Write($"Vote: yes({yes}) no({no})");
            Console.WriteLine();
            return question;
        }

        public static ICreateQuestionResult CreateQuestion(CreateQuestionCmd createQuestion)
        {
            if (string.IsNullOrWhiteSpace(createQuestion.Body))
            {
                var errors = new List<string>() { "Describe the question" };
                return new QuestionValidationFailed(errors);
            }
            if (createQuestion.Body.Length>1000)
            {
                var errors = new List<string>() { "Question must have less than 1000 characters" };
                return new QuestionValidationFailed(errors);
            }
            if(createQuestion.Tags.Length<1 || createQuestion.Tags.Length>3)
            {
                var errors = new List<string>() { "You must have min. 1 tag or max. 3 tags" };
                return new QuestionValidationFailed(errors);
            }
            if(string.IsNullOrEmpty(createQuestion.Title))
            {
                return new QuestionNotPosted("Choose a title");
            }

            var questionId = Guid.NewGuid();
            var result = new QuestionPosted(questionId, createQuestion.Body, createQuestion.Title, createQuestion.Tags, createQuestion.Vote); 

            return result;
        }
    }
}
