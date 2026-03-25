using System;
using backend.Models;
using MongoDB.Driver;

namespace backend.Service;

public interface IMongoDbService
{

    IMongoCollection<Product> Products { get; }
    IMongoCollection<User> Users { get; }
}