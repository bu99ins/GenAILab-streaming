using System.Text;
using OpenAI.Chat;

namespace GenAiForDotNet;

public class StreamingChatClient(string model, string apiKey, ChatCompletionOptions chatCompletionOptions)
{
    private readonly ChatClient _client = new(model, apiKey);

    public async Task<(string, ChatFinishReason?)> CompleteAsync(List<ChatMessage> chatMessages)
    {
        var answerBuilder = new StringBuilder();
        ChatFinishReason? lastReason = null;

        var streamingResult = _client.CompleteChatStreamingAsync(chatMessages, chatCompletionOptions);

        await foreach (var update in streamingResult)
        {
            lastReason = update.FinishReason;
            if (update.ContentUpdate.Count == 0)
                continue;

            var textUpdate = update.ContentUpdate[0].Text;

            Console.Write(textUpdate);
            answerBuilder.Append(textUpdate);
        }

        return (answerBuilder.ToString(), lastReason);
    }
}