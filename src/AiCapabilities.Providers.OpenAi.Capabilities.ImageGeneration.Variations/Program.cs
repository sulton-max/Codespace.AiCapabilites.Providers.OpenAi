using AiCapabilities.Providers.OpenAi.Capabilities.ImageGeneration.Variations.Models;
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
var imageGenerationClient = openAiClient.GetImageClient("dall-e-2");

var options = new ImageGenerationOptions
{
    Quality = GeneratedImageQuality.High,
    Size = GeneratedImageSize.W512xH512,
    Style = GeneratedImageStyle.Vivid,
    ResponseFormat = GeneratedImageFormat.Bytes,
};

// Generating multiple variations based on a prompt
var prompt = "Little cat playing around with a ball of yarn";
var resultA = await imageGenerationClient.GenerateImagesAsync(prompt, 2, options);
var imagesA = resultA.Value.ToList();

var variationsA = Path.Combine(imagesPath, "VariationsA");
if (!Directory.Exists(variationsA)) Directory.CreateDirectory(variationsA);
await Task.WhenAll(imagesA.Select(async image =>
{
    var newFileStream = File.OpenWrite(Path.Combine(variationsA, Path.ChangeExtension(Guid.NewGuid().ToString(), "jpg")));
    var imageStream = image.ImageBytes.ToStream();

    await imageStream.CopyToAsync(newFileStream);
    await imageStream.FlushAsync();

    imageStream.Close();
    newFileStream.Close();
}));

// TODO : Image upload isn't working
var imagePath = Path.Combine(Directory.GetCurrentDirectory(), "Assets", "cat.png");
var inputImage = File.OpenRead(imagePath);

var buffer = new byte[inputImage.Length];
await inputImage.ReadAsync(buffer, 0, (int)inputImage.Length);

var variationOptions = new ImageVariationOptions
{
    Size = GeneratedImageSize.W512xH512,
    ResponseFormat = GeneratedImageFormat.Bytes,
};

// Generate one new variation based on an image
var resultB = await imageGenerationClient.GenerateImageVariationAsync(inputImage, "cat.png", variationOptions);
var binaryData = resultB.Value.ImageBytes;

var variationsB = Path.Combine(imagesPath, "VariationsB");
if (!Directory.Exists(variationsB)) Directory.CreateDirectory(variationsB);
var fileStream = File.OpenWrite(Path.Combine(variationsB, Path.ChangeExtension(Guid.NewGuid().ToString(), "jpg")));
var bytesStream = binaryData.ToStream();

await bytesStream.CopyToAsync(fileStream);
await bytesStream.FlushAsync();

bytesStream.Close();
fileStream.Close();

// Generating multiple variations based on an image
var resultC = await imageGenerationClient.GenerateImageVariationsAsync(inputImage, "cat.png", 2, variationOptions);
var imagesC = resultC.Value.ToList();

var variationsC = Path.Combine(imagesPath, "VariationsC");
if (!Directory.Exists(variationsC)) Directory.CreateDirectory(variationsC);
await Task.WhenAll(imagesC.Select(async image =>
{
    var newFileStream = File.OpenWrite(Path.Combine(variationsC, Path.ChangeExtension(Guid.NewGuid().ToString(), "jpg")));
    var imageStream = image.ImageBytes.ToStream();

    await imageStream.CopyToAsync(newFileStream);
    await imageStream.FlushAsync();

    imageStream.Close();
    newFileStream.Close();
}));