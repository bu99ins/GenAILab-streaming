using Microsoft.Extensions.AI;

namespace GenAiForDotNet;

public class BatchChatClient(IChatClient chatClient) : ICompletionStrategy
{
    public async Task<(string, ChatFinishReason?)> CompleteAsync(List<ChatMessage> chatMessages)
    {
        var result = await chatClient.GetResponseAsync(chatMessages);

        Console.Write(result.Text);

        return (result.Text, result.FinishReason);
    }
}