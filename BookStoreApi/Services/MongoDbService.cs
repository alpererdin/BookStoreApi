using BookStoreApi.Attributes;
using BookStoreApi.Models;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using System.Reflection;
using ZstdSharp.Unsafe;
using BookStoreApi.Interfaces;
namespace BookStoreApi.Services;

public class MongoDbService<T> where T : IBaseEntity
{
    protected readonly IMongoCollection<T> _collection;

    public MongoDbService(IMongoDatabase database)
    {
        var collectionName = typeof(T)
                                .GetCustomAttribute<CollectionNameAttribute>()?
                                .Name;

        if (string.IsNullOrEmpty(collectionName))
            throw new ArgumentException("CollectionName attribute not defined for this entity.");

        _collection = database.GetCollection<T>(collectionName);
    }

    public virtual async Task<List<T>> GetAsync() =>
        await _collection.Find(_ => true).ToListAsync();

    public virtual async Task<T?> GetAsync(string id) =>
        await _collection.Find(x => x.Id == id).FirstOrDefaultAsync();

    
    public virtual async Task CreateAsync(T newItem) =>  
        await _collection.InsertOneAsync(newItem);

  
    public virtual async Task UpdateAsync(string id, T updatedItem) =>  
        await _collection.ReplaceOneAsync(x => x.Id == id, updatedItem);


    public virtual async Task RemoveAsync(string id) =>
        await _collection.DeleteOneAsync(x => x.Id == id);
}

