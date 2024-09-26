using System.Text.Json;
using AiCapabilities.Providers.OpenAi.Capabilities.FunctionCalling.Extensions;
using AiCapabilities.Providers.OpenAi.Capabilities.FunctionCalling.Models;
using AiCapabilities.Providers.OpenAi.Capabilities.FunctionCalling.Services;
using Microsoft.Extensions.Configuration;
using OpenAI;
using OpenAI.Chat;

// Create configuration builder
var configuration = new ConfigurationBuilder()
    .AddUserSecrets<Program>()
    .Build();

// Get configuration
var openAiApiSettings = new OpenAiApiSettings();
configuration.GetSection(nameof(OpenAiApiSettings)).Bind(openAiApiSettings);

var openAiClient = new OpenAIClient(openAiApiSettings.ApiKey);
var chatCompletionClient = openAiClient.GetChatClient("gpt-4o-mini");
var chatMessages = new List<ChatMessage>()
{
    new SystemChatMessage("You're helpful assistant as part of company dashboard.")
};
var employeeService = new EmployeeService();

var tools = new List<ChatTool>
{
    ChatTool.CreateFunctionTool(
        functionName: nameof(EmployeeService.GetById),
        functionDescription: "Gets an employee by their unique ID, may return null if not found.",
        functionParameters: BinaryData.FromString(
            """
            {
                "type": "object",
                "properties": {
                    "employeeId": {
                    "type": "string",
                    "description": "The unique identifier of the employee."
                    }
                }
            }
            """),
        functionSchemaIsStrict: true),
    ChatTool.CreateFunctionTool(
        functionName: nameof(EmployeeService.GetByName),
        functionDescription: "Gets an employee by their first and last name, may return null if not found.",
        functionParameters: BinaryData.FromString(
            """
            {
                "type": "object",
                "properties": {
                    "firstName": {
                        "type": "string",
                        "description": "The first name of the employee."
                    },
                    "lastName": {
                        "type": "string",
                        "description": "The last name of the employee."
                    }
                }
            }
            """),
        functionSchemaIsStrict: true)
};

var chatCompletionOptions = new ChatCompletionOptions
{
    ParallelToolCallsEnabled = true,
};
tools.ForEach(tool => chatCompletionOptions.Tools.Add(tool));

// Get a user message
var userIntent = Console.ReadLine();
var userMessage = new UserChatMessage(userIntent);
chatMessages.Add(userMessage);

var result = await chatCompletionClient.CompleteChatAsync(chatMessages, chatCompletionOptions);

while (result.Value.FinishReason != ChatFinishReason.Stop)
{
    // Add the assistant message to history
    chatMessages.Add(new AssistantChatMessage(result.Value));

    // Execute the tool calls
    foreach (var toolCall in result.Value.ToolCalls)
    {
        switch (toolCall.FunctionName)
        {
            case nameof(EmployeeService.GetById):
            {
                using var argumentsJson = JsonDocument.Parse(toolCall.FunctionArguments);
                var hasId = argumentsJson.RootElement.TryGetProperty("employeeId", out var employeeId);
                if (!hasId) throw new InvalidOperationException("Employee ID not found in the tool call.");

                var employee = employeeService.GetById(employeeId.GetGuid());
                chatMessages.Add(new ToolChatMessage(toolCall.Id, $"Employee found: {JsonSerializer.Serialize(employee)}"));
                break;
            }
            case nameof(EmployeeService.GetByName):
            {
                using var argumentsJson = JsonDocument.Parse(toolCall.FunctionArguments);
                var hasFirstName = argumentsJson.RootElement.TryGetProperty("firstName", out var firstNameElement);
                var hasLastName = argumentsJson.RootElement.TryGetProperty("lastName", out var lastNameElement);
                if (!hasFirstName || !hasLastName) throw new InvalidOperationException("First name or last name not found in the tool call.");

                var employee = employeeService.GetByName(firstNameElement.GetString()!, lastNameElement.GetString()!);
                chatMessages.Add(new ToolChatMessage(toolCall.Id, $"Employee found: {JsonSerializer.Serialize(employee)}"));
                break;
            }
        }

        result = await chatCompletionClient.CompleteChatAsync(chatMessages, chatCompletionOptions);
    }
}

Console.WriteLine(result.Value.Content.GetResult());