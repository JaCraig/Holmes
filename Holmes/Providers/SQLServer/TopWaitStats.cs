using Holmes.BaseClasses;
using Holmes.Interfaces;
using System.Collections.Generic;
using System.Data.Common;
using System.Data.SqlClient;

namespace Holmes.Providers.SQLServer
{
    /// <summary>
    /// Reports the top non-benign wait types on the SQL Server instance, indicating resource pressure areas.
    /// </summary>
    public class TopWaitStats : AnalyzerBaseClass
    {
        /// <inheritdoc/>
        public override DbProviderFactory[] SupportedFactories { get; } = new DbProviderFactory[] { SqlClientFactory.Instance };

        /// <inheritdoc/>
        protected override string QueryString { get; } = @"
SELECT TOP 10
    wait_type AS WaitType,
    wait_time_ms AS WaitTimeMs,
    waiting_tasks_count AS WaitingTasksCount,
    CASE WHEN waiting_tasks_count = 0 THEN 0
         ELSE wait_time_ms / waiting_tasks_count
    END AS AvgWaitMs
FROM sys.dm_os_wait_stats
WHERE wait_type NOT IN (
    'SLEEP_TASK', 'BROKER_TO_FLUSH', 'BROKER_TASK_STOP', 'CLR_AUTO_EVENT',
    'DISPATCHER_QUEUE_SEMAPHORE', 'FT_IFTS_SCHEDULER_IDLE_WAIT',
    'HADR_FILESTREAM_IOMGR_IOCOMPLETION', 'HADR_WORK_QUEUE',
    'LAZYWRITER_SLEEP', 'LOGMGR_QUEUE', 'ONDEMAND_TASK_QUEUE',
    'REQUEST_FOR_DEADLOCK_MONITOR', 'RESOURCE_QUEUE', 'SERVER_IDLE_CHECK',
    'SLEEP_DBSTARTUP', 'SLEEP_DCOMSTARTUP', 'SLEEP_MASTERDBREADY',
    'SLEEP_MASTERMDREADY', 'SLEEP_MASTERUPGRADED', 'SLEEP_MSDBSTARTUP',
    'SLEEP_SYSTEMTASK', 'SLEEP_TEMPDBSTARTUP', 'SNI_HTTP_ACCEPT',
    'SP_SERVER_DIAGNOSTICS_SLEEP', 'SQLTRACE_BUFFER_FLUSH',
    'SQLTRACE_INCREMENTAL_FLUSH_SLEEP', 'WAIT_XTP_OFFLINE_CKPT_NEW_LOG',
    'WAITFOR', 'XE_DISPATCHER_WAIT', 'XE_TIMER_EVENT', 'BROKER_EVENTHANDLER',
    'CHECKPOINT_QUEUE', 'DBMIRROR_EVENTS_QUEUE', 'SQLTRACE_WAIT_ENTRIES',
    'WAIT_XTP_HOST_WAIT', 'WAIT_XTP_IDLE', 'XE_DISPATCHER_JOIN'
)
AND waiting_tasks_count > 0
ORDER BY wait_time_ms DESC;";

        /// <inheritdoc/>
        public override IEnumerable<Finding> Analyze(IEnumerable<dynamic> results)
        {
            if (results is null)
                yield break;
            foreach (dynamic x in results)
            {
                yield return new Finding(
                    $"Wait type '{x.WaitType}' has accumulated {x.WaitTimeMs} ms of wait time across {x.WaitingTasksCount} tasks (avg {x.AvgWaitMs} ms per wait).",
                    x,
                    "",
                    FindingSeverity.Info,
                    "Performance");
            }
        }
    }
}