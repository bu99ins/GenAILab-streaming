using Microsoft.Extensions.AI;

namespace GenAiForDotNet.AIClient;

public interface ICompletionStrategy
{
    Task<(string, ChatFinishReason?)> CompleteAsync(List<ChatMessage> chatMessages);
}