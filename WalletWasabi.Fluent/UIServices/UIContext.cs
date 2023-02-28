using Avalonia;
using Avalonia.Input.Platform;
using WalletWasabi.Fluent.ViewModels.Navigation;

namespace WalletWasabi.Fluent.UIServices;

public class UIContext
{
	private INavigate _navigate;

	public UIContext(IQrCodeGenerator qrCodeGenerator, IClipboard clipboard)
	{
		QrCodeGenerator = qrCodeGenerator;
		Clipboard = clipboard;
	}

	public IClipboard Clipboard { get; }
	public IQrCodeGenerator QrCodeGenerator { get; }

	public void RegisterNavigation(INavigate navigate)
	{
		_navigate ??= navigate;
	}

	public INavigationStack<RoutableViewModel> Navigate(NavigationTarget target)
	{
		if (_navigate is null)
		{
			throw new InvalidOperationException("UIContext Navigation hasn't been initialized.");
		}

		return _navigate.Navigate(target);
	}

	public INavigate Navigate()
	{
		if (_navigate is null)
		{
			throw new InvalidOperationException("UIContext Navigation hasn't been initialized.");
		}

		return _navigate;
	}

	public static UIContext Default => new(new QrGenerator(), Application.Current?.Clipboard);
}

public class FluentNavigate : IFluentNavigate
{
	public FluentNavigate(UIContext uiContext)
	{
		UIContext = uiContext;
	}

	public UIContext UIContext { get; }
}

public interface IFluentNavigate
{
	UIContext UIContext { get; }
}
