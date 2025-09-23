using BookStoreApi.Models;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using ZstdSharp.Unsafe;

namespace BookStoreApi.Services;

public class MongoDbService<T> where T : IBaseEntity
{
    private readonly IMongoCollection<T> _collection;

    public MongoDbService(IOptions<BookStoreDatabaseSettings> options)
    {

        var mongoClient = new MongoClient(
            options.Value.ConnectionString);

        var mongoDatabase = mongoClient.GetDatabase(
            options.Value.DatabaseName);

        var collectionName = typeof(T).Name + "s";

        _collection = mongoDatabase.GetCollection<T>(collectionName);   

      
    }
 
    public async Task<List<T>> GetAsync() =>
        await _collection.Find(_ => true).ToListAsync();

    public async Task<T?> GetAsync(string id) =>
        await _collection.Find(x => x.Id == id).FirstOrDefaultAsync();

    
    public async Task CreateAsync(T newItem) =>  
        await _collection.InsertOneAsync(newItem);

  
    public async Task UpdateAsync(string id, T updatedItem) =>  
        await _collection.ReplaceOneAsync(x => x.Id == id, updatedItem);

    
    public async Task RemoveAsync(string id) =>
        await _collection.DeleteOneAsync(x => x.Id == id);
}

