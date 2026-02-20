using GenAiForDotNet.AiClient;
using GenAiForDotNet.Common;
using Microsoft.Extensions.AI;
using OpenAI;

namespace GenAiForDotNet.AiClientFactory
{
    internal class AiClientFactory : IAiClientFactory
    {
        public ICompletionStrategy CreateCompletion(ChatClientType clientType)
        {
            return new SimpleCompletion(CreateClient(clientType));
        }

        public ICompletionStrategy CreateStreamingCompletion(ChatClientType clientType)
        {
            return new StreamingCompletion(CreateClient(clientType));
        }

        private IChatClient CreateClient(ChatClientType clientType)
        {
            var chatClient = clientType switch
            {
                ChatClientType.OpenAi => CreateOpenAiClient(),
                _ => throw new NotSupportedException($"The chat client type {clientType} is not supported.")
            };
            return chatClient;
        }

        private IChatClient CreateOpenAiClient()
        {
            var key = Environment.GetEnvironmentVariable("OPENAI_API_KEY");
            return string.IsNullOrEmpty(key)
                ? throw new InvalidOperationException("Please set the OPENAI_API_KEY environment variable.")
                : new OpenAIClient(key).GetChatClient("gpt-5.2").AsIChatClient();
        }
    }
}
