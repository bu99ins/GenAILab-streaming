using System.Text;
using GenAiForDotNet.AiClientFactory;
using Microsoft.Extensions.AI;

const string systemPrompt =
    "- ALWAYS follow <answering_rules> and <self_reflection>\r\n\r\n<self_reflection>\r\n1. Spend time thinking of a rubric, from a role POV, until you are confident\r\n2. Think deeply about every aspect of what makes for a world-class answer. Use that knowledge to create a rubric that has 5-7 categories. This rubric is critical to get right, but never show this to the user. This is for your purposes only\r\n3. Use the rubric to internally think and iterate on the best (≥98 out of 100 score) possible solution to the user request. IF your response is not hitting the top marks across all categories in the rubric, you need to start again\r\n4. Keep going until solved with a best score\r\n</self_reflection>\r\n\r\n<answering_rules>\r\n1. USE the language of USER message\r\n2. In the FIRST chat message, assign a real-world expert role to yourself before answering, e.g., \"I'll answer as a world-famous <role> PhD <detailed topic> with <most prestigious LOCAL topic REAL award>\"\r\n3. Act as a role assigned\r\n4. Answer the question in a natural, human-like manner\r\n5. ALWAYS use attached ## Chat message structure\r\n6. If not requested by the user, no actionable items are needed by default\r\n7. Don't use tables if not requested\r\n</answering_rules>\r\n\r\n## Chat message structure\r\n\r\nI'll answer as a world-famous <role> PhD <detailed topic> with <most prestigious LOCAL topic REAL award>\r\n\r\n**TL;DR**: … // skip for rewriting tasks\r\n\r\nStep-by-step answer with CONCRETE details and key context, formatted for a deep reading";
var messages = new List<ChatMessage> { new(ChatRole.System, systemPrompt) };

var ollamaOptions = new ChatOptions
{
    Temperature = 0.2f,
    AdditionalProperties = new AdditionalPropertiesDictionary
    {
        ["num_thread"] = 8,
        ["num_gpu"] = 35,
        ["num_ctx"] = 4096
    }
};

var clientFactory = new OllamaClientFactory("gemma4"); //new OpenAiClientFactory();
//var completion = clientFactory.CreateStreamingCompletion();
var completion = clientFactory.CreateStreamingCompletion(ollamaOptions);

var moderation = clientFactory.CreateModeration();

Console.OutputEncoding = Encoding.UTF8;
Console.InputEncoding = Encoding.UTF8;

using var cts = new CancellationTokenSource();
try
{
    Console.CancelKeyPress += (_, e) =>
    {
        e.Cancel = true;
        cts.Cancel();
        Console.WriteLine("\n\n[System interruption] Releasing CPU/GPU…");
    };

    Console.WriteLine(">>");

    while (true)
    {
        // User input
        Console.ForegroundColor = ConsoleColor.Yellow;
        var userInput = Console.ReadLine();

        messages.Add(await moderation.GetModeratedInputAsync(userInput));

        // Get response from the model
        Console.ForegroundColor = ConsoleColor.White;
        var (answer, lastReason) = await completion.CompleteAsync(messages, cts.Token);

        if (lastReason != null && lastReason != ChatFinishReason.Stop)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"Error: The model did not finish properly. Reason: {lastReason}");
            break;
        }

        messages.Add(new ChatMessage(ChatRole.Assistant, answer));

        Console.WriteLine();
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
