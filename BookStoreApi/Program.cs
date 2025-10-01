using BookStoreApi.Models;
using BookStoreApi.Services;
using MongoDB.Driver;

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

builder.Services.AddControllers()
    .AddJsonOptions(options =>
        options.JsonSerializerOptions.PropertyNamingPolicy = null);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();
app.Run();