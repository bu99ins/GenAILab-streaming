using GenAiForDotNet.AiClient;
using Microsoft.Extensions.AI;
using OllamaSharp;

namespace GenAiForDotNet.AiClientFactory
{
    internal class OllamaClientFactory(string model = "mistral") : AiClientFactory
    {
        public override IModeration CreateModeration()
        {
            return new EmptyModeration();
        }

        protected override IChatClient CreateClient()
        {
            return new OllamaApiClient(new Uri("http://localhost:11434"), model);
        }
    }
}
