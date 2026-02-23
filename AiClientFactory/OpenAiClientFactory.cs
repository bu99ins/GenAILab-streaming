using GenAiForDotNet.AiClient;
using Microsoft.Extensions.AI;
using OpenAI;

namespace GenAiForDotNet.AiClientFactory
{
    internal class OpenAiClientFactory(string model = "gpt-5.2") : AiClientFactory
    {
        private readonly string? _apiKey = Environment.GetEnvironmentVariable("OPENAI_API_KEY");

        public override IModeration CreateModeration()
        {
            return string.IsNullOrEmpty(_apiKey) ? new EmptyModeration() : new OpenAiModeration(_apiKey);
        }

        protected override IChatClient CreateClient()
        {
            return string.IsNullOrEmpty(_apiKey)
                ? throw new InvalidOperationException("Please set the OPENAI_API_KEY environment variable.")
                : new OpenAIClient(_apiKey).GetChatClient(model).AsIChatClient();
        }
    }
}
