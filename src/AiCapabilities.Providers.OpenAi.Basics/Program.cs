// create servcies collection

using System.Text;
using AiCapabilities.Providers.OpenAi.Basics;
using AiCapabilities.Providers.OpenAi.Basics.Models;
using Microsoft.Extensions.Configuration;
using OpenAI.Chat;

// create configuration builder
var configuration = new ConfigurationBuilder()
    .AddUserSecrets<Program>()
    .Build();

// Get configuration
var openAiApiSettings = new OpenAiApiSettings();
configuration.GetSection(nameof(OpenAiApiSettings)).Bind(openAiApiSettings);

// Create chat completion client
var client = new ChatClient("gpt-4o-mini", openAiApiSettings.ApiKey);

// Get completion
var chatCompletion = await client.CompleteChatAsync(new UserChatMessage("Hello there!"));

// Combine completion
var builder = new StringBuilder();
foreach (var content in chatCompletion.Value.Content)
    builder.Append(content.Text);

Console.WriteLine($"Chat completion result - {builder}");