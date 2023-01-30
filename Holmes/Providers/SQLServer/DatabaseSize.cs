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
    public class DatabaseSize : AnalyzerBaseClass
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
        protected override string QueryString { get; } = @"select
a.name,
SUM(((size*8)/1024)) [Size_MB]
FROM sys.databases a
INNER JOIN sys.master_files b ON a.database_id=b.database_id
where a.database_id=DB_ID()
group by a.name";

        /// <summary>
        /// Analyzes the specified connection's source database.
        /// </summary>
        /// <param name="results">The results of the analysis.</param>
        /// <returns>The list of suggestions for the database.</returns>
        public override IEnumerable<Finding> Analyze(IEnumerable<dynamic> results)
        {
            return results.Select(x => new Finding("The database size.", x, ""));
        }
    }
}