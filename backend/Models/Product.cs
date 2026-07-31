using System;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson.Serialization.IdGenerators;

namespace backend.Models;

public class Product
{
    [BsonId(IdGenerator = typeof(StringObjectIdGenerator))]
    [BsonRepresentation(BsonType.String)]
    public string Id { get; set; } = null!;

    public string UserId { get; set; } = null!;
    public string Name { get; set; } = null!;
    public string Description { get; set; } = null!;
    public string Dimensions { get; set; } = null!;
    public double Price { get; set; }
    public int Quantity { get; set; }
    public double Weight { get; set; }

    public Product() { }
}