using GenAiForDotNet.AiClient;
using Microsoft.Extensions.AI;
using OllamaSharp;

namespace GenAiForDotNet.AiClientFactory;

internal class OllamaClientFactory(string model = "mistral") : AiClientFactory
{
    private IChatClient? _chatClient;

    public override IModeration CreateModeration()
    {
        return new GenericModeration(GetInnerClient());
    }

    protected override IChatClient GetClient()
    {
        return GetInnerClient();
    }
    private IChatClient GetInnerClient()
    {
        _chatClient ??= new OllamaApiClient(new Uri("http://localhost:11434"), model);
        return _chatClient;
    }
}