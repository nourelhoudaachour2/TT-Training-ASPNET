using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using OpenAI.Responses;

#pragma warning disable OPENAI001

namespace Training_tunisie_telecome.Services
{
    public class OpenAIService
    {
        private readonly IConfiguration _configuration;

        public OpenAIService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task<string> AskAsync(string question)
        {
            var apiKey = _configuration["OpenAI:ApiKey"];

            if (string.IsNullOrWhiteSpace(apiKey))
            {
                throw new InvalidOperationException(
                    "La clé API OpenAI n'est pas configurée."
                );
            }

            var client = new ResponsesClient(
                apiKey: apiKey
            );

            ResponseResult response =
                await client.CreateResponseAsync(
                    "gpt-5",
                    question
                );

            return response.GetOutputText();
        }
    }
}

#pragma warning restore OPENAI001