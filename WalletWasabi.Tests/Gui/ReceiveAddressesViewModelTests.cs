using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using DynamicData;
using FluentAssertions;
using NBitcoin;
using WalletWasabi.Blockchain.Transactions;
using WalletWasabi.Fluent.Models.Wallets;
using WalletWasabi.Fluent.ViewModels.Wallets.Labels;
using WalletWasabi.Fluent.ViewModels.Wallets.Receive;
using WalletWasabi.Tests.Gui.TestDoubles;
using Xunit;

#pragma warning disable CA2000

namespace WalletWasabi.Tests.Gui;

public class ReceiveAddressesViewModelTests
{
	[Fact]
	public void Adding_address_increments_source_item_count()
	{
		var addresses = AddressList("addr1");

		var sut = SutWithAddresses(addresses);

		sut.Source.Items.Should().HaveCount(1);
	}

	[Fact]
	public void Adding_two_addresses_should_reflect_on_items_source()
	{
		var addresses = AddressList("addr1", "addr2");
		var sut = SutWithAddresses(addresses);

		sut.Source.Items.Should().HaveCount(2);
	}

	[Fact]
	public void Adding_different_instances_of_same_address_does_increment_source_item_count()
	{
		var sut = SutWithAddresses(AddressList("addr1", "addr1"));

		sut.Source.Items.Should().HaveCount(1);
	}

	// Demo 1:
	// We create this test to reproduce a new issue.
	[Fact]
	public void When_address_becomes_used_its_removed_from_source()
	{
		var firstAddress = new TestAddress("addr1");
		var addressList = AddressList(firstAddress, new TestAddress("addr2"), new TestAddress("addr3"));
		var sut = SutWithAddresses(addressList);

		firstAddress.IsUsed = true;

		sut.Source.Items.Should().HaveCount(2);
	}

	private static ISourceCache<IAddress, string> AddressList(params string[] addresses)
	{
		return AddressList(addresses.Select(s => (IAddress) new TestAddress(s)).ToArray());
	}

	private static ISourceCache<IAddress, string> AddressList(params IAddress[] addresses)
	{
		var cache = new SourceCache<IAddress, string>(s => s.Text);
		cache.PopulateFrom(addresses.ToObservable());
		return cache;
	}

	private static ReceiveAddressesViewModel SutWithAddresses(IConnectableCache<IAddress, string> source)
	{
		return new ReceiveAddressesViewModel(new TestWallet(source.Connect()), Testing.NullUIContext);
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
