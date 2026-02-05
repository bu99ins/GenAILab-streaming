using Microsoft.Extensions.AI;
using OpenAI;
using ChatFinishReason = Microsoft.Extensions.AI.ChatFinishReason;
using ChatMessage = Microsoft.Extensions.AI.ChatMessage;

namespace GenAiForDotNet;

public class BatchChatClient(string model, string apiKey) : ICompletionStrategy
{
    private readonly IChatClient _client = new OpenAIClient(apiKey)
        .GetChatClient(model)
        .AsIChatClient();

    public async Task<(string, ChatFinishReason?)> CompleteAsync(List<ChatMessage> chatMessages)
    {
        var result = await _client.GetResponseAsync(chatMessages);

        Console.Write(result.Text);

        return (result.Text, result.FinishReason);
    }
}