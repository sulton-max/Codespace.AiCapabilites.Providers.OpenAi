using Microsoft.Extensions.Configuration;
using OpenAI;
using Shared.Models;

// Create configuration builder
var configuration = new ConfigurationBuilder()
    .AddUserSecrets<Program>()
    .Build();

// Get configuration
var openAiApiSettings = new OpenAiApiSettings();
configuration.GetSection(nameof(OpenAiApiSettings)).Bind(openAiApiSettings);

// Create a moderation client
var openAiClient = new OpenAIClient(openAiApiSettings.ApiKey);
var moderationClient = openAiClient.GetModerationClient("omni-moderation-latest");

var userComments = new List<string>
{
    // Normal comments
    "Keep up the good work, dude!",
    "Wow! The new syntax looks legit)",
    "Would be great to add official docs to each section",
    "You forgot to mention collection expressions",
    "This video is a complete waste of time, you don’t even know what you’re talking about.",
    "You're just reading the docs, go actually learn something",
    "Mf dropped the best video and thought we won't notice"
};

foreach (var comment in userComments)
{
    var response = await moderationClient.ClassifyTextAsync(comment);
    
    var result = response.Value;

    Console.WriteLine(
        $"""
         Comment - {comment}         

         Flagged: {result.Flagged}
         Sexual - Flagged: {result.Sexual.Flagged}, Score: {result.Sexual.Score:F2}         
         Harassment - Flagged: {result.Harassment.Flagged}, Score: {result.Harassment.Score:F2}
         Hate - Flagged: {result.Hate.Flagged}, Score: {result.Hate.Score:F2}
         Harassment Threatening - Flagged: {result.HarassmentThreatening.Flagged}, Score: {result.HarassmentThreatening.Score:F2}
         Violence - Flagged: {result.Violence.Flagged}, Score: {result.Violence.Score:F2}
         Hate Threatening - Flagged: {result.HateThreatening.Flagged}, Score: {result.HateThreatening.Score:F2}
         Self Harm - Flagged: {result.SelfHarm.Flagged}, Score: {result.SelfHarm.Score:F2}
         Sexual Minors - Flagged: {result.SexualMinors.Flagged}, Score: {result.SexualMinors.Score:F2}
         Violence Graphic - Flagged: {result.ViolenceGraphic.Flagged}, Score: {result.ViolenceGraphic.Score:F2}
         Self Harm Instructions - Flagged: {result.SelfHarmInstructions.Flagged}, Score: {result.SelfHarmInstructions.Score:F2}
         Self Harm Intent - Flagged: {result.SelfHarmIntent.Flagged}, Score: {result.SelfHarmIntent.Score:F2}
         """);
    
    Console.WriteLine();
}