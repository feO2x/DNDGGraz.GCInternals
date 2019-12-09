using System.Collections.ObjectModel;
using Light.GuardClauses;
using Light.ViewModels;

namespace ListManager.WpfClient
{
    public sealed class NamedList : BaseNotifyPropertyChanged
    {
        private string _name;

        public NamedList(string name, ObservableCollection<ListEntry> entries = null)
        {
            _name = name.MustNotBeNullOrWhiteSpace(nameof(name));
            Entries = entries ?? new ObservableCollection<ListEntry>();
        }

        public string Name
        {
            get => _name;
            set => _name = value.MustNotBeNullOrWhiteSpace();
        }

        public ObservableCollection<ListEntry> Entries { get; }

        public override string ToString() => Name;
    }
}