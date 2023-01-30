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
            foreach (var Analyzer in analyzers ?? Array.Empty<IAnalyzer>())
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
        /// <returns>The results</returns>
        public async Task<IEnumerable<Finding>> AnalyzeAsync(IConnection connection)
        {
            if (connection is null || !Analyzers.ContainsKey(connection.Factory))
                return Array.Empty<Finding>();
            Batch.CreateBatch(connection);
            var AnalyzersUsed = Analyzers[connection.Factory].ToArray();
            for (int x = 0; x < AnalyzersUsed.Length; ++x)
            {
                AnalyzersUsed[x].AddQuery(Batch);
            }
            var Result = await Batch.ExecuteAsync().ConfigureAwait(false);
            var ReturnValue = new List<Finding>();
            for (int x = 0; x < AnalyzersUsed.Length; ++x)
            {
                ReturnValue.Add(AnalyzersUsed[x].Analyze(Result[x]));
            }
            return ReturnValue;
        }
    }
}