using GenAiForDotNet.AiClient;
using Microsoft.Extensions.AI;
using OpenAI;

namespace GenAiForDotNet.AiClientFactory;

internal class OpenAiClientFactory(string model = "gpt-5.2") : AiClientFactory
{
    private readonly string? _apiKey = Environment.GetEnvironmentVariable("OPENAI_API_KEY");
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
        if (_chatClient != null) return _chatClient;

        if (string.IsNullOrEmpty(_apiKey))
            throw new InvalidOperationException("Please set the OPENAI_API_KEY environment variable.");

        _chatClient = new OpenAIClient(_apiKey).GetChatClient(model).AsIChatClient();

        return _chatClient;
    }
}