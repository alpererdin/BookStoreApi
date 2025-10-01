
using BookStoreApi.Attributes;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System.Text.Json.Serialization;


namespace BookStoreApi.Models
{

    [CollectionName("Books")]
    public class Book : IBaseEntity
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? Id { get; set; }
        [BsonElement("Name")]
        [JsonPropertyName("Name")]
        public string BookName { get; set; } = null!;
        public decimal Price { get; set; }
        public string Category { get; set; } = null!;
        public required string AuthorId { get; set; }


    }
}
