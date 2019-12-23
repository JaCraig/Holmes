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
    /// Data class for an individual
    /// </summary>
    public class Finding
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Finding"/> class.
        /// </summary>
        /// <param name="text">The text.</param>
        /// <param name="metrics">The metrics.</param>
        /// <param name="fix">The fix.</param>
        public Finding(string text, IDictionary<string, object> metrics, string fix)
        {
            Text = text ?? "";
            Metrics = metrics ?? new Dictionary<string, object>();
            Fix = fix ?? "";
        }

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
        /// Gets the text of the finding.
        /// </summary>
        /// <value>The text of the finding.</value>
        public string Text { get; }

        /// <summary>
        /// Returns a <see cref="string"/> that represents this instance.
        /// </summary>
        /// <returns>A <see cref="string"/> that represents this instance.</returns>
        public override string ToString()
        {
            return Text;
        }
    }
}