using Magicube.Quartz.Jobs;
using Magicube.Quartz.Models;
using Magicube.Quartz.ViewModels;
using Quartz;
using Quartz.Impl.Matchers;
using Quartz.Impl.Triggers;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace Magicube.Quartz.Services {
    public class JobService : IJobService {
        private IScheduler _scheduler;
        public JobService(IScheduler scheduler) {
            _scheduler = scheduler;
            Task.WaitAll(Initialize());
        }

        public async Task RegisterJob<TJob>(JobDescriptor jobDescriptor) where TJob : IJob {
            var casting = new JobDescriptorAdapter(jobDescriptor);
            var jobDetail = casting.RetrieveJobDetail<TJob>();
            await RemoveJob(jobDetail.Key);
            await _scheduler.ScheduleJob(casting.RetrieveJobDetail<TJob>(), casting.RetrieveJobTrigger());
        }

        public async Task RegisterJob(JobDescriptor jobDescriptor, Type type) {
            var casting = new JobDescriptorAdapter(jobDescriptor);
            var jobDetail = casting.RetrieveJobDetail(type);
            await RemoveJob(jobDetail.Key);
            await _scheduler.ScheduleJob(casting.RetrieveJobDetail(type), casting.RetrieveJobTrigger());
        }

        public async Task<List<JobViewModel>> GetAllJobs() {
            var result = new List<JobViewModel>();
            var jboKeyList = new List<JobKey>();
            var groupNames = await _scheduler.GetJobGroupNames();
            foreach (var groupName in groupNames.OrderBy(x => x)) {
                jboKeyList.AddRange(await _scheduler.GetJobKeys(GroupMatcher<JobKey>.GroupEquals(groupName)));
                result.Add(new JobViewModel { Group = groupName });
            }

            foreach (var jobKey in jboKeyList.OrderBy(x => x.Name)) {
                var jobDetail = await _scheduler.GetJobDetail(jobKey);
                var triggersList = await _scheduler.GetTriggersOfJob(jobKey);
                var triggers = triggersList.FirstOrDefault();

                var interval = string.Empty;
                if (triggers is SimpleTriggerImpl)
                    interval = (triggers as SimpleTriggerImpl)?.RepeatInterval.ToString();
                else
                    interval = (triggers as CronTriggerImpl)?.CronExpressionString;

                foreach (var jobInfo in result) {
                    if (jobInfo.Group == jobKey.Group) {
                        jobInfo.JobInfos.Add(new JobInfoViewModel{
                            Name             = jobKey.Name,
                            LastErrMsg       = jobDetail.JobDataMap.GetString("ErrorMessage"),
                            BeginTime        = triggers?.StartTimeUtc.LocalDateTime,
                            Interval         = interval,
                            TriggerState     = triggers != null ? await _scheduler.GetTriggerState(triggers.Key) : TriggerState.None,
                            NextAimTime      = triggers?.GetNextFireTimeUtc()?.LocalDateTime,
                            PreviousAimTime  = triggers?.GetPreviousFireTimeUtc()?.LocalDateTime,
                            EndTime          = triggers?.EndTimeUtc?.LocalDateTime,
                            Description      = jobDetail.Description,
                        });
                        continue;
                    }
                }
            }

            return result;
        }

        public async Task AddTrigger<T>(string jobKey, DateTime startAt, Action<TriggerBuilder> config) where T : IJob {
            var _jobKey = new JobKey(jobKey);

            if (!await _scheduler.CheckExists(_jobKey)) {
                await RegisterJob<T>(jobKey);
            }

            var triggerBuilder = TriggerBuilder.Create()
                .WithIdentity(BuildTriggerKey<T>())
                .ForJob(_jobKey)
                .StartAt(startAt);

            config(triggerBuilder);

            await _scheduler.ScheduleJob(triggerBuilder.Build());
        }

        public async Task AddTrigger<T>(DateTime startAt, Action<TriggerBuilder> config) where T : IJob {
            var jobKey = BuildJobKey<T>();

            if (!await _scheduler.CheckExists(jobKey)) {
                await RegisterJob<T>();
            }

            var triggerBuilder = TriggerBuilder.Create()
                .WithIdentity(BuildTriggerKey<T>())
                .ForJob(jobKey)
                .StartAt(startAt);

            config(triggerBuilder);
            await _scheduler.ScheduleJob(triggerBuilder.Build());
        }

        public async Task<bool> ExistJob<T>(string jobKey) {
            var _jobKey = new JobKey(jobKey);
            return await _scheduler.CheckExists(_jobKey);
        }

        public async Task<bool> ExistJob<T>() {
            var _jobKey = BuildJobKey<T>();
            return await _scheduler.CheckExists(_jobKey);
        }

        public async Task<bool> ExistTrigger<T>(string jobKey) {
            var _jobKey = new JobKey(jobKey);
            var jobTriggers = await _scheduler.GetTriggersOfJob(_jobKey);
            return jobTriggers.Any();
        }

        public async Task<bool> ExistTrigger<T>() {
            var _jobKey = BuildJobKey<T>();
            var jobTriggers = await _scheduler.GetTriggersOfJob(_jobKey);
            return jobTriggers.Any();
        }

        public async Task RegisterJob<T>() where T : IJob {
            var jobKey = BuildJobKey<T>();
            Trace.WriteLine($"add job {typeof(T).FullName} with jobKey=>{jobKey}");
            if (!await _scheduler.CheckExists(jobKey))
                await _scheduler.AddJob(JobBuilder.Create<T>().WithIdentity(jobKey).Build(), true, true);
        }

        public async Task RegisterJob<T>(string jobKey) where T : IJob {
            var _jobKey = new JobKey(jobKey);
            if (!await _scheduler.CheckExists(_jobKey))
                await _scheduler.AddJob(JobBuilder.Create<T>().WithIdentity(jobKey).Build(), true, true);
        }

        public async Task ReplaceTrigger<T>(DateTime startAt, Action<TriggerBuilder> config) where T : IJob {
            var jobKey = BuildJobKey<T>();

            if (await _scheduler.CheckExists(jobKey)) {
                await RemoveJob<T>();
            }

            await AddTrigger<T>(startAt, config);
        }

        public async Task ReplaceTrigger<T>(string jobKey, DateTime startAt, Action<TriggerBuilder> config) where T : IJob {
            var _jobKey = new JobKey(jobKey);

            if (await _scheduler.CheckExists(_jobKey)) {
                await RemoveJob<T>(jobKey);
            }

            await AddTrigger<T>(jobKey, startAt, config);
        }

        public async Task RemoveJob<T>() where T : IJob {
            var jobKey = BuildJobKey<T>();
            await _scheduler.DeleteJob(jobKey);

            await RemoveJobTriggers(jobKey);
        }

        public async Task RemoveJob<T>(string jobKey) where T : IJob {
            var _jobKey = new JobKey(jobKey);
            await _scheduler.DeleteJob(_jobKey);

            await RemoveJobTriggers(_jobKey);
        }

        public async Task RemoveJob(JobKey jobKey) {
            await _scheduler.DeleteJob(jobKey);

            await RemoveJobTriggers(jobKey);
        }

        private JobKey BuildJobKey<T>() {
            var type = typeof(T);
            return new JobKey($"{type.Name}", type.Namespace);
        }

        private TriggerKey BuildTriggerKey<T>() {
            var type = typeof(T);
            return new TriggerKey($"{type.Name}-{Guid.NewGuid().ToString("n")}", type.Namespace);
        }

        private async Task Initialize() {
            if (!_scheduler.IsStarted)
                await _scheduler.Start();
        }

        private async Task RemoveJobTriggers(JobKey jobKey) {
            var triggers = await _scheduler.GetTriggersOfJob(jobKey);
            await _scheduler.UnscheduleJobs(triggers.Select(x => x.Key).ToArray());
        }
    }
}
