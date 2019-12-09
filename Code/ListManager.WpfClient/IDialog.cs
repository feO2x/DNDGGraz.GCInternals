using System.Threading.Tasks;

namespace ListManager.WpfClient
{
    public interface IDialog
    {
        Task<bool> ShowDialogAsync();
    }
}