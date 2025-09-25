using FeedsApi.Data.Entities;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace FeedsApi.Data.Converters
{
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
                "Text" => new Feed
                {
                    FeedId = doc.RootElement.GetProperty("feedId").GetInt32(),
                    UserId = doc.RootElement.GetProperty("userId").GetInt32(),
                    Title = doc.RootElement.GetProperty("title").GetString(),
                    Description = doc.RootElement.GetProperty("description").GetString(),
                    Discriminator = discriminator
                },
                _ => throw new JsonException($"Unknown discriminator value: {discriminator}")
            };

            return result;
        }

        public override void Write(Utf8JsonWriter writer, Feed value, JsonSerializerOptions options)
        {
            switch (value)
            {
                case ImageFeed imageFeed:
                    JsonSerializer.Serialize(writer, imageFeed, options);
                    break;
                case VideoFeed videoFeed:
                    JsonSerializer.Serialize(writer, videoFeed, options);
                    break;
                case Feed feed:
                    writer.WriteStartObject();
                    writer.WriteNumber("FeedId", feed.FeedId);
                    writer.WriteNumber("UserId", feed.UserId);
                    writer.WriteString("Title", feed.Title);
                    writer.WriteString("Description", feed.Description);
                    writer.WriteString("Discriminator", feed.Discriminator);
                    writer.WriteEndObject();
                    break;
            }
        }
    }
}