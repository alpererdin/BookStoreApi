using BookStoreApi.Models;
using BookStoreApi.Services;
using MongoDB.Driver;
using FluentValidation;  // ✅ Sadece bunu kullanıyoruz
using BookStoreApi.Models.DTOs.Validators;

var builder = WebApplication.CreateBuilder(args);

 
var dbSettings = builder.Configuration
    .GetSection("BookStoreDatabase")
    .Get<BookStoreDatabaseSettings>()
    ?? throw new InvalidOperationException("BookStoreDatabase config missing!");

builder.Services.AddSingleton<IMongoDatabase>(sp =>
{
    var client = new MongoClient(dbSettings.ConnectionString);
    return client.GetDatabase(dbSettings.DatabaseName);
});

builder.Services.AddSingleton(typeof(MongoDbService<>));

 
builder.Services.AddSingleton<BooksService>();
builder.Services.AddSingleton<AuthorsService>();

 
builder.Services.AddValidatorsFromAssemblyContaining<CreateBookRequestValidator>();
builder.Services.AddValidatorsFromAssemblyContaining<CreateAuthorRequestValidator>();
 
builder.Services.AddControllers()
    .AddJsonOptions(options =>
        options.JsonSerializerOptions.PropertyNamingPolicy = null);
 
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new() { Title = "BookStore API", Version = "v1" });
});

var app = builder.Build();
 
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "BookStore API v1"));
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();