using BookStoreApi.Attributes;
using BookStoreApi.Models;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using System.Reflection;
using ZstdSharp.Unsafe;

namespace BookStoreApi.Services;

public class MongoDbService<T> where T : IBaseEntity
{
    protected readonly IMongoCollection<T> _collection;

    public MongoDbService(IOptions<BookStoreDatabaseSettings> options)
    {

        var mongoClient = new MongoClient("mongodb://localhost:27017");
        var mongoDatabase = mongoClient.GetDatabase("BookStore");

        var collectionName = typeof(T)
                                .GetCustomAttribute<CollectionNameAttribute>()?
                                .Name;

        if (string.IsNullOrEmpty(collectionName))
        {
            throw new ArgumentException("CollectionName attribute not defined.");
        }

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

