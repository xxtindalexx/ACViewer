using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace ACViewer.View
{
    /// <summary>
    /// History panel - shows chronological log of viewed files
    /// </summary>
    public partial class HistoryPanel : UserControl
    {
        public static HistoryPanel Instance { get; set; }

        public ObservableCollection<HistoryItem> HistoryItems { get; set; }

        public HistoryPanel()
        {
            InitializeComponent();
            Instance = this;

            HistoryItems = new ObservableCollection<HistoryItem>();
            HistoryListBox.ItemsSource = HistoryItems;
        }

        public void RefreshHistory()
        {
            if (FileExplorer.Instance?.History == null)
                return;

            var history = FileExplorer.Instance.History;
            var backList = history.GetBackList();
            var forwardList = history.GetForwardList();

            HistoryItems.Clear();

            // Add forward history (newest first)
            foreach (var did in forwardList.AsEnumerable().Reverse())
            {
                var label = FileExplorer.Instance.GetFileTypeLabel(did);
                HistoryItems.Add(new HistoryItem { DID = did, Label = label, Offset = forwardList.Count - forwardList.IndexOf(did) });
            }

            // Add current item (if there is history)
            if (backList.Count > 0 || forwardList.Count > 0)
            {
                // Get current DID from history
                // Since backList is "items before current" and forwardList is "items after current",
                // we need to track the current item separately
                // For now, we'll mark the boundary between back and forward
            }

            // Add back history (most recent first)
            foreach (var did in backList)
            {
                var label = FileExplorer.Instance.GetFileTypeLabel(did);
                HistoryItems.Add(new HistoryItem { DID = did, Label = label, Offset = -(backList.IndexOf(did) + 1) });
            }
        }

        private void HistoryListBox_OnClick(object sender, MouseButtonEventArgs e)
        {
            var selected = ItemsControl.ContainerFromElement(sender as ListBox, e.OriginalSource as DependencyObject) as ListBoxItem;
            if (selected == null)
                return;

            var historyItem = selected.Content as HistoryItem;
            if (historyItem == null)
                return;

            // Navigate to this history item by offset
            FileExplorer.Instance.NavigateToHistoryOffset(historyItem.Offset);
        }
    }

    public class HistoryItem
    {
        public uint DID { get; set; }
        public string Label { get; set; }
        public int Offset { get; set; }

        public override string ToString()
        {
            return Label;
        }
    }
}
