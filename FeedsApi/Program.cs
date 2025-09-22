using FeedsApi.Data.Converters;
using FeedsApi.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace FeedsApi
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddControllers()
                .AddJsonOptions(options =>
                {
                    options.JsonSerializerOptions.Converters.Add(new PostFeedDtoJsonConverter());
                    options.JsonSerializerOptions.Converters.Add(new FeedJsonConverter());
                });
            builder.Services.AddSwaggerGen();

            // Add DbContext with SQLite
            builder.Services.AddDbContext<FeedDbContext>(options =>
                options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));

            builder.Services.AddAuthorization();
            builder.Services.AddIdentityApiEndpoints<ApplicationUser>()
                .AddEntityFrameworkStores<FeedDbContext>();

            var app = builder.Build();

            // Configure the HTTP request pipeline.

            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();

            app.UseAuthorization();
            app.MapIdentityApi<ApplicationUser>();

            app.MapControllers();

            app.Run();
        }
    }
}
