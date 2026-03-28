using Microsoft.Extensions.AI;
using System.Text;

namespace GenAiForDotNet.AiClient;

public class StreamingCompletion(IChatClient chatClient, ChatOptions? options = null) : ICompletionStrategy
{
    public async Task<(string, ChatFinishReason?)> CompleteAsync(List<ChatMessage> chatMessages, CancellationToken token)
    {
        var answerBuilder = new StringBuilder();
        ChatFinishReason? lastReason = null;

        var result = chatClient.GetStreamingResponseAsync(chatMessages, options, token);

        await foreach (var update in result)
        {
            lastReason = update.FinishReason;
            if (update.Contents.Count == 0) continue;

            Console.Write(update.Text);
            answerBuilder.Append(update.Text);
        }

        return (answerBuilder.ToString(), lastReason);
    }
}