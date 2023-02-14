using System.Collections.Generic;
using System.Collections.Immutable;
using DynamicData;
using NBitcoin;
using WalletWasabi.Blockchain.Transactions;
using WalletWasabi.Fluent.Models.Wallets;
using WalletWasabi.Fluent.ViewModels.Wallets.Labels;

namespace WalletWasabi.Tests.Gui.TestDoubles;

public class TestWallet : IWalletModel
{
	public TestWallet(IConnectableCache<IAddress, string> addresses)
	{
		Addresses = addresses.Connect();
	}

	public string Name { get; }

	public IObservable<IChangeSet<TransactionSummary, uint256>> Transactions { get; }

	public IAddress CreateReceiveAddress(IEnumerable<string> destinationLabels)
	{
		throw new NotImplementedException();
	}

	public IObservable<Money> Balance { get; }

	public IObservable<IChangeSet<IAddress, string>> Addresses { get; }

	public IEnumerable<(string Label, int Score)> GetMostUsedLabels(Intent intent)
	{
		return ImmutableArray<(string Label, int Score)>.Empty;
	}

	public bool IsHardwareWallet()
	{
		return false;
	}
}
