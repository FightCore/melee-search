using System.Data;
using MeleeSearch.Data.Context;
using MeleeSearch.Repositories.Caching;
using MeleeSearch.Repositories.Implementations;
using MeleeSearch.Repositories.Interfaces;
using MeleeSearch.Services.Implementations;
using MeleeSearch.Services.Interfaces;
using MeleeSearch.Services.Mappers;
using MeleeSearch.Services.Utilities;
using Microsoft.EntityFrameworkCore;
using Npgsql;

var builder = WebApplication.CreateBuilder(args);

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

builder.Services.AddDbContext<MeleeSearchDbContext>(options =>
    options.UseNpgsql(connectionString));

builder.Services.AddScoped<IDbConnection>(sp =>
{
    var connection = new NpgsqlConnection(connectionString);
    connection.Open();
    return connection;
});

builder.Services.AddCors(options => options.AddDefaultPolicy(policy => policy.AllowAnyHeader().AllowAnyMethod().AllowAnyOrigin()));

builder.Services.AddDistributedMemoryCache();
builder.Services.AddSingleton<ICacheService, CacheService>();

builder.Services.AddScoped<ISearchRepository, SearchRepository>();
builder.Services.AddScoped<IQueryPreprocessor, NormalizationPreprocessor>();
builder.Services.AddScoped<IQueryPreprocessor, CharacterMatcherPreprocessor>();
builder.Services.AddScoped<IQueryPreprocessor, AliasPreprocessor>();

builder.Services.AddSingleton<IStringMatcher, FuzzySharpMatcher>();
builder.Services.AddScoped<SearchScorer>();
builder.Services.AddScoped<ISearchService, SearchService>();

builder.Services.AddSingleton<ICardMapper, FrameDataCardMapper>();
builder.Services.AddSingleton<ICardMapper, CharacterAttributeCardMapper>();
builder.Services.AddSingleton<ICardMapper, DefaultCardMapper>();
builder.Services.AddSingleton<ICardMapperFactory, CardMapperFactory>();

builder.Services.AddControllers();
builder.Services.AddOpenApi();

var app = builder.Build();

app.UseCors();

using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<MeleeSearchDbContext>();
    await dbContext.Database.MigrateAsync();
}

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();