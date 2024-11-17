using AiCapabilities.Providers.OpenAi.Capabilities.AudioGeneration;
using AiCapabilities.Providers.OpenAi.Capabilities.AudioGeneration.Models;
using Microsoft.Extensions.Configuration;
using OpenAI;
using OpenAI.Audio;

// Create configuration builder
var configuration = new ConfigurationBuilder()
    .AddUserSecrets<Program>()
    .Build();

// Get configuration
var openAiApiSettings = new OpenAiApiSettings();
configuration.GetSection(nameof(OpenAiApiSettings)).Bind(openAiApiSettings);

// Create audio generation client
var openAiClient = new OpenAIClient(openAiApiSettings.ApiKey);
var audioGenerationClient = openAiClient.GetAudioClient("gpt-4o-audio-preview");

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
try
{
    var test = new Test();
    await test.ExecuteAsync(openAiApiSettings.ApiKey, Path.Combine(speechFilesPath, Path.ChangeExtension(Guid.NewGuid().ToString(), "wav")));

    // var test = await audioGenerationClient.GenerateSpeechAsync("Hey, this is text example of speech generation", GeneratedSpeechVoice.Echo);
    // var resultA = test.Value.ToStream();
    // var resultAStream = File.OpenWrite(Path.Combine(speechFilesPath, Path.ChangeExtension(Guid.NewGuid().ToString(), "wav")));
    // await resultA.CopyToAsync(resultAStream);
    // await resultA.FlushAsync();
}
catch (Exception e)
{
    Console.WriteLine(e);
    throw;
}

// Transcribe audio

// Translate 

// Input - audio, Output - text + audio

// Input - audio, Output - text

// Input - text + audio, Output - text + audio

// Input - text + audio, Output - text