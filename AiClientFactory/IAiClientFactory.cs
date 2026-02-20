using GenAiForDotNet.AiClient;
using GenAiForDotNet.Common;

namespace GenAiForDotNet.AiClientFactory
{
    public interface IAiClientFactory
    {
        ICompletionStrategy CreateCompletion(ChatClientType clientType);
        ICompletionStrategy CreateStreamingCompletion(ChatClientType clientType);
    }
}
