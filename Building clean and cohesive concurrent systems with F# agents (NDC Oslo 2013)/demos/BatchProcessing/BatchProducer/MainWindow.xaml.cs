using System;
using System.Collections.ObjectModel;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Microsoft.FSharp.Core;

namespace BatchProducer
{
    public partial class MainWindow : Window
    {
        private readonly BatchProcessing.BatchProcessorAgent<char> _batchProcessorAgent;
        private readonly ObservableCollection<string> _batches = new ObservableCollection<string>();

        public MainWindow()
        {
            InitializeComponent();
         
            _batchProcessorAgent = new BatchProcessing.BatchProcessorAgent<char>(9, 2000, SynchronizationContext.Current.AsSome());
            _batchProcessorAgent.BatchProduced += OnBatchProduced;
        }

        public ObservableCollection<string> Batches
        {
            get { return _batches; }
        }

        void OnBatchProduced(object sender, char[] args)
        {
            var batch = string.Join(String.Empty, args);
            Batches.Add(string.Format("{0} ({1})", batch, DateTime.Now.ToLongTimeString()));
        }

        private void UIElement_OnKeyUp(object sender, KeyEventArgs e)
        {
            _batchProcessorAgent.Enqueue(CharUtil.GetCharFromKey(e.Key));
        }
    }
}
