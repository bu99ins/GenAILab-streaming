using Microsoft.Extensions.AI;

namespace GenAiForDotNet.AiClient;

public interface ICompletionStrategy
{
    Task<(string, ChatFinishReason?)> CompleteAsync(List<ChatMessage> chatMessages);
}