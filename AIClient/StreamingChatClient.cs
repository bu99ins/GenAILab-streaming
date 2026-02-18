using Microsoft.Extensions.AI;
using System.Text;

namespace GenAiForDotNet.AiClient;

public class StreamingChatClient(IChatClient chatClient) : ICompletionStrategy
{
    public async Task<(string, ChatFinishReason?)> CompleteAsync(List<ChatMessage> chatMessages)
    {
        var answerBuilder = new StringBuilder();
        ChatFinishReason? lastReason = null;

        var result = chatClient.GetStreamingResponseAsync(chatMessages);

        await foreach (var update in result)
        {
            lastReason = update.FinishReason;
            if (update.Contents.Count == 0)
                continue;

            Console.Write(update.Text);
            answerBuilder.Append(update.Text);
        }

        return (answerBuilder.ToString(), lastReason);
    }
}