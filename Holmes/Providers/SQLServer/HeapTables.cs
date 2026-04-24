using Holmes.BaseClasses;
using Holmes.Interfaces;
using System.Collections.Generic;
using System.Data.Common;
using System.Data.SqlClient;

namespace Holmes.Providers.SQLServer
{
    /// <summary>
    /// Detects user tables that have no clustered index (heap tables).
    /// </summary>
    public class HeapTables : AnalyzerBaseClass
    {
        /// <inheritdoc/>
        public override DbProviderFactory[] SupportedFactories { get; } = new DbProviderFactory[] { SqlClientFactory.Instance };

        /// <inheritdoc/>
        protected override string QueryString { get; } = @"
SELECT
    s.name AS SchemaName,
    o.name AS TableName,
    p.rows AS RowCount
FROM sys.objects o
INNER JOIN sys.schemas s ON o.schema_id = s.schema_id
INNER JOIN sys.partitions p ON o.object_id = p.object_id AND p.index_id = 0
WHERE o.type = 'U'
    AND NOT EXISTS (
        SELECT 1 FROM sys.indexes i
        WHERE i.object_id = o.object_id AND i.type = 1
    )
ORDER BY p.rows DESC;";

        /// <inheritdoc/>
        public override IEnumerable<Finding> Analyze(IEnumerable<dynamic> results)
        {
            if (results is null)
                yield break;
            foreach (dynamic x in results)
            {
                yield return new Finding(
                    $"Table '{x.SchemaName}.{x.TableName}' has no clustered index ({x.RowCount} rows). Heap tables can cause poor read performance.",
                    x,
                    $"CREATE CLUSTERED INDEX [IX_CLUSTERED_{x.TableName}] ON [{x.SchemaName}].[{x.TableName}] (<column>);",
                    FindingSeverity.Warning,
                    "Schema");
            }
        }
    }
}