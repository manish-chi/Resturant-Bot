using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RestroQnABot.Serializable
{

    public class CustomQnARequest
    {
        public string question { get; set; }
        public int top { get; set; }
        public string userId { get; set; }
        public float confidenceScoreThreshold { get; set; }
        public Context context { get; set; }
        public string rankerType { get; set; }
        public Filters filters { get; set; }
        public Answerspanrequest answerSpanRequest { get; set; }
        public bool includeUnstructuredSources { get; set; }
    }

    public class Context
    {
        public int previousQnaId { get; set; }
        public string previousUserQuery { get; set; }
    }

    public class Filters
    {
        public Metadatafilter metadataFilter { get; set; }
        public string[] sourceFilter { get; set; }
        public string logicalOperation { get; set; }
    }

    public class Metadatafilter
    {
        public MetadataX[] metadata { get; set; }
        public string logicalOperation { get; set; }
    }

    public class MetadataX
    {
        public string key { get; set; }
        public string value { get; set; }
    }

    public class Answerspanrequest
    {
        public bool enable { get; set; }
        public float confidenceScoreThreshold { get; set; }
        public int topAnswersWithSpan { get; set; }
    }


}
