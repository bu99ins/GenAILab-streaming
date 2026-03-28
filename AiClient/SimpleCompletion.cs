using Microsoft.Extensions.AI;

namespace GenAiForDotNet.AiClient;

public class SimpleCompletion(IChatClient chatClient, ChatOptions? options = null) : ICompletionStrategy
{
    public async Task<(string, ChatFinishReason?)> CompleteAsync(List<ChatMessage> chatMessages, CancellationToken token)
    {
        var result = await chatClient.GetResponseAsync(chatMessages, options, token);

        Console.Write(result.Text);

        return (result.Text, result.FinishReason);
    }
}