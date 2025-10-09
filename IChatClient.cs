using OpenAI.Chat;

namespace GenAiForDotNet;

public interface IChatClient
{
    Task<(string, ChatFinishReason?)> CompleteAsync(List<ChatMessage> chatMessages);
}