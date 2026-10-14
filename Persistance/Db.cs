namespace robot_controller_api.Persistence
{
    /// <summary>
    /// Database connection string, set once at startup from
    /// ConnectionStrings:DefaultConnection (user secrets or environment).
    /// </summary>
    public static class Db
    {
        public static string ConnectionString { get; set; } = string.Empty;
    }
}
