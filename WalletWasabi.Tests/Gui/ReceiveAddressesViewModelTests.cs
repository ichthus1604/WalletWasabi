using System.Collections.Generic;
using DynamicData;
using FluentAssertions;
using NBitcoin;
using WalletWasabi.Blockchain.Transactions;
using WalletWasabi.Fluent.Models.Wallets;
using WalletWasabi.Fluent.ViewModels.Wallets.Labels;
using WalletWasabi.Fluent.ViewModels.Wallets.Receive;
using WalletWasabi.Tests.Gui.TestDoubles;
using Xunit;

namespace WalletWasabi.Tests.Gui;

public class ReceiveAddressesViewModelTests
{
	[Fact]
	public void Hiding_address_should_remove_address_from_list()
	{
		var source = new SourceCache<IAddress, string>(s => s.Text);
		var addr = new TestAddress("Address 1");
		source.AddOrUpdate(addr);
		var sut = new ReceiveAddressesViewModel(new SomeWallet(source.Connect()));
		addr.Hide();
		sut.Source.Items.Should().BeEmpty();
	}

	public class SomeWallet : IWalletModel
	{
		public SomeWallet(IObservable<IChangeSet<IAddress, string>> addresses)
		{
			Addresses = addresses;
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
			throw new NotImplementedException();
		}

		public bool IsHardwareWallet()
		{
			throw new NotImplementedException();
		}
	}
}
