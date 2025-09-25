using FeedsApi.Data.Converters;
using FeedsApi.Data.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace FeedsApi
{
    public class Program
    {
        public static async Task Main(string[] args)
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
                options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection"))
                        .LogTo(Console.WriteLine, LogLevel.Information));

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

            // Apply migrations and create test user
            using (var scope = app.Services.CreateScope())
            {
                var serviceProvider = scope.ServiceProvider;
                await serviceProvider.GetRequiredService<FeedDbContext>().Database.MigrateAsync();
                await CreateTestUser(serviceProvider);
            }

            app.Run();
        }
    

    private static async Task CreateTestUser(IServiceProvider serviceProvider)
        {
            var userManager = serviceProvider.GetRequiredService<UserManager<ApplicationUser>>();
          
            var testUser = new ApplicationUser
            {
                UserName = "test",
                Email = "user@test.com"
            };

            string userPWD = "Almafa123!";
            var _user = await userManager.FindByEmailAsync("user@test.com");

            if (_user == null)
            {
                await userManager.CreateAsync(testUser, userPWD);
            }
        }
    }
}
