using System.ComponentModel.DataAnnotations;

namespace N8nApiWork.Api.Models;

/// <summary>
/// 사용자 생성 요청 DTO
/// </summary>
public class CreateUserDto
{
    /// <summary>
    /// 사용자명 (3-30자, 영문/숫자만 가능)
    /// </summary>
    [Required(ErrorMessage = "사용자명은 필수입니다.")]
    [StringLength(30, MinimumLength = 3, ErrorMessage = "사용자명은 3-30자 사이여야 합니다.")]
    [RegularExpression(@"^[a-zA-Z0-9_]+$", ErrorMessage = "사용자명은 영문, 숫자, 언더스코어만 사용할 수 있습니다.")]
    public string Username { get; set; } = null!;

    /// <summary>
    /// 이메일 주소
    /// </summary>
    [Required(ErrorMessage = "이메일은 필수입니다.")]
    [EmailAddress(ErrorMessage = "올바른 이메일 형식이 아닙니다.")]
    public string Email { get; set; } = null!;

    /// <summary>
    /// 비밀번호 (최소 8자)
    /// </summary>
    [Required(ErrorMessage = "비밀번호는 필수입니다.")]
    [StringLength(100, MinimumLength = 8, ErrorMessage = "비밀번호는 최소 8자 이상이어야 합니다.")]
    public string Password { get; set; } = null!;

    /// <summary>
    /// 사용자 전체 이름 (선택)
    /// </summary>
    [StringLength(100, ErrorMessage = "이름은 최대 100자까지 가능합니다.")]
    public string? FullName { get; set; }

    /// <summary>
    /// 사용자 역할 (기본값: User)
    /// </summary>
    [StringLength(50, ErrorMessage = "역할은 최대 50자까지 가능합니다.")]
    public string Role { get; set; } = "User";
}
