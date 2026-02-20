using Microsoft.Extensions.AI;

namespace GenAiForDotNet.AiClient;

public interface IModeration
{
    Task<ChatMessage> GetModeratedInputAsync(string? s);
}