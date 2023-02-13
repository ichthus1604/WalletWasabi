using Avalonia;
using Avalonia.Input.Platform;
using WalletWasabi.Fluent.Models.Wallets;

namespace WalletWasabi.Fluent;

#nullable disable

/// <summary>
/// Encapsulates external dependencies that might be mocked or otherwise replaced in unit tests.
/// Decoupling from Avalonia is an ultimate long-term goal that we're not targeting right now, therefore this class depends on it.
/// </summary>
public static class UIContext
{
	public static void InitializeForRuntime()
	{
		if (Application.Current is null)
		{
			throw new InvalidOperationException($"Avalonia is not Initialized.");
		}

		Initialize(new QrGenerator(), Application.Current.Clipboard!);
	}

	public static void Initialize(IQrCodeGenerator qrCodeGenerator, IClipboard clipboard)
	{
		QrCodeGenerator = qrCodeGenerator;
		Clipboard = clipboard;
	}

	public static IQrCodeGenerator QrCodeGenerator { get; private set; }

	public static IClipboard Clipboard { get; private set; }
}

#nullable enable
