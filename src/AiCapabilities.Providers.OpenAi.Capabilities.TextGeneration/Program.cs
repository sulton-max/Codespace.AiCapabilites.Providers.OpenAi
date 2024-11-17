using Microsoft.Extensions.Configuration;
using OpenAI.Chat;
using Shared.Extensions;
using Shared.Models;

// Create configuration builder
var configuration = new ConfigurationBuilder()
    .AddUserSecrets<Program>()
    .Build();

// Get configuration
var openAiApiSettings = new OpenAiApiSettings();
configuration.GetSection(nameof(OpenAiApiSettings)).Bind(openAiApiSettings);

// Create chat completion client
var client = new ChatClient("gpt-4o-mini", openAiApiSettings.ApiKey);

// Text generation
var resultA = await client.CompleteChatAsync(new UserChatMessage("Hello there!"));
Console.WriteLine($"Chat completion result - {resultA.Value.Content.GetResult()}");