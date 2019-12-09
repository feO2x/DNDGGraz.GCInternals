using System.Diagnostics;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using Light.GuardClauses;
using MahApps.Metro.Controls;
using MahApps.Metro.Controls.Dialogs;

namespace ListManager.WpfClient
{
    public sealed partial class NewItemDialog : INewItemDialog
    {
        private readonly MetroWindow _owner;
        private readonly TaskCompletionSource<bool> _taskCompletionSource = new TaskCompletionSource<bool>();
        private bool _isShown;

        public NewItemDialog()
        {
            InitializeComponent();
            OkButton.IsEnabled = false;
        }

        public NewItemDialog(MetroWindow owner) : this()
        {
            _owner = owner.MustNotBeNull(nameof(owner));
        }

        public string ItemName => ItemNameBox.Text;

        public Task<bool> ShowDialogAsync()
        {
            TryShow();
            return _taskCompletionSource.Task;
        }

        private void OnListNameChanged(object sender, TextChangedEventArgs e)
        {
            OkButton.IsEnabled = !ItemNameBox.Text.IsNullOrWhiteSpace();
        }

        private async void ShowDialogAndFocusTextBox()
        {
            await _owner.ShowMetroDialogAsync(this);
            ItemNameBox.Focus();
        }

        private void OnOk(object sender, RoutedEventArgs e)
        {
            Debug.Assert(!ItemName.IsNullOrWhiteSpace());
            _taskCompletionSource.TrySetResult(true);
            TryHide();
        }

        private void OnCancel(object sender, RoutedEventArgs e)
        {
            _taskCompletionSource.TrySetResult(false);
            TryHide();
        }

        private void TryShow()
        {
            if (_isShown)
                return;
            _isShown = true;
            ShowDialogAndFocusTextBox();
        }

        private void TryHide()
        {
            if (!_isShown)
                return;
            _owner.HideMetroDialogAsync(this);
            _isShown = false;
        }
    }
}