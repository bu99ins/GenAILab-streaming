using GeminiDotnet;
using GeminiDotnet.Extensions.AI;
using GenAiForDotNet.AiClient;
using Microsoft.Extensions.AI;

namespace GenAiForDotNet.AiClientFactory;

internal class GeminiClientFactory(string model = "gemini-2.5-flash") : AiClientFactory
{
    private readonly string? _apiKey = Environment.GetEnvironmentVariable("GEMINI_API_KEY");
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
            throw new InvalidOperationException("Please set the GEMINI_API_KEY environment variable.");

        _chatClient = new GeminiChatClient(new GeminiClientOptions { ApiKey = _apiKey, ModelId = model });

        return _chatClient;
    }
}