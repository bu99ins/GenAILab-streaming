using OpenAI.Chat;

namespace GenAiForDotNet;

public class BatchChatClient(string model, string apiKey, ChatCompletionOptions chatCompletionOptions) : IChatClient
{
    private readonly ChatClient _client = new(model, apiKey);

    public async Task<(string, ChatFinishReason?)> CompleteAsync(List<ChatMessage> chatMessages)
    {
        var result = (await _client.CompleteChatAsync(chatMessages, chatCompletionOptions)).Value;

        Console.Write(result.Content[0].Text);

        return (result.Content[0].Text, result.FinishReason);
    }
}