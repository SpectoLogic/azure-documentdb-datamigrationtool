using Microsoft.DataTransfer.Basics;
using Microsoft.DataTransfer.DocumentDb.Client;
using Microsoft.DataTransfer.DocumentDb.Client.Enumeration;
using Microsoft.DataTransfer.DocumentDb.Shared;
using Microsoft.DataTransfer.DocumentDb.Transformation;
using Microsoft.DataTransfer.Extensibility;
using Microsoft.DataTransfer.Extensibility.Basics.Collections;
using Microsoft.DataTransfer.Extensibility.Basics.Source;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.DataTransfer.DocumentDb.Source
{
    sealed class DocumentDbSourceAdapter : DocumentDbAdapterBase<IDocumentDbReadClient, IDocumentDbSourceAdapterInstanceConfiguration>, IDataSourceAdapter
    {
        private const string DocumentIdFieldName = "id";

        private IAsyncEnumerator<IReadOnlyDictionary<string, object>> documentsCursor;

        public DocumentDbSourceAdapter(IDocumentDbReadClient client, IDataItemTransformation transformation, IDocumentDbSourceAdapterInstanceConfiguration configuration)
            : base(client, transformation, configuration) { }

        public async Task InitializeAsync(CancellationToken cancellation)
        {
            if (Configuration.UseChangeFeed)
            {
                // Query change feed
                documentsCursor = await Client
                    .QueryDocumentChangeFeedAsync(
                        Configuration.Collection,
                        Configuration.StartFromBeginning,
                        Configuration.StartTime,
                        await this.LoadContinuationTokensFromFile(Configuration.ContinuationTokensFileName),
                        cancellation);
            }
            else
            {
                // Query SQL-Query
                documentsCursor = await Client
                    .QueryDocumentsAsync(Configuration.Collection, Configuration.Query, cancellation);
            }
        }

        private async Task SaveContinuationTokensToFile(string filename, Dictionary<string, string> partitionKeyRangeContinuationTokens)
        {
            if (filename == null) return;
            if (!Configuration.UpdateContinuationTokensFile) return;
            using (StreamWriter sw = new StreamWriter(filename))
            {
                await sw.WriteAsync(JsonConvert.SerializeObject(partitionKeyRangeContinuationTokens));
                sw.Close();
            }
        }

        private async Task<Dictionary<string, string>> LoadContinuationTokensFromFile(string filename)
        {
            Dictionary<string, string> partitionKeyRangeContinuationTokens = new Dictionary<string, string>();
            if (!string.IsNullOrEmpty(filename))
            {
                if (File.Exists(filename))
                {
                    using (StreamReader sr = new StreamReader(filename))
                    {
                        partitionKeyRangeContinuationTokens = JsonConvert.DeserializeObject<Dictionary<string, string>>(await sr.ReadToEndAsync());
                        sr.Close();
                    }
                }
            }
            return partitionKeyRangeContinuationTokens;
        }

        /// <summary>
        /// In case of a change feed update the continuation token file, if requested.
        /// </summary>
        /// <returns></returns>
        private async Task FinalizeRead()
        {
            if (Configuration.UseChangeFeed && Configuration.UpdateContinuationTokensFile)
            {
                var changeFeedEnumerator = documentsCursor as IAsyncChangeFeedEnumerator;
                if (changeFeedEnumerator != null)
                {
                    await this.SaveContinuationTokensToFile(
                            Configuration.ContinuationTokensFileName,
                            changeFeedEnumerator.PartitionKeyRangeContinuationTokens
                        );
                }
            }
        }

        public async Task<IDataItem> ReadNextAsync(ReadOutputByRef readOutput, CancellationToken cancellation)
        {
            if (documentsCursor == null)
                throw Errors.SourceIsNotInitialized();

            if (!(await documentsCursor.MoveNextAsync(cancellation)))
            {
                await this.FinalizeRead();
                return null;
            }

            var document = documentsCursor.Current;

            object idValue;
            if (document.TryGetValue(DocumentIdFieldName, out idValue))
                readOutput.DataItemId = idValue.ToString();

            return Transformation.Transform(new DictionaryDataItem(document));
        }

        public override void Dispose()
        {
            TrashCan.Throw(ref documentsCursor);
            base.Dispose();
        }
    }
}
