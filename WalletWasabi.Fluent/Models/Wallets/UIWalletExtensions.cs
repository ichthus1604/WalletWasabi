using DynamicData;

namespace WalletWasabi.Fluent.Models.Wallets;

public static class UIWalletExtensions
{
	public static IObservable<IChangeSet<IAddress, string>> GetUnusedAddresses(this IUiWallet wallet) =>
		wallet.Addresses.Filter(x => !x.IsUsed);
}
