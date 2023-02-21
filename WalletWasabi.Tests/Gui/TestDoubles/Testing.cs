using WalletWasabi.Fluent.UIServices;

namespace WalletWasabi.Tests.Gui.TestDoubles;

public static class Testing
{
	public static UIContext NullUIContext => new(null, null, null, null);
}
