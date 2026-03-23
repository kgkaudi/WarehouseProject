using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace backend.Models;

public class User
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string Id { get; set; } = null!;

    public string Username { get; set; } = null!;
    public string Email { get; set; } = null!;

    public string CompanyName { get; set; } = null!;
    public string CompanyAddress { get; set; } = null!;

    public byte[] PasswordHash { get; set; } = null!;
    public byte[] PasswordSalt { get; set; } = null!;

    public bool EmailConfirmed { get; set; } = false;

    public string? EmailVerificationToken { get; set; }
    public DateTime? EmailVerificationTokenExpires { get; set; }

    public string? PasswordResetToken { get; set; }
    public DateTime? PasswordResetTokenExpires { get; set; }
    
    public string Role { get; set; } = "user";
}
