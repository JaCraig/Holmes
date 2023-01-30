/*
Copyright 2017 James Craig

Licensed under the Apache License, Version 2.0 (the "License");
you may not use this file except in compliance with the License.
You may obtain a copy of the License at

    http://www.apache.org/licenses/LICENSE-2.0

Unless required by applicable law or agreed to in writing, software
distributed under the License is distributed on an "AS IS" BASIS,
WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
See the License for the specific language governing permissions and
limitations under the License.
*/

using Holmes.BaseClasses;
using Holmes.Interfaces;
using System.Collections.Generic;
using System.Data.Common;
using Microsoft.Data.SqlClient;
using System.Linq;

namespace Holmes.Providers.SQLServer
{
    /// <summary>
    /// Makes suggestions on indexes that are not used.
    /// </summary>
    /// <seealso cref="IAnalyzer"/>
    public class UnusedIndexes : AnalyzerBaseClass
    {
        /// <summary>
        /// Gets the factory the analyzer supports.
        /// </summary>
        /// <value>Gets the factory the analyzer supports.</value>
        public override DbProviderFactory SupportedFactory { get; } = SqlClientFactory.Instance;

        /// <summary>
        /// Gets the query string.
        /// </summary>
        /// <value>The query string.</value>
        protected override string QueryString { get; } = @"SELECT TOP 25
o.name AS ObjectName
, i.name AS IndexName
, i.index_id AS IndexID
, dm_ius.user_seeks AS UserSeek
, dm_ius.user_scans AS UserScans
, dm_ius.user_lookups AS UserLookups
, dm_ius.user_updates AS UserUpdates
, p.TableRows
, 'DROP INDEX ' + QUOTENAME(i.name)
+ ' ON ' + QUOTENAME(s.name) + '.'
+ QUOTENAME(OBJECT_NAME(dm_ius.OBJECT_ID)) AS 'drop statement'
FROM sys.dm_db_index_usage_stats dm_ius
INNER JOIN sys.indexes i ON i.index_id = dm_ius.index_id
AND dm_ius.OBJECT_ID = i.OBJECT_ID
INNER JOIN sys.objects o ON dm_ius.OBJECT_ID = o.OBJECT_ID
INNER JOIN sys.schemas s ON o.schema_id = s.schema_id
INNER JOIN (SELECT SUM(p.rows) TableRows, p.index_id, p.OBJECT_ID
FROM sys.partitions p GROUP BY p.index_id, p.OBJECT_ID) p
ON p.index_id = dm_ius.index_id AND dm_ius.OBJECT_ID = p.OBJECT_ID
WHERE OBJECTPROPERTY(dm_ius.OBJECT_ID,'IsUserTable') = 1
AND dm_ius.database_id = DB_ID()
AND i.type_desc = 'nonclustered'
AND i.is_primary_key = 0
AND i.is_unique_constraint = 0
AND (dm_ius.user_scans+dm_ius.user_lookups)>dm_ius.user_seeks
ORDER BY (dm_ius.user_scans + dm_ius.user_lookups) DESC";

        /// <summary>
        /// Analyzes the specified connection's source database.
        /// </summary>
        /// <param name="results">The results of the analysis.</param>
        /// <returns>The list of suggestions for the database.</returns>
        public override IEnumerable<Finding> Analyze(IEnumerable<dynamic> results)
        {
            return results.Select(x => new Finding($"Potential index ({x.IndexName}) for removal for {x.ObjectName}.", x, x.drop_statement));
        }
    }
}