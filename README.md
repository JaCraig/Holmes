# Holmes

[![Build status](https://ci.appveyor.com/api/projects/status/w95i5th94ayc3f6q?svg=true)](https://ci.appveyor.com/project/JaCraig/holmes)

Holmes is a database analysis library. It scans a database and returns suggestions for improvement. Supports .Net Core as well as full .Net

## Setting Up the Library

Holmes relies on [Canister](https://github.com/JaCraig/Canister) in order to hook itself up. In order for this to work, you must do the following at startup:

Canister.Builder.CreateContainer(new List<ServiceDescriptor>())
                    .RegisterHolmes()
                    .Build();
					
The RegisterHolmes function is an extension method that registers it with the IoC container. When this is done, Holmes is ready to use.

## Basic Usage

The main class of interest is the Sherlock class:

	var Results = Sherlock.Analyze(new Connection(...));
	
The Sherlock class contains one function which is Analyze. This function takes a Connection object defining where the library should be pointed. It will then, based on the provider specified in the connection object, run any analysis classes that it has and returns a list of Finding objects. The Finding class looks like this:

    public class Finding
	{
		public string Fix { get; }
		public IDictionary<string, object> Metrics { get; }
		public string Text { get; }
	}
	
The Text property contains the explination of what was found, the Metrics property contains the data that was returned by the analysis object, and the Fix property contains the SQL command that can be used to remedy the issue. Note that not all analysis classes will contain a suggested fix. This is optional where as the other two properties will always contain some information.

## Adding Your Own Analyzer

The system a couple of built in analyzers:

* SQL Server
  * Recent expensive queries
  * Missing indexes
  * Overlapping indexes
  * Unused indexes
  
However you can easily add your own analyzer by simply creating a class that inherits from IAnalyzer and registering your DLL with Canister:

    Canister.Builder.CreateContainer(new List<ServiceDescriptor>())
                    .RegisterHolmes()
					.AddAssembly(typeof(MyAnalyzer).GetTypeInfo().Assembly)
                    .Build();
					
The system will then pick it up automatically and run it as well. For simple analyzers there is also an AnalyzerBaseClass that will simplify the process of setting up your analyzer. The IAnalyzer interface itself is rather simple though:

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

The SupportedFactory property is the DbProviderFactory that this analyzer should be run against. All analyzer queries are batched together by the system and run at once. As such there is an AddQuery function. With this function the system passes you the SQLHelper object it is using to batch the various queries. The one method on that you will probably use is AddQuery. The only other method is Analyze. This method recieves the results of the query as a list of dynamic objects. The names of each property is the same as the result set of the query you specified previously. In turn you should return a list of Finding objects.

## Installation

The library is available via Nuget with the package name "Holmes". To install it run the following command in the Package Manager Console:

Install-Package Holmes

## Build Process

In order to build the library you will require the following:

1. Visual Studio 2017

Other than that, just clone the project and you should be able to load the solution and build without too much effort.

