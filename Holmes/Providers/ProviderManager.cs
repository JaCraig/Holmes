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
using SQLHelper.HelperClasses.Interfaces;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;

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
        public ProviderManager()
            : this(new List<IAnalyzer>())
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ProviderManager"/> class.
        /// </summary>
        /// <param name="analyzers">The analyzers.</param>
        public ProviderManager(IEnumerable<IAnalyzer> analyzers)
        {
            Analyzers = new ListMapping<DbProviderFactory, IAnalyzer>();
            foreach (var Analyzer in analyzers)
            {
                Analyzers.Add(Analyzer.SupportedFactory, Analyzer);
            }
        }

        /// <summary>
        /// Gets the analyers.
        /// </summary>
        /// <value>The analyers.</value>
        private ListMapping<DbProviderFactory, IAnalyzer> Analyzers { get; }

        /// <summary>
        /// Analyzes the specified connection.
        /// </summary>
        /// <param name="connection">The connection.</param>
        /// <returns>The results</returns>
        public IEnumerable<Finding> Analyze(IConnection connection)
        {
            if (!Analyzers.ContainsKey(connection.Factory))
                return new List<Finding>();
            SQLHelper.SQLHelper Batch = new SQLHelper.SQLHelper(connection);
            var AnalyzersUsed = Analyzers[connection.Factory].ToArray();
            for (int x = 0; x < AnalyzersUsed.Length; ++x)
            {
                AnalyzersUsed[x].AddQuery(Batch);
            }
            var Result = Batch.Execute();
            var ReturnValue = new List<Finding>();
            for (int x = 0; x < AnalyzersUsed.Length; ++x)
            {
                ReturnValue.Add(AnalyzersUsed[x].Analyze(Result[x]));
            }
            return ReturnValue;
        }
    }
}