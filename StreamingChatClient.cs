using Microsoft.Extensions.AI;
using OpenAI;
using System.Text;
using ChatFinishReason = Microsoft.Extensions.AI.ChatFinishReason;
using ChatMessage = Microsoft.Extensions.AI.ChatMessage;

namespace GenAiForDotNet;

public class StreamingChatClient(string model, string apiKey) : ICompletionStrategy
{
    private readonly IChatClient _client = new OpenAIClient(apiKey)
        .GetChatClient(model)
        .AsIChatClient();

    public async Task<(string, ChatFinishReason?)> CompleteAsync(List<ChatMessage> chatMessages)
    {
        var answerBuilder = new StringBuilder();
        ChatFinishReason? lastReason = null;

        var result = _client.GetStreamingResponseAsync(chatMessages);

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