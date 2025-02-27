using Dotnet.Quartz.Api.Context;
using Dotnet.Quartz.Api.Domain.Dto;
using Dotnet.Quartz.Api.Domain.Entities;
using Dotnet.Quartz.Api.Jobs;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Quartz;

namespace Dotnet.Quartz.Api.Controllers;

[ApiController]
[Route("[controller]")]
public class UserController : ControllerBase
{
    private readonly AppDataContext _context;
    private readonly ISchedulerFactory _schedulerFactory;

    public UserController(
        AppDataContext context,
        ISchedulerFactory schedulerFactory)
    {
        _context = context;
        _schedulerFactory = schedulerFactory;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        try
        {
            var result = await _context.User.ToListAsync();
            return Ok(result);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateUserDto dto)
    {
        try
        {
            var result = await _context.User.AddAsync(new UserEntity
            {
                UserName = dto.UserName,
                CreatedAt = DateTime.Now
            });

            await _context.SaveChangesAsync();
            await ScheludeCreateEmailTokenValidationJob(result.Entity);

            return Ok(result.Entity);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    private async Task ScheludeCreateEmailTokenValidationJob(UserEntity user)
    {
        var schedule = await _schedulerFactory.GetScheduler();
        var jobData = new JobDataMap
        {
            { "UserId", user.Id}
        };
        var trigger = TriggerBuilder.Create()
            .ForJob(CreateEmailTokenValidationJob.Name)
            .WithIdentity($"create-email-token-validation-{user.Id}")
            .UsingJobData(jobData)
            //.StartNow()
            .StartAt(DateTime.Now.AddSeconds(5))
            .Build();

        await schedule.ScheduleJob(trigger);
    }
}