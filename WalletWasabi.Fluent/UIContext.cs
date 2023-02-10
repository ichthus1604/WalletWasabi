using WalletWasabi.Fluent.Models;

namespace WalletWasabi.Fluent;

public static class UIContext
{
	public static void Initialize(IQrCodeGenerator qrCodeGenerator)
	{
		QrCodeGenerator = qrCodeGenerator;
	}

	public static IQrCodeGenerator QrCodeGenerator { get; private set; }
}
