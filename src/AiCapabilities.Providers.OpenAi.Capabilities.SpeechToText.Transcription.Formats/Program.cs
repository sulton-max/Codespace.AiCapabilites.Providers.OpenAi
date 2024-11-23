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

// VTT transcription
var transcriptionOptionsA = new AudioTranscriptionOptions
{
    ResponseFormat = AudioTranscriptionFormat.Vtt,
};

var responseA = await audioClient.TranscribeAudioAsync(
    File.OpenRead(audioFilePath), 
    Path.GetFileName(audioFilePath), 
    transcriptionOptionsA);
Console.WriteLine($"Transcription: {responseA.Value.Text}, \n Language: {responseA.Value.Language} \n Duration {responseA.Value.Duration}");

// SRT transcription
var transcriptionOptionsB = new AudioTranscriptionOptions
{
    ResponseFormat = AudioTranscriptionFormat.Srt,
};

var responseB = await audioClient.TranscribeAudioAsync(
    File.OpenRead(audioFilePath), 
    Path.GetFileName(audioFilePath), 
    transcriptionOptionsB);
Console.WriteLine($"Transcription: {responseB.Value.Text}, \n Language: {responseB.Value.Language} \n Duration {responseB.Value.Duration}");