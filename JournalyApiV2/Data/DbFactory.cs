namespace JournalyApiV2.Data;

public interface IDbFactory
{
    JournalyDbContext Journaly();
}

public class DbFactory : IDbFactory
{
    private readonly IConfiguration _config;

    public DbFactory(IConfiguration config)
    {
        _config = config;
    }

    public JournalyDbContext Journaly()
    {
        return new JournalyDbContext(_config);
    }
}