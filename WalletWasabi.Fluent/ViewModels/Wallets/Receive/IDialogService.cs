using System.Threading.Tasks;
using WalletWasabi.Fluent.ViewModels.Dialogs.Base;

namespace WalletWasabi.Fluent.ViewModels.Wallets.Receive;

public interface IDialogService
{
	Task<DialogResult<T>> Show<T>(DialogViewModelBase<T> confirmHideAddressViewModel);
}
