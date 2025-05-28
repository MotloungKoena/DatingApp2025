/*public class VisitDto
{
    public string? Username { get; set; }
    public string? KnownAs { get; set; }
    public DateTime VisitedOn { get; set; }
    public string? PhotoUrl { get; set; }
}*/

/*public class VisitDto
{
    public int Id { get; set; } // Required for Angular trackBy and like check
    public string? Username { get; set; }
    public string? KnownAs { get; set; }
    public string? PhotoUrl { get; set; }
    public string? City { get; set; }
    public int Age { get; set; }
    public DateTime Created { get; set; }
    public DateTime LastActive { get; set; }
    public DateTime VisitedOn { get; set; }
    
}*/
public class VisitDto
{
        public int Id { get; set; } // Required for Angular trackBy and like check

    public string? Username { get; set; }
    public string? KnownAs { get; set; }
    public string? Gender { get; set; }
    public int Age { get; set; }
    public string? Introduction { get; set; }
    public string? LookingFor { get; set; }
    public string? Interests { get; set; }
    public string? City { get; set; }
    public string? Country { get; set; }
    public DateTime Created { get; set; }
    public DateTime LastActive { get; set; }
    public string PhotoUrl { get; set; }
    //public List<PhotoDto> Photos { get; set; }  // Add this if used
    public DateTime VisitedOn { get; set; }
}

