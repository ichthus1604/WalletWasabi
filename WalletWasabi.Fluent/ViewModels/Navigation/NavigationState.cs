using NBitcoin;
using System.Threading.Tasks;
using WalletWasabi.Fluent.UIServices;
using WalletWasabi.Fluent.ViewModels.Dialogs.Base;

namespace WalletWasabi.Fluent.ViewModels.Navigation;

public interface INavigate
{
	INavigationStack<RoutableViewModel> Navigate(NavigationTarget target);

	IFluentNavigate To();
}

public class NavigationState : INavigate
{
	public NavigationState(
		INavigationStack<RoutableViewModel> homeScreenNavigation,
		INavigationStack<RoutableViewModel> dialogScreenNavigation,
		INavigationStack<RoutableViewModel> fullScreenNavigation,
		INavigationStack<RoutableViewModel> compactDialogScreenNavigation)
	{
		HomeScreenNavigation = homeScreenNavigation;
		DialogScreenNavigation = dialogScreenNavigation;
		FullScreenNavigation = fullScreenNavigation;
		CompactDialogScreenNavigation = compactDialogScreenNavigation;
	}

	public static NavigationState Instance { get; private set; } = null!;

	public INavigationStack<RoutableViewModel> HomeScreenNavigation { get; }

	public INavigationStack<RoutableViewModel> DialogScreenNavigation { get; }

	public INavigationStack<RoutableViewModel> FullScreenNavigation { get; }

	public INavigationStack<RoutableViewModel> CompactDialogScreenNavigation { get; }

	public static void Register(
		INavigationStack<RoutableViewModel> homeScreenNavigation,
		INavigationStack<RoutableViewModel> dialogScreenNavigation,
		INavigationStack<RoutableViewModel> fullScreenNavigation,
		INavigationStack<RoutableViewModel> compactDialogScreenNavigation)
	{
		Instance = new NavigationState(
			homeScreenNavigation,
			dialogScreenNavigation,
			fullScreenNavigation,
			compactDialogScreenNavigation);
	}

	public INavigationStack<RoutableViewModel> Navigate(NavigationTarget target)
	{
		return target switch
		{
			NavigationTarget.HomeScreen => HomeScreenNavigation,
			NavigationTarget.DialogScreen => DialogScreenNavigation,
			NavigationTarget.FullScreen => FullScreenNavigation,
			NavigationTarget.CompactDialogScreen => CompactDialogScreenNavigation,
			_ => throw new NotSupportedException(),
		};
	}

	public IFluentNavigate To()
	{
		return new FluentNavigate(this);
	}
}

public static class NavigateDialogExtensions
{
	public static async Task<DialogResult<TResult>> NavigateDialogAsync<TResult>(this INavigate navigate, DialogViewModelBase<TResult> dialog)
		=> await NavigateDialogAsync(navigate, dialog, dialog.CurrentTarget);

	public static async Task<DialogResult<TResult>> NavigateDialogAsync<TResult>(this INavigate navigate, DialogViewModelBase<TResult> dialog, NavigationTarget target, NavigationMode navigationMode = NavigationMode.Normal)
	{
		return await navigate.Navigate(target).NavigateDialogAsync(dialog, navigationMode);
	}

	public static async Task<DialogResult<TResult>> NavigateDialogAsync<TResult>(this INavigationStack<RoutableViewModel> navigate, DialogViewModelBase<TResult> dialog, NavigationMode navigationMode = NavigationMode.Normal)
	{
		var dialogTask = dialog.GetDialogResultAsync();

		navigate.To(dialog, navigationMode);

		var result = await dialogTask;

		navigate.Back();

		return result;
	}

	public static void To<T>(this INavigate navigate, T viewModel, NavigationTarget target = NavigationTarget.Default, NavigationMode mode = NavigationMode.Normal) where T : RoutableViewModel
	{
		var actualTarget = target;
		if (actualTarget == NavigationTarget.Default)
		{
			actualTarget = viewModel.CurrentTarget;
		}

		if (actualTarget == NavigationTarget.Default)
		{
			actualTarget = viewModel.DefaultTarget;
		}

		navigate.Navigate(actualTarget).To(viewModel, mode);
	}
}
