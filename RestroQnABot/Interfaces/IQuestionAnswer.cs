using Azure.AI.Language.QuestionAnswering;
using RestroQnABot.Serializable;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RestroQnABot.Interfaces
{
    public interface IQuestionAnswer
    {
        Task<CustomQnAResponse> GetAnswers(string question, string knowleadgeBaseSource);
    }
}
