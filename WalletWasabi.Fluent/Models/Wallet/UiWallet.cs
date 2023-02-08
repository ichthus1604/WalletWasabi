using DynamicData;
using NBitcoin;
using System.Collections.Generic;
using System.Reactive.Linq;
using System.Reactive;
using WalletWasabi.Blockchain.TransactionProcessing;
using WalletWasabi.Wallets;
using WalletWasabi.Blockchain.Transactions;
using ReactiveUI;
using System.Linq;

namespace WalletWasabi.Fluent.Models;

internal class UiWallet : IUiWallet
{
	private readonly Wallet _wallet;
	private readonly TransactionHistoryBuilder _historyBuilder;

	public UiWallet(Wallet wallet)
	{
		_wallet = wallet;
		_historyBuilder = new TransactionHistoryBuilder(_wallet);

		RelevantTransactionProcessed =
			Observable.FromEventPattern<ProcessedResult?>(_wallet, nameof(_wallet.WalletRelevantTransactionProcessed))
					  .ObserveOn(RxApp.MainThreadScheduler);

		Transactions =
			Observable.Defer(() => BuildSummary().ToObservable())
					  .Concat(RelevantTransactionProcessed.SelectMany(_ => BuildSummary()))
					  .ToObservableChangeSet(x => x.TransactionId);

		UnusedAddresses =
			Observable.Defer(() => _wallet.GetUnusedAddresses().ToObservable())
					  .Concat(RelevantTransactionProcessed.SelectMany(_ => _wallet.GetUnusedAddresses()))
					  .ToObservableChangeSet(x => x.Text);
	}

	public IObservable<EventPattern<ProcessedResult?>> RelevantTransactionProcessed { get; }

	public string Name => throw new NotImplementedException();

	public IObservable<Money> Balance => throw new NotImplementedException();

	public IObservable<IChangeSet<Address, string>> UnusedAddresses { get; }

	public IObservable<IChangeSet<TransactionSummary, uint256>> Transactions { get; }

	public Address CreateReceiveAddress(IEnumerable<string> destinationLabels)
	{
		return _wallet.CreateReceiveAddress(destinationLabels);
	}

	private IEnumerable<TransactionSummary> BuildSummary()
	{
		return _historyBuilder.BuildHistorySummary();
	}
}
