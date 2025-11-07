using Microsoft.Extensions.Options;
using MongoDB.Driver;
using N8nApiWork.Core.Entities;
using N8nApiWork.Core.Interfaces;

namespace N8nApiWork.Infrastructure.Repositories;

/// <summary>
/// 사용자 리포지토리 구현
/// </summary>
public class UserRepository : IUserRepository
{
    private readonly IMongoCollection<User> _users;

    public UserRepository(IMongoClient mongoClient, IOptions<MongoDbSettings> settings)
    {
        var database = mongoClient.GetDatabase(settings.Value.DatabaseName);
        _users = database.GetCollection<User>("users");

        // 인덱스 생성 (사용자명, 이메일은 고유해야 함)
        CreateIndexes();
    }

    /// <summary>
    /// 필수 인덱스 생성
    /// </summary>
    private void CreateIndexes()
    {
        // 사용자명 고유 인덱스
        var usernameIndexKeys = Builders<User>.IndexKeys.Ascending(u => u.Username);
        var usernameIndexOptions = new CreateIndexOptions { Unique = true };
        _users.Indexes.CreateOneAsync(
            new CreateIndexModel<User>(usernameIndexKeys, usernameIndexOptions)
        );

        // 이메일 고유 인덱스
        var emailIndexKeys = Builders<User>.IndexKeys.Ascending(u => u.Email);
        var emailIndexOptions = new CreateIndexOptions { Unique = true };
        _users.Indexes.CreateOneAsync(
            new CreateIndexModel<User>(emailIndexKeys, emailIndexOptions)
        );

        // 생성일 인덱스 (조회 성능 향상)
        var createdAtIndexKeys = Builders<User>.IndexKeys.Descending(u => u.CreatedAt);
        _users.Indexes.CreateOneAsync(
            new CreateIndexModel<User>(createdAtIndexKeys)
        );
    }

    public async Task<List<User>> GetAllAsync()
    {
        return await _users.Find(_ => true).ToListAsync();
    }

    public async Task<User?> GetByIdAsync(string id)
    {
        return await _users.Find(u => u.Id == id).FirstOrDefaultAsync();
    }

    public async Task<User?> GetByUsernameAsync(string username)
    {
        return await _users.Find(u => u.Username == username).FirstOrDefaultAsync();
    }

    public async Task<User?> GetByEmailAsync(string email)
    {
        return await _users.Find(u => u.Email == email).FirstOrDefaultAsync();
    }

    public async Task CreateAsync(User user)
    {
        await _users.InsertOneAsync(user);
    }

    public async Task UpdateAsync(string id, User user)
    {
        user.UpdatedAt = DateTime.UtcNow;
        await _users.ReplaceOneAsync(u => u.Id == id, user);
    }

    public async Task DeleteAsync(string id)
    {
        await _users.DeleteOneAsync(u => u.Id == id);
    }

    public async Task<bool> UsernameExistsAsync(string username)
    {
        var count = await _users.CountDocumentsAsync(u => u.Username == username);
        return count > 0;
    }

    public async Task<bool> EmailExistsAsync(string email)
    {
        var count = await _users.CountDocumentsAsync(u => u.Email == email);
        return count > 0;
    }
}
