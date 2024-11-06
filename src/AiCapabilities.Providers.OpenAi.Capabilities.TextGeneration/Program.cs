using AiCapabilities.Providers.OpenAi.Capabilities.TextGeneration.Extensions;
using AiCapabilities.Providers.OpenAi.Capabilities.TextGeneration.Models;
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

// Image processing by binary data
var imagePath = Path.Combine(Directory.GetCurrentDirectory(), "Images", "cat.png");
var imageBinaryData = BinaryData.FromBytes(await File.ReadAllBytesAsync(imagePath));
var resultB = await client.CompleteChatAsync(new UserChatMessage(
    ChatMessageContentPart.CreateImagePart(imageBinaryData, "image/png"),
    ChatMessageContentPart.CreateTextPart("What's in the image ?")
));
Console.WriteLine($"Chat completion result - {resultB.Value.Content.GetResult()}");

// Image processing by URL
var imageUrl = "https://encrypted-tbn0.gstatic.com/images?q=tbn:ANd9GcTzDhN6PT71Exuhr6j6KayhENg5ofz4iXGR1A&s";
var resultC = await client.CompleteChatAsync(new UserChatMessage(
    ChatMessageContentPart.CreateImagePart(new Uri(imageUrl)),
    ChatMessageContentPart.CreateTextPart("What's in the image ?")
));
Console.WriteLine($"Chat completion result - {resultC.Value.Content.GetResult()}");

// Image processing for multiple images
var imageUrls = new[]
{
    "https://encrypted-tbn0.gstatic.com/images?q=tbn:ANd9GcR8e9Y4mPCcKbVrwPQBhrw-6yTdAKxjUhmGkA&s",
    "https://encrypted-tbn0.gstatic.com/images?q=tbn:ANd9GcR2UOW09a8y-Ue_FtTFn01C4U4-dZmIax-P_g&s"
};
var resultD = await client.CompleteChatAsync(new UserChatMessage(
    ChatMessageContentPart.CreateImagePart(new Uri(imageUrls[0])),
    ChatMessageContentPart.CreateImagePart(new Uri(imageUrls[1])),
    ChatMessageContentPart.CreateTextPart("What's relation between the images ?")
));
Console.WriteLine($"Chat completion result - {resultD.Value.Content.GetResult()}");