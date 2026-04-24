using Holmes.BaseClasses;
using Holmes.Interfaces;
using System.Collections.Generic;
using System.Data.Common;
using System.Data.SqlClient;

namespace Holmes.Providers.SQLServer
{
    /// <summary>
    /// Detects user table statistics that have not been updated recently.
    /// </summary>
    public class StaleStatistics : AnalyzerBaseClass
    {
        /// <inheritdoc/>
        public override DbProviderFactory[] SupportedFactories { get; } = new DbProviderFactory[] { SqlClientFactory.Instance };

        /// <inheritdoc/>
        protected override string QueryString { get; } = @"
SELECT TOP 25
    OBJECT_NAME(s.object_id) AS TableName,
    s.name AS StatisticsName,
    STATS_DATE(s.object_id, s.stats_id) AS LastUpdated,
    DATEDIFF(DAY, STATS_DATE(s.object_id, s.stats_id), GETDATE()) AS DaysSinceUpdate,
    'UPDATE STATISTICS [' + OBJECT_NAME(s.object_id) + '] [' + s.name + '];' AS FixStatement
FROM sys.stats s
INNER JOIN sys.objects o ON s.object_id = o.object_id
WHERE o.type = 'U'
    AND STATS_DATE(s.object_id, s.stats_id) IS NOT NULL
    AND DATEDIFF(DAY, STATS_DATE(s.object_id, s.stats_id), GETDATE()) > 7
ORDER BY DaysSinceUpdate DESC;";

        /// <inheritdoc/>
        public override IEnumerable<Finding> Analyze(IEnumerable<dynamic> results)
        {
            if (results is null)
                yield break;
            foreach (dynamic x in results)
            {
                yield return new Finding(
                    $"Statistics '{x.StatisticsName}' on table '{x.TableName}' are {x.DaysSinceUpdate} days old and may cause suboptimal query plans.",
                    x,
                    x.FixStatement,
                    FindingSeverity.Warning,
                    "Query");
            }
        }
    }
}