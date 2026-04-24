using Holmes.BaseClasses;
using Holmes.Interfaces;
using System.Collections.Generic;
using System.Data.Common;
using System.Data.SqlClient;
using System.Linq;

namespace Holmes.Providers.SQLServer
{
    /// <summary>
    /// Detects highly fragmented indexes in the current database.
    /// </summary>
    public class TableFragmentation : AnalyzerBaseClass
    {
        /// <inheritdoc/>
        public override DbProviderFactory[] SupportedFactories { get; } = new DbProviderFactory[] { SqlClientFactory.Instance };

        /// <inheritdoc/>
        protected override string QueryString { get; } = @"
SELECT TOP 25
    OBJECT_NAME(ips.object_id) AS TableName,
    i.name AS IndexName,
    ips.index_type_desc AS IndexType,
    ROUND(ips.avg_fragmentation_in_percent, 2) AS FragmentationPercent,
    CASE
        WHEN ips.avg_fragmentation_in_percent > 30
            THEN 'ALTER INDEX [' + i.name + '] ON [' + OBJECT_NAME(ips.object_id) + '] REBUILD;'
        ELSE
            'ALTER INDEX [' + i.name + '] ON [' + OBJECT_NAME(ips.object_id) + '] REORGANIZE;'
    END AS FixStatement
FROM sys.dm_db_index_physical_stats(DB_ID(), NULL, NULL, NULL, 'LIMITED') ips
INNER JOIN sys.indexes i ON ips.object_id = i.object_id AND ips.index_id = i.index_id
WHERE ips.avg_fragmentation_in_percent > 10
    AND ips.index_id > 0
ORDER BY ips.avg_fragmentation_in_percent DESC;";

        /// <inheritdoc/>
        public override IEnumerable<Finding> Analyze(IEnumerable<dynamic> results)
        {
            if (results is null)
                yield break;
            foreach (dynamic x in results)
            {
                yield return new Finding(
                    $"Index '{x.IndexName}' on '{x.TableName}' has {x.FragmentationPercent}% fragmentation.",
                    x,
                    x.FixStatement,
                    FindingSeverity.Warning,
                    "Index");
            }
        }
    }
}