using Microsoft.Extensions.Options;
using MongoDB.Driver;
using N8nApiWork.Core.Entities;
using N8nApiWork.Core.Interfaces;

namespace N8nApiWork.Infrastructure.Repositories;

/// <summary>
/// 워크플로우 리포지토리 구현
/// </summary>
public class WorkflowRepository : IWorkflowRepository
{
    private readonly IMongoCollection<Workflow> _workflows;

    public WorkflowRepository(IMongoClient mongoClient, IOptions<MongoDbSettings> settings)
    {
        var database = mongoClient.GetDatabase(settings.Value.DatabaseName);
        _workflows = database.GetCollection<Workflow>("workflows");
    }

    public async Task<List<Workflow>> GetAllAsync()
    {
        return await _workflows.Find(_ => true).ToListAsync();
    }

    public async Task<Workflow?> GetByIdAsync(string id)
    {
        return await _workflows.Find(w => w.Id == id).FirstOrDefaultAsync();
    }

    public async Task CreateAsync(Workflow workflow)
    {
        workflow.CreatedAt = DateTime.UtcNow;
        workflow.UpdatedAt = DateTime.UtcNow;
        await _workflows.InsertOneAsync(workflow);
    }

    public async Task UpdateAsync(string id, Workflow workflow)
    {
        workflow.UpdatedAt = DateTime.UtcNow;
        await _workflows.ReplaceOneAsync(w => w.Id == id, workflow);
    }

    public async Task DeleteAsync(string id)
    {
        await _workflows.DeleteOneAsync(w => w.Id == id);
    }
}
