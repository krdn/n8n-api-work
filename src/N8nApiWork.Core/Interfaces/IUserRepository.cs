using N8nApiWork.Core.Entities;

namespace N8nApiWork.Core.Interfaces;

/// <summary>
/// 사용자 리포지토리 인터페이스
/// </summary>
public interface IUserRepository
{
    /// <summary>
    /// 모든 사용자 목록 조회
    /// </summary>
    /// <returns>사용자 목록</returns>
    Task<List<User>> GetAllAsync();

    /// <summary>
    /// ID로 사용자 조회
    /// </summary>
    /// <param name="id">사용자 ID</param>
    /// <returns>사용자 정보 또는 null</returns>
    Task<User?> GetByIdAsync(string id);

    /// <summary>
    /// 사용자명으로 사용자 조회
    /// </summary>
    /// <param name="username">사용자명</param>
    /// <returns>사용자 정보 또는 null</returns>
    Task<User?> GetByUsernameAsync(string username);

    /// <summary>
    /// 이메일로 사용자 조회
    /// </summary>
    /// <param name="email">이메일 주소</param>
    /// <returns>사용자 정보 또는 null</returns>
    Task<User?> GetByEmailAsync(string email);

    /// <summary>
    /// 사용자 생성
    /// </summary>
    /// <param name="user">사용자 정보</param>
    /// <returns>Task</returns>
    Task CreateAsync(User user);

    /// <summary>
    /// 사용자 정보 수정
    /// </summary>
    /// <param name="id">사용자 ID</param>
    /// <param name="user">수정할 사용자 정보</param>
    /// <returns>Task</returns>
    Task UpdateAsync(string id, User user);

    /// <summary>
    /// 사용자 삭제
    /// </summary>
    /// <param name="id">사용자 ID</param>
    /// <returns>Task</returns>
    Task DeleteAsync(string id);

    /// <summary>
    /// 사용자명 중복 확인
    /// </summary>
    /// <param name="username">사용자명</param>
    /// <returns>중복 여부 (true: 존재함, false: 사용 가능)</returns>
    Task<bool> UsernameExistsAsync(string username);

    /// <summary>
    /// 이메일 중복 확인
    /// </summary>
    /// <param name="email">이메일 주소</param>
    /// <returns>중복 여부 (true: 존재함, false: 사용 가능)</returns>
    Task<bool> EmailExistsAsync(string email);
}
