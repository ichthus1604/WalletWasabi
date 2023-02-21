#nullable disable
using WalletWasabi;
using WalletWasabi.Fluent.ViewModels.Navigation;

namespace WalletWasabi.Fluent.UIServices;

public interface INavigationService
{
	public void Go(RoutableViewModel to);
	void GoBack();
}
