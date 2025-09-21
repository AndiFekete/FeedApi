using FeedApi.Data.Entities;
using System.Text.Json;
using System.Text.Json.Serialization;

public class FeedJsonConverter : JsonConverter<Feed>
{
    public override Feed Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        using var doc = JsonDocument.ParseValue(ref reader);
        var discriminator = doc.RootElement.GetProperty("discriminator").GetString();

        Feed result = discriminator switch
        {
            "Image" => JsonSerializer.Deserialize<ImageFeed>(doc.RootElement.GetRawText(), options),
            "Video" => JsonSerializer.Deserialize<VideoFeed>(doc.RootElement.GetRawText(), options),
            _ => new Feed
            {
                FeedId = doc.RootElement.GetProperty("feedId").GetInt32(),
                UserId = doc.RootElement.GetProperty("userId").GetInt32(),
                Title = doc.RootElement.GetProperty("title").GetString(),
                Description = doc.RootElement.GetProperty("description").GetString(),
                Discriminator = discriminator
            }
        };

        return result;
    }

    public override void Write(Utf8JsonWriter writer, Feed value, JsonSerializerOptions options)
    {
        writer.WriteStartObject();
        writer.WriteNumber("FeedId", value.FeedId);
        writer.WriteNumber("UserId", value.UserId);
        writer.WriteString("Title", value.Title);
        writer.WriteString("Description", value.Description);
        writer.WriteString("Discriminator", value.Discriminator);
        writer.WriteEndObject();
    }
}