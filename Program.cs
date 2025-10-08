using System.Text;
using GenAiForDotNet;
using OpenAI.Chat;

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
var inputModerator = new InputModerator(key);

var messages = new List<ChatMessage>
{
    new SystemChatMessage("Вы - раздражающе дружелюбный чатбот с искусственным интеллектом" +
                          "на конференции Azure Saturday AI в Сиэтле. " +
                          "Будьте кратки.")
};

var completionOptions = new ChatCompletionOptions();

try
{
    while (true)
    {
        // Get response from the model
        Console.ForegroundColor = ConsoleColor.White;
        var (answer, lastReason) = await CompleteStreamingAsync(messages, completionOptions);

        if (lastReason != null && lastReason != ChatFinishReason.Stop)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"Error: The model did not finish properly. Reason: {lastReason}");
            break;
        }

        messages.Add(new AssistantChatMessage(answer));

        Console.WriteLine();

        // User input
        Console.ForegroundColor = ConsoleColor.Yellow;
        var userInput = Console.ReadLine();

        messages.Add(await inputModerator.GetModeratedInputAsync(userInput));
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

return;

async Task<(string, ChatFinishReason?)> CompleteStreamingAsync(
    List<ChatMessage> chatMessages,ChatCompletionOptions chatCompletionOptions)
{
    var answerBuilder = new StringBuilder();
    ChatFinishReason? lastReason = null;

    var streamingResult = client.CompleteChatStreamingAsync(chatMessages, chatCompletionOptions);

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
