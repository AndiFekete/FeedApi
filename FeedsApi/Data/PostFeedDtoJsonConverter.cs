using FeedApi.Data.Entities;
using FeedsApi.DTOs;
using System.Text.Json;
using System.Text.Json.Serialization;

public class PostFeedDtoJsonConverter : JsonConverter<FeedDto>
{
    public override FeedDto Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        using var doc = JsonDocument.ParseValue(ref reader);
        var discriminator = doc.RootElement.GetProperty("discriminator").GetString();

        FeedDto result = discriminator switch
        {
            "Image" => JsonSerializer.Deserialize<ImageFeedDto>(doc.RootElement.GetRawText(), options),
            "Video" => JsonSerializer.Deserialize<VideoFeedDto>(doc.RootElement.GetRawText(), options),
            _ => new FeedDto
            {
                Title = doc.RootElement.GetProperty("title").GetString(),
                Description = doc.RootElement.GetProperty("description").GetString(),
                Discriminator = discriminator
            }
        };

        return result;
    }

    public override void Write(Utf8JsonWriter writer, FeedDto value, JsonSerializerOptions options)
    {
        writer.WriteStartObject();
        writer.WriteString("Title", value.Title);
        writer.WriteString("Description", value.Description);
        writer.WriteString("Discriminator", value.Discriminator);
        writer.WriteEndObject();
    }
}