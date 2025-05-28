namespace API.Helpers
{
    public class VisitsParams : PaginationParams
    {
        public string Predicate { get; set; } = "visitedBy"; 
        public string Filter { get; set; } = "all"; 
    }
}