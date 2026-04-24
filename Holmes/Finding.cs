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

namespace Holmes
{
    /// <summary>
    /// Severity level of a <see cref="Finding"/>.
    /// </summary>
    public enum FindingSeverity
    {
        /// <summary>Informational — no action required.</summary>
        Info,

        /// <summary>Should be investigated and likely remediated.</summary>
        Warning,

        /// <summary>Requires immediate attention.</summary>
        Critical
    }

    /// <summary>
    /// Data class for an individual finding.
    /// </summary>
    public class Finding
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Finding"/> class.
        /// </summary>
        /// <param name="text">The text.</param>
        /// <param name="metrics">The metrics.</param>
        /// <param name="fix">The fix.</param>
        /// <param name="severity">The severity of the finding.</param>
        /// <param name="category">The category of the finding (e.g. "Index", "Query", "Storage").</param>
        public Finding(string text, IDictionary<string, object> metrics, string fix,
            FindingSeverity severity = FindingSeverity.Info, string category = "")
        {
            Text = text ?? "";
            Metrics = metrics ?? new Dictionary<string, object>();
            Fix = fix ?? "";
            Severity = severity;
            Category = category ?? "";
        }

        /// <summary>
        /// Gets the category of the finding (e.g. "Index", "Query", "Storage").
        /// </summary>
        /// <value>The category of the finding.</value>
        public string Category { get; }

        /// <summary>
        /// Gets the fix (usually a SQL statement) for the finding.
        /// </summary>
        /// <value>The fix (usually a SQL statement) for the finding.</value>
        public string Fix { get; }

        /// <summary>
        /// Gets the metrics associated with the finding.
        /// </summary>
        /// <value>The metrics associated with the finding.</value>
        public IDictionary<string, object> Metrics { get; }

        /// <summary>
        /// Gets the severity of the finding.
        /// </summary>
        /// <value>The severity of the finding.</value>
        public FindingSeverity Severity { get; }

        /// <summary>
        /// Gets the text of the finding.
        /// </summary>
        /// <value>The text of the finding.</value>
        public string Text { get; }

        /// <summary>
        /// Returns a <see cref="string"/> that represents this instance.
        /// </summary>
        /// <returns>A <see cref="string"/> that represents this instance.</returns>
        public override string ToString() => Text;
    }
}