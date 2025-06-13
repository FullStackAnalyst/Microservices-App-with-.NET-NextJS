namespace SearchService.Entities.Requests;

public class PagedSearchRequest
{
    public string? SearchTerm { get; set; }

    public int PageNumber { get; set; } = 1;

    public int PageSize { get; set; } = 5;

    public string? Seller { get; set; }

    public string? Winner { get; set; }

    public string? OrderBy { get; set; }

    public string? FilterBy { get; set; }
}
