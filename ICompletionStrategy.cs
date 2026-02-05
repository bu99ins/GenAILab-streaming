using Microsoft.Extensions.AI;

namespace GenAiForDotNet;

public interface ICompletionStrategy
{
    Task<(string, ChatFinishReason?)> CompleteAsync(List<ChatMessage> chatMessages);
}