using GeminiDotnet;
using GenAiForDotNet.AiClient;
using Microsoft.Extensions.AI;
using GeminiDotnet.Extensions.AI;

namespace GenAiForDotNet.AiClientFactory
{
    internal class GeminiClientFactory(string model = "gemini-2.5-flash") : AiClientFactory
    {
        private readonly string? _apiKey = Environment.GetEnvironmentVariable("GEMINI_API_KEY");

        public override IModeration CreateModeration()
        {
            return new EmptyModeration();
        }

        protected override IChatClient CreateClient()
        {
            return string.IsNullOrEmpty(_apiKey)
                ? throw new InvalidOperationException("Please set the GEMINI_API_KEY environment variable.")
                : new GeminiChatClient(new GeminiClientOptions { ApiKey = _apiKey, ModelId = model });
        }
    }
}
