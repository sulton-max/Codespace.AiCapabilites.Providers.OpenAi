using Microsoft.Extensions.Configuration;
using OpenAI;
using OpenAI.Audio;
using OpenAI.Chat;
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

// Transcription with a prompt
var transcriptionOptionsA = new AudioTranscriptionOptions
{
    ResponseFormat = AudioTranscriptionFormat.Simple,
    Prompt = "3 bags, 4 books."
};

var response = await audioClient.TranscribeAudioAsync(File.OpenRead(audioFilePath), Path.GetFileName(audioFilePath), transcriptionOptionsA);
Console.WriteLine($"Transcription: {response.Value.Text}");