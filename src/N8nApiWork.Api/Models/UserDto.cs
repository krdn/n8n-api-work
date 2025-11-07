namespace N8nApiWork.Api.Models;

/// <summary>
/// 사용자 응답 DTO (비밀번호 제외)
/// </summary>
public class UserDto
{
    /// <summary>
    /// 사용자 ID
    /// </summary>
    public string Id { get; set; } = null!;

    /// <summary>
    /// 사용자명
    /// </summary>
    public string Username { get; set; } = null!;

    /// <summary>
    /// 이메일 주소
    /// </summary>
    public string Email { get; set; } = null!;

    /// <summary>
    /// 사용자 전체 이름
    /// </summary>
    public string? FullName { get; set; }

    /// <summary>
    /// 사용자 역할
    /// </summary>
    public string Role { get; set; } = null!;

    /// <summary>
    /// 활성 상태 여부
    /// </summary>
    public bool IsActive { get; set; }

    /// <summary>
    /// 생성 일시 (UTC)
    /// </summary>
    public DateTime CreatedAt { get; set; }

    /// <summary>
    /// 수정 일시 (UTC)
    /// </summary>
    public DateTime UpdatedAt { get; set; }

    /// <summary>
    /// 마지막 로그인 일시 (UTC)
    /// </summary>
    public DateTime? LastLoginAt { get; set; }
}
