﻿/*
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

using Canister.Interfaces;
using Holmes;
using Holmes.Interfaces;
using Holmes.Providers;

namespace Microsoft.Extensions.DependencyInjection
{
    /// <summary>
    /// Registration extension methods
    /// </summary>
    public static class RegistrationExtensions
    {
        /// <summary>
        /// Registers the library with the bootstrapper.
        /// </summary>
        /// <param name="bootstrapper">The bootstrapper.</param>
        /// <returns>The bootstrapper</returns>
        public static ICanisterConfiguration? RegisterHolmes(this ICanisterConfiguration? bootstrapper)
        {
            return bootstrapper?.AddAssembly(typeof(RegistrationExtensions).Assembly)
                                .RegisterSQLHelper();
        }

        /// <summary>
        /// Registers the Holmes services with the specified service collection.
        /// </summary>
        /// <param name="services">The service collection to add the services to.</param>
        /// <returns>The service collection with the Holmes services registered.</returns>
        public static IServiceCollection? RegisterHolmes(this IServiceCollection services)
        {
            if (services.Exists<ProviderManager>())
                return services;
            return services?.AddAllTransient<IAnalyzer>()
                         ?.AddSingleton<ProviderManager>()
                         ?.AddSingleton<Sherlock>()
                         ?.RegisterSQLHelper();
        }
    }
}