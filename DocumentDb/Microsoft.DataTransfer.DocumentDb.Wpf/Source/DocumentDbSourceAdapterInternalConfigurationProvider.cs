using Microsoft.DataTransfer.Basics;
using Microsoft.DataTransfer.DocumentDb.Wpf.Shared;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Windows.Controls;

namespace Microsoft.DataTransfer.DocumentDb.Wpf.Source
{
    sealed class DocumentDbSourceAdapterInternalConfigurationProvider : 
        DocumentDbAdapterConfigurationProvider<DocumentDbSourceAdapterConfiguration, ISharedDocumentDbAdapterConfiguration>
    {
        protected override UserControl CreatePresenter(DocumentDbSourceAdapterConfiguration configuration)
        {
            return new DocumentDbSourceAdapterConfigurationPage { DataContext = configuration };
        }

        protected override UserControl CreateSummaryPresenter(DocumentDbSourceAdapterConfiguration configuration)
        {
            return new DocumentDbSourceAdapterConfigurationSummaryPage { DataContext = configuration };
        }

        protected override DocumentDbSourceAdapterConfiguration CreateValidatableConfiguration()
        {
            return new DocumentDbSourceAdapterConfiguration(new SharedDocumentDbAdapterConfiguration());
        }

        /// <summary>
        /// </summary>
        /// <param name="configuration"></param>
        /// <param name="arguments"></param>
        protected override void PopulateCommandLineArguments(DocumentDbSourceAdapterConfiguration configuration, IDictionary<string, string> arguments)
        {
            base.PopulateCommandLineArguments(configuration, arguments);

            Guard.NotNull("configuration", configuration);
            Guard.NotNull("arguments", arguments);

            arguments.Add(DocumentDbSourceAdapterConfiguration.CollectionPropertyName, configuration.Collection);

            if (configuration.InternalFields)
                arguments.Add(DocumentDbSourceAdapterConfiguration.InternalFieldsPropertyName, null);

            if (configuration.UseChangeFeed)
            {
                arguments.Add(DocumentDbSourceAdapterConfiguration.UseChangeFeedName, null);

                if (configuration.StartFromBeginning)
                    arguments.Add(DocumentDbSourceAdapterConfiguration.StartFromBeginningName, null);

                if (configuration.StartTime!=null)
                {
                    arguments.Add(DocumentDbSourceAdapterConfiguration.StartTimeName, configuration.StartTime.Value.ToString(CultureInfo.InvariantCulture));
                }

                if (!String.IsNullOrEmpty(configuration.ContinuationTokensFileName))
                    arguments.Add(DocumentDbSourceAdapterConfiguration.ContinuationTokensFileNameName, configuration.ContinuationTokensFileName);

                if (configuration.UpdateContinuationTokensFile)
                    arguments.Add(DocumentDbSourceAdapterConfiguration.UpdateContinuationTokensFileName, null);
            }
            else
            {
                if (configuration.UseQueryFile)
                {
                    if (!String.IsNullOrEmpty(configuration.QueryFile))
                        arguments.Add(DocumentDbSourceAdapterConfiguration.QueryFilePropertyName, configuration.QueryFile);
                }
                else
                {
                    if (!String.IsNullOrEmpty(configuration.Query))
                        arguments.Add(DocumentDbSourceAdapterConfiguration.QueryPropertyName, configuration.Query);
                }
            }
        }
    }
}
