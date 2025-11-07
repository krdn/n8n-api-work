using Microsoft.AspNetCore.Mvc;
using N8nApiWork.Core.Entities;
using N8nApiWork.Core.Interfaces;

namespace N8nApiWork.Api.Controllers;

/// <summary>
/// 워크플로우 관리 API 컨트롤러
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class WorkflowsController : ControllerBase
{
    private readonly IWorkflowRepository _workflowRepository;
    private readonly ILogger<WorkflowsController> _logger;

    public WorkflowsController(IWorkflowRepository workflowRepository, ILogger<WorkflowsController> logger)
    {
        _workflowRepository = workflowRepository;
        _logger = logger;
    }

    /// <summary>
    /// 모든 워크플로우 조회
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<IEnumerable<Workflow>>> GetWorkflows()
    {
        try
        {
            var workflows = await _workflowRepository.GetAllAsync();
            return Ok(workflows);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "워크플로우 목록 조회 중 오류 발생");
            return StatusCode(500, "워크플로우 목록을 조회하는 중 오류가 발생했습니다.");
        }
    }

    /// <summary>
    /// 특정 워크플로우 조회
    /// </summary>
    [HttpGet("{id}")]
    public async Task<ActionResult<Workflow>> GetWorkflow(string id)
    {
        try
        {
            var workflow = await _workflowRepository.GetByIdAsync(id);
            if (workflow == null)
            {
                return NotFound($"ID가 '{id}'인 워크플로우를 찾을 수 없습니다.");
            }
            return Ok(workflow);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "워크플로우 조회 중 오류 발생 (ID: {WorkflowId})", id);
            return StatusCode(500, "워크플로우를 조회하는 중 오류가 발생했습니다.");
        }
    }

    /// <summary>
    /// 워크플로우 생성
    /// </summary>
    [HttpPost]
    public async Task<ActionResult<Workflow>> CreateWorkflow([FromBody] Workflow workflow)
    {
        try
        {
            await _workflowRepository.CreateAsync(workflow);
            return CreatedAtAction(nameof(GetWorkflow), new { id = workflow.Id }, workflow);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "워크플로우 생성 중 오류 발생");
            return StatusCode(500, "워크플로우를 생성하는 중 오류가 발생했습니다.");
        }
    }

    /// <summary>
    /// 워크플로우 수정
    /// </summary>
    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateWorkflow(string id, [FromBody] Workflow workflow)
    {
        try
        {
            var existing = await _workflowRepository.GetByIdAsync(id);
            if (existing == null)
            {
                return NotFound($"ID가 '{id}'인 워크플로우를 찾을 수 없습니다.");
            }

            await _workflowRepository.UpdateAsync(id, workflow);
            return NoContent();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "워크플로우 수정 중 오류 발생 (ID: {WorkflowId})", id);
            return StatusCode(500, "워크플로우를 수정하는 중 오류가 발생했습니다.");
        }
    }

    /// <summary>
    /// 워크플로우 삭제
    /// </summary>
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteWorkflow(string id)
    {
        try
        {
            var existing = await _workflowRepository.GetByIdAsync(id);
            if (existing == null)
            {
                return NotFound($"ID가 '{id}'인 워크플로우를 찾을 수 없습니다.");
            }

            await _workflowRepository.DeleteAsync(id);
            return NoContent();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "워크플로우 삭제 중 오류 발생 (ID: {WorkflowId})", id);
            return StatusCode(500, "워크플로우를 삭제하는 중 오류가 발생했습니다.");
        }
    }
}
