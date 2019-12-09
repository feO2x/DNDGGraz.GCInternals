namespace ListManager.WpfClient
{
    public interface INewItemDialog : IDialog
    {
        string ItemName { get; }
    }
}