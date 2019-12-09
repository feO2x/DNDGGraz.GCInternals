using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using GongSolutions.Wpf.DragDrop;
using Light.GuardClauses;
using Light.ViewModels;

namespace ListManager.WpfClient
{
    public sealed class MainWindowViewModel : BaseNotifyPropertyChanged, IDropTarget
    {
        private readonly DelegateCommand _addListCommand;
        private readonly DelegateCommand _addListEntryCommand;
        private readonly Func<IDialog> _createConfirmRemovalDialog;
        private readonly Func<INewItemDialog> _createNewItemDialog;
        private readonly DelegateCommand _removeListCommand;
        private readonly DelegateCommand _removeListEntryCommand;
        private bool _isDialogOpen;
        private NamedList _selectedList;
        private ListEntry _selectedListEntry;

        public MainWindowViewModel(ObservableCollection<NamedList> namedLists,
                                   Func<INewItemDialog> createNewItemDialog,
                                   Func<IDialog> createConfirmRemovalDialog)
        {
            NamedLists = namedLists.MustNotBeNull(nameof(namedLists));
            _createNewItemDialog = createNewItemDialog.MustNotBeNull(nameof(createNewItemDialog));
            _createConfirmRemovalDialog = createConfirmRemovalDialog.MustNotBeNull(nameof(createConfirmRemovalDialog));
            _addListCommand = new DelegateCommand(AddList, () => !_isDialogOpen);
            _removeListCommand = new DelegateCommand(RemoveList, CanRemoveList);
            _addListEntryCommand = new DelegateCommand(AddListEntry, CanAddListEntry);
            _removeListEntryCommand = new DelegateCommand(RemoveListEntry, CanRemoveListEntry);
        }

        public ObservableCollection<NamedList> NamedLists { get; }

        public NamedList SelectedList
        {
            get => _selectedList;
            set
            {
                if (!this.SetIfDifferent(ref _selectedList, value))
                    return;

                OnPropertyChanged(nameof(SelectedListEntries));
                UpdateCommandsCanExecute();
            }
        }

        public ObservableCollection<ListEntry> SelectedListEntries => _selectedList?.Entries;

        public ListEntry SelectedListEntry
        {
            get => _selectedListEntry;
            set
            {
                if (!this.SetIfDifferent(ref _selectedListEntry, value))
                    return;

                UpdateCommandsCanExecute();
            }
        }

        public ICommand AddListCommand => _addListCommand;
        public ICommand RemoveListCommand => _removeListCommand;
        public ICommand AddListEntryCommand => _addListEntryCommand;
        public ICommand RemoveListEntryCommand => _removeListEntryCommand;

        void IDropTarget.DragOver(IDropInfo dropInfo)
        {
            if (!dropInfo.IsSameDragDropContextAsSource)
                return;

            dropInfo.Effects = DragDropEffects.Move;
            dropInfo.DropTargetAdorner = DropTargetAdorners.Insert;
            dropInfo.EffectText = "Change priority";
        }

        void IDropTarget.Drop(IDropInfo dropInfo)
        {
            if (!dropInfo.IsSameDragDropContextAsSource ||
                !(dropInfo.Data is ListEntry listEntry))
                return;

            var currentList = SelectedListEntries;
            var oldIndex = currentList.IndexOf(listEntry);
            var newIndex = dropInfo.InsertIndex;

            if (newIndex > oldIndex)
            {
                if (newIndex == currentList.Count)
                    currentList.Add(listEntry);
                else
                    currentList.Insert(newIndex, listEntry);

                currentList.RemoveAt(oldIndex);
            }
            else
            {
                currentList.Move(oldIndex, newIndex);
            }

            for (var i = 0; i < SelectedListEntries.Count; ++i)
            {
                currentList[i].Priority = i;
            }
        }

        private async void AddList()
        {
            var dialog = _createNewItemDialog();
            var wasDialogAccepted = await ShowDialogAsync(dialog);
            if (!wasDialogAccepted)
                return;

            AddNamedList(dialog.ItemName);
        }

        private void AddNamedList(string listName)
        {
            var namedList = new NamedList(listName);
            NamedLists.Add(namedList);
        }

        private bool CanRemoveList() => _selectedList != null && !_isDialogOpen;

        private async void RemoveList()
        {
            var dialog = _createConfirmRemovalDialog();
            var wasDialogAccepted = await ShowDialogAsync(dialog);
            if (!wasDialogAccepted)
                return;

            RemoveSelectedList();
        }

        private void RemoveSelectedList()
        {
            NamedLists.Remove(SelectedList);
            SelectedList = null;
            SelectedListEntry = null;
        }

        private bool CanAddListEntry() => SelectedList != null && !_isDialogOpen;

        private async void AddListEntry()
        {
            var dialog = _createNewItemDialog();
            var wasDialogAccepted = await ShowDialogAsync(dialog);
            if (!wasDialogAccepted)
                return;

            AddListEntry(dialog.ItemName);
        }

        private void AddListEntry(string entryName)
        {
            var newListEntry = new ListEntry(entryName);
            newListEntry.Priority = SelectedListEntries.Count;
            SelectedListEntries.Add(newListEntry);
        }

        private bool CanRemoveListEntry() => SelectedListEntry != null && !_isDialogOpen;

        private async void RemoveListEntry()
        {
            var dialog = _createConfirmRemovalDialog();
            var wasDialogAccepted = await ShowDialogAsync(dialog);
            if (!wasDialogAccepted)
                return;

            RemoveSelectedListEntry();
        }

        private void RemoveSelectedListEntry()
        {
            SelectedListEntries.Remove(SelectedListEntry);
            SelectedListEntry = null;
        }

        private void UpdateCommandsCanExecute()
        {
            _addListCommand.RaiseCanExecuteChanged();
            _removeListCommand.RaiseCanExecuteChanged();
            _addListEntryCommand.RaiseCanExecuteChanged();
            _removeListEntryCommand.RaiseCanExecuteChanged();
        }

        private async Task<bool> ShowDialogAsync(IDialog dialog)
        {
            _isDialogOpen = true;
            UpdateCommandsCanExecute();
            var wasDialogAccepted = await dialog.ShowDialogAsync();
            _isDialogOpen = false;
            UpdateCommandsCanExecute();
            return wasDialogAccepted;
        }
    }
}