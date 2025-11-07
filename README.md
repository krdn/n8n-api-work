# n8n API Work

n8n 워크플로우 자동화 플랫폼과 연동하는 ASP.NET Core 기반 RESTful API 프로젝트입니다.

## 📋 목차

- [프로젝트 소개](#-프로젝트-소개)
- [기술 스택](#-기술-스택)
- [시작하기](#-시작하기)
- [프로젝트 구조](#-프로젝트-구조)
- [API 문서](#-api-문서)
- [개발 가이드](#-개발-가이드)
- [라이선스](#-라이선스)

## 🎯 프로젝트 소개

n8n 워크플로우 자동화와 관련된 API 작업을 관리하는 백엔드 시스템입니다. Clean Architecture 패턴을 적용하여 유지보수성과 확장성을 높였으며, MongoDB를 데이터베이스로 사용합니다.

### 주요 기능

- ✅ n8n 워크플로우 관리
- ✅ RESTful API 제공
- ✅ MongoDB 기반 데이터 저장
- ✅ Swagger/OpenAPI 문서 자동 생성
- ✅ Docker 기반 배포

## 🛠 기술 스택

### Backend
- **Framework**: ASP.NET Core 6.0+ (Web API)
- **Language**: C# 10+
- **Database**: MongoDB 7.0+
- **ODM**: MongoDB.Driver

### Infrastructure
- **Container**: Docker
- **Documentation**: Swagger/OpenAPI
- **Testing**: xUnit, Moq, FluentAssertions

### Architecture
- **Pattern**: Clean Architecture
- **Layers**: Api → Core → Infrastructure

## 🚀 시작하기

### 필수 요구사항

- [.NET SDK 6.0+](https://dotnet.microsoft.com/download)
- [Docker](https://www.docker.com/) (MongoDB 실행용)
- [Git](https://git-scm.com/)

### 설치 방법

#### 1. 저장소 클론

```bash
git clone git@github.com:krdn/n8n-api-work.git
cd n8n-api-work
```

#### 2. MongoDB 실행 확인

시스템에 Docker로 MongoDB가 실행 중인지 확인:

```bash
docker ps | grep mongo
```

MongoDB가 실행 중이 아니면 시작:

```bash
docker start <mongodb-container-name>
```

#### 3. 프로젝트 생성 및 설정

```bash
# 솔루션 및 프로젝트 생성
dotnet new sln -n N8nApiWork
dotnet new webapi -n N8nApiWork.Api -o src/N8nApiWork.Api
dotnet new classlib -n N8nApiWork.Core -o src/N8nApiWork.Core
dotnet new classlib -n N8nApiWork.Infrastructure -o src/N8nApiWork.Infrastructure

# 솔루션에 프로젝트 추가
dotnet sln add src/N8nApiWork.Api/N8nApiWork.Api.csproj
dotnet sln add src/N8nApiWork.Core/N8nApiWork.Core.csproj
dotnet sln add src/N8nApiWork.Infrastructure/N8nApiWork.Infrastructure.csproj

# 프로젝트 참조 설정
dotnet add src/N8nApiWork.Api reference src/N8nApiWork.Core
dotnet add src/N8nApiWork.Api reference src/N8nApiWork.Infrastructure
dotnet add src/N8nApiWork.Infrastructure reference src/N8nApiWork.Core

# 패키지 설치
dotnet add src/N8nApiWork.Infrastructure package MongoDB.Driver
dotnet add src/N8nApiWork.Api package Swashbuckle.AspNetCore
```

#### 4. MongoDB 연결 설정

User Secrets를 사용하여 연결 문자열 설정:

```bash
cd src/N8nApiWork.Api
dotnet user-secrets init
dotnet user-secrets set "MongoDB:ConnectionString" "mongodb://localhost:27017"
dotnet user-secrets set "MongoDB:DatabaseName" "n8n_api_work"
```

#### 5. 빌드 및 실행

```bash
# 빌드
dotnet build

# 실행
dotnet run --project src/N8nApiWork.Api/N8nApiWork.Api.csproj
```

#### 6. API 문서 확인

브라우저에서 Swagger UI 접속:

```
http://localhost:5000/swagger
```

## 📁 프로젝트 구조

```
n8n-api-work/
├── src/
│   ├── N8nApiWork.Api/              # Web API 계층
│   │   ├── Controllers/             # API 컨트롤러
│   │   ├── Middleware/              # 커스텀 미들웨어
│   │   ├── Models/                  # DTO, Request/Response 모델
│   │   ├── Program.cs               # 애플리케이션 진입점
│   │   └── appsettings.json         # 설정 파일
│   │
│   ├── N8nApiWork.Core/             # 도메인 계층
│   │   ├── Entities/                # 도메인 엔티티
│   │   ├── Interfaces/              # 인터페이스 정의
│   │   └── Services/                # 비즈니스 로직
│   │
│   └── N8nApiWork.Infrastructure/   # 인프라 계층
│       ├── Data/                    # 데이터베이스 컨텍스트
│       ├── Repositories/            # 리포지토리 구현
│       └── MongoDbSettings.cs       # MongoDB 설정
│
├── tests/
│   ├── N8nApiWork.Api.Tests/        # API 통합 테스트
│   └── N8nApiWork.Core.Tests/       # 단위 테스트
│
├── CLAUDE.md                        # Claude Code 개발 가이드
├── README.md                        # 프로젝트 문서
└── N8nApiWork.sln                   # 솔루션 파일
```

### 계층 설명

- **Api**: HTTP 요청/응답 처리, 의존성 주입 설정, 미들웨어
- **Core**: 비즈니스 규칙, 도메인 모델, 인터페이스 정의 (다른 계층에 의존하지 않음)
- **Infrastructure**: 데이터베이스 연결, 리포지토리 구현, 외부 서비스 통합

## 📚 API 문서

API 실행 후 Swagger UI에서 전체 엔드포인트 확인 가능:

```
http://localhost:5000/swagger
```

### 주요 엔드포인트 (예정)

```
GET    /api/workflows           # 워크플로우 목록 조회
GET    /api/workflows/{id}      # 특정 워크플로우 조회
POST   /api/workflows           # 워크플로우 생성
PUT    /api/workflows/{id}      # 워크플로우 수정
DELETE /api/workflows/{id}      # 워크플로우 삭제
```

## 💻 개발 가이드

### 개발 모드 실행

```bash
# Watch 모드 (코드 변경 시 자동 재시작)
dotnet watch --project src/N8nApiWork.Api/N8nApiWork.Api.csproj run
```

### 테스트 실행

```bash
# 모든 테스트 실행
dotnet test

# 특정 테스트 프로젝트 실행
dotnet test tests/N8nApiWork.Core.Tests/N8nApiWork.Core.Tests.csproj

# 커버리지와 함께 테스트
dotnet test /p:CollectCoverage=true
```

### 코드 포맷팅

```bash
# 포맷 검사
dotnet format --verify-no-changes

# 포맷 자동 적용
dotnet format
```

### 브랜치 전략

```bash
# 새 기능 개발
git checkout -b feature/기능명

# 버그 수정
git checkout -b bugfix/버그명

# 긴급 수정
git checkout -b hotfix/수정명
```

### 커밋 메시지 컨벤션

```
feat: 새로운 기능 추가
fix: 버그 수정
docs: 문서 수정
refactor: 코드 리팩토링
test: 테스트 추가/수정
chore: 빌드, 도구 설정 등
```

## 🔐 보안

### 민감 정보 관리

개발 환경에서는 **User Secrets** 사용:

```bash
dotnet user-secrets set "MongoDB:ConnectionString" "your-connection-string"
dotnet user-secrets set "Jwt:Key" "your-secret-key"
```

프로덕션 환경에서는 **환경 변수** 사용:

```bash
export MongoDB__ConnectionString="mongodb://user:pass@host:27017"
export Jwt__Key="your-production-secret-key"
```

### 절대 커밋하지 말 것

- ❌ MongoDB 연결 문자열 (사용자명/비밀번호 포함)
- ❌ API 키, JWT Secret
- ❌ `appsettings.Production.json`의 민감 정보
- ❌ `.pfx`, `.publishsettings` 파일

## 🐳 Docker 배포

```bash
# Docker 이미지 빌드 (Dockerfile 생성 후)
docker build -t n8n-api-work:latest .

# Docker 컨테이너 실행
docker run -d -p 5000:80 \
  -e MongoDB__ConnectionString="mongodb://host.docker.internal:27017" \
  -e MongoDB__DatabaseName="n8n_api_work" \
  --name n8n-api \
  n8n-api-work:latest
```

## 🤝 기여 방법

1. Fork the Project
2. Create your Feature Branch (`git checkout -b feature/AmazingFeature`)
3. Commit your Changes (`git commit -m 'feat: Add some AmazingFeature'`)
4. Push to the Branch (`git push origin feature/AmazingFeature`)
5. Open a Pull Request

## 📝 라이선스

이 프로젝트는 MIT 라이선스 하에 배포됩니다. 자세한 내용은 [LICENSE](LICENSE) 파일을 참조하세요.

## 📞 문의

프로젝트 관련 문의사항은 [Issues](https://github.com/krdn/n8n-api-work/issues)를 통해 남겨주세요.

---

**Made with ❤️ for n8n automation**
