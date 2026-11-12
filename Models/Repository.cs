using System.Text.Json.Serialization;

namespace GitHubDeveloperExplorer.Models;

public class Repository
{
    [JsonPropertyName("name")]
    public string? Name { get; set; }

    [JsonPropertyName("description")]
    public string? Description { get; set; }

    [JsonPropertyName("language")]
    public string? Language { get; set; }

    [JsonPropertyName("stargazers_count")]
    public int Stars { get; set; }

    [JsonPropertyName("html_url")]
    public string? RepositoryUrl { get; set; }
}