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
using WalletWasabi.Fluent.ViewModels.Wallets.Labels;
using WalletWasabi.Fluent.Helpers;
using System.Reactive.Subjects;
using WalletWasabi.Fluent.Extensions;

namespace WalletWasabi.Fluent.Models;

internal class UiWallet : IUiWallet
{
	private readonly Wallet _wallet;
	private readonly TransactionHistoryBuilder _historyBuilder;
	private readonly Subject<Unit> _addressUpdated;

	public UiWallet(Wallet wallet)
	{
		_wallet = wallet;
		_historyBuilder = new TransactionHistoryBuilder(_wallet);
		_addressUpdated = new();

		RelevantTransactionProcessed =
			Observable.FromEventPattern<ProcessedResult?>(_wallet, nameof(_wallet.WalletRelevantTransactionProcessed))
					  .ObserveOn(RxApp.MainThreadScheduler);

		Transactions =
			Observable.Defer(() => BuildSummary().ToObservable())
					  .Concat(RelevantTransactionProcessed.SelectMany(_ => BuildSummary()))
					  .ToObservableChangeSet(x => x.TransactionId);

		UnusedAddresses =
			Observable.Defer(() => GetAddresses().ToObservable())
					  .Concat(RelevantTransactionProcessed.ToSignal().Merge(_addressUpdated).SelectMany(_ => GetAddresses()))
					  .ToObservableChangeSet(x => x.Text)
					  .Filter(x => !x.IsUsed);
	}

	public IObservable<EventPattern<ProcessedResult?>> RelevantTransactionProcessed { get; }

	public string Name => _wallet.WalletName;

	public IObservable<Money> Balance => throw new NotImplementedException();

	public IObservable<IChangeSet<IAddress, string>> UnusedAddresses { get; }

	public IObservable<IChangeSet<TransactionSummary, uint256>> Transactions { get; }

	public IAddress CreateReceiveAddress(IEnumerable<string> destinationLabels)
	{
		var pubKey = _wallet.CreateReceiveAddress(destinationLabels);
		return new Address(_wallet, pubKey);
	}

	public bool IsHardwareWallet() => _wallet.KeyManager.IsHardwareWallet;

	public IEnumerable<(string Label, int Score)> GetMostUsedLabels(Intent intent) =>
		_wallet.GetMostUsedLabels(intent);

	private IEnumerable<TransactionSummary> BuildSummary()
	{
		return _historyBuilder.BuildHistorySummary();
	}

	private IEnumerable<IAddress> GetAddresses()
	{
		return _wallet.KeyManager
					  .GetKeys()
					  .Reverse()
					  .Select(x => new Address(_wallet, x));
	}
}
