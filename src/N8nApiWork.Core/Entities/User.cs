using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace N8nApiWork.Core.Entities;

/// <summary>
/// 사용자 엔티티
/// </summary>
public class User
{
    /// <summary>
    /// MongoDB ObjectId (Primary Key)
    /// </summary>
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string? Id { get; set; }

    /// <summary>
    /// 사용자명 (고유)
    /// </summary>
    [BsonElement("username")]
    public string Username { get; set; } = null!;

    /// <summary>
    /// 이메일 주소 (고유)
    /// </summary>
    [BsonElement("email")]
    public string Email { get; set; } = null!;

    /// <summary>
    /// 비밀번호 해시 (실제 환경에서는 해시된 값 저장)
    /// </summary>
    [BsonElement("passwordHash")]
    public string PasswordHash { get; set; } = null!;

    /// <summary>
    /// 사용자 전체 이름
    /// </summary>
    [BsonElement("fullName")]
    public string? FullName { get; set; }

    /// <summary>
    /// 사용자 역할 (예: Admin, User)
    /// </summary>
    [BsonElement("role")]
    public string Role { get; set; } = "User";

    /// <summary>
    /// 활성 상태 여부
    /// </summary>
    [BsonElement("isActive")]
    public bool IsActive { get; set; } = true;

    /// <summary>
    /// 생성 일시 (UTC)
    /// </summary>
    [BsonElement("createdAt")]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// 수정 일시 (UTC)
    /// </summary>
    [BsonElement("updatedAt")]
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// 마지막 로그인 일시 (UTC)
    /// </summary>
    [BsonElement("lastLoginAt")]
    public DateTime? LastLoginAt { get; set; }
}
