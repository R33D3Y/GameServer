using CommonModels.Hubs;
using CommonModels.Managers;
using CommonModels.Services;
using Microsoft.AspNetCore.ResponseCompression;

// Setup Json file
JsonManager.SetupJsonSettings();

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

string? gameLocation = JsonManager.GetPropertyValue("GameFolderLocation");
if (gameLocation is null)
{
    throw new ArgumentNullException(nameof(gameLocation));
}

builder.Services.AddSingleton(gameLocation);
builder.Services.AddSingleton<GameService>();
builder.Services.AddSignalR()
    .AddHubOptions<MessagingHub>(options =>
    {
        options.EnableDetailedErrors = true;
    });

builder.Services.AddResponseCompression(opts =>
{
    opts.MimeTypes = ResponseCompressionDefaults.MimeTypes.Concat(
          new[] { "application/octet-stream" });
});

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowBlazorApp",
        builder =>
        {
            builder.AllowAnyOrigin()
            .AllowAnyHeader()
            .AllowAnyMethod();
        });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
//if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseRouting();

app.UseCors("AllowBlazorApp");

app.UseAuthorization();
app.UseResponseCompression();

app.MapControllers();
app.MapHub<MessagingHub>("/chathub");

app.Run();