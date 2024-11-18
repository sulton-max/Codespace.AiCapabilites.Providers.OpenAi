using Microsoft.Extensions.Configuration;
using OpenAI;
using OpenAI.Chat;
using Shared.Extensions;
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
var chatClient = openAiClient.GetChatClient("gpt-4o-mini");

// Get an audio file
var projectDirectory = Path.Combine(Directory.GetCurrentDirectory(), "..", "..", "..");
var audioFilesPath = Path.Combine(projectDirectory, "Speech");
var audioFilePath = Directory.GetFiles(audioFilesPath, "*.mp3").First();

// Post-processing with GPT
var response = await audioGenerationClient.TranscribeAudioAsync(File.OpenRead(audioFilePath), Path.GetFileName(audioFilePath));
var chatMessages = new List<ChatMessage>
{
    new SystemChatMessage(
        """
        You're helpful assistant. Your task is to correct any spelling discrepancies in the given text. Make sure following product names are spelled 
        correctly - ZentriQix, Digique Plus, CynapseFive, and VortiQore V8.
        """),
    new UserChatMessage(response.Value.Text)
};

var result = await chatClient.CompleteChatAsync(chatMessages);
Console.WriteLine($"Transcription: {result.Value.Content.GetResult()}");