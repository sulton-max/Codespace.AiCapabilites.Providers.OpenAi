using AiCapabilities.Providers.OpenAi.Capabilities.ImageGeneration.Editing.Models;
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
var projectDirectory = Path.Combine(Directory.GetCurrentDirectory(), "..", "..", "..");
var imagesPath = Path.Combine(projectDirectory, "Images");
var assetsPath = Path.Combine(projectDirectory, "Assets");
if (!Directory.Exists(imagesPath)) Directory.CreateDirectory(imagesPath);

var openAiClient = new OpenAIClient(openAiApiSettings.ApiKey);
var imageGenerationClient = openAiClient.GetImageClient("dall-e-2");

var prompt = "A sunlit indoor lounge area with a pool and an orange cat looking down at the pool";
var originalImageName = "image-original.png";
var maskImageName = "image-mask.png";
var originalImage = new MemoryStream(await File.ReadAllBytesAsync(Path.Combine(assetsPath, originalImageName)));
var maskImage = new MemoryStream(await File.ReadAllBytesAsync(Path.Combine(assetsPath, maskImageName)));

var editOptions = new ImageEditOptions
{
    Size = GeneratedImageSize.W1024xH1024,
    ResponseFormat = GeneratedImageFormat.Bytes
};

// Generate a single image edit without a mask
// originalImage.Position = 0;
//
// var resultA = await imageGenerationClient.GenerateImageEditAsync(originalImage, originalImageName, prompt, editOptions);
//
// var resultAImageStream = resultA.Value.ImageBytes.ToStream();
// var variationA = Path.Combine(imagesPath, "VariationA");
// if (!Directory.Exists(variationA)) Directory.CreateDirectory(variationA);
// var newFileAStream = File.OpenWrite(Path.Combine(variationA, Path.ChangeExtension(Guid.NewGuid().ToString(), "png")));
//
// await resultAImageStream.CopyToAsync(newFileAStream);
// await resultAImageStream.FlushAsync();
//
// resultAImageStream.Close();
// newFileAStream.Close();
//
// // Generate multiple image edit variants without a mask   
// originalImage.Position = 0;
//
// var resultC = await imageGenerationClient.GenerateImageEditsAsync(originalImage, originalImageName, prompt, 2, editOptions);
//
// var imagesC = resultC.Value.ToList();
//
// var variationsC = Path.Combine(imagesPath, "VariationsC");
// if (!Directory.Exists(variationsC)) Directory.CreateDirectory(variationsC);
// await Task.WhenAll(imagesC.Select(async image =>
// {
//     var fileStream = File.OpenWrite(Path.Combine(variationsC, Path.ChangeExtension(Guid.NewGuid().ToString(), "png")));
//     var imageStream = image.ImageBytes.ToStream();
//
//     await imageStream.CopyToAsync(fileStream);
//     await imageStream.FlushAsync();
//
//     imageStream.Close();
//     fileStream.Close();
// }));

// Generate a single image edit with a mask
originalImage.Seek(0, SeekOrigin.Begin);
maskImage.Seek(0, SeekOrigin.Begin);

var resultB = await imageGenerationClient.GenerateImageEditAsync(originalImage, originalImageName, prompt, maskImage, maskImageName, editOptions);

var resultBImageStream = resultB.Value.ImageBytes.ToStream();
var variationB = Path.Combine(imagesPath, "VariationB");
if (!Directory.Exists(variationB)) Directory.CreateDirectory(variationB);
var newFileBStream = File.OpenWrite(Path.Combine(variationB, Path.ChangeExtension(Guid.NewGuid().ToString(), "png")));

await resultBImageStream.CopyToAsync(newFileBStream);
await resultBImageStream.FlushAsync();

resultBImageStream.Close();
newFileBStream.Close();

// Generate multiple image edit variants with mask   
originalImage.Seek(0, SeekOrigin.Begin);
maskImage.Seek(0, SeekOrigin.Begin);

var resultD = await imageGenerationClient.GenerateImageEditsAsync(originalImage, originalImageName, prompt, maskImage, maskImageName, 2, editOptions);

var imagesD = resultD.Value.ToList();
var variationsD = Path.Combine(imagesPath, "VariationsD");
if (!Directory.Exists(variationsD)) Directory.CreateDirectory(variationsD);
await Task.WhenAll(imagesD.Select(async image =>
{
    var fileStream = File.OpenWrite(Path.Combine(variationsD, Path.ChangeExtension(Guid.NewGuid().ToString(), "png")));
    var imageStream = image.ImageBytes.ToStream();

    await imageStream.CopyToAsync(fileStream);
    await imageStream.FlushAsync();

    imageStream.Close();
    fileStream.Close();
}));