using System.ComponentModel.DataAnnotations;

namespace N8nApiWork.Api.Models;

/// <summary>
/// 사용자 수정 요청 DTO
/// </summary>
public class UpdateUserDto
{
    /// <summary>
    /// 이메일 주소
    /// </summary>
    [EmailAddress(ErrorMessage = "올바른 이메일 형식이 아닙니다.")]
    public string? Email { get; set; }

    /// <summary>
    /// 사용자 전체 이름
    /// </summary>
    [StringLength(100, ErrorMessage = "이름은 최대 100자까지 가능합니다.")]
    public string? FullName { get; set; }

    /// <summary>
    /// 사용자 역할
    /// </summary>
    [StringLength(50, ErrorMessage = "역할은 최대 50자까지 가능합니다.")]
    public string? Role { get; set; }

    /// <summary>
    /// 활성 상태 여부
    /// </summary>
    public bool? IsActive { get; set; }
}
