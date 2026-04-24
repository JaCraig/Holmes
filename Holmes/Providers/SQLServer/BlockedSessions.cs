using Holmes.BaseClasses;
using Holmes.Interfaces;
using System.Collections.Generic;
using System.Data.Common;
using System.Data.SqlClient;

namespace Holmes.Providers.SQLServer
{
    /// <summary>
    /// Detects sessions that are currently blocked by another session, indicating lock contention.
    /// </summary>
    public class BlockedSessions : AnalyzerBaseClass
    {
        /// <inheritdoc/>
        public override DbProviderFactory[] SupportedFactories { get; } = new DbProviderFactory[] { SqlClientFactory.Instance };

        /// <inheritdoc/>
        protected override string QueryString { get; } = @"
SELECT
    r.session_id AS BlockedSessionId,
    r.blocking_session_id AS BlockingSessionId,
    r.wait_time AS WaitTimeMs,
    r.wait_type AS WaitType,
    SUBSTRING(st.text, (r.statement_start_offset / 2) + 1,
        ((CASE r.statement_end_offset
              WHEN -1 THEN DATALENGTH(st.text)
              ELSE r.statement_end_offset
          END - r.statement_start_offset) / 2) + 1) AS BlockedQuery
FROM sys.dm_exec_requests r
CROSS APPLY sys.dm_exec_sql_text(r.sql_handle) st
WHERE r.blocking_session_id > 0;";

        /// <inheritdoc/>
        public override IEnumerable<Finding> Analyze(IEnumerable<dynamic> results)
        {
            if (results is null)
                yield break;
            foreach (dynamic x in results)
            {
                yield return new Finding(
                    $"Session {x.BlockedSessionId} is blocked by session {x.BlockingSessionId} (wait: {x.WaitTimeMs} ms, type: {x.WaitType}).",
                    x,
                    "",
                    FindingSeverity.Critical,
                    "Connections");
            }
        }
    }
}