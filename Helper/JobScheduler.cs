using Hangfire;
using OneDirect.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace OneDirect.Helper
{
    public sealed class JobScheduler
    {
        private OneDirectContext context;

        public JobScheduler(OneDirectContext _context)
        {
            context = _context;
        }
        public JobScheduler()
        {
        }
        public static JobScheduler Instance { get { return HangfireServiceInstance.Instance; } }

        private class HangfireServiceInstance
        {
            static HangfireServiceInstance()
            {
            }

            internal static readonly JobScheduler Instance = new JobScheduler();
        }
        public void ResetScheduledJobs()
        {
            try
            {
                //cancel token
                var cancellationToken = (new CancellationTokenSource()).Token;

                Task.Run(() =>
                {

                    //first remove existing recurring jobs
                    using (var connection = JobStorage.Current.GetConnection())
                    {
                        var servers = JobStorage.Current.GetMonitoringApi().Servers();
                        for (int srv = 0; srv < servers.Count(); srv++)
                        {
                            //Cancel current task if cancel requested (eg: when system getting shutdown)
                            if (cancellationToken != null && cancellationToken.IsCancellationRequested)
                            {
                                cancellationToken.ThrowIfCancellationRequested();
                                return;
                            }
                            if (servers[srv].Name.StartsWith("prabhu-pc"))
                            {
                                connection.RemoveServer(servers[srv].Name);
                            }
                        }

                    }
                }, cancellationToken);
            }
            catch (Exception ex)
            {

            }
        }

        [Queue("critical")]
        [AutomaticRetry(Attempts = 0, LogEvents = false, OnAttemptsExceeded = AttemptsExceededAction.Delete)]
        public void NotifyNewComment()
        {
            try
            {
                int a = 0;

              

            }
            catch (Exception ex)
            {

            }
        }
    }
}
