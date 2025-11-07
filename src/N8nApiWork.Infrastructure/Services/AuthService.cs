using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using BCrypt.Net;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using N8nApiWork.Core.Entities;
using N8nApiWork.Core.Interfaces;
using N8nApiWork.Core.Settings;

namespace N8nApiWork.Infrastructure.Services;

/// <summary>
/// 인증 서비스 구현
/// </summary>
public class AuthService : IAuthService
{
    private readonly IUserRepository _userRepository;
    private readonly JwtSettings _jwtSettings;

    public AuthService(IUserRepository userRepository, IOptions<JwtSettings> jwtSettings)
    {
        _userRepository = userRepository;
        _jwtSettings = jwtSettings.Value;
    }

    /// <summary>
    /// 사용자 등록
    /// </summary>
    public async Task<(User User, string Token, DateTime ExpiresAt)> RegisterAsync(string name, string email, string password)
    {
        // 이메일 중복 확인
        var existingUser = await _userRepository.GetByEmailAsync(email);
        if (existingUser != null)
        {
            throw new InvalidOperationException("이미 등록된 이메일입니다.");
        }

        // 비밀번호 해시
        var passwordHash = HashPassword(password);

        // 사용자 생성
        var user = new User
        {
            Username = email.Split('@')[0], // 이메일 @ 앞부분을 username으로 사용
            Email = email,
            PasswordHash = passwordHash,
            FullName = name,
            Role = "User",
            IsActive = true,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        await _userRepository.CreateAsync(user);

        // JWT 토큰 생성
        var (token, expiresAt) = GenerateJwtToken(user);

        return (user, token, expiresAt);
    }

    /// <summary>
    /// 사용자 로그인
    /// </summary>
    public async Task<(User User, string Token, DateTime ExpiresAt)?> LoginAsync(string email, string password)
    {
        // 이메일로 사용자 조회
        var user = await _userRepository.GetByEmailAsync(email);
        if (user == null)
        {
            return null;
        }

        // 비밀번호 확인
        if (!VerifyPassword(password, user.PasswordHash))
        {
            return null;
        }

        // 활성 상태 확인
        if (!user.IsActive)
        {
            throw new InvalidOperationException("비활성화된 계정입니다.");
        }

        // 마지막 로그인 시간 업데이트
        user.LastLoginAt = DateTime.UtcNow;
        await _userRepository.UpdateAsync(user.Id!, user);

        // JWT 토큰 생성
        var (token, expiresAt) = GenerateJwtToken(user);

        return (user, token, expiresAt);
    }

    /// <summary>
    /// 비밀번호 해시 생성
    /// </summary>
    public string HashPassword(string password)
    {
        return BCrypt.Net.BCrypt.HashPassword(password, BCrypt.Net.BCrypt.GenerateSalt(12));
    }

    /// <summary>
    /// 비밀번호 검증
    /// </summary>
    public bool VerifyPassword(string password, string hashedPassword)
    {
        try
        {
            return BCrypt.Net.BCrypt.Verify(password, hashedPassword);
        }
        catch
        {
            return false;
        }
    }

    /// <summary>
    /// JWT 토큰 생성
    /// </summary>
    public (string Token, DateTime ExpiresAt) GenerateJwtToken(User user)
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        var key = Encoding.UTF8.GetBytes(_jwtSettings.SecretKey);

        var expiresAt = DateTime.UtcNow.AddMinutes(_jwtSettings.ExpirationInMinutes);

        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id!),
            new Claim(ClaimTypes.Name, user.Username),
            new Claim(ClaimTypes.Email, user.Email),
            new Claim(ClaimTypes.Role, user.Role),
            new Claim(JwtRegisteredClaimNames.Sub, user.Id!),
            new Claim(JwtRegisteredClaimNames.Email, user.Email),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(claims),
            Expires = expiresAt,
            Issuer = _jwtSettings.Issuer,
            Audience = _jwtSettings.Audience,
            SigningCredentials = new SigningCredentials(
                new SymmetricSecurityKey(key),
                SecurityAlgorithms.HmacSha256Signature)
        };

        var token = tokenHandler.CreateToken(tokenDescriptor);
        var tokenString = tokenHandler.WriteToken(token);

        return (tokenString, expiresAt);
    }
}
