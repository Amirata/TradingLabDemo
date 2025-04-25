namespace BuildingBlocks.Pagination;

public class PaginationRequest
{
    public int PageNumber { get; set; }

    public int PageSize { get; set; }

    public string? Search { get; set; }
    
    
    public IEnumerable<Sort>? Sorts { get; set; }
    
}
