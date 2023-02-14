using System.Linq;
using System.Threading.Tasks;
using DynamicData;
using WalletWasabi.Fluent.Models.Wallets;
using WalletWasabi.Fluent.ViewModels.Wallets.Receive;
using WalletWasabi.Tests.Gui.TestDoubles;
using Xunit;

namespace WalletWasabi.Tests.Gui;

public class ReceiveViewModelTests
{
	[Fact]
	public async Task Empty_address_list()
	{
		HasAnyUnusedAddresses(_ => { }, false);
	}

	[Fact]
	public async Task Used_address()
	{
		HasAnyUnusedAddresses(addresses => addresses.Used("addr"), false);
	}

	[Fact]
	public async Task Unused_address()
	{
		HasAnyUnusedAddresses(
			addresses =>
			{
				addresses.Unused("addr");
			},
			true);
	}

	[Fact]
	public async Task Unused_becomes_used()
	{
		HasAnyUnusedAddresses(
			addresses =>
			{
				addresses.Unused("addr");
				addresses.Used("addr");
			},
			false);
	}

	private static void HasAnyUnusedAddresses(Action<AddressConfiguration> configureAddresses, bool statuses)
	{
		var addresses = new AddressConfiguration();
		var receiveViewModel = new ReceiveViewModel(new TestWallet(addresses.Cache));
		var expected = receiveViewModel.HasUnusedAddresses.SubscribeList();
		configureAddresses(addresses);
		Assert.Equal(statuses, expected.Last());
	}

	private class AddressConfiguration
	{
		private readonly SourceCache<IAddress, string> _cache;

		public AddressConfiguration()
		{
			_cache = new SourceCache<IAddress, string>(address => address.Text);
		}

		public IConnectableCache<IAddress, string> Cache => _cache;

		public void Unused(string address)
		{
			_cache.AddOrUpdate(new TestAddress(address) { IsUsed = false });
		}

		public void Used(string address)
		{
			_cache.AddOrUpdate(new TestAddress(address) { IsUsed = true });	
		}
	}
}
