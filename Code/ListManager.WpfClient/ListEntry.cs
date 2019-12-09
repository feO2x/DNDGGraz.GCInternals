using Light.GuardClauses;

namespace ListManager.WpfClient
{
    public sealed class ListEntry
    {
        private int _priority;

        public ListEntry(string name) =>
            Name = name.MustNotBeNullOrWhiteSpace(nameof(name));

        public string Name { get; }

        public int Priority
        {
            get => _priority;
            set => _priority = value.MustNotBeLessThan(0);
        }

        public override string ToString() => Name;
    }
}