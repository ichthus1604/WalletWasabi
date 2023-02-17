using WalletWasabi.Fluent.ViewModels.Navigation;

namespace WalletWasabi.Fluent.UIServices;

public class NavigationService : INavigationService
{
	public void Go(RoutableViewModel to)
	{
		NavigationState.Instance.DialogScreenNavigation.To(to);
	}
}
