using BigBook;
using Holmes;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SQLHelperDB.HelperClasses;
using System.Data.SqlClient;

namespace Example
{
    /// <summary>
    /// This is an example program to show how to use Holmes
    /// </summary>
    internal class Program
    {
        /// <summary>
        /// Defines the entry point of the application.
        /// </summary>
        /// <param name="args">The arguments.</param>
        private static async Task Main(string[] args)
        {
            // Setup the system by adding the services and configuration
            var Services = new ServiceCollection().AddCanisterModules()?.BuildServiceProvider();
            // Get the configuration
            var Configuration = Services?.GetService<IConfiguration>();
            // Get the Sherlock service
            var Sherlock = Services?.GetService<Sherlock>();
            if (Sherlock is null || Configuration is null)
            {
                return;
            }

            // Analyze the database specified in the configuration (Default is the connection string name) and output the results
            var Results = await Sherlock.AnalyzeAsync(new Connection(Configuration, SqlClientFactory.Instance, "Default")).ConfigureAwait(false);

            // Output the results to the console
            foreach (var Result in Results)
            {
                // The result is the result of the analysis
                Console.WriteLine("Result: {0}", Result.Text);
                // The fix is the suggested fix for the analysis (if any)
                Console.WriteLine("Fix: {0}", Result.Fix);
                // The metrics are the metrics for the analysis
                Console.WriteLine(Result.Metrics.ToString(x => x.Key + ": " + x.Value, "\n"));
            }
        }
    }
}