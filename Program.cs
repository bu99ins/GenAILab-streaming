using System.Text;
using GenAiForDotNet;
using Microsoft.Extensions.AI;
using OpenAI;

var key = Environment.GetEnvironmentVariable("OPENAI_API_KEY");
if (string.IsNullOrEmpty(key))
{
    Console.WriteLine("Please set the OPENAI_API_KEY environment variable.");
    return;
}

var chatClient = new StreamingChatClient(new OpenAIClient(key).GetChatClient("gpt-5.2").AsIChatClient());
//var chatClient = new BatchChatClient(new OpenAIClient(key).GetChatClient("gpt-5.2").AsIChatClient());

var inputModerator = new InputModerator();
inputModerator.Init();

var messages = new List<ChatMessage>
{
    new(ChatRole.System, "You are an annoyingly friendly chatbot with artificial intelligence " +
                         "at the Azure Saturday AI conference in Seattle. " +
                         "Be brief.")
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

        messages.Add(new ChatMessage(ChatRole.Assistant, answer));

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
