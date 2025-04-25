namespace AuthService.DataTransferObjects.Results;

public class CreateResultId<T>
{
    public required T Id { get; set; }
}