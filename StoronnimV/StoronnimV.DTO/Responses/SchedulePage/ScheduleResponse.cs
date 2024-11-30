namespace StoronnimV.DTO.Responses.SchedulePage;

public class ScheduleResponse
{
    public long Id { get; set; }
    public string Photo  { get; set; }
    public string Title  { get; set; }
    public string PerformanceDateTime  { get; set; }
    public string Description { get; set; }
    public string Location { get; set; }
    public string Status { get; set; }
    
    public ScheduleResponse() {}

    public ScheduleResponse(long id, string photo, string title, string performanceDateTime,
        string description, string location, string status)
    {
        Id = id;
        Photo = photo;
        Title = title;
        PerformanceDateTime = performanceDateTime;
        Description = description;
        Location = location;
        Status = status;
    }
}