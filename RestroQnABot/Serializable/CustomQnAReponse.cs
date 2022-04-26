using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RestroQnABot.Serializable
{

    public class CustomQnAResponse
    {
        public Answer[] answers { get; set; }
    }

    public class Answer
    {
        public string[] questions { get; set; }
        public string answer { get; set; }
        public float confidenceScore { get; set; }
        public int id { get; set; }
        public string source { get; set; }
        public MetaDataY metadata { get; set; }
        public Dialog dialog { get; set; }
        public Answerspan answerSpan { get; set; }
    }

    public class MetaDataY
    {
        public string system_metadata_qna_edited_manually { get; set; }
        public string responsetype { get; set; }
    }

    public class Dialog
    {
        public bool isContextOnly { get; set; }
        public Prompt[] prompts { get; set; }
    }

    public class Prompt
    {
        public int displayOrder { get; set; }
        public int qnaId { get; set; }
        public string displayText { get; set; }
    }

    public class Answerspan
    {
        public string text { get; set; }
        public float confidenceScore { get; set; }
        public int offset { get; set; }
        public int length { get; set; }
    }

}
