using WalletWasabi.Fluent.ViewModels.Navigation;

namespace WalletWasabi.Fluent.UIServices;

public class NavigationService : INavigationService
{
	public NavigationState NavigationState()
	{
		return ViewModels.Navigation.NavigationState.Instance;
	}

	public void Go(RoutableViewModel to)
	{
		NavigationState.Instance.DialogScreenNavigation.To(to);
	}

	public void GoBack()
	{
		NavigationState.Instance.DialogScreenNavigation.Back();
	}
}

public class Navigate
{
	public Navigate(UIContext uIContext)
	{
		UIContext = uIContext;
	}

	public UIContext UIContext { get; }
}

public class NavigateTo
{
	public NavigateTo(UIContext uIContext)
	{
		UIContext = uIContext;
	}

	private UIContext UIContext { get; }
}
