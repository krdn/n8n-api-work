# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Language Preference

**중요: 모든 상호작용은 한국어(한글)로 응답하세요**

---

## 프로젝트 개요

n8n 워크플로우 자동화와 관련된 API 작업 모음 저장소입니다. ASP.NET Core 기반 RESTful API를 제공하며, MongoDB를 데이터베이스로 사용합니다.

### 기술 스택

- **백엔드**: ASP.NET Core (Web API)
- **데이터베이스**: MongoDB (Docker 컨테이너로 실행)
- **언어**: C# (.NET)
- **ODM**: MongoDB.Driver
- **인증**: JWT (예정)
- **API 문서**: Swagger/OpenAPI

---

## 개발 환경 설정

### 필수 요구사항

- **.NET SDK**: .NET 6.0 이상 (또는 .NET 8.0)
- **Docker**: MongoDB 컨테이너 실행용
- **IDE**: Visual Studio 2022, VS Code, 또는 Rider

### 초기 설정

```bash
# 저장소 클론
git clone git@github.com:krdn/n8n-api-work.git
cd n8n-api-work

# .NET 버전 확인
dotnet --version

# MongoDB Docker 컨테이너 확인 (시스템에 이미 설치됨)
docker ps | grep mongo

# 프로젝트 복원 (프로젝트 생성 후)
dotnet restore

# 빌드
dotnet build

# 실행
dotnet run --project src/N8nApiWork/N8nApiWork.csproj
```

### MongoDB 연결 설정

시스템에 Docker로 MongoDB가 실행 중입니다. 연결 정보는 `appsettings.json` 또는 `appsettings.Development.json`에 설정:

```json
{
  "MongoDB": {
    "ConnectionString": "mongodb://localhost:27017",
    "DatabaseName": "n8n_api_work",
    "CollectionName": "workflows"
  }
}
```

**중요**: 프로덕션 환경의 MongoDB 연결 정보는 환경 변수나 User Secrets로 관리

### 환경 변수 관리

#### Development (User Secrets)
```bash
# User Secrets 초기화
dotnet user-secrets init

# MongoDB 연결 문자열 설정
dotnet user-secrets set "MongoDB:ConnectionString" "mongodb://username:password@localhost:27017"
dotnet user-secrets set "MongoDB:DatabaseName" "n8n_api_work"
```

#### Production (환경 변수)
```bash
export MongoDB__ConnectionString="mongodb://username:password@host:27017"
export MongoDB__DatabaseName="n8n_api_work"
```

---

## Git 워크플로우

### 브랜치 전략

```bash
# 현재 브랜치 확인
git branch

# 새 기능 브랜치 생성
git checkout -b feature/기능명

# 작업 후 커밋
git add .
git commit -m "feat: 기능 설명"

# 원격 저장소에 푸시
git push origin feature/기능명
```

### 커밋 메시지 규칙

다음 컨벤션을 따르는 것을 권장합니다:
- `feat:` - 새로운 기능 추가
- `fix:` - 버그 수정
- `docs:` - 문서 수정
- `refactor:` - 코드 리팩토링
- `test:` - 테스트 코드 추가/수정
- `chore:` - 빌드 프로세스, 도구 설정 등

---

## 프로젝트 구조

ASP.NET Core Clean Architecture 패턴을 따르는 권장 구조:

```
n8n-api-work/
├── src/
│   ├── N8nApiWork.Api/              # Web API 프로젝트 (컨트롤러, Startup)
│   │   ├── Controllers/             # API 엔드포인트
│   │   ├── Middleware/              # 커스텀 미들웨어
│   │   ├── Program.cs               # 애플리케이션 진입점
│   │   ├── appsettings.json         # 설정 파일
│   │   └── appsettings.Development.json
│   │
│   ├── N8nApiWork.Core/             # 비즈니스 로직 (Domain Layer)
│   │   ├── Entities/                # 도메인 엔티티/모델
│   │   ├── Interfaces/              # 리포지토리/서비스 인터페이스
│   │   └── Services/                # 비즈니스 로직 서비스
│   │
│   └── N8nApiWork.Infrastructure/   # 데이터 접근 (Infrastructure Layer)
│       ├── Data/                    # MongoDB 컨텍스트
│       ├── Repositories/            # 리포지토리 구현
│       └── MongoDbSettings.cs       # MongoDB 설정 모델
│
├── tests/
│   ├── N8nApiWork.Api.Tests/        # API 통합 테스트
│   ├── N8nApiWork.Core.Tests/       # 단위 테스트
│   └── N8nApiWork.Infrastructure.Tests/
│
├── docs/                            # API 문서
├── .gitignore
├── N8nApiWork.sln                   # 솔루션 파일
└── README.md
```

### 레이어 설명

- **Api**: HTTP 요청/응답 처리, 의존성 주입 설정, 미들웨어
- **Core**: 비즈니스 규칙, 도메인 모델, 인터페이스 정의 (의존성 없음)
- **Infrastructure**: MongoDB 연결, 리포지토리 구현, 외부 서비스 통합

---

## 개발 명령어

### 프로젝트 생성 (최초 1회)

```bash
# 솔루션 생성
dotnet new sln -n N8nApiWork

# API 프로젝트 생성
dotnet new webapi -n N8nApiWork.Api -o src/N8nApiWork.Api

# Core 클래스 라이브러리 생성
dotnet new classlib -n N8nApiWork.Core -o src/N8nApiWork.Core

# Infrastructure 클래스 라이브러리 생성
dotnet new classlib -n N8nApiWork.Infrastructure -o src/N8nApiWork.Infrastructure

# 테스트 프로젝트 생성
dotnet new xunit -n N8nApiWork.Api.Tests -o tests/N8nApiWork.Api.Tests
dotnet new xunit -n N8nApiWork.Core.Tests -o tests/N8nApiWork.Core.Tests

# 솔루션에 프로젝트 추가
dotnet sln add src/N8nApiWork.Api/N8nApiWork.Api.csproj
dotnet sln add src/N8nApiWork.Core/N8nApiWork.Core.csproj
dotnet sln add src/N8nApiWork.Infrastructure/N8nApiWork.Infrastructure.csproj
dotnet sln add tests/N8nApiWork.Api.Tests/N8nApiWork.Api.Tests.csproj
dotnet sln add tests/N8nApiWork.Core.Tests/N8nApiWork.Core.Tests.csproj

# 프로젝트 참조 설정
dotnet add src/N8nApiWork.Api/N8nApiWork.Api.csproj reference src/N8nApiWork.Core/N8nApiWork.Core.csproj
dotnet add src/N8nApiWork.Api/N8nApiWork.Api.csproj reference src/N8nApiWork.Infrastructure/N8nApiWork.Infrastructure.csproj
dotnet add src/N8nApiWork.Infrastructure/N8nApiWork.Infrastructure.csproj reference src/N8nApiWork.Core/N8nApiWork.Core.csproj
```

### NuGet 패키지 설치

```bash
# MongoDB Driver
dotnet add src/N8nApiWork.Infrastructure/N8nApiWork.Infrastructure.csproj package MongoDB.Driver

# Swagger (API 문서)
dotnet add src/N8nApiWork.Api/N8nApiWork.Api.csproj package Swashbuckle.AspNetCore

# 테스트 라이브러리
dotnet add tests/N8nApiWork.Api.Tests/N8nApiWork.Api.Tests.csproj package Microsoft.AspNetCore.Mvc.Testing
dotnet add tests/N8nApiWork.Core.Tests/N8nApiWork.Core.Tests.csproj package Moq
dotnet add tests/N8nApiWork.Core.Tests/N8nApiWork.Core.Tests.csproj package FluentAssertions
```

### 빌드 및 실행

```bash
# 전체 솔루션 복원
dotnet restore

# 전체 빌드
dotnet build

# 릴리스 빌드
dotnet build --configuration Release

# API 실행 (Development)
dotnet run --project src/N8nApiWork.Api/N8nApiWork.Api.csproj

# 특정 포트로 실행
dotnet run --project src/N8nApiWork.Api/N8nApiWork.Api.csproj --urls "http://localhost:5000"

# Watch 모드 (코드 변경 시 자동 재시작)
dotnet watch --project src/N8nApiWork.Api/N8nApiWork.Api.csproj run
```

### 테스트

```bash
# 모든 테스트 실행
dotnet test

# 특정 프로젝트 테스트
dotnet test tests/N8nApiWork.Core.Tests/N8nApiWork.Core.Tests.csproj

# 커버리지와 함께 테스트
dotnet test /p:CollectCoverage=true /p:CoverletOutputFormat=opencover

# 특정 테스트만 실행
dotnet test --filter "FullyQualifiedName~WorkflowController"

# Verbose 출력
dotnet test --verbosity detailed
```

### 코드 품질

```bash
# 포맷 검사
dotnet format --verify-no-changes

# 포맷 자동 적용
dotnet format

# 사용하지 않는 참조 제거
dotnet remove package [package-name]
```

### 배포

```bash
# 게시 (Publish)
dotnet publish src/N8nApiWork.Api/N8nApiWork.Api.csproj -c Release -o ./publish

# Docker 이미지 빌드 (Dockerfile 생성 후)
docker build -t n8n-api-work:latest .

# Docker 컨테이너 실행
docker run -d -p 5000:80 --name n8n-api n8n-api-work:latest
```

---

## n8n 통합 참고사항

이 프로젝트는 n8n 워크플로우 자동화 플랫폼과 통합될 예정입니다. n8n 관련 작업 시 참고사항:

### n8n API 엔드포인트

n8n 인스턴스의 일반적인 API 엔드포인트:
- 워크플로우 관리: `/api/v1/workflows`
- 실행: `/api/v1/executions`
- 크레덴셜: `/api/v1/credentials`
- 웹훅: `/webhook/`

### 인증

n8n API 접근 시 다음 중 하나의 인증 방법 사용:
- API 키 (헤더: `X-N8N-API-KEY`)
- Basic Auth
- 토큰 기반 인증

---

## MongoDB 연동 가이드

### MongoDB 설정 클래스

`Infrastructure/MongoDbSettings.cs`:
```csharp
public class MongoDbSettings
{
    public string ConnectionString { get; set; } = null!;
    public string DatabaseName { get; set; } = null!;
}
```

### 의존성 주입 설정

`Api/Program.cs`:
```csharp
// MongoDB 설정
builder.Services.Configure<MongoDbSettings>(
    builder.Configuration.GetSection("MongoDB"));

// MongoDB 클라이언트 싱글톤 등록
builder.Services.AddSingleton<IMongoClient>(s =>
{
    var settings = s.GetRequiredService<IOptions<MongoDbSettings>>().Value;
    return new MongoClient(settings.ConnectionString);
});

// 리포지토리 등록
builder.Services.AddScoped<IWorkflowRepository, WorkflowRepository>();
```

### 리포지토리 패턴 예제

`Infrastructure/Repositories/WorkflowRepository.cs`:
```csharp
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
        await _workflows.InsertOneAsync(workflow);
    }

    public async Task UpdateAsync(string id, Workflow workflow)
    {
        await _workflows.ReplaceOneAsync(w => w.Id == id, workflow);
    }

    public async Task DeleteAsync(string id)
    {
        await _workflows.DeleteOneAsync(w => w.Id == id);
    }
}
```

### 엔티티 예제

`Core/Entities/Workflow.cs`:
```csharp
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

public class Workflow
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string? Id { get; set; }

    [BsonElement("name")]
    public string Name { get; set; } = null!;

    [BsonElement("description")]
    public string? Description { get; set; }

    [BsonElement("isActive")]
    public bool IsActive { get; set; } = true;

    [BsonElement("createdAt")]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    [BsonElement("updatedAt")]
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
}
```

---

## 보안 고려사항

### 민감 정보 관리

**절대 커밋하지 말 것:**
- MongoDB 연결 문자열 (사용자명/비밀번호 포함)
- API 키, JWT Secret
- appsettings.Production.json의 민감 정보
- User Secrets 파일
- `.pfx`, `.publishsettings` 파일

**권장 사항:**
1. **Development**: User Secrets 사용
   ```bash
   dotnet user-secrets set "MongoDB:ConnectionString" "mongodb://user:pass@localhost:27017"
   ```

2. **Production**: 환경 변수 또는 Azure Key Vault 사용
   ```bash
   export MongoDB__ConnectionString="mongodb://user:pass@production-host:27017"
   ```

3. **appsettings.json**: 기본값만 포함 (민감 정보 제외)
   ```json
   {
     "MongoDB": {
       "DatabaseName": "n8n_api_work"
     }
   }
   ```

### API 보안

```csharp
// CORS 설정
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.WithOrigins("http://localhost:3000")
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

// Rate Limiting (ASP.NET Core 7+)
builder.Services.AddRateLimiter(options =>
{
    options.GlobalLimiter = PartitionedRateLimiter.Create<HttpContext, string>(context =>
        RateLimitPartition.GetFixedWindowLimiter(
            partitionKey: context.User.Identity?.Name ?? context.Request.Headers.Host.ToString(),
            factory: partition => new FixedWindowRateLimiterOptions
            {
                AutoReplenishment = true,
                PermitLimit = 100,
                QueueLimit = 0,
                Window = TimeSpan.FromMinutes(1)
            }));
});

// 인증/인가 (JWT)
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidAudience = builder.Configuration["Jwt:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]))
        };
    });
```

### 입력 검증

```csharp
// Data Annotations
public class CreateWorkflowDto
{
    [Required(ErrorMessage = "이름은 필수입니다.")]
    [StringLength(100, MinimumLength = 3, ErrorMessage = "이름은 3-100자 사이여야 합니다.")]
    public string Name { get; set; } = null!;

    [StringLength(500, ErrorMessage = "설명은 최대 500자까지 가능합니다.")]
    public string? Description { get; set; }
}

// FluentValidation (선택적)
public class CreateWorkflowDtoValidator : AbstractValidator<CreateWorkflowDto>
{
    public CreateWorkflowDtoValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("이름은 필수입니다.")
            .Length(3, 100).WithMessage("이름은 3-100자 사이여야 합니다.");

        RuleFor(x => x.Description)
            .MaximumLength(500).WithMessage("설명은 최대 500자까지 가능합니다.");
    }
}
```

---

## 일반적인 개발 작업 흐름

### 1. 새 기능 추가 (예: 워크플로우 관리 API)

```bash
# 1. 기능 브랜치 생성
git checkout -b feature/workflow-management

# 2. 엔티티 생성 (Core/Entities/Workflow.cs)
# 3. 인터페이스 정의 (Core/Interfaces/IWorkflowRepository.cs)
# 4. 리포지토리 구현 (Infrastructure/Repositories/WorkflowRepository.cs)
# 5. 서비스 생성 (Core/Services/WorkflowService.cs)
# 6. 컨트롤러 추가 (Api/Controllers/WorkflowsController.cs)
# 7. DTO 생성 (Api/Models/CreateWorkflowDto.cs)

# 8. 테스트 작성
dotnet test

# 9. 실행 및 Swagger 확인
dotnet run --project src/N8nApiWork.Api/N8nApiWork.Api.csproj
# 브라우저에서 http://localhost:5000/swagger 접속

# 10. 커밋 및 푸시
git add .
git commit -m "feat: 워크플로우 관리 API 추가"
git push origin feature/workflow-management
```

### 2. MongoDB 인덱스 생성

```csharp
// Program.cs 또는 별도 초기화 클래스에서
var database = mongoClient.GetDatabase(settings.DatabaseName);
var collection = database.GetCollection<Workflow>("workflows");

// 인덱스 생성
var indexKeys = Builders<Workflow>.IndexKeys.Ascending(w => w.Name);
var indexOptions = new CreateIndexOptions { Unique = true };
await collection.Indexes.CreateOneAsync(new CreateIndexModel<Workflow>(indexKeys, indexOptions));
```

### 3. API 엔드포인트 문서화

```csharp
/// <summary>
/// 워크플로우 목록을 조회합니다.
/// </summary>
/// <returns>워크플로우 목록</returns>
/// <response code="200">성공</response>
/// <response code="500">서버 오류</response>
[HttpGet]
[ProducesResponseType(typeof(IEnumerable<WorkflowDto>), StatusCodes.Status200OK)]
[ProducesResponseType(StatusCodes.Status500InternalServerError)]
public async Task<ActionResult<IEnumerable<WorkflowDto>>> GetWorkflows()
{
    // 구현
}
```

---

## 문제 해결 (Troubleshooting)

### MongoDB 연결 오류

```bash
# 1. MongoDB 컨테이너 상태 확인
docker ps | grep mongo

# 2. MongoDB 컨테이너 시작
docker start <container-name>

# 3. MongoDB 로그 확인
docker logs <container-name>

# 4. 연결 테스트
docker exec -it <container-name> mongosh
```

### 빌드 오류

```bash
# 캐시 정리
dotnet clean
dotnet nuget locals all --clear

# 복원 및 재빌드
dotnet restore
dotnet build
```

### 포트 충돌

```bash
# 사용 중인 포트 확인
sudo lsof -i :5000

# 다른 포트로 실행
dotnet run --project src/N8nApiWork.Api/N8nApiWork.Api.csproj --urls "http://localhost:5001"
```

---

## 향후 Claude 인스턴스를 위한 중요 참고사항

### 필수 준수사항

1. **언어**: 모든 응답과 코드 주석은 **한국어**로 작성
2. **아키텍처**: Clean Architecture 패턴 유지 (Core → Infrastructure → Api)
3. **MongoDB 연결**:
   - Development: User Secrets 사용
   - Production: 환경 변수 사용
   - 절대 appsettings.json에 연결 문자열 하드코딩 금지
4. **Git 워크플로우**:
   - 항상 `feature/`, `bugfix/`, `hotfix/` 브랜치 사용
   - main 브랜치에 직접 커밋 금지
5. **테스트**: 새 기능 추가 시 반드시 단위 테스트 작성

### 코딩 규칙

- **Async/Await**: 모든 I/O 작업은 비동기로 처리
- **Repository Pattern**: 데이터 접근은 리포지토리를 통해서만
- **DTO 사용**: 엔티티를 직접 반환하지 않고 DTO로 변환
- **예외 처리**: Global Exception Handler 사용
- **의존성 주입**: 생성자 주입 패턴 사용
- **네이밍**: C# 네이밍 규칙 준수 (PascalCase, camelCase)

### 성능 최적화

- MongoDB 쿼리에 적절한 인덱스 사용
- 페이징 구현 (대량 데이터 조회 시)
- 응답 캐싱 고려 (특히 자주 조회되는 데이터)
- 비동기 프로그래밍 활용

### 보안 체크리스트

- [ ] 모든 API 엔드포인트에 인증/인가 적용 (개발 단계 제외)
- [ ] 입력 검증 (Data Annotations 또는 FluentValidation)
- [ ] HTTPS 강제 (프로덕션)
- [ ] CORS 정책 설정
- [ ] Rate Limiting 적용
- [ ] SQL Injection 방지 (MongoDB는 기본 방지되지만 동적 쿼리 주의)

---

## 유용한 명령어 참고

### Docker MongoDB 관련

```bash
# MongoDB 컨테이너 시작
docker start my-mongodb-container

# MongoDB 쉘 접속
docker exec -it my-mongodb-container mongosh -u superadmin -p

# MongoDB 데이터베이스 목록 확인
docker exec -it my-mongodb-container mongosh -u superadmin -p --eval "db.adminCommand('listDatabases')"
```

### .NET CLI 유용한 명령어

```bash
# 솔루션 내 모든 프로젝트 나열
dotnet sln list

# 특정 프로젝트의 NuGet 패키지 목록
dotnet list src/N8nApiWork.Api/N8nApiWork.Api.csproj package

# 오래된 NuGet 패키지 확인
dotnet list package --outdated

# NuGet 패키지 업데이트
dotnet add package <PackageName>
```

---

**최종 업데이트**: 2025-11-07
**프로젝트 상태**: ASP.NET Core + MongoDB 기반 n8n API 프로젝트 초기 설정 완료
**주요 기술**: ASP.NET Core Web API, MongoDB, Clean Architecture, Docker
