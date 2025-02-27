namespace Dotnet.Quartz.Api.Domain.Entities;

public class UserEntity
{
    public int Id { get; set; }
    public required string UserName { get; set; }
    public string? EmailValidationToken { get; set; }
    public DateTime? SendValidationTokenDate { get; set; }
    public DateTime CreatedAt { get; set; }
}