namespace BuildingBlocks.Pagination;

public class PaginationRequestWithId<T>:PaginationRequest
{
    public T Id { get; set; } = default!;
}