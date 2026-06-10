using GenAiForDotNet.AiClient;
using Microsoft.Extensions.AI;

namespace GenAiForDotNet.AiClientFactory;

public abstract class AiClientFactory
{
    public ICompletionStrategy CreateCompletion(ChatOptions? options = null)
    {
        return new SimpleCompletion(GetClient(), options);
    }

    public ICompletionStrategy CreateStreamingCompletion(ChatOptions? options = null)
    {
        return new StreamingCompletion(GetClient(), options);
    }

    public abstract IModeration CreateModeration();

    protected abstract IChatClient GetClient();
}