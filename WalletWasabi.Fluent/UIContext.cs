using Avalonia;
using Avalonia.Input.Platform;
using WalletWasabi.Fluent.Models.Wallets;
using WalletWasabi.Fluent.ViewModels.Wallets.Receive;

namespace WalletWasabi.Fluent;

#nullable disable

///// <summary>
///// Encapsulates external dependencies that might be mocked or otherwise replaced in unit tests.
///// Decoupling from Avalonia is an ultimate long-term goal that we're not targeting right now, therefore this class depends on it.
///// </summary>
//public static class UIContext
//{
//	public static void InitializeForRuntime()
//	{
//		if (Application.Current is null)
//		{
//			throw new InvalidOperationException($"Avalonia is not Initialized.");
//		}

//		Initialize(new QrGenerator(), Application.Current.Clipboard!, new DialogService());
//	}

//	public static void Initialize(IQrCodeGenerator qrCodeGenerator, IClipboard clipboard, IDialogService dialogService)
//	{
//		QrCodeGenerator = qrCodeGenerator;
//		Clipboard = clipboard;
//		DialogService = dialogService;
//	}

//	public static IQrCodeGenerator QrCodeGenerator { get; private set; }

//	public static IClipboard Clipboard { get; private set; }

//	public static IDialogService DialogService { get; private set; }
//}

public class UIContext
{
	public UIContext(IQrCodeGenerator qrCodeGenerator, IClipboard clipboard, IDialogService dialogService)
	{
		QrCodeGenerator = qrCodeGenerator;
		Clipboard = clipboard;
		DialogService = dialogService;
	}

	public IClipboard Clipboard { get; set; }
	public IQrCodeGenerator QrCodeGenerator { get; set; }
	public IDialogService DialogService { get; set; }

	public static UIContext Default => new(new QrGenerator(), Application.Current?.Clipboard, new DialogService());
}

#nullable enable
