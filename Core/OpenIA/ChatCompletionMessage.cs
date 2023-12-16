namespace Akiled.Core.OpenIA;

using Newtonsoft.Json;

public class ChatCompletionMessage
{
    [JsonProperty("role")]
    public string Role { get; set; }

    [JsonProperty("content")]
    public string Content { get; set; }
}
