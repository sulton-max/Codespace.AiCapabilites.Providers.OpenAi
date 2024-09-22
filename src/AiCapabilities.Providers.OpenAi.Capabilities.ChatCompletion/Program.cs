using AiCapabilities.Providers.OpenAi.Capabilities.ChatCompletion.Extensions;
using AiCapabilities.Providers.OpenAi.Capabilities.ChatCompletion.Models;
using Microsoft.Extensions.Configuration;
using OpenAI.Chat;

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

// Image processing
var filePath = Path.Combine(Directory.GetCurrentDirectory(), "Images", "cat.png");
var binaryData = BinaryData.FromBytes(await File.ReadAllBytesAsync(filePath));
var resultB = await client.CompleteChatAsync(new UserChatMessage(
    ChatMessageContentPart.CreateImagePart(binaryData, "image/png"),
    ChatMessageContentPart.CreateTextPart("What's in the image ?")
));
Console.WriteLine($"Chat completion result - {resultB.Value.Content.GetResult()}");