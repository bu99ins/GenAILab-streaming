using Microsoft.Extensions.AI;
using OpenAI;

namespace GenAiForDotNet.AiClientFactory
{
    internal class OpenAiClientFactory(string model = "gpt-5.2") : AiClientFactory
    {
        protected override IChatClient CreateClient()
        {
            var key = Environment.GetEnvironmentVariable("OPENAI_API_KEY");
            return string.IsNullOrEmpty(key)
                ? throw new InvalidOperationException("Please set the OPENAI_API_KEY environment variable.")
                : new OpenAIClient(key).GetChatClient(model).AsIChatClient();
        }
    }
}
