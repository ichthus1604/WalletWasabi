using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using DynamicData;
using NBitcoin;
using WalletWasabi.Blockchain.TransactionProcessing;
using WalletWasabi.Blockchain.Transactions;
using WalletWasabi.Fluent.Models.Wallets;
using WalletWasabi.Fluent.ViewModels.Wallets.Labels;

namespace WalletWasabi.Tests.Gui.TestDoubles;

public class TestWallet : IUiWallet
{
	public TestWallet(IEnumerable<IAddress> addresses)
	{
		Addresses = addresses
			.Cast<IAddress>()
			.ToObservable()
			.ToObservableChangeSet(x => x.Text)
			.Replay()
			.RefCount();
	}

	public string Name { get; }
	public IObservable<IChangeSet<TransactionSummary, uint256>> Transactions { get; }
	public IAddress CreateReceiveAddress(IEnumerable<string> destinationLabels)
	{
		throw new NotImplementedException();
	}

	public IObservable<Money> Balance { get; }
	public IObservable<IChangeSet<IAddress, string>> Addresses { get; }
	public IObservable<EventPattern<ProcessedResult?>> RelevantTransactionProcessed { get; }
	public IEnumerable<(string Label, int Score)> GetMostUsedLabels(Intent intent)
	{
		return ImmutableArray<(string Label, int Score)>.Empty;
	}

	public bool IsHardwareWallet()
	{
		return false;
	}
}
