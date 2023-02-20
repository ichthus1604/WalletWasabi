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
		var addr = new TestAddress("Address 1");
		var source = new SourceCache<IAddress, string>(s => s.Text);
		var sut = CreateSut(source);
		source.AddOrUpdate(addr);

		addr.Hide();

		sut.Source.Items.Should().BeEmpty();
	}

	[Fact]
	public void Souce_is_synced_when_item_added()
	{
		var addr = new TestAddress("Address 1");
		var source = new SourceCache<IAddress, string>(s => s.Text);
		var sut = CreateSut(source);

		source.AddOrUpdate(addr);

		sut.Source.Items.Should().HaveCount(1);
	}

	[Fact]
	public void Same_item_is_updated()
	{
		var addr1 = new TestAddress("Address 1");
		var addr2 = new TestAddress("Address 1");

		var source = new SourceCache<IAddress, string>(s => s.Text);
		var sut = CreateSut(source);

		source.AddOrUpdate(addr1);
		source.AddOrUpdate(addr2);

		sut.Source.Items.Should().HaveCount(1);
	}

	private static ReceiveAddressesViewModel CreateSut(SourceCache<IAddress, string> source)
	{
		return new ReceiveAddressesViewModel(new TestWallet(source.Connect()), TestingUIContext.NullUIContext);
	}

	private class TestWallet : IWalletModel
	{
		public TestWallet(IObservable<IChangeSet<IAddress, string>> addresses)
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
