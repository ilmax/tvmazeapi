using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Net.Http.Headers;
using Microsoft.OpenApi.Models;
using TvMaze.ApplicationServices;
using TvMaze.Data;
using TvMaze.Models;

namespace TvMaze;

class Program
{
    public static async Task Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);
        // Add services to the container.

        builder.Services.AddControllers().AddJsonOptions(opt => opt.JsonSerializerOptions.AddContext<ShowModelContext>());
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen(opt => opt.SwaggerDoc("v1", new OpenApiInfo { Title = "TvMaze Proxy Api", Version = "v1" }));

        builder.Services.AddSingleton(builder.Configuration.GetSection("tvmaze").Get<ScraperConfig>() ?? new ScraperConfig());
        builder.Services.AddSingleton(builder.Configuration.GetSection("pagination").Get<PaginationConfig>() ?? new PaginationConfig());
        builder.Services.AddDbContext<TvMazeDbContext>(opt =>
        {
            opt.UseModel(Data.OptimizedModel.TvMazeDbContextModel.Instance);
            opt.UseSqlServer(builder.Configuration.GetConnectionString("Database"));
        });
        builder.Services.AddTransient<IShowManager, ShowManager>();
        builder.Services.AddTransient<ScraperService>();
        builder.Services.AddHttpClient<ITvMazeHttpService,TvMazeHttpService>(opt =>
        {
            opt.BaseAddress = new Uri("https://api.tvmaze.com");
            opt.DefaultRequestHeaders.Add(HeaderNames.UserAgent, "tvmazeapiproxy");
        });
        builder.Services.AddHostedService<ApiScraperBackgroundService>();
        builder.Services.AddMediatR(typeof(Program));
        builder.Services.AddTransient<ShowProvider>();

        var app = builder.Build();

        // Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        app.UseHttpsRedirection();

        app.MapControllers();

        await app.RunAsync();
    }
}