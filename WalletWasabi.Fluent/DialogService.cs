#nullable disable
using System.Threading.Tasks;
using WalletWasabi.Fluent.ViewModels;
using WalletWasabi.Fluent.ViewModels.Dialogs.Base;
using WalletWasabi.Fluent.ViewModels.Navigation;
using WalletWasabi.Fluent.ViewModels.Wallets.Receive;

namespace WalletWasabi.Fluent;

public class DialogService : IDialogService
{
	public Task<DialogResult<T>> Show<T>(DialogViewModelBase<T> confirmHideAddressViewModel)
	{
		return MainViewModel.Instance.DialogScreen.NavigateDialogAsync(confirmHideAddressViewModel);
	}
}
