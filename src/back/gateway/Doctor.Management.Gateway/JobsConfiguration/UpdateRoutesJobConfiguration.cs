using Quartz;
using Microsoft.Extensions.Options;
using Doctor.Management.Gateway.Jobs;

namespace Doctor.Management.Gateway.JobsConfiguration;

public sealed class UpdateRoutesJobConfiguration
    : IConfigureOptions<QuartzOptions>
{
    public void Configure(QuartzOptions options)
    {
#if DEBUG
        Action<SimpleScheduleBuilder> customoSchedule = schedule => schedule.WithIntervalInMinutes(1).RepeatForever();
#else
        Action<SimpleScheduleBuilder> customoSchedule = schedule => schedule.WithIntervalInHours(1).RepeatForever();
#endif

        JobKey jobKey = JobKey.Create(nameof(UpdateRoutesJob));
        options
            .AddJob<UpdateRoutesJob>(builder => builder.WithIdentity(jobKey))
            .AddTrigger(triger =>
                triger.ForJob(jobKey).WithSimpleSchedule(customoSchedule));
    }
}