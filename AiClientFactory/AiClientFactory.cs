using GenAiForDotNet.AiClient;
using Microsoft.Extensions.AI;

namespace GenAiForDotNet.AiClientFactory;

public abstract class AiClientFactory
{
    public ICompletionStrategy CreateCompletion()
    {
        return new SimpleCompletion(CreateClient());
    }

    public ICompletionStrategy CreateStreamingCompletion()
    {
        return new StreamingCompletion(CreateClient());
    }

    protected abstract IChatClient CreateClient();
}