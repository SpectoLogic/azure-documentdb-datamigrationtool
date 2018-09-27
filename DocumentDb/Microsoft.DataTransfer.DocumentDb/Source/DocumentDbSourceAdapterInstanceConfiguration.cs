using System;

namespace Microsoft.DataTransfer.DocumentDb.Source
{
    sealed class DocumentDbSourceAdapterInstanceConfiguration : IDocumentDbSourceAdapterInstanceConfiguration
    {
        public string Collection { get; set; }
        public string Query { get; set; }
        public bool UseChangeFeed { get; set; }
        public bool StartFromBeginning { get; set; }
        public DateTime? StartTime { get; set; }
        public string ContinuationTokensFileName { get; set; }
        public bool UpdateContinuationTokensFile { get; set; }
    }
}
