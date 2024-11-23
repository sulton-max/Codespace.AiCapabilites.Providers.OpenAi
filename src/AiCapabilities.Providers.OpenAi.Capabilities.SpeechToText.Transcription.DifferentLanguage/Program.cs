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
var audioClient = openAiClient.GetAudioClient("whisper-1");

// Get an audio file
var projectDirectory = Path.Combine(Directory.GetCurrentDirectory(), "..", "..", "..");
var audioFilesPath = Path.Combine(projectDirectory, "Speech");
var audioFilePath = Directory.GetFiles(audioFilesPath, "*.mp3").First();

// Simple transaction of an audio with different language
var transcriptionOptions = new AudioTranscriptionOptions
{
    Temperature = 0,
    Language = "ru",
    ResponseFormat = AudioTranscriptionFormat.Simple
};

var response = await audioClient.TranscribeAudioAsync(File.OpenRead(audioFilePath), Path.GetFileName(audioFilePath), transcriptionOptions);
Console.WriteLine($"Transcription: {response.Value.Text}");