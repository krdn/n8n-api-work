namespace N8nApiWork.Core.Settings;

/// <summary>
/// JWT 토큰 설정
/// </summary>
public class JwtSettings
{
    /// <summary>
    /// JWT 비밀 키 (최소 256비트 권장)
    /// </summary>
    public string SecretKey { get; set; } = string.Empty;

    /// <summary>
    /// 토큰 발행자
    /// </summary>
    public string Issuer { get; set; } = string.Empty;

    /// <summary>
    /// 토큰 수신자
    /// </summary>
    public string Audience { get; set; } = string.Empty;

    /// <summary>
    /// 토큰 만료 시간 (분)
    /// </summary>
    public int ExpirationInMinutes { get; set; } = 60;

    /// <summary>
    /// 리프레시 토큰 만료 시간 (일)
    /// </summary>
    public int RefreshTokenExpirationInDays { get; set; } = 7;
}
