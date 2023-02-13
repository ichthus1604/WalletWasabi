using DynamicData;

namespace WalletWasabi.Fluent.Models.Wallets;

public static class WalletModelExtensions
{
	public static IObservable<IChangeSet<IAddress, string>> GetUnusedAddresses(this IWalletModel wallet) =>
		wallet.Addresses.Filter(x => !x.IsUsed);
}
