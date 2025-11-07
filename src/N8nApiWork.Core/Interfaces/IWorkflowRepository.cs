using N8nApiWork.Core.Entities;

namespace N8nApiWork.Core.Interfaces;

/// <summary>
/// 워크플로우 리포지토리 인터페이스
/// </summary>
public interface IWorkflowRepository
{
    /// <summary>
    /// 모든 워크플로우 조회
    /// </summary>
    Task<List<Workflow>> GetAllAsync();

    /// <summary>
    /// ID로 워크플로우 조회
    /// </summary>
    Task<Workflow?> GetByIdAsync(string id);

    /// <summary>
    /// 워크플로우 생성
    /// </summary>
    Task CreateAsync(Workflow workflow);

    /// <summary>
    /// 워크플로우 수정
    /// </summary>
    Task UpdateAsync(string id, Workflow workflow);

    /// <summary>
    /// 워크플로우 삭제
    /// </summary>
    Task DeleteAsync(string id);
}
