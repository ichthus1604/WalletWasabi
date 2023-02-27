using System.Collections.Generic;
using System.Linq;
using WalletWasabi.Fluent.Models.Wallets;
using WalletWasabi.Fluent.UIServices;
using WalletWasabi.Fluent.ViewModels.Wallets.Receive;

namespace WalletWasabi.Fluent.ViewModels.Navigation;

public partial class NavigationStack<T> : ViewModelBase, INavigationStack<T> where T : class, INavigatable
{
	private readonly Stack<T> _backStack;
	[AutoNotify] private T? _currentPage;
	[AutoNotify] private bool _canNavigateBack;
	private bool _operationsEnabled = true;

	protected NavigationStack()
	{
		_backStack = new Stack<T>();
	}

	protected virtual void OnNavigated(T? oldPage, bool oldInStack, T? newPage, bool newInStack)
	{
	}

	protected virtual void OnPopped(T page)
	{
	}

	private void NavigationOperation(T? oldPage, bool oldInStack, T? newPage, bool newInStack)
	{
		if (_operationsEnabled)
		{
			oldPage?.OnNavigatedFrom(oldInStack);
		}

		CurrentPage = newPage;

		if (!oldInStack && oldPage is { })
		{
			OnPopped(oldPage);
		}

		if (_operationsEnabled)
		{
			OnNavigated(oldPage, oldInStack, newPage, newInStack);
		}

		if (_operationsEnabled && newPage is { })
		{
			newPage.OnNavigatedTo(newInStack);
		}

		UpdateCanNavigateBack();
	}

	protected IEnumerable<T> Stack => _backStack;

	public virtual void Clear()
	{
		Clear(false);
	}

	protected virtual void Clear(bool keepRoot)
	{
		var root = _backStack.Count > 0 ? _backStack.Last() : CurrentPage;

		if ((keepRoot && CurrentPage == root) || (!keepRoot && _backStack.Count == 0 && CurrentPage is null))
		{
			return;
		}

		var oldPage = CurrentPage;

		var oldItems = _backStack.ToList();

		_backStack.Clear();

		if (keepRoot)
		{
			foreach (var item in oldItems)
			{
				if (item != root)
				{
					OnPopped(item);
				}
			}
			CurrentPage = root;
		}
		else
		{
			foreach (var item in oldItems)
			{
				if (item is INavigatable navigatable)
				{
					navigatable.OnNavigatedFrom(false);
				}

				OnPopped(item);
			}

			CurrentPage = null;
		}

		NavigationOperation(oldPage, false, CurrentPage, CurrentPage is { });
	}

	public void BackTo(T viewmodel)
	{
		if (CurrentPage == viewmodel)
		{
			return;
		}

		if (_backStack.Contains(viewmodel))
		{
			var oldPage = CurrentPage;

			while (_backStack.Pop() != viewmodel)
			{
			}

			NavigationOperation(oldPage, false, viewmodel, true);
		}
	}

	public void BackTo<TViewModel>() where TViewModel : T
	{
		var previous = _backStack.Reverse().SingleOrDefault(x => x is TViewModel);

		if (previous is { })
		{
			BackTo(previous);
		}
	}

	public void To(T viewmodel, NavigationMode mode = NavigationMode.Normal)
	{
		var oldPage = CurrentPage;

		bool oldInStack = true;
		bool newInStack = false;

		switch (mode)
		{
			case NavigationMode.Normal:
				if (oldPage is { })
				{
					_backStack.Push(oldPage);
				}
				break;

			case NavigationMode.Clear:
				oldInStack = false;
				_operationsEnabled = false;
				Clear();
				_operationsEnabled = true;
				break;

			case NavigationMode.Skip:
				// Do not push old page on the back stack.
				break;
		}

		NavigationOperation(oldPage, oldInStack, viewmodel, newInStack);
	}

	public void Back()
	{
		if (_backStack.Count > 0)
		{
			var oldPage = CurrentPage;

			CurrentPage = _backStack.Pop();

			NavigationOperation(oldPage, false, CurrentPage, true);
		}
		else
		{
			Clear(); // in this case only CurrentPage might be set and Clear will provide correct behavior.
		}
	}

	private void UpdateCanNavigateBack()
	{
		CanNavigateBack = _backStack.Count > 0;
	}

	public IFluentRoutes<T> To(NavigationMode mode = NavigationMode.Normal)
	{
		return new FluentRoutes(this);
	}

	private class FluentRoutes : IFluentRoutes<T>
	{
		public FluentRoutes(NavigationStack<T> navigationStack)
		{
			NavigationStack = navigationStack;
		}

		public NavigationStack<T> NavigationStack { get; }

		public UIContext UIContext => NavigationStack.UIContext;

		public void To(T viewmodel, NavigationMode mode = NavigationMode.Normal)
		{
			NavigationStack.To(viewmodel, mode);
		}
	}
}

public static class FluentRoutesExtensions
{
	public static void ReceiveAddress(this IFluentRoutes<RoutableViewModel> fluentRoutes, IWalletModel wallet, IAddress model, bool isAutoCopyEnabled)
	{
		fluentRoutes.To(new ReceiveAddressViewModel(wallet, model, isAutoCopyEnabled, fluentRoutes.UIContext));
	}
}
