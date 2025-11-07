using MongoDB.Driver;
using N8nApiWork.Core.Interfaces;
using N8nApiWork.Infrastructure;
using N8nApiWork.Infrastructure.Repositories;

var builder = WebApplication.CreateBuilder(args);

// MongoDB 설정
builder.Services.Configure<MongoDbSettings>(
    builder.Configuration.GetSection("MongoDB"));

// MongoDB 클라이언트 등록 (싱글톤)
builder.Services.AddSingleton<IMongoClient>(sp =>
{
    var settings = builder.Configuration.GetSection("MongoDB").Get<MongoDbSettings>();
    return new MongoClient(settings?.ConnectionString ?? "mongodb://localhost:27017");
});

// 리포지토리 등록
builder.Services.AddScoped<IWorkflowRepository, WorkflowRepository>();

// 컨트롤러 추가
builder.Services.AddControllers();

// Swagger/OpenAPI 설정
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

// 컨트롤러 라우팅 사용
app.MapControllers();

app.Run();
