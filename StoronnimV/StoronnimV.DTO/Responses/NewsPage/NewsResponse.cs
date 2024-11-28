namespace StoronnimV.DTO.Responses.NewsPage;

public class NewsResponse(string photo, string title, string description, string priority, string date)
{
    public string Photo { get; set; } = photo;
    public string Title { get; set; } = title;
    public string Description { get; set; } = description;
    public string Priority { get; set; } = priority;
    public string Date { get; set; } = date;
}