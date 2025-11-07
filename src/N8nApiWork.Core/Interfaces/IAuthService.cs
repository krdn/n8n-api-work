using N8nApiWork.Core.Entities;

namespace N8nApiWork.Core.Interfaces;

/// <summary>
/// 인증 서비스 인터페이스
/// </summary>
public interface IAuthService
{
    /// <summary>
    /// 사용자 등록
    /// </summary>
    /// <param name="name">이름</param>
    /// <param name="email">이메일</param>
    /// <param name="password">비밀번호</param>
    /// <returns>생성된 사용자 정보와 토큰</returns>
    Task<(User User, string Token, DateTime ExpiresAt)> RegisterAsync(string name, string email, string password);

    /// <summary>
    /// 사용자 로그인
    /// </summary>
    /// <param name="email">이메일</param>
    /// <param name="password">비밀번호</param>
    /// <returns>사용자 정보와 토큰 (인증 실패 시 null)</returns>
    Task<(User User, string Token, DateTime ExpiresAt)?> LoginAsync(string email, string password);

    /// <summary>
    /// 비밀번호 해시 생성
    /// </summary>
    /// <param name="password">평문 비밀번호</param>
    /// <returns>해시된 비밀번호</returns>
    string HashPassword(string password);

    /// <summary>
    /// 비밀번호 검증
    /// </summary>
    /// <param name="password">평문 비밀번호</param>
    /// <param name="hashedPassword">해시된 비밀번호</param>
    /// <returns>일치 여부</returns>
    bool VerifyPassword(string password, string hashedPassword);

    /// <summary>
    /// JWT 토큰 생성
    /// </summary>
    /// <param name="user">사용자 정보</param>
    /// <returns>토큰과 만료 시간</returns>
    (string Token, DateTime ExpiresAt) GenerateJwtToken(User user);
}
