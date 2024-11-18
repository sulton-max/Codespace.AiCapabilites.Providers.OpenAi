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
var audioGenerationClient = openAiClient.GetAudioClient("whisper-1");

// Get an audio file
var projectDirectory = Path.Combine(Directory.GetCurrentDirectory(), "..", "..", "..");
var audioFilesPath = Path.Combine(projectDirectory, "Speech");
var audioFilePath = Directory.GetFiles(audioFilesPath, "*.mp3").First();

// Simple transaction of an audio
var responseA = await audioGenerationClient.TranscribeAudioAsync(File.OpenRead(audioFilePath), Path.GetFileName(audioFilePath));
Console.WriteLine($"Transcription: {responseA.Value.Text}");

// Verbose transaction of an audio
var transcriptionOptions = new AudioTranscriptionOptions
{
    Temperature = 0,
    Language = "en",
    ResponseFormat = AudioTranscriptionFormat.Verbose,
    TimestampGranularities = AudioTimestampGranularities.Word
};

var responseB = await audioGenerationClient.TranscribeAudioAsync(File.OpenRead(audioFilePath), Path.GetFileName(audioFilePath), transcriptionOptions);
Console.WriteLine($"Transcription: {responseB.Value.Text}, \n Language: {responseB.Value.Language} \n Duration: {responseB.Value.Duration} \n Words : {responseB.Value.Words.Count}");