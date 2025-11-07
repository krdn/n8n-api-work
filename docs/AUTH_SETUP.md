# JWT 인증 시스템 설정 완료

## 개요

ASP.NET Core Web API에 JWT(JSON Web Token) 기반 인증 시스템을 성공적으로 추가했습니다.

## 추가된 주요 기능

### 1. NuGet 패키지

```xml
<!-- Api 프로젝트 -->
<PackageReference Include="Microsoft.AspNetCore.Authentication.JwtBearer" Version="8.0.0" />

<!-- Infrastructure 프로젝트 -->
<PackageReference Include="BCrypt.Net-Next" Version="4.0.3" />
<PackageReference Include="System.IdentityModel.Tokens.Jwt" Version="7.0.3" />
<PackageReference Include="Microsoft.Extensions.Options" Version="8.0.0" />
```

### 2. 주요 컴포넌트

#### Core 계층
- **`JwtSettings.cs`**: JWT 설정 모델
- **`IAuthService.cs`**: 인증 서비스 인터페이스

#### Infrastructure 계층
- **`AuthService.cs`**: 인증 로직 구현
  - 회원가입 (RegisterAsync)
  - 로그인 (LoginAsync)
  - 비밀번호 해싱/검증 (BCrypt)
  - JWT 토큰 생성

#### API 계층
- **`AuthController.cs`**: 인증 엔드포인트
  - `POST /api/auth/register` - 회원가입
  - `POST /api/auth/login` - 로그인
  - `GET /api/auth/me` - 현재 사용자 정보 (인증 필요)

- **DTO 모델**:
  - `LoginRequestDto.cs` - 로그인 요청
  - `RegisterRequestDto.cs` - 회원가입 요청
  - `AuthResponseDto.cs` - 인증 응답 (토큰 포함)

### 3. 설정 파일

**appsettings.Development.json**:
```json
{
  "Jwt": {
    "SecretKey": "YourSuperSecretKeyForJWTTokenGeneration123456789",
    "Issuer": "N8nApiWork",
    "Audience": "N8nApiWorkUsers",
    "ExpirationInMinutes": 60,
    "RefreshTokenExpirationInDays": 7
  }
}
```

> ⚠️ **중요**: 프로덕션 환경에서는 SecretKey를 환경 변수나 User Secrets로 관리하세요.

### 4. Program.cs 설정

- JWT 인증 미들웨어 추가
- Swagger에 JWT 인증 UI 추가
- 의존성 주입 설정
- 인증/인가 파이프라인 구성

## API 사용법

### 1. 회원가입

```bash
curl -X POST http://localhost:5175/api/auth/register \
  -H "Content-Type: application/json" \
  -d '{
    "name": "홍길동",
    "email": "test@example.com",
    "password": "password123",
    "confirmPassword": "password123"
  }'
```

**응답 예시**:
```json
{
  "userId": "507f1f77bcf86cd799439011",
  "email": "test@example.com",
  "name": "홍길동",
  "token": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...",
  "expiresAt": "2025-11-07T08:30:00Z"
}
```

### 2. 로그인

```bash
curl -X POST http://localhost:5175/api/auth/login \
  -H "Content-Type: application/json" \
  -d '{
    "email": "test@example.com",
    "password": "password123"
  }'
```

**응답 예시**:
```json
{
  "userId": "507f1f77bcf86cd799439011",
  "email": "test@example.com",
  "name": "홍길동",
  "token": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...",
  "expiresAt": "2025-11-07T08:30:00Z"
}
```

### 3. 인증이 필요한 API 호출

받은 토큰을 Authorization 헤더에 포함:

```bash
curl -X GET http://localhost:5175/api/auth/me \
  -H "Authorization: Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9..."
```

**응답 예시**:
```json
{
  "userId": "507f1f77bcf86cd799439011",
  "email": "test@example.com",
  "name": "test",
  "role": "User"
}
```

## Swagger UI에서 테스트

1. 브라우저에서 `http://localhost:5175/swagger` 접속
2. "Authorize" 버튼 클릭
3. "Bearer {token}" 형식으로 토큰 입력
4. 인증이 필요한 엔드포인트 테스트

## 보안 고려사항

### 현재 경고
- `System.IdentityModel.Tokens.Jwt 7.0.3` 패키지에 알려진 중간 수준 취약점 존재
- 향후 패치된 버전으로 업데이트 권장

### 권장 사항

1. **프로덕션 배포 전**:
   - JWT SecretKey를 User Secrets 또는 환경 변수로 관리
   - HTTPS 강제 사용
   - Rate Limiting 추가
   - CORS 정책 설정

2. **SecretKey 관리**:
```bash
# Development: User Secrets 사용
dotnet user-secrets set "Jwt:SecretKey" "your-super-secret-key-here"

# Production: 환경 변수 사용
export Jwt__SecretKey="your-super-secret-key-here"
```

3. **기타 보안**:
   - 토큰 만료 시간 적절히 설정 (현재 60분)
   - 리프레시 토큰 구현 고려
   - 비밀번호 정책 강화 (길이, 복잡도)
   - 계정 잠금 정책 추가

## 기존 컨트롤러에 인증 적용

기존 API 엔드포인트에 인증을 추가하려면:

```csharp
using Microsoft.AspNetCore.Authorization;

[ApiController]
[Route("api/[controller]")]
public class WorkflowsController : ControllerBase
{
    // 인증이 필요한 엔드포인트
    [HttpGet]
    [Authorize]
    public async Task<ActionResult> GetWorkflows()
    {
        // 현재 사용자 ID 가져오기
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        // ...
    }

    // 특정 역할이 필요한 엔드포인트
    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult> DeleteWorkflow(string id)
    {
        // ...
    }

    // 인증이 필요 없는 공개 엔드포인트
    [HttpGet("public")]
    [AllowAnonymous]
    public async Task<ActionResult> GetPublicData()
    {
        // ...
    }
}
```

## 실행 방법

```bash
# 프로젝트 빌드
dotnet build

# 애플리케이션 실행
dotnet run --project src/N8nApiWork.Api/N8nApiWork.Api.csproj

# 또는 Watch 모드로 실행 (자동 재시작)
dotnet watch --project src/N8nApiWork.Api/N8nApiWork.Api.csproj run
```

애플리케이션이 시작되면:
- API: http://localhost:5175
- Swagger UI: http://localhost:5175/swagger

## 다음 단계

1. **리프레시 토큰 구현**: 액세스 토큰 만료 후 재발급
2. **이메일 인증**: 회원가입 시 이메일 확인
3. **비밀번호 재설정**: 비밀번호 찾기 기능
4. **소셜 로그인**: Google, GitHub 등 OAuth 통합
5. **역할 기반 권한 관리**: Admin, User 등 세분화
6. **감사 로그**: 인증 관련 이벤트 로깅

## 문제 해결

### 빌드 오류
```bash
dotnet clean
dotnet restore
dotnet build
```

### 패키지 버전 충돌
```bash
dotnet list package --vulnerable
dotnet add package [PackageName] --version [SafeVersion]
```

### MongoDB 연결 오류
- MongoDB 컨테이너가 실행 중인지 확인
- `appsettings.Development.json`의 ConnectionString 확인

---

**작성일**: 2025-11-07
**버전**: 1.0
**작성자**: Claude Code
