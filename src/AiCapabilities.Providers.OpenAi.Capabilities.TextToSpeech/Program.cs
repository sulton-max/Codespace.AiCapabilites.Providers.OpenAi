using Microsoft.Extensions.Configuration;
using OpenAI;
using OpenAI.Audio;
using Shared.Models;

// Create configuration builder
var configuration = new ConfigurationBuilder()
    .AddUserSecrets<Program>()
    .Build();

// Get configuration
var openAiApiSettings = new OpenAiApiSettings();
configuration.GetSection(nameof(OpenAiApiSettings)).Bind(openAiApiSettings);

// Create an audio generation client
var openAiClient = new OpenAIClient(openAiApiSettings.ApiKey);
var audioGenerationClient = openAiClient.GetAudioClient("tts-1");

// Create configs
var voiceOptions = new GeneratedSpeechVoice("alloy");
var speechGenerationOptions = new SpeechGenerationOptions
{
    SpeedRatio = 1
};

var projectDirectory = Path.Combine(Directory.GetCurrentDirectory(), "..", "..", "..");
var speechFilesPath = Path.Combine(projectDirectory, "Speech");
if (!Directory.Exists(speechFilesPath)) Directory.CreateDirectory(speechFilesPath);

// Speech generation
var text = "0 days since the last AI news and 0 hours since last JS framework )";
var response = await audioGenerationClient.GenerateSpeechAsync(text, voiceOptions, speechGenerationOptions);

var audioStream = response.Value.ToStream();
var filePath = Path.Combine(speechFilesPath, Path.ChangeExtension(Guid.NewGuid().ToString(), "mp3"));
var fileStream = File.OpenWrite(filePath);
await audioStream.CopyToAsync(fileStream);
await audioStream.FlushAsync();