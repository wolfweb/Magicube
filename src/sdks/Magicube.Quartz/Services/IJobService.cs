using Magicube.Quartz.Models;
using Magicube.Quartz.ViewModels;
using Quartz;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Magicube.Quartz.Services {
    public interface IJobService {
        Task RegisterJob<TJob>(JobDescriptor jobDescriptor) where TJob : IJob;
        Task RegisterJob(JobDescriptor jobDescriptor,Type type);

        Task<List<JobViewModel>> GetAllJobs();

        Task AddTrigger<T>(string jobKey, DateTime startAt, Action<TriggerBuilder> config) where T : IJob;

        Task AddTrigger<T>(DateTime startAt, Action<TriggerBuilder> config) where T : IJob;

        Task<bool> ExistJob<T>(string jobKey);

        Task<bool> ExistJob<T>();

        Task<bool> ExistTrigger<T>(string jobKey);

        Task<bool> ExistTrigger<T>();

        Task RegisterJob<T>() where T : IJob;

        Task RegisterJob<T>(string jobKey) where T : IJob;

        Task ReplaceTrigger<T>(DateTime startAt, Action<TriggerBuilder> config) where T : IJob;

        Task ReplaceTrigger<T>(string jobKey, DateTime startAt, Action<TriggerBuilder> config) where T : IJob;

        Task RemoveJob<T>() where T : IJob;

        Task RemoveJob<T>(string jobKey) where T : IJob;

        Task RemoveJob(JobKey jobKey);
    }
}
