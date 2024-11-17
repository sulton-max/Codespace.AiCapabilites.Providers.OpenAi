using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

namespace AiCapabilities.Providers.OpenAi.Capabilities.AudioGeneration;

public class Test
{
    private static readonly HttpClient client = new();

    public async Task ExecuteAsync(string apiKey, string filePath)
    {
        var requestUri = "https://api.openai.com/v1/audio/speech";
        var inputText = "Hey, this is a text example of speech generation.";
        var model = "tts-1";
        var voice = "alloy";

        var requestBody = new
        {
            model,
            input = inputText,
            voice
        };

        var jsonContent = new StringContent(
            JsonSerializer.Serialize(requestBody),
            Encoding.UTF8,
            "application/json"
        );

        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", apiKey);

        var response = await client.PostAsync(requestUri, jsonContent);

        if (response.IsSuccessStatusCode)
        {
            var audioBytes = await response.Content.ReadAsByteArrayAsync();
            await File.WriteAllBytesAsync(filePath, audioBytes);
            Console.WriteLine("Audio file saved successfully.");
        }
        else
        {
            var errorContent = await response.Content.ReadAsStringAsync();
            Console.WriteLine($"Error: {response.StatusCode}");
            Console.WriteLine(errorContent);
        }
    }
}