using System;

namespace Microsoft.DataTransfer.DocumentDb.Source
{
    interface IDocumentDbSourceAdapterInstanceConfiguration
    {
        string Collection { get; }
        string Query { get; }

        /// <summary>
        /// Should we read the change feed instead of querying the collection?
        /// </summary>
        bool UseChangeFeed { get; }
        /// <summary>
        /// Should we start reading the change feed from the beginning?
        /// </summary>
        bool StartFromBeginning { get; set; }
        /// <summary>
        /// Define a start time for the change feed processing
        /// </summary>
        DateTime? StartTime { get; set; }
        /// <summary>
        /// Reference to a file that has continuation tokens from a previous import
        /// stored. These will be used to continue from there.
        /// </summary>
        string ContinuationTokensFileName { get; set; }
        /// <summary>
        /// Defines if we should update the ContinuationTokens file with the new
        /// continuation tokens
        /// </summary>
        bool UpdateContinuationTokensFile { get; }
    }
}
