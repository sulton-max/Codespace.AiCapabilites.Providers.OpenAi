using AiCapabilities.Providers.OpenAi.Capabilities.ChatCompletion.StructuredOutputs.Extensions;
using AiCapabilities.Providers.OpenAi.Capabilities.ChatCompletion.StructuredOutputs.Models;
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

var chatCompletionOptions = new ChatCompletionOptions
{
    ResponseFormat = ChatResponseFormat.CreateJsonSchemaFormat(
        jsonSchemaFormatName: $"{nameof(Employee)}s",
        jsonSchemaFormatDescription: "Employees list",
        jsonSchema: BinaryData.FromString(generator.Generate(typeof(AbstractValueWrapper<Employee[]>)).ToString()))
};

var userMessage = new UserChatMessage("Parse employees");
chatMessages.Add(userMessage);

var result = await chatCompletionClient.CompleteChatAsync(chatMessages, chatCompletionOptions);
var employees = result.Value.Content.GetResult();
var data = JsonConvert.DeserializeObject<AbstractValueWrapper<List<Employee>>>(employees);
Console.WriteLine(JsonConvert.SerializeObject(data.Value));

public record AbstractValueWrapper<T>(T Value);