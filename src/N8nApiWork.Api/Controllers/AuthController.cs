using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using N8nApiWork.Api.Models;
using N8nApiWork.Core.Interfaces;

namespace N8nApiWork.Api.Controllers;

/// <summary>
/// 인증 관련 API 컨트롤러
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;
    private readonly ILogger<AuthController> _logger;

    public AuthController(IAuthService authService, ILogger<AuthController> logger)
    {
        _authService = authService;
        _logger = logger;
    }

    /// <summary>
    /// 회원가입
    /// </summary>
    /// <param name="request">회원가입 요청 정보</param>
    /// <returns>생성된 사용자 정보 및 토큰</returns>
    /// <response code="200">회원가입 성공</response>
    /// <response code="400">잘못된 요청 또는 이미 존재하는 이메일</response>
    [HttpPost("register")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(AuthResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<AuthResponseDto>> Register([FromBody] RegisterRequestDto request)
    {
        try
        {
            _logger.LogInformation("회원가입 시도: {Email}", request.Email);

            var (user, token, expiresAt) = await _authService.RegisterAsync(
                request.Name,
                request.Email,
                request.Password
            );

            var response = new AuthResponseDto
            {
                UserId = user.Id!,
                Email = user.Email,
                Name = user.FullName ?? user.Username,
                Token = token,
                ExpiresAt = expiresAt
            };

            _logger.LogInformation("회원가입 성공: {UserId}", user.Id);
            return Ok(response);
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogWarning("회원가입 실패: {Message}", ex.Message);
            return BadRequest(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "회원가입 중 오류 발생");
            return StatusCode(500, new { message = "회원가입 처리 중 오류가 발생했습니다." });
        }
    }

    /// <summary>
    /// 로그인
    /// </summary>
    /// <param name="request">로그인 요청 정보</param>
    /// <returns>사용자 정보 및 토큰</returns>
    /// <response code="200">로그인 성공</response>
    /// <response code="401">인증 실패 (잘못된 이메일 또는 비밀번호)</response>
    [HttpPost("login")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(AuthResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<AuthResponseDto>> Login([FromBody] LoginRequestDto request)
    {
        try
        {
            _logger.LogInformation("로그인 시도: {Email}", request.Email);

            var result = await _authService.LoginAsync(request.Email, request.Password);

            if (result == null)
            {
                _logger.LogWarning("로그인 실패: 잘못된 자격 증명 - {Email}", request.Email);
                return Unauthorized(new { message = "이메일 또는 비밀번호가 올바르지 않습니다." });
            }

            var (user, token, expiresAt) = result.Value;

            var response = new AuthResponseDto
            {
                UserId = user.Id!,
                Email = user.Email,
                Name = user.FullName ?? user.Username,
                Token = token,
                ExpiresAt = expiresAt
            };

            _logger.LogInformation("로그인 성공: {UserId}", user.Id);
            return Ok(response);
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogWarning("로그인 실패: {Message}", ex.Message);
            return BadRequest(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "로그인 중 오류 발생");
            return StatusCode(500, new { message = "로그인 처리 중 오류가 발생했습니다." });
        }
    }

    /// <summary>
    /// 현재 사용자 정보 조회 (인증 필요)
    /// </summary>
    /// <returns>현재 로그인한 사용자 정보</returns>
    /// <response code="200">조회 성공</response>
    /// <response code="401">인증되지 않음</response>
    [HttpGet("me")]
    [Authorize]
    [ProducesResponseType(typeof(object), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public ActionResult GetCurrentUser()
    {
        var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
        var email = User.FindFirst(System.Security.Claims.ClaimTypes.Email)?.Value;
        var name = User.FindFirst(System.Security.Claims.ClaimTypes.Name)?.Value;
        var role = User.FindFirst(System.Security.Claims.ClaimTypes.Role)?.Value;

        return Ok(new
        {
            userId,
            email,
            name,
            role
        });
    }
}
