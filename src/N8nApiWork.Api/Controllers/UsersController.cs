using Microsoft.AspNetCore.Mvc;
using N8nApiWork.Api.Models;
using N8nApiWork.Core.Entities;
using N8nApiWork.Core.Interfaces;

namespace N8nApiWork.Api.Controllers;

/// <summary>
/// 사용자 관리 API 컨트롤러
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class UsersController : ControllerBase
{
    private readonly IUserRepository _userRepository;
    private readonly ILogger<UsersController> _logger;

    public UsersController(IUserRepository userRepository, ILogger<UsersController> logger)
    {
        _userRepository = userRepository;
        _logger = logger;
    }

    /// <summary>
    /// 모든 사용자 목록 조회
    /// </summary>
    /// <returns>사용자 목록</returns>
    /// <response code="200">성공</response>
    /// <response code="500">서버 오류</response>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<UserDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<IEnumerable<UserDto>>> GetUsers()
    {
        try
        {
            var users = await _userRepository.GetAllAsync();
            var userDtos = users.Select(MapToDto);
            return Ok(userDtos);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "사용자 목록 조회 중 오류 발생");
            return StatusCode(500, new { message = "사용자 목록 조회 중 오류가 발생했습니다." });
        }
    }

    /// <summary>
    /// ID로 사용자 조회
    /// </summary>
    /// <param name="id">사용자 ID</param>
    /// <returns>사용자 정보</returns>
    /// <response code="200">성공</response>
    /// <response code="404">사용자를 찾을 수 없음</response>
    /// <response code="500">서버 오류</response>
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(UserDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<UserDto>> GetUser(string id)
    {
        try
        {
            var user = await _userRepository.GetByIdAsync(id);

            if (user == null)
            {
                return NotFound(new { message = $"ID가 '{id}'인 사용자를 찾을 수 없습니다." });
            }

            return Ok(MapToDto(user));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "사용자 조회 중 오류 발생: {UserId}", id);
            return StatusCode(500, new { message = "사용자 조회 중 오류가 발생했습니다." });
        }
    }

    /// <summary>
    /// 새 사용자 생성
    /// </summary>
    /// <param name="createUserDto">사용자 생성 정보</param>
    /// <returns>생성된 사용자 정보</returns>
    /// <response code="201">생성 성공</response>
    /// <response code="400">잘못된 요청 (유효성 검증 실패 또는 중복)</response>
    /// <response code="500">서버 오류</response>
    [HttpPost]
    [ProducesResponseType(typeof(UserDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<UserDto>> CreateUser([FromBody] CreateUserDto createUserDto)
    {
        try
        {
            // 사용자명 중복 확인
            if (await _userRepository.UsernameExistsAsync(createUserDto.Username))
            {
                return BadRequest(new { message = $"사용자명 '{createUserDto.Username}'은(는) 이미 사용 중입니다." });
            }

            // 이메일 중복 확인
            if (await _userRepository.EmailExistsAsync(createUserDto.Email))
            {
                return BadRequest(new { message = $"이메일 '{createUserDto.Email}'은(는) 이미 사용 중입니다." });
            }

            // User 엔티티 생성
            var user = new User
            {
                Username = createUserDto.Username,
                Email = createUserDto.Email,
                PasswordHash = HashPassword(createUserDto.Password), // 실제로는 BCrypt 등 사용
                FullName = createUserDto.FullName,
                Role = createUserDto.Role,
                IsActive = true,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            await _userRepository.CreateAsync(user);

            _logger.LogInformation("새 사용자 생성 완료: {Username} (ID: {UserId})", user.Username, user.Id);

            var userDto = MapToDto(user);
            return CreatedAtAction(nameof(GetUser), new { id = user.Id }, userDto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "사용자 생성 중 오류 발생: {Username}", createUserDto.Username);
            return StatusCode(500, new { message = "사용자 생성 중 오류가 발생했습니다." });
        }
    }

    /// <summary>
    /// 사용자 정보 수정
    /// </summary>
    /// <param name="id">사용자 ID</param>
    /// <param name="updateUserDto">수정할 사용자 정보</param>
    /// <returns>수정 결과</returns>
    /// <response code="200">수정 성공</response>
    /// <response code="400">잘못된 요청</response>
    /// <response code="404">사용자를 찾을 수 없음</response>
    /// <response code="500">서버 오류</response>
    [HttpPut("{id}")]
    [ProducesResponseType(typeof(UserDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<UserDto>> UpdateUser(string id, [FromBody] UpdateUserDto updateUserDto)
    {
        try
        {
            var user = await _userRepository.GetByIdAsync(id);

            if (user == null)
            {
                return NotFound(new { message = $"ID가 '{id}'인 사용자를 찾을 수 없습니다." });
            }

            // 이메일 변경 시 중복 확인
            if (!string.IsNullOrEmpty(updateUserDto.Email) && updateUserDto.Email != user.Email)
            {
                if (await _userRepository.EmailExistsAsync(updateUserDto.Email))
                {
                    return BadRequest(new { message = $"이메일 '{updateUserDto.Email}'은(는) 이미 사용 중입니다." });
                }
                user.Email = updateUserDto.Email;
            }

            // 변경 가능한 필드만 업데이트
            if (!string.IsNullOrEmpty(updateUserDto.FullName))
                user.FullName = updateUserDto.FullName;

            if (!string.IsNullOrEmpty(updateUserDto.Role))
                user.Role = updateUserDto.Role;

            if (updateUserDto.IsActive.HasValue)
                user.IsActive = updateUserDto.IsActive.Value;

            user.UpdatedAt = DateTime.UtcNow;

            await _userRepository.UpdateAsync(id, user);

            _logger.LogInformation("사용자 정보 수정 완료: {Username} (ID: {UserId})", user.Username, id);

            return Ok(MapToDto(user));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "사용자 수정 중 오류 발생: {UserId}", id);
            return StatusCode(500, new { message = "사용자 수정 중 오류가 발생했습니다." });
        }
    }

    /// <summary>
    /// 사용자 삭제
    /// </summary>
    /// <param name="id">사용자 ID</param>
    /// <returns>삭제 결과</returns>
    /// <response code="204">삭제 성공</response>
    /// <response code="404">사용자를 찾을 수 없음</response>
    /// <response code="500">서버 오류</response>
    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> DeleteUser(string id)
    {
        try
        {
            var user = await _userRepository.GetByIdAsync(id);

            if (user == null)
            {
                return NotFound(new { message = $"ID가 '{id}'인 사용자를 찾을 수 없습니다." });
            }

            await _userRepository.DeleteAsync(id);

            _logger.LogInformation("사용자 삭제 완료: {Username} (ID: {UserId})", user.Username, id);

            return NoContent();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "사용자 삭제 중 오류 발생: {UserId}", id);
            return StatusCode(500, new { message = "사용자 삭제 중 오류가 발생했습니다." });
        }
    }

    /// <summary>
    /// 사용자명 중복 확인
    /// </summary>
    /// <param name="username">확인할 사용자명</param>
    /// <returns>중복 여부</returns>
    /// <response code="200">확인 성공</response>
    [HttpGet("check-username/{username}")]
    [ProducesResponseType(typeof(object), StatusCodes.Status200OK)]
    public async Task<ActionResult> CheckUsername(string username)
    {
        var exists = await _userRepository.UsernameExistsAsync(username);
        return Ok(new { exists, available = !exists });
    }

    /// <summary>
    /// 이메일 중복 확인
    /// </summary>
    /// <param name="email">확인할 이메일</param>
    /// <returns>중복 여부</returns>
    /// <response code="200">확인 성공</response>
    [HttpGet("check-email/{email}")]
    [ProducesResponseType(typeof(object), StatusCodes.Status200OK)]
    public async Task<ActionResult> CheckEmail(string email)
    {
        var exists = await _userRepository.EmailExistsAsync(email);
        return Ok(new { exists, available = !exists });
    }

    #region Helper Methods

    /// <summary>
    /// User 엔티티를 UserDto로 변환
    /// </summary>
    private static UserDto MapToDto(User user)
    {
        return new UserDto
        {
            Id = user.Id!,
            Username = user.Username,
            Email = user.Email,
            FullName = user.FullName,
            Role = user.Role,
            IsActive = user.IsActive,
            CreatedAt = user.CreatedAt,
            UpdatedAt = user.UpdatedAt,
            LastLoginAt = user.LastLoginAt
        };
    }

    /// <summary>
    /// 비밀번호 해싱 (단순 예제 - 실제로는 BCrypt 사용 권장)
    /// </summary>
    private static string HashPassword(string password)
    {
        // 실제 프로덕션에서는 BCrypt.Net-Next 등을 사용하세요
        // 예: BCrypt.Net.BCrypt.HashPassword(password)

        // 임시 구현 (보안상 취약 - 실제 사용 금지)
        using var sha256 = System.Security.Cryptography.SHA256.Create();
        var bytes = System.Text.Encoding.UTF8.GetBytes(password);
        var hash = sha256.ComputeHash(bytes);
        return Convert.ToBase64String(hash);
    }

    #endregion
}
