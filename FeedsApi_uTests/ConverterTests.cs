using FeedApi.Data.Entities;
using FeedsApi.Data.Converters;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace FeedsApi_uTests
{
    public class ConverterTests
    {
        private FeedJsonConverter _converter;
        private JsonSerializerOptions _options;

        [OneTimeSetUp]
        public void Setup()
        {
            _converter = new FeedJsonConverter();
            _options = new JsonSerializerOptions
            {
                UnmappedMemberHandling = JsonUnmappedMemberHandling.Disallow,
                PropertyNameCaseInsensitive = true
            };
        }

        [Test]
        public void ValidJsonString_ConvertedCorrectlyToFeedObject()
        {
            var json = @"{""feedId"":1,""userId"":1,""title"":""Text Feed"",""description"":""This is a text feed."",""discriminator"":""Text""}";
            using var ms = new MemoryStream(Encoding.UTF8.GetBytes(json));
            var reader = new Utf8JsonReader(ms.ToArray());
            reader.Read();
            var result = _converter.Read(ref reader, typeof(Feed), _options);
           
            Assert.That(result, Is.TypeOf<Feed>());
            var feed = (Feed)result;
            Assert.Multiple(() =>
            {
                Assert.That(feed.FeedId, Is.EqualTo(1));
                Assert.That(feed.UserId, Is.EqualTo(1));
                Assert.That(feed.Title, Is.EqualTo("Text Feed"));
                Assert.That(feed.Description, Is.EqualTo("This is a text feed."));
                Assert.That(feed.Discriminator, Is.EqualTo("Text"));
            });
        }

        [Test]
        public void InvalidDiscriminator_ThrowsJsonException()
        {
            var json = @"{""feedId"":1,""userId"":1,""title"":""Text Feed"",""description"":""This is a text feed."",""discriminator"":""adasdadad""}";
            using var ms = new MemoryStream(Encoding.UTF8.GetBytes(json));
           
            Assert.Throws<JsonException>(() => {
                var reader = new Utf8JsonReader(ms.ToArray());
                reader.Read();
                _converter.Read(ref reader, typeof(Feed), _options);
            });
        }

        [Test]
        public void DiscriminatorDoesntMatchValue_ThrowsJsonException()
        {
            var json = @"{""feedId"":1,""userId"":1,""title"":""Text Feed"",""description"":""This is a feed."",""discriminator"":""Video"", ""imageData"":""AQID""}";
            using var ms = new MemoryStream(Encoding.UTF8.GetBytes(json));

            Assert.Throws<JsonException>(() => {
                var reader = new Utf8JsonReader(ms.ToArray());
                reader.Read();
                _converter.Read(ref reader, typeof(Feed), _options);
            });
        }

        [Test]
        public void FeedObject_ConvertedCorrectlyToJson()
        {
            var feed = new Feed()
            {
                FeedId = 1,
                UserId = 1,
                Title = "Text Feed",
                Description = "This is a text feed.",
                Discriminator = "Text"
            };
            using var ms = new MemoryStream();
            using var writer = new Utf8JsonWriter(ms);
            _converter.Write(writer, feed, _options);
            writer.Flush();
            var result = Encoding.UTF8.GetString(ms.ToArray());
           
            Assert.That(result, Is.EqualTo(@"{""FeedId"":1,""UserId"":1,""Title"":""Text Feed"",""Description"":""This is a text feed."",""Discriminator"":""Text""}"));
        }

        [Test]
        public void ImageFeedObject_ConvertedCorrectlyToJson()
        {
            var feed = new ImageFeed()
            {
                FeedId = 1,
                UserId = 1,
                Title = "Image Feed",
                Description = "This is an image feed.",
                Discriminator = "Image",
                ImageData = new byte[] { 1, 2, 3 }
            };
            using var ms = new MemoryStream();
            using var writer = new Utf8JsonWriter(ms);
            _converter.Write(writer, feed, _options);
            writer.Flush();
            var result = Encoding.UTF8.GetString(ms.ToArray());
           
            Assert.That(result, Is.EqualTo(@"{""ImageData"":""AQID"",""FeedId"":1,""UserId"":1,""Title"":""Image Feed"",""Description"":""This is an image feed."",""Discriminator"":""Image""}"));
        }

        [Test]
        public void VideoFeedObject_ConvertedCorrectlyToJson()
        {
            var feed = new VideoFeed()
            {
                FeedId = 1,
                UserId = 1,
                Title = "Video Feed",
                Description = "This is a video feed.",
                Discriminator = "Video",
                Url = "https://example.com/video.mp4"
            };
            using var ms = new MemoryStream();
            using var writer = new Utf8JsonWriter(ms);
            _converter.Write(writer, feed, _options);
            writer.Flush();
            var result = Encoding.UTF8.GetString(ms.ToArray());
           
            Assert.That(result, Is.EqualTo(@"{""Url"":""https://example.com/video.mp4"",""FeedId"":1,""UserId"":1,""Title"":""Video Feed"",""Description"":""This is a video feed."",""Discriminator"":""Video""}"));
        }
    }
}
