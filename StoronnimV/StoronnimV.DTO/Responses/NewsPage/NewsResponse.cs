namespace StoronnimV.DTO.Responses.NewsPage;

public class NewsResponse(long id, string photo, string title, string description, string priority, string date)
{
    public long Id { get; set; } = id;
    public string Photo { get; set; } = photo;
    public string Title { get; set; } = title;
    public string Description { get; set; } = description;
    public string Priority { get; set; } = priority;
    public string Date { get; set; } = date;
}