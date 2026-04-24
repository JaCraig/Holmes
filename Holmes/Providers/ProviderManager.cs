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
using Holmes.Interfaces;
using SQLHelperDB;
using SQLHelperDB.HelperClasses.Interfaces;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Holmes.Providers
{
    /// <summary>
    /// Provider manager
    /// </summary>
    public class ProviderManager
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ProviderManager"/> class.
        /// </summary>
        /// <param name="analyzers">The analyzers.</param>
        /// <param name="helper">The helper.</param>
        public ProviderManager(IEnumerable<IAnalyzer> analyzers, SQLHelper helper)
        {
            Analyzers = new ListMapping<DbProviderFactory, IAnalyzer>();
            foreach (var Analyzer in analyzers ?? [])
            {
                foreach (var SupportedFactory in Analyzer.SupportedFactories)
                {
                    Analyzers.Add(SupportedFactory, Analyzer);
                }
            }

            Batch = helper;
        }

        /// <summary>
        /// Gets the analyers.
        /// </summary>
        /// <value>The analyers.</value>
        private ListMapping<DbProviderFactory, IAnalyzer> Analyzers { get; }

        /// <summary>
        /// Gets the helper.
        /// </summary>
        /// <value>The helper.</value>
        private SQLHelper Batch { get; }

        /// <summary>
        /// Analyzes the specified connection.
        /// </summary>
        /// <param name="connection">The connection.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>The results</returns>
        public async Task<IEnumerable<Finding>> AnalyzeAsync(IConnection connection, CancellationToken cancellationToken = default)
        {
            if (connection is null || !Analyzers.ContainsKey(connection.Factory))
                return [];
            var BatchCopy = Batch.Copy();
            BatchCopy.CreateBatch(connection);
            var AnalyzersUsed = Analyzers[connection.Factory].ToArray();
            for (int X = 0; X < AnalyzersUsed.Length; ++X)
            {
                AnalyzersUsed[X].AddQuery(BatchCopy);
            }
            var Result = await BatchCopy.ExecuteAsync().ConfigureAwait(false);
            var ReturnValue = new List<Finding>();
            for (int X = 0; X < AnalyzersUsed.Length; ++X)
            {
                ReturnValue.AddRange(AnalyzersUsed[X].Analyze(Result[X]));
            }
            return ReturnValue;
        }
    }
}