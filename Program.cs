using System.Text;
using GenAiForDotNet;
using GenAiForDotNet.AiClientFactory;
using GenAiForDotNet.Common;
using Microsoft.Extensions.AI;

var completion = new AiClientFactory().CreateStreamingCompletion(ChatClientType.OpenAi);
//var completion = new AiClientFactory().CreateCompletion(ChatClientType.OpenAi);

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
        var (answer, lastReason) = await completion.CompleteAsync(messages);

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
