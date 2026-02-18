using GenAiForDotNet.AiClient;
using GenAiForDotNet.Common;

namespace GenAiForDotNet.AiClientFactory
{
    public interface IAiClientFactory
    {
        ICompletionStrategy CreateChatClient(ChatClientType clientType);
        ICompletionStrategy CreateStreamingChatClient(ChatClientType clientType);
    }
}
