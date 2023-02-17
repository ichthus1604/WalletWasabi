using Avalonia;
using Avalonia.Input.Platform;

namespace WalletWasabi.Fluent.UIServices;

#nullable disable

public class UIContext
{
	public UIContext(IQrCodeGenerator qrCodeGenerator, IClipboard clipboard, IDialogService dialogService, INavigationService navigationService)
	{
		QrCodeGenerator = qrCodeGenerator;
		Clipboard = clipboard;
		DialogService = dialogService;
		NavigationService = navigationService;
	}

	public IClipboard Clipboard { get; }
	public IQrCodeGenerator QrCodeGenerator { get; }
	public IDialogService DialogService { get; }
	public INavigationService NavigationService { get; }

	public static UIContext Default => new(new QrGenerator(), Application.Current?.Clipboard, new DialogService(), new NavigationService());
}

#nullable enable
