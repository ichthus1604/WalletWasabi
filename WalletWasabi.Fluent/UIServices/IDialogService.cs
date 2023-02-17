using System.Threading.Tasks;
using WalletWasabi.Fluent.ViewModels.Dialogs.Base;

namespace WalletWasabi.Fluent.UIServices;

public interface IDialogService
{
	Task<DialogResult<T>> Show<T>(DialogViewModelBase<T> confirmHideAddressViewModel);
}
