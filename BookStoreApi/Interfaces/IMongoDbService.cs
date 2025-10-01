using BookStoreApi.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BookStoreApi.Interfaces;

public interface IMongoDbService<T> where T : IBaseEntity
{
    Task<List<T>> GetAsync();
    Task<T?> GetAsync(string id);
    Task CreateAsync(T newItem);
    Task UpdateAsync(string id, T updatedItem);
    Task RemoveAsync(string id);
}