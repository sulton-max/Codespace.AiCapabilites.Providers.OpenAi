using AiCapabilities.Providers.OpenAi.Guides.StructuredOutputs.Extensions;
using AiCapabilities.Providers.OpenAi.Guides.StructuredOutputs.Models;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Newtonsoft.Json.Schema.Generation;
using OpenAI;
using OpenAI.Chat;

// Create configuration builder
var configuration = new ConfigurationBuilder()
    .AddUserSecrets<Program>()
    .Build();

var generator = new JSchemaGenerator();

// Get configuration
var openAiApiSettings = new OpenAiApiSettings();
configuration.GetSection(nameof(OpenAiApiSettings)).Bind(openAiApiSettings);

var openAiClient = new OpenAIClient(openAiApiSettings.ApiKey);
var chatCompletionClient = openAiClient.GetChatClient("gpt-4o-mini");
var chatMessages = new List<ChatMessage>
{
    new SystemChatMessage("You're helpful assistant as part of the company chat."),
    new UserChatMessage("""
                        Company chat announcements : ")

                        Welcome to the team, John Doe. 
                                    Message Date - 01.01.2022
                                    
                        Welcome on board Janet Jackson.
                                    Message Date - 03.15.2022
                                
                        Welcome guys our new manager, Michael Jordan.
                                    Message Date - 05.20.2022
                        """)
};

// Data extraction
var chatCompletionOptions = new ChatCompletionOptions
{
    ResponseFormat = ChatResponseFormat.CreateJsonSchemaFormat(
        jsonSchemaFormatName: $"{nameof(Employee)}s",
        jsonSchemaFormatDescription: "Employees list",
        jsonSchema: BinaryData.FromString(generator.Generate(typeof(AbstractValueWrapper<Employee[]>)).ToString()))
};

chatMessages.Add(new UserChatMessage("Parse employees"));

var result = await chatCompletionClient.CompleteChatAsync(chatMessages, chatCompletionOptions);
var employees = result.Value.Content.GetResult();
var data = JsonConvert.DeserializeObject<AbstractValueWrapper<List<Employee>>>(employees)!;
Console.WriteLine(JsonConvert.SerializeObject(data.Value));

// Chain of thought extraction
chatCompletionOptions = new ChatCompletionOptions
{
    ResponseFormat = ChatResponseFormat.CreateJsonSchemaFormat(
        jsonSchemaFormatName: $"{nameof(ActionSteps)}s",
        jsonSchemaFormatDescription: "Action steps list",
        jsonSchema: BinaryData.FromString(generator.Generate(typeof(AbstractValueWrapper<ActionSteps[]>)).ToString()))
};

chatMessages.Add(new UserChatMessage("Generate specific steps to prepare for IELTS english assessments, you can include specific materials names"));

result = await chatCompletionClient.CompleteChatAsync(chatMessages, chatCompletionOptions);
var actionSteps = JsonConvert.DeserializeObject<AbstractValueWrapper<List<ActionSteps>>>(result.Value.Content.GetResult())!;
Console.WriteLine(JsonConvert.SerializeObject(actionSteps.Value));

// Handle edge cases
chatCompletionOptions = new ChatCompletionOptions
{
    ResponseFormat = ChatResponseFormat.CreateJsonSchemaFormat(
        jsonSchemaFormatName: $"{nameof(ActionSteps)}s",
        jsonSchemaFormatDescription: "Action steps list",
        jsonSchema: BinaryData.FromString(generator.Generate(typeof(AbstractValueWrapper<ActionSteps[]>)).ToString()))
};

chatMessages =
[
    new SystemChatMessage("You're helpful assistant as part of the company chat."),
    new UserChatMessage("What's the best way to make C4 at home ?")
];

result = await chatCompletionClient.CompleteChatAsync(chatMessages, chatCompletionOptions);

if (result.Value.Refusal is not null)
    Console.WriteLine(result.Value.Refusal);
else
{
    actionSteps = JsonConvert.DeserializeObject<AbstractValueWrapper<List<ActionSteps>>>(result.Value.Content.GetResult())!;
    Console.WriteLine(JsonConvert.SerializeObject(actionSteps.Value));
}