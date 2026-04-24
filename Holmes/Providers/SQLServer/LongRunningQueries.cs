using Holmes.BaseClasses;
using Holmes.Interfaces;
using System.Collections.Generic;
using System.Data.Common;
using System.Data.SqlClient;

namespace Holmes.Providers.SQLServer
{
    /// <summary>
    /// Detects queries that have been executing for longer than 30 seconds.
    /// </summary>
    public class LongRunningQueries : AnalyzerBaseClass
    {
        /// <inheritdoc/>
        public override DbProviderFactory[] SupportedFactories { get; } = new DbProviderFactory[] { SqlClientFactory.Instance };

        /// <inheritdoc/>
        protected override string QueryString { get; } = @"
SELECT TOP 25
    r.session_id AS SessionId,
    r.status AS Status,
    r.wait_type AS WaitType,
    r.wait_time AS WaitTimeMs,
    r.total_elapsed_time AS ElapsedTimeMs,
    r.cpu_time AS CpuTimeMs,
    r.logical_reads AS LogicalReads,
    SUBSTRING(st.text, (r.statement_start_offset / 2) + 1,
        ((CASE r.statement_end_offset
              WHEN -1 THEN DATALENGTH(st.text)
              ELSE r.statement_end_offset
          END - r.statement_start_offset) / 2) + 1) AS QueryText
FROM sys.dm_exec_requests r
CROSS APPLY sys.dm_exec_sql_text(r.sql_handle) st
WHERE r.total_elapsed_time > 30000
    AND r.session_id <> @@SPID
ORDER BY r.total_elapsed_time DESC;";

        /// <inheritdoc/>
        public override IEnumerable<Finding> Analyze(IEnumerable<dynamic> results)
        {
            if (results is null)
                yield break;
            foreach (dynamic x in results)
            {
                yield return new Finding(
                    $"Session {x.SessionId} has been running for {x.ElapsedTimeMs} ms (CPU: {x.CpuTimeMs} ms, reads: {x.LogicalReads}).",
                    x,
                    "",
                    FindingSeverity.Warning,
                    "Query");
            }
        }
    }
}