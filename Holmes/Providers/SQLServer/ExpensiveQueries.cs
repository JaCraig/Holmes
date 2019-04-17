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

using BigBook;
using Holmes.BaseClasses;
using Holmes.Interfaces;
using System.Collections.Generic;
using System.Data.Common;
using System.Data.SqlClient;

namespace Holmes.Providers.SQLServer
{
    /// <summary>
    /// Makes suggestions on expensive queries to look at.
    /// </summary>
    /// <seealso cref="IAnalyzer"/>
    public class ExpensiveQueries : AnalyzerBaseClass
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
        protected override string QueryString { get; } = @"SELECT TOP 25 SUBSTRING(qt.TEXT, (qs.statement_start_offset/2)+1,
((CASE qs.statement_end_offset
WHEN -1 THEN DATALENGTH(qt.TEXT)
ELSE qs.statement_end_offset
END - qs.statement_start_offset)/2)+1) as [query_text],
qs.execution_count,
qs.total_logical_reads, qs.last_logical_reads,
qs.total_logical_writes, qs.last_logical_writes,
qs.total_worker_time,
qs.last_worker_time,
qs.total_elapsed_time/1000000 total_elapsed_time_in_S,
qs.last_elapsed_time/1000000 last_elapsed_time_in_S,
qs.last_execution_time
FROM sys.dm_exec_query_stats qs CROSS APPLY sys.dm_exec_sql_text(qs.plan_handle) st
CROSS APPLY sys.dm_exec_sql_text(qs.sql_handle) qt
CROSS APPLY sys.dm_exec_query_plan(qs.plan_handle) qp
where DB_NAME(st.dbid)=DB_NAME()
ORDER BY qs.total_worker_time DESC";

        /// <summary>
        /// Analyzes the specified connection's source database.
        /// </summary>
        /// <param name="results">The results of the analysis.</param>
        /// <returns>The list of suggestions for the database.</returns>
        public override IEnumerable<Finding> Analyze(IEnumerable<dynamic> results)
        {
            return results.ForEach(x =>
            {
                return new Finding(string.Format("These are the most expensive queries by total CPU time found."),
                       x,
                       "");
            });
        }
    }
}