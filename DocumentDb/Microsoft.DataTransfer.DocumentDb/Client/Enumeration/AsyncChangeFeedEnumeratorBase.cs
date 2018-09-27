using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Client;
using Microsoft.Azure.Documents.Client.TransientFaultHandling;
using Microsoft.Azure.Documents.Linq;
using Microsoft.DataTransfer.Basics;
using Microsoft.DataTransfer.Extensibility.Basics.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.DataTransfer.DocumentDb.Client.Enumeration
{
    /// <summary>
    /// Allows an asynchronous iteration over the change feed of an cosmosdb collection.
    /// The iterator iterates over all at the initialization known partition key ranges.
    /// </summary>
    /// <typeparam name="TIn"></typeparam>
    /// <typeparam name="TOut"></typeparam>
    abstract class AsyncChangeFeedEnumeratorBase<TIn, TOut> : IAsyncEnumerator<TOut>, IAsyncChangeFeedEnumerator
    {
        private IReliableReadWriteDocumentClient docDBClient = null;
        private IDocumentQuery<Document> documentQuery = null;
        private readonly ChangeFeedOptions changeFeedOptions = null;
        private readonly string collectionUri = null;

        private bool completed;

        private Task<FeedResponse<TIn>> chunkDownloadTask;
        private IEnumerator<TIn> chunkCursor;

        private int partitionKeyRangeCursor = -1;
        private List<string> partitionKeyRanges = null;
        public Dictionary<string, string> PartitionKeyRangeContinuationTokens { get; private set; } = null;

        public TOut Current { get; private set; }

        public AsyncChangeFeedEnumeratorBase(
            IReliableReadWriteDocumentClient client, 
            string collectionUri, 
            List<string> partitionKeyRanges,
            Dictionary<string, string> partitionKeyRangeContinuationTokens,
            ChangeFeedOptions changeFeedOptions):this (client, collectionUri,partitionKeyRanges,changeFeedOptions )
        {
            if (partitionKeyRangeContinuationTokens!=null)
                this.PartitionKeyRangeContinuationTokens = partitionKeyRangeContinuationTokens;
        }

        public AsyncChangeFeedEnumeratorBase(
            IReliableReadWriteDocumentClient client,
            string collectionUri, 
            List<string> partitionKeyRanges, 
            ChangeFeedOptions changeFeedOptions)
        {
            this.docDBClient = client;
            this.partitionKeyRanges = partitionKeyRanges;
            this.PartitionKeyRangeContinuationTokens = new Dictionary<string, string>();
            this.changeFeedOptions = changeFeedOptions;
            this.documentQuery = null;
            this.partitionKeyRangeCursor = -1;
            this.collectionUri = collectionUri;
            completed = false;
        }

        public async Task<bool> MoveNextAsync(CancellationToken cancellation)
        {
            if (completed)
                return false;

            if (chunkDownloadTask == null)
            {
                RequestNextChunk(cancellation);
            }

            var currentCursor = await GetCurrentChunkCursor(cancellation);
            if (currentCursor == null)
                return false;

            Current = ToOutputItem(currentCursor.Current);

            if (!currentCursor.MoveNext() && !(completed = !documentQuery.HasMoreResults))
            {
                RequestNextChunk(cancellation);
            }

            return true;
        }

        protected abstract TOut ToOutputItem(TIn input);

        private string GetCurrentParitionKeyRange()
        {
            return this.partitionKeyRanges[this.partitionKeyRangeCursor];
        }

        private async Task<IEnumerator<TIn>> GetEnumeratorForChunkDownloadTaskAndProcessContinuationTokenAsync()
        {
            FeedResponse<TIn> feedResponse = await this.chunkDownloadTask;
            this.PartitionKeyRangeContinuationTokens[this.GetCurrentParitionKeyRange()] = feedResponse.ResponseContinuation;
            return feedResponse.GetEnumerator();
        }

        private async Task<IEnumerator<TIn>> GetCurrentChunkCursor(CancellationToken cancellation)
        {
            if (chunkCursor == null)
            {
                chunkCursor = await GetEnumeratorForChunkDownloadTaskAndProcessContinuationTokenAsync();

                // New chunk: adjust to first record to make sure that there is data to read, if not - request next one
                var hasData = false;
                while (!(hasData = chunkCursor.MoveNext()) && (documentQuery.HasMoreResults || this.IsNextPartitionKeyRangeCursorValid))
                {
                    RequestNextChunk(cancellation);
                    chunkCursor = await GetEnumeratorForChunkDownloadTaskAndProcessContinuationTokenAsync();
                }

                completed = !hasData;
                if (completed)
                    TrashCan.Throw(ref chunkCursor);
            }

            return chunkCursor;
        }

        private void RequestNextChunk(CancellationToken cancellation)
        {
            PrepareDocumentQuery(cancellation);
            if (documentQuery != null)
            {
                TrashCan.Throw(ref chunkCursor);
                chunkDownloadTask = documentQuery.ExecuteNextAsync<TIn>(cancellation);
            }
        }

        /// <summary>
        /// Prepare the Document Query for the change feed for the next partition key range.
        /// </summary>
        /// <param name="cancellation"></param>
        private void PrepareDocumentQuery(CancellationToken cancellation)
        {
            // If there are no more results, proceed with the next partition range
            if ((this.documentQuery == null)|| (!documentQuery.HasMoreResults))
            {
                partitionKeyRangeCursor++;  // Advance Partition Key Range Cursor
                if (this.IsPartitionKeyRangeCursorValid)
                {
                    AdaptChangeFeedOptionsPartitionKeyRangeAndRequestContinuation();

                    IDocumentQuery<Document> documentQuery = docDBClient.CreateDocumentChangeFeedQuery(
                        this.collectionUri,
                        changeFeedOptions);

                    Guard.NotNull("documentQuery", documentQuery);
                    this.documentQuery = documentQuery;
                }
                else
                    this.documentQuery = null;
            }
        }

        private void AdaptChangeFeedOptionsPartitionKeyRangeAndRequestContinuation()
        {
            this.changeFeedOptions.PartitionKeyRangeId = this.partitionKeyRanges[this.partitionKeyRangeCursor];
            if (this.PartitionKeyRangeContinuationTokens!=null)
            {
                if (this.PartitionKeyRangeContinuationTokens.ContainsKey(this.changeFeedOptions.PartitionKeyRangeId))
                {
                    this.changeFeedOptions.RequestContinuation = this.PartitionKeyRangeContinuationTokens[this.changeFeedOptions.PartitionKeyRangeId];
                }
            }
        }

        private bool IsPartitionKeyRangeCursorValid => this.partitionKeyRangeCursor < this.partitionKeyRanges.Count;

        private bool IsNextPartitionKeyRangeCursorValid => this.partitionKeyRangeCursor+1 < this.partitionKeyRanges.Count;
        
        public virtual void Dispose()
        {
            TrashCan.Throw(ref chunkCursor);
        }
    }
}
