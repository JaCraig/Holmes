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
using Microsoft.Data.SqlClient;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;

namespace Holmes.Providers.SQLServer
{
    /// <summary>
    /// Shows drive latency
    /// </summary>
    /// <seealso cref="IAnalyzer"/>
    public class DriveLatency : AnalyzerBaseClass
    {
        /// <summary>
        /// Gets the factory the analyzer supports.
        /// </summary>
        /// <value>Gets the factory the analyzer supports.</value>
        public override DbProviderFactory[] SupportedFactories { get; } = new DbProviderFactory[] { SqlClientFactory.Instance, System.Data.SqlClient.SqlClientFactory.Instance };

        /// <summary>
        /// Gets the query string.
        /// </summary>
        /// <value>The query string.</value>
        protected override string QueryString { get; } = @"SELECT [Drive],
               CASE
                              WHEN num_of_reads = 0 THEN 0
                              ELSE (io_stall_read_ms/num_of_reads)
               END AS [Read Latency],
               CASE
                              WHEN io_stall_write_ms = 0 THEN 0
                              ELSE (io_stall_write_ms/num_of_writes)
               END AS [Write Latency],
               CASE
                              WHEN (num_of_reads = 0 AND num_of_writes = 0) THEN 0
                              ELSE (io_stall/(num_of_reads + num_of_writes))
               END AS [Overall Latency],
               CASE
                              WHEN num_of_reads = 0 THEN 0
                              ELSE (num_of_bytes_read/num_of_reads)
               END AS [Avg Bytes/Read],
               CASE
                              WHEN io_stall_write_ms = 0 THEN 0
                              ELSE (num_of_bytes_written/num_of_writes)
               END AS [Avg Bytes/Write],
               CASE
                              WHEN (num_of_reads = 0 AND num_of_writes = 0) THEN 0
                              ELSE ((num_of_bytes_read + num_of_bytes_written)/(num_of_reads + num_of_writes))
               END AS [Avg Bytes/Transfer]
FROM (SELECT LEFT(UPPER(mf.physical_name), 2) AS Drive, SUM(num_of_reads) AS num_of_reads,
                        SUM(io_stall_read_ms) AS io_stall_read_ms,
                        SUM(num_of_writes) AS num_of_writes,
                        SUM(io_stall_write_ms) AS io_stall_write_ms,
                        SUM(num_of_bytes_read) AS num_of_bytes_read,
                        SUM(num_of_bytes_written) AS num_of_bytes_written,
                        SUM(io_stall) AS io_stall
      FROM sys.dm_io_virtual_file_stats(NULL, NULL) AS vfs
      INNER JOIN sys.master_files AS mf WITH (NOLOCK)
      ON vfs.database_id = mf.database_id AND vfs.file_id = mf.file_id
      GROUP BY LEFT(UPPER(mf.physical_name), 2)) AS tab
ORDER BY [Overall Latency] OPTION (RECOMPILE);";

        /// <summary>
        /// Analyzes the specified connection's source database.
        /// </summary>
        /// <param name="results">The results of the analysis.</param>
        /// <returns>The list of suggestions for the database.</returns>
        public override IEnumerable<Finding> Analyze(IEnumerable<dynamic> results)
        {
            return results.Select(x => new Finding("This is the latency for the drive.", x, ""));
        }
    }
}