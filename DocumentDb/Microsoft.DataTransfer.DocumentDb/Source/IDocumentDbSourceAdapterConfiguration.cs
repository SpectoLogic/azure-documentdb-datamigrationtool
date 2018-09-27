using Microsoft.DataTransfer.DocumentDb.Shared;
using System;
using System.ComponentModel.DataAnnotations;

namespace Microsoft.DataTransfer.DocumentDb.Source
{
    /// <summary>
    /// Contains configuration for DocumentDB data source adapter.
    /// </summary>
    public interface IDocumentDbSourceAdapterConfiguration : IDocumentDbAdapterConfiguration
    {
        /// <summary>
        /// Gets the documents collection name pattern.
        /// </summary>
        [Display(ResourceType = typeof(ConfigurationResources), Description = "Source_Collection")]
        string Collection { get; }

        /// <summary>
        /// Gets the value that indicates whether internal DocumentDB fields should be emitted in the output.
        /// </summary>
        [Display(ResourceType = typeof(ConfigurationResources), Description = "Source_InternalFields")]
        bool InternalFields { get; }

        /// <summary>
        /// Gets the documents query.
        /// </summary>
        [Display(ResourceType = typeof(ConfigurationResources), Description = "Source_Query")]
        string Query { get; }

        /// <summary>
        /// Gets the path to the file that contains documents query.
        /// </summary>
        [Display(ResourceType = typeof(ConfigurationResources), Description = "Source_QueryFile")]
        string QueryFile { get; }

        /// <summary>
        /// Should we read the change feed instead of querying the collection?
        /// </summary>
        [Display(ResourceType = typeof(ConfigurationResources), Description = "Source_UseChangeFeed")]
        bool UseChangeFeed { get; }

        /// <summary>
        /// Reference to a file that has continuation tokens from a previous import
        /// stored. These will be used to continue from there.
        /// </summary>
        [Display(ResourceType = typeof(ConfigurationResources), Description = "Source_ContinuationTokensFileName")]
        string ContinuationTokensFileName { get; }

        /// <summary>
        /// Should we start reading the change feed from the beginning?
        /// </summary>
        [Display(ResourceType = typeof(ConfigurationResources), Description = "Source_StartFromBeginning")]
        bool StartFromBeginning { get; }

        /// <summary>
        /// Define a start time for the change feed processing
        /// </summary>
        [Display(ResourceType = typeof(ConfigurationResources), Description = "Source_StartTime")]
        DateTime? StartTime { get; }

        /// <summary>
        /// Defines if we should update the ContinuationTokens file with the new
        /// continuation tokens
        /// </summary>
        [Display(ResourceType = typeof(ConfigurationResources), Description = "Source_UpdateContinuationTokensFile")]
        bool UpdateContinuationTokensFile { get; }
    }
}
