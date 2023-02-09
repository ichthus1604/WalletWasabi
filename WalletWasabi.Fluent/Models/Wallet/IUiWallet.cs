using DynamicData;
using NBitcoin;
using System.Collections.Generic;
using System.Reactive;
using WalletWasabi.Blockchain.TransactionProcessing;
using WalletWasabi.Wallets;
using WalletWasabi.Blockchain.Transactions;
using WalletWasabi.Fluent.ViewModels.Wallets.Labels;

namespace WalletWasabi.Fluent.Models;

/// <summary>
/// This interface serves the purpose of enabling Mocks for unit testing of the ViewModels that consume it.
/// </summary>
public interface IUiWallet
{
	public string Name { get; }

	IObservable<IChangeSet<Address, string>> UnusedAddresses { get; }

	IObservable<IChangeSet<TransactionSummary, uint256>> Transactions { get; }

	Address CreateReceiveAddress(IEnumerable<string> destinationLabels);

	IObservable<Money> Balance { get; }

	IObservable<EventPattern<ProcessedResult?>> RelevantTransactionProcessed { get; }

	IEnumerable<(string Label, int Score)> GetMostUsedLabels(Intent intent);
}
