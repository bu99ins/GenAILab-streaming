using System.Text;
using GenAiForDotNet;
using OpenAI.Chat;

var key = Environment.GetEnvironmentVariable("OPENAI_API_KEY");
if (string.IsNullOrEmpty(key))
{
    Console.WriteLine("Please set the OPENAI_API_KEY environment variable.");
    return;
}

//var chatClient = new StreamingChatClient("gpt-4.1", key, new ChatCompletionOptions());
var chatClient = new BatchChatClient("gpt-5", key, new ChatCompletionOptions());
var inputModerator = new InputModerator(key);

var messages = new List<ChatMessage>
{
    new SystemChatMessage("Вы - раздражающе дружелюбный чатбот с искусственным интеллектом" +
                          "на конференции Azure Saturday AI в Сиэтле. " +
                          "Будьте кратки.")
};

Console.OutputEncoding = Encoding.UTF8;
Console.InputEncoding = Encoding.UTF8;

try
{
    while (true)
    {
        // Get response from the model
        Console.ForegroundColor = ConsoleColor.White;
        var (answer, lastReason) = await chatClient.CompleteAsync(messages);

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
