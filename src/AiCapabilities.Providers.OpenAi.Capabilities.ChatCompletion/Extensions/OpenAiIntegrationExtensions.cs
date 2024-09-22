using System.Text;
using OpenAI.Chat;

namespace AiCapabilities.Providers.OpenAi.Capabilities.ChatCompletion.Extensions;

public static class OpenAiIntegrationExtensions
{
    public static string GetResult(this IEnumerable<ChatMessageContentPart> content)
    {
        var builder = new StringBuilder();
        foreach (var contentPart in content)
            builder.Append(contentPart.Text);

        return builder.ToString();
    }
}