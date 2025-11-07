using System.ComponentModel.DataAnnotations;

namespace N8nApiWork.Api.Models;

/// <summary>
/// 회원가입 요청 DTO
/// </summary>
public class RegisterRequestDto
{
    /// <summary>
    /// 이름
    /// </summary>
    [Required(ErrorMessage = "이름은 필수입니다.")]
    [StringLength(50, MinimumLength = 2, ErrorMessage = "이름은 2-50자 사이여야 합니다.")]
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// 이메일
    /// </summary>
    [Required(ErrorMessage = "이메일은 필수입니다.")]
    [EmailAddress(ErrorMessage = "올바른 이메일 형식이 아닙니다.")]
    public string Email { get; set; } = string.Empty;

    /// <summary>
    /// 비밀번호
    /// </summary>
    [Required(ErrorMessage = "비밀번호는 필수입니다.")]
    [StringLength(100, MinimumLength = 6, ErrorMessage = "비밀번호는 6-100자 사이여야 합니다.")]
    public string Password { get; set; } = string.Empty;

    /// <summary>
    /// 비밀번호 확인
    /// </summary>
    [Required(ErrorMessage = "비밀번호 확인은 필수입니다.")]
    [Compare("Password", ErrorMessage = "비밀번호가 일치하지 않습니다.")]
    public string ConfirmPassword { get; set; } = string.Empty;
}
