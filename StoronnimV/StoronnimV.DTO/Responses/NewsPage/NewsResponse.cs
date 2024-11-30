using StoronnimV.DTO.Responses.Shared;

namespace StoronnimV.DTO.Responses.NewsPage;

public class NewsResponse : BaseDto
{
    public string Photo { get; set; }
    public string Title { get; set; }
    public string Description { get; set; }
    public string Priority { get; set; }
    public string Date { get; set; }
    
    public NewsResponse() { }
    
    public NewsResponse(long id, string photo, string title, string description, string priority, string date)
    {
        Id = id;
        Photo = photo;
        Title = title;
        Description = description;
        Priority = priority;
        Date = date;
    }
}