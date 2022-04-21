using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RestroQnABot.Models
{
    public class Language
    {
        public Dictionary<string, string> LanguageInformation { get; set; }

        public Language()
        {
            LanguageInformation = new Dictionary<string, string>();
        }
    }
}
