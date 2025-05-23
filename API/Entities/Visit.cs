using API.Entities;

public class Visit
{
    public int SourceUserId { get; set; }
    public AppUser? SourceUser { get; set; }
    public int TargetUserId { get; set; }
    public AppUser? TargetUser { get; set; }
    public DateTime LastVisited { get; set; }
} 