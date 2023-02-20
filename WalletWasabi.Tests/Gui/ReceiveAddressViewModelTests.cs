using System.Collections.Generic;
using Avalonia.Input.Platform;
using DynamicData;
using Moq;
using NBitcoin;
using WalletWasabi.Blockchain.Transactions;
using WalletWasabi.Fluent;
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
		var context = new UIContext(Mock.Of<IQrCodeGenerator>(MockBehavior.Loose), clipboard, Mock.Of<IDialogService>(), null);
		var sut = new ReceiveAddressViewModel(new ThisWallet(), new TestAddress("SomeAddress"), context, false);

		sut.CopyAddressCommand.Execute(null);
		
		var mock = Mock.Get(clipboard);
		mock.Verify(x => x.SetTextAsync("SomeAddress"));
	}

	private class ThisWallet : IWalletModel
	{
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
