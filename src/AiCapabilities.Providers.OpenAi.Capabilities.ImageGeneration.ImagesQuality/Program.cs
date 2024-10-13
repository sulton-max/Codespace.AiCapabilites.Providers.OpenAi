using AiCapabilities.Providers.OpenAi.Capabilities.ImageGeneration.ImagesQuality.Models;
using Microsoft.Extensions.Configuration;
using OpenAI;
using OpenAI.Images;

// Create configuration builder
var configuration = new ConfigurationBuilder()
    .AddUserSecrets<Program>()
    .Build();

// Get configuration
var openAiApiSettings = new OpenAiApiSettings();
configuration.GetSection(nameof(OpenAiApiSettings)).Bind(openAiApiSettings);
var projectDirectory = Directory.GetCurrentDirectory();
var imagesPath = Path.Combine(projectDirectory, "..", "..", "..", "Images");
if (!Directory.Exists(imagesPath)) Directory.CreateDirectory(imagesPath);

var openAiClient = new OpenAIClient(openAiApiSettings.ApiKey);
var imageGenerationClient = openAiClient.GetImageClient("dall-e-3");

var prompt = "Cute cat looking up.";

// Generating a medium quality vivid image
var options = new ImageGenerationOptions
{
    Quality = GeneratedImageQuality.High,
    Size = GeneratedImageSize.W1024xH1024,
    Style = GeneratedImageStyle.Vivid,
    ResponseFormat = GeneratedImageFormat.Bytes
};

var generatedImage = await imageGenerationClient.GenerateImageAsync(prompt, options);
var binaryData = generatedImage.Value.ImageBytes;
var fileStream = File.OpenWrite(Path.Combine(imagesPath, Path.ChangeExtension(Guid.NewGuid().ToString(), "jpg")));
var bytesStream = binaryData.ToStream();

await bytesStream.CopyToAsync(fileStream);
await bytesStream.FlushAsync();

bytesStream.Close();
fileStream.Close();

// Generating a high quality neutral image
options = new ImageGenerationOptions
{
    Quality = GeneratedImageQuality.High,
    Size = GeneratedImageSize.W1024xH1792,
    Style = GeneratedImageStyle.Natural,
    ResponseFormat = GeneratedImageFormat.Bytes
};

// Generating an image
generatedImage = await imageGenerationClient.GenerateImageAsync(prompt, options);
binaryData = generatedImage.Value.ImageBytes;
fileStream = File.OpenWrite(Path.Combine(imagesPath, Path.ChangeExtension(Guid.NewGuid().ToString(), "jpg")));
bytesStream = binaryData.ToStream();

// Saving the generated image
await bytesStream.CopyToAsync(fileStream);
await bytesStream.FlushAsync();

bytesStream.Close();
fileStream.Close();