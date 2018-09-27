using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Client;
using Microsoft.Azure.Documents.Client.TransientFaultHandling;
using Microsoft.Azure.Documents.Linq;
using Microsoft.DataTransfer.DocumentDb.Client.Serialization;
using System.Collections.Generic;

namespace Microsoft.DataTransfer.DocumentDb.Client.Enumeration
{
    sealed class DocumentSurrogateChangeFeedQueryAsyncEnumerator : AsyncChangeFeedEnumeratorBase<DocumentSurrogate, IReadOnlyDictionary<string, object>>
    {
        public DocumentSurrogateChangeFeedQueryAsyncEnumerator(IReliableReadWriteDocumentClient client, string collectionName, List<string> partitionKeyRanges, Dictionary<string,string> partitionKeyRangeContinuationTokens, ChangeFeedOptions options)
            : base(client, collectionName, partitionKeyRanges, partitionKeyRangeContinuationTokens, options) { }

        protected override IReadOnlyDictionary<string, object> ToOutputItem(DocumentSurrogate input)
        {
            return input.Properties;
        }
    }
}
