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

using Holmes.Providers;
using SQLHelperDB.HelperClasses.Interfaces;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Holmes
{
    /// <summary>
    /// Main class
    /// </summary>
    public class Sherlock
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Sherlock"/> class.
        /// </summary>
        /// <param name="providerManager">The provider manager.</param>
        public Sherlock(ProviderManager providerManager)
        {
            ProviderManager = providerManager;
        }

        /// <summary>
        /// Gets the provider manager.
        /// </summary>
        /// <value>The provider manager.</value>
        private ProviderManager ProviderManager { get; }

        /// <summary>
        /// Analyzes the specified connection.
        /// </summary>
        /// <param name="connection">The connection.</param>
        /// <returns>The results</returns>
        public async Task<IEnumerable<Finding>> AnalyzeAsync(IConnection connection)
        {
            return (await ProviderManager.AnalyzeAsync(connection).ConfigureAwait(false)) ?? Array.Empty<Finding>();
        }
    }
}