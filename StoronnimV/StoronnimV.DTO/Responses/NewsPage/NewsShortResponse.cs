namespace StoronnimV.DTO.Responses.NewsPage;

public class NewsShortResponse
{
    public long Id { get; set; }
    public string Photo { get; set; }
    public string Title { get; set; }
    public string Priority { get; set; }
    public string Date { get; set; }
    
    public NewsShortResponse() { }
    
    public NewsShortResponse(long id, string photo, string title, string priority, string date)
    {
        Id = id;
        Photo = photo;
        Title = title;
        Priority = priority;
        Date = date;
    }
}