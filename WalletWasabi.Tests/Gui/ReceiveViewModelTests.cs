using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
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

public class ReceiveViewModelTests
{
	[Fact]
	public void Empty_address_list()
	{
		HasUnusedAddresses(_ => { }).Should().BeFalse();
	}

	[Fact]
	public void Used_address()
	{
		HasUnusedAddresses(addresses => addresses.WithUsed("addr")).Should().BeFalse();
	}

	[Fact]
	public void Unused_address()
	{
		HasUnusedAddresses(addresses => { addresses.WithUnused("addr"); }).Should().BeTrue();
	}

	[Fact]
	public void Unused_becomes_used()
	{
		HasUnusedAddresses(
			addresses =>
			{
				addresses.WithUnused("addr");
				addresses.WithUsed("addr");
			}).Should().BeFalse();
	}

	private static bool HasUnusedAddresses(Action<AddressConfiguration> configureAddresses)
	{
		var addresses = new AddressConfiguration();
		var receiveViewModel = new ReceiveViewModel(new TestWallet(addresses.Cache), Testing.NullUIContext);
		var history = receiveViewModel.HasUnusedAddresses.SubscribeList();
		configureAddresses(addresses);
		return history.Last();
	}

	private class TestWallet : IWalletModel
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
	
	private class AddressConfiguration
	{
		private readonly SourceCache<IAddress, string> _cache;

		public AddressConfiguration()
		{
			_cache = new SourceCache<IAddress, string>(address => address.Text);
		}

		public IConnectableCache<IAddress, string> Cache => _cache;

		public void WithUnused(string address)
		{
			_cache.AddOrUpdate(new TestAddress(address) { IsUsed = false });
		}

		public void WithUsed(string address)
		{
			_cache.AddOrUpdate(new TestAddress(address) { IsUsed = true });	
		}
	}
}
