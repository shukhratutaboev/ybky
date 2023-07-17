using System.Text.Json.Serialization;

namespace CustomTLV;

[Serializable]
public class Person
{
    [JsonPropertyName("firstName")]
    public string FirstName { get; set; }

    [JsonPropertyName("lastName")]
    public string LastName { get; set; }

    [JsonPropertyName("age")]
    public int Age { get; set; }
}