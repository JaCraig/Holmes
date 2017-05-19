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

using System.Collections.Generic;
using System.Data.Common;

namespace Holmes.Interfaces
{
    /// <summary>
    /// Analyzer interface
    /// </summary>
    public interface IAnalyzer
    {
        /// <summary>
        /// Gets the factory the analyzer supports.
        /// </summary>
        /// <value>Gets the factory the analyzer supports.</value>
        DbProviderFactory SupportedFactory { get; }

        /// <summary>
        /// Adds the query the analyzer needs to the batch.
        /// </summary>
        /// <param name="batch">The batch.</param>
        /// <returns>This.</returns>
        IAnalyzer AddQuery(SQLHelper.SQLHelper batch);

        /// <summary>
        /// Analyzes the specified connection's source database.
        /// </summary>
        /// <param name="results">The results of the analysis.</param>
        /// <returns>The list of suggestions for the database.</returns>
        IEnumerable<Finding> Analyze(IEnumerable<dynamic> results);
    }
}