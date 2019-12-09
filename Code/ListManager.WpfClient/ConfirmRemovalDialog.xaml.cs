using System.Threading.Tasks;
using System.Windows;
using Light.GuardClauses;
using MahApps.Metro.Controls;
using MahApps.Metro.Controls.Dialogs;

namespace ListManager.WpfClient
{
    public sealed partial class ConfirmRemovalDialog : IDialog
    {
        private readonly MetroWindow _owner;
        private readonly TaskCompletionSource<bool> _taskCompletionSource = new TaskCompletionSource<bool>();
        private bool _isShown;

        public ConfirmRemovalDialog()
        {
            InitializeComponent();
        }

        public ConfirmRemovalDialog(MetroWindow owner) : this()
        {
            _owner = owner.MustNotBeNull(nameof(owner));
        }

        public Task<bool> ShowDialogAsync()
        {
            TryShow();
            return _taskCompletionSource.Task;
        }

        private void OnOk(object sender, RoutedEventArgs e)
        {
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
            _owner.ShowMetroDialogAsync(this);
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