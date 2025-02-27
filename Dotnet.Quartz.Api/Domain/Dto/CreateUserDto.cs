using System.ComponentModel.DataAnnotations;

namespace Dotnet.Quartz.Api.Domain.Dto;

public class CreateUserDto
{
    [Required]
    [MinLength(4)]
    [MaxLength(20)]
    public required string UserName { get; set; }
}