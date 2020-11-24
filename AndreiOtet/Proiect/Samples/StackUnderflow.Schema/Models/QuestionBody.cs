using System;
using System.Collections.Generic;
using System.Text;

namespace StackUnderflow.DatabaseModel.Models
{
    public class QuestionBody
    {
        public int QuestionId { get; set; }
        public string Title { get; set; }
        public string Body { get; set; }
        public string Tags { get; set; }
    }
}
