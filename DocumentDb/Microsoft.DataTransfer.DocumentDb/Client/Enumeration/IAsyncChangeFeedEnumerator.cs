using System.Collections.Generic;

namespace Microsoft.DataTransfer.DocumentDb.Client.Enumeration
{
    /// <summary>
    /// 
    /// </summary>
    public interface IAsyncChangeFeedEnumerator
    {
        /// <summary>
        /// 
        /// </summary>
        Dictionary<string, string> PartitionKeyRangeContinuationTokens { get; }
    }

}
