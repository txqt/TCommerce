namespace T.Library.Model
{
    public class DatabaseControlResponse
    {
        public string? dbname { get; set; }
        public string? source { get; set; }
        public int state { get; set; }
        public bool can_connect { get; set; }
        public List<string>? list_applied_migration { get; set; }
        public List<string>? list_migration_pending { get; set; }
        public List<string?>? list_tables { get; set; }
    }
}
