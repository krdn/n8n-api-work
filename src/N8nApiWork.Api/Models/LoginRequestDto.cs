using System.ComponentModel.DataAnnotations;

namespace N8nApiWork.Api.Models;

/// <summary>
/// 로그인 요청 DTO
/// </summary>
public class LoginRequestDto
{
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
    public string Password { get; set; } = string.Empty;
}
