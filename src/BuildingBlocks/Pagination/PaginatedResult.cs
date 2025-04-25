namespace BuildingBlocks.Pagination;

public class PaginatedResult<T>(List<T> data, int totalCount = 0, int currentPage = 1, int pageSize = 10)
{
    public List<T> Data { get; init; } = data;
    public int CurrentPage { get; init; } = currentPage;
    public int TotalPages { get; init; } = (int)Math.Ceiling(totalCount / (double)pageSize);
    public int TotalCount { get; init; } = totalCount;
    public int PageSize { get; init; } = pageSize;

    public bool HasPreviousPage => CurrentPage > 1;
    public bool HasNextPage => CurrentPage < TotalPages;

    public static PaginatedResult<T> Create(List<T> data, int totalCount, int currentPage, int pageSize)
    {
        return new PaginatedResult<T>(data, totalCount, currentPage, pageSize);
    }
}