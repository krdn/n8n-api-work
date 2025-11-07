namespace N8nApiWork.Infrastructure;

/// <summary>
/// MongoDB 연결 설정
/// </summary>
public class MongoDbSettings
{
    /// <summary>
    /// MongoDB 연결 문자열
    /// </summary>
    public string ConnectionString { get; set; } = null!;

    /// <summary>
    /// 데이터베이스 이름
    /// </summary>
    public string DatabaseName { get; set; } = null!;
}
