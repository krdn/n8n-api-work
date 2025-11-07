namespace N8nApiWork.Api.Models;

/// <summary>
/// 인증 응답 DTO
/// </summary>
public class AuthResponseDto
{
    /// <summary>
    /// 사용자 ID
    /// </summary>
    public string UserId { get; set; } = string.Empty;

    /// <summary>
    /// 사용자 이메일
    /// </summary>
    public string Email { get; set; } = string.Empty;

    /// <summary>
    /// 사용자 이름
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// 액세스 토큰
    /// </summary>
    public string Token { get; set; } = string.Empty;

    /// <summary>
    /// 토큰 만료 시간
    /// </summary>
    public DateTime ExpiresAt { get; set; }

    /// <summary>
    /// 리프레시 토큰 (선택적)
    /// </summary>
    public string? RefreshToken { get; set; }
}
