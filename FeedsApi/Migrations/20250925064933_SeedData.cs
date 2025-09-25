using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace FeedsApi.Migrations
{
    /// <inheritdoc />
    public partial class SeedData : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Comments",
                columns: new[] { "Id", "FeedId", "Text", "Timestamp", "UserId" },
                values: new object[,]
                {
                    { 1, 1, "Wow!", new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 1 },
                    { 2, 3, "Such comment.", new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 1 }
                });

            migrationBuilder.InsertData(
                table: "Feeds",
                columns: new[] { "FeedId", "Description", "Discriminator", "Title", "UserId" },
                values: new object[] { 1, "This is a text feed.", "Text", "Text Feed", 1 });

            migrationBuilder.InsertData(
                table: "Feeds",
                columns: new[] { "FeedId", "Description", "Discriminator", "ImageData", "Title", "UserId" },
                values: new object[] { 2, "This is an image feed.", "Image", new byte[] { 1, 2, 3 }, "Image Feed", 2 });

            migrationBuilder.InsertData(
                table: "Feeds",
                columns: new[] { "FeedId", "Description", "Discriminator", "Title", "Url", "UserId" },
                values: new object[] { 3, "This is a video feed.", "Video", "Video Feed", "https://example.com/video.mp4", 3 });

            migrationBuilder.InsertData(
                table: "Likes",
                columns: new[] { "FeedId", "UserId" },
                values: new object[,]
                {
                    { 1, 1 },
                    { 2, 1 }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Comments",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Comments",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Feeds",
                keyColumn: "FeedId",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Feeds",
                keyColumn: "FeedId",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Feeds",
                keyColumn: "FeedId",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "Likes",
                keyColumns: new[] { "FeedId", "UserId" },
                keyValues: new object[] { 1, 1 });

            migrationBuilder.DeleteData(
                table: "Likes",
                keyColumns: new[] { "FeedId", "UserId" },
                keyValues: new object[] { 2, 1 });
        }
    }
}
