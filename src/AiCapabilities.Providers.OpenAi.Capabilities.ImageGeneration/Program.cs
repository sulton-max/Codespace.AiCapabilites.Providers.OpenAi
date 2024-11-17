using Microsoft.Extensions.Configuration;
using OpenAI;
using OpenAI.Images;
using Shared.Models;

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

// Create client
var openAiClient = new OpenAIClient(openAiApiSettings.ApiKey);
var imageGenerationClient = openAiClient.GetImageClient("dall-e-3");

var prompt = "A photograph of a beautiful sunset over the city.";

var options = new ImageGenerationOptions
{
    Quality = GeneratedImageQuality.High,
    Size = GeneratedImageSize.W1792xH1024,
    Style = GeneratedImageStyle.Vivid,
    ResponseFormat = GeneratedImageFormat.Bytes
};

// Generating an image
var generatedImage = await imageGenerationClient.GenerateImageAsync(prompt, options);
var binaryData = generatedImage.Value.ImageBytes;
var fileStream = File.OpenWrite(Path.Combine(imagesPath, "sunset.jpg"));
var bytesStream = binaryData.ToStream();

// Saving the generated image
await bytesStream.CopyToAsync(fileStream);
await bytesStream.FlushAsync();

bytesStream.Close();
fileStream.Close();