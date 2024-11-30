namespace StoronnimV.DTO.Responses.SchedulePage;

public class ScheduleShortResponse
{
    public long Id { get; set; }
    public string Photo  { get; set; }
    public string Title  { get; set; }
    public string PerformanceDateTime  { get; set; }
    public string Location { get; set; }
    public string Status { get; set; }
    
    public ScheduleShortResponse() {}

    public ScheduleShortResponse(long id, string photo, string title,
        string performanceDateTime, string location, string status)
    {
        Id = id;
        Photo = photo;
        Title = title;
        PerformanceDateTime = performanceDateTime;
        Location = location;
        Status = status;
    }
}