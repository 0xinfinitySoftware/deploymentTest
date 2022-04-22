namespace DeploymentTest.Database
{
    public class AppSettings
    {
        public DbConfig Db { get; set; }
    }

    public class DbConfig
    {
        public string ConnectionString { get; set; }
    }
}
