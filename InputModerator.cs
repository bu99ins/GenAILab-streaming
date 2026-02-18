using Microsoft.Extensions.AI;
using OpenAI;
using OpenAI.Moderations;

namespace GenAiForDotNet;

public class InputModerator
{
    public const string ModeratorModel = "omni-moderation-latest";

    private ModerationClient? _moderator;

    public void Init()
    {
        var key = Environment.GetEnvironmentVariable("OPENAI_API_KEY");
        if (string.IsNullOrEmpty(key))
        {
            Console.WriteLine("No moderation is possible without API key provided.");
            return;
        }

        _moderator = new OpenAIClient(key).GetModerationClient(ModeratorModel);
    }

    public async Task<ChatMessage> GetModeratedInputAsync(string? s)
    {
        if (_moderator is null) return new ChatMessage(ChatRole.User, s);

        var moderationMessage = await ModerateInputAsync(s);
        if (moderationMessage is null) return new ChatMessage(ChatRole.User, s);

        Console.ForegroundColor = ConsoleColor.Red;
        Console.WriteLine("Your input was flagged by the moderation system. Please try again.");
        Console.ForegroundColor = ConsoleColor.DarkYellow;
        Console.WriteLine(moderationMessage);

        return new ChatMessage(ChatRole.System,
            "The content moderation system has flagged the user's answer. " +
            "Here is the moderation system's output. React accordingly to the user in the user's language, " +
            "explicitly mentioning that the user's specific violation will not be tolerated. \n\n"
            + moderationMessage);
    }

    private async Task<string?> ModerateInputAsync(string? userInput)
    {
        var moderationResult = (await _moderator!.ClassifyTextAsync(userInput)).Value;
        return !moderationResult.Flagged ? null : GetModerationMessage(moderationResult);
    }

    private static string GetModerationMessage(ModerationResult moderationResult1)
    {
        var hits = moderationResult1.GetType().GetProperties()
            .Where(p => p.PropertyType == typeof(ModerationCategory))
            .Select(cat => (cat.Name, (ModerationCategory)cat.GetValue(moderationResult1)!))
            .Where(c => c.Item2.Flagged)
            .Select(c => $"{c.Name}: {c.Item2.Score}");

        return string.Join(", ", hits);
    }
}