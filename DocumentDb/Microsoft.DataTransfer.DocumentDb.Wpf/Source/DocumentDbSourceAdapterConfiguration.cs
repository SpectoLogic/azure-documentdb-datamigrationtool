using Microsoft.DataTransfer.Basics.Extensions;
using Microsoft.DataTransfer.DocumentDb.Source;
using Microsoft.DataTransfer.DocumentDb.Wpf.Shared;
using System;

namespace Microsoft.DataTransfer.DocumentDb.Wpf.Source
{
    sealed class DocumentDbSourceAdapterConfiguration : DocumentDbAdapterConfiguration<ISharedDocumentDbAdapterConfiguration>, IDocumentDbSourceAdapterConfiguration
    {
        public static readonly string CollectionPropertyName =
            ObjectExtensions.MemberName<IDocumentDbSourceAdapterConfiguration>(c => c.Collection);

        public static readonly string InternalFieldsPropertyName =
            ObjectExtensions.MemberName<IDocumentDbSourceAdapterConfiguration>(c => c.InternalFields);

        public static readonly string QueryPropertyName =
            ObjectExtensions.MemberName<IDocumentDbSourceAdapterConfiguration>(c => c.Query);

        public static readonly string QueryFilePropertyName =
            ObjectExtensions.MemberName<IDocumentDbSourceAdapterConfiguration>(c => c.QueryFile);

        public static readonly string UseChangeFeedName =
            ObjectExtensions.MemberName<IDocumentDbSourceAdapterConfiguration>(c => c.UseChangeFeed);

        public static readonly string StartFromBeginningName =
            ObjectExtensions.MemberName<IDocumentDbSourceAdapterConfiguration>(c => c.StartFromBeginning);

        public static readonly string StartTimeName =
            ObjectExtensions.MemberName<IDocumentDbSourceAdapterConfiguration>(c => c.StartTime);

        public static readonly string ContinuationTokensFileNameName =
            ObjectExtensions.MemberName<IDocumentDbSourceAdapterConfiguration>(c => c.ContinuationTokensFileName);

        public static readonly string UpdateContinuationTokensFileName =
            ObjectExtensions.MemberName<IDocumentDbSourceAdapterConfiguration>(c => c.UpdateContinuationTokensFile);

        private string collection;
        private bool internalFields;
        private bool useQueryFile;
        private string query;
        private string queryFile;
        private bool useChangeFeed = false;
        private string continuationTokensFileName = null;
        private bool startFromBeginning = true;
        private DateTime? startTime = null;
        private bool updateContinuationTokensFile = true;

        public string Collection
        {
            get { return collection; }
            set { SetProperty(ref collection, value, ValidateNonEmptyString); }
        }

        public bool InternalFields
        {
            get { return internalFields; }
            set { SetProperty(ref internalFields, value); }
        }

        public bool UseQueryFile
        {
            get { return useQueryFile; }
            set { SetProperty(ref useQueryFile, value); }
        }

        public string Query
        {
            get { return query; }
            set { SetProperty(ref query, value); }
        }

        public string QueryFile
        {
            get { return queryFile; }
            set { SetProperty(ref queryFile, value); }
        }

        public bool UseChangeFeed
        {
            get { return useChangeFeed; }
            set { SetProperty(ref useChangeFeed, value); }
        }

        public string ContinuationTokensFileName
        {
            get { return continuationTokensFileName; }
            set { SetProperty(ref continuationTokensFileName, value); }
        }

        public bool StartFromBeginning
        {
            get { return startFromBeginning; }
            set { SetProperty(ref startFromBeginning, value); }
        }

        public DateTime? StartTime
        {
            get { return startTime; }
            set { SetProperty(ref startTime, value); }
        }

        public bool UpdateContinuationTokensFile
        {
            get { return updateContinuationTokensFile; }
            set { SetProperty(ref updateContinuationTokensFile, value); }
        }

        public DocumentDbSourceAdapterConfiguration(ISharedDocumentDbAdapterConfiguration sharedConfiguration)
            : base(sharedConfiguration) { }
    }
}
