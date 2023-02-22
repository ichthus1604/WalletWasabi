using System.Collections.Generic;
using System.Reactive.Linq;
using Avalonia.Input.Platform;
using DynamicData;
using Moq;
using NBitcoin;
using WalletWasabi.Blockchain.Transactions;
using WalletWasabi.Fluent.Models.Wallets;
using WalletWasabi.Fluent.UIServices;
using WalletWasabi.Fluent.ViewModels.Wallets.Labels;
using WalletWasabi.Fluent.ViewModels.Wallets.Receive;
using WalletWasabi.Tests.Gui.TestDoubles;
using Xunit;

namespace WalletWasabi.Tests.Gui;

public class ReceiveAddressViewModelTests
{
	[Fact]
	public void Copy_command_should_set_address_in_clipboard()
	{
		var clipboard = Mock.Of<IClipboard>(MockBehavior.Loose);
		var context = ContextWith(clipboard);
		var sut = new ReceiveAddressViewModel(new TestWallet(), new TestAddress("SomeAddress"), context, false);

		sut.CopyAddressCommand.Execute(null);

		var mock = Mock.Get(clipboard);
		mock.Verify(x => x.SetTextAsync("SomeAddress"));
	}

	[Fact]
	public void Auto_copy_enabled_should_copy_to_clipboard()
	{
		var clipboard = Mock.Of<IClipboard>(MockBehavior.Loose);
		var context = ContextWith(clipboard);
		new ReceiveAddressViewModel(new TestWallet(), new TestAddress("SomeAddress"), context, true);
		var mock = Mock.Get(clipboard);
		mock.Verify(x => x.SetTextAsync("SomeAddress"));
	}

	// Demo 2: 
	//[Fact]
	//public void When_address_becomes_used_navigation_goes_back()
	//{
	//	var navigationService = Mock.Of<INavigationService>();
	//	var uiContext = ContextWith(navigationService);
	//	var address = new TestAddress("SomeAddress");
	//	var wallet = WalletWithAddresses(address);
	//	new ReceiveAddressViewModel(wallet, address, uiContext, true);

	//	address.IsUsed = true;

	//	Mock.Get(navigationService).Verify(x => x.GoBack(), Times.Once);
	//}

	private static IWalletModel WalletWithAddresses(TestAddress address)
	{
		return Mock.Of<IWalletModel>(x => x.Addresses == AddressList(address).Connect(null).AutoRefresh(null, null, null));
	}

	private static UIContext ContextWith(INavigationService navigationService)
	{
		return new UIContext(Mock.Of<IQrCodeGenerator>(x => x.Generate(It.IsAny<string>()) == Observable.Return(new bool[0, 0])), Mock.Of<IClipboard>(), Mock.Of<IDialogService>(), navigationService);
	}

	private static UIContext ContextWith(IClipboard clipboard)
	{
		return new UIContext(Mock.Of<IQrCodeGenerator>(x => x.Generate(It.IsAny<string>()) == Observable.Return(new bool[0, 0])), clipboard, Mock.Of<IDialogService>(), Mock.Of<INavigationService>(MockBehavior.Loose));
	}

	private static ISourceCache<IAddress, string> AddressList(params IAddress[] addresses)
	{
		var cache = new SourceCache<IAddress, string>(s => s.Text);
		cache.PopulateFrom(addresses.ToObservable());
		return cache;
	}

	private class TestWallet : IWalletModel
	{
		public TestWallet()
		{
			Addresses = new SourceCache<IAddress, string>(address => address.Text).Connect();
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
			return false;
		}
	}
}
