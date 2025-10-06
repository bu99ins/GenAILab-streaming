using System.Text;
using OpenAI.Chat;
using OpenAI.Moderations;

Console.OutputEncoding = Encoding.UTF8;
Console.InputEncoding = Encoding.UTF8;

const string model = "gpt-4.1";
var key = Environment.GetEnvironmentVariable("OPENAI_API_KEY");

if (string.IsNullOrEmpty(key))
{
    Console.WriteLine("Please set the OPENAI_API_KEY environment variable.");
    return;
}

var client = new ChatClient(model, key);
var moderator = new ModerationClient("omni-moderation-latest", key);

var messages = new List<ChatMessage>
{
    new SystemChatMessage("Вы - раздражающе дружелюбный чатбот с искусственным интеллектом" +
                          "на конференции Azure Saturday AI в Сиэтле. " +
                          "Будьте кратки.")
};

var completionOptions = new ChatCompletionOptions();
var answerBuilder = new StringBuilder();
ChatFinishReason? lastReason = null;

try
{
    while (true)
    {
        // Get response from the model
        Console.ForegroundColor = ConsoleColor.White;
        var streamingResult = client.CompleteChatStreamingAsync(messages, completionOptions);

        await foreach (var update in streamingResult)
        {
            lastReason = update.FinishReason;
            if (update.ContentUpdate.Count == 0)
                continue;

            var textUpdate = update.ContentUpdate[0].Text;

            Console.Write(textUpdate);
            answerBuilder.Append(textUpdate);
        }

        if (lastReason != null && lastReason != ChatFinishReason.Stop)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"Error: The model did not finish properly. Reason: {lastReason}");
            break;
        }

        var answer = answerBuilder.ToString();
        messages.Add(new AssistantChatMessage(answer));

        Console.WriteLine();

        // User input
        Console.ForegroundColor = ConsoleColor.Yellow;
        var userInput = Console.ReadLine();

        var moderationResult = (await moderator.ClassifyTextAsync(userInput)).Value;
        if (moderationResult.Flagged)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("Your input was flagged by the moderation system. Please try again.");

            var hits = moderationResult.GetType().GetProperties()
                .Where(p => p.PropertyType == typeof(ModerationCategory))
                .Select(cat => (cat.Name, (ModerationCategory)cat.GetValue(moderationResult)!))
                .Where(c => c.Item2.Flagged)
                .Select(c => $"{c.Name}: {c.Item2.Score}");

            var moderationMessage = string.Join(", ", hits);

            Console.ForegroundColor = ConsoleColor.DarkYellow;
            Console.WriteLine(moderationMessage);

            messages.Add(new SystemChatMessage(
                "The content moderation system has flagged the user's answer. " +
                "Here is the moderation system's output. React accordingly to the user in the user's language, " +
                "explicitly mentioning that the user's specific violation will not be tolerated. \n\n"
                + moderationMessage));

            continue;
        }

        messages.Add(new UserChatMessage(userInput));
    }
}
catch(Exception ex)
{
    Console.ForegroundColor = ConsoleColor.Red;
    Console.WriteLine($"Error: {ex.Message}");
}
finally
{
    Console.ResetColor();
}
