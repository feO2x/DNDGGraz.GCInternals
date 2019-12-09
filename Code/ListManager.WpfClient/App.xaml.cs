using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Windows;
using Newtonsoft.Json;

namespace ListManager.WpfClient
{
    public sealed partial class App
    {
        private const string FilePath = "Lists.json";
        private MainWindowViewModel _mainWindowViewModel;

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            // Composition Root
            var namedLists = TryLoadListsFromFile() ?? new ObservableCollection<NamedList>();
            var mainWindow = new MainWindow();
            var createNewItemDialog = new Func<INewItemDialog>(() => new NewItemDialog(mainWindow));
            var createConfirmRemovalDialog = new Func<IDialog>(() => new ConfirmRemovalDialog(mainWindow));
            _mainWindowViewModel = new MainWindowViewModel(namedLists, createNewItemDialog, createConfirmRemovalDialog);
            mainWindow.DataContext = _mainWindowViewModel;
            MainWindow = mainWindow;
            MainWindow.Show();
        }

        protected override void OnExit(ExitEventArgs e)
        {
            TrySaveNamedLists();
            base.OnExit(e);
        }

        protected override void OnSessionEnding(SessionEndingCancelEventArgs e)
        {
            TrySaveNamedLists();
            base.OnSessionEnding(e);
        }

        private void TrySaveNamedLists()
        {
            if (_mainWindowViewModel == null)
                return;

            try
            {
                var json = JsonConvert.SerializeObject(_mainWindowViewModel.NamedLists, Formatting.Indented);
                File.WriteAllText(FilePath, json);
            }
            catch
            {
                // ignored
            }
        }

        private static ObservableCollection<NamedList> TryLoadListsFromFile()
        {
            if (!File.Exists(FilePath))
                return null;

            try
            {
                var json = File.ReadAllText(FilePath);
                return JsonConvert.DeserializeObject<ObservableCollection<NamedList>>(json);
            }
            catch
            {
                return null;
            }
        }
    }
}