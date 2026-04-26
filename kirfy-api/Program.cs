using System.Net.Http.Headers;
using kirfy_api.Application.Abstractions;
using kirfy_api.Infrastructure.Services.Genius;
using Microsoft.Extensions.Options;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenApi();
builder.Services.AddControllers();

builder.Services.Configure<GeniusApiOptions>(
    builder.Configuration.GetSection(GeniusApiOptions.SectionName));

builder.Services.AddHttpClient<IArtistAlbumsProvider, GeniusArtistAlbumsProvider>((serviceProvider, client) =>
{
    var options = serviceProvider.GetRequiredService<IOptions<GeniusApiOptions>>().Value;
    var baseUrl = string.IsNullOrWhiteSpace(options.BaseUrl) ? "https://api.genius.com" : options.BaseUrl;

    client.BaseAddress = new Uri(baseUrl);
    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

    if (!string.IsNullOrWhiteSpace(options.ClientAccessToken))
    {
        client.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", options.ClientAccessToken);
    }
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();
app.MapControllers();

app.Run();
