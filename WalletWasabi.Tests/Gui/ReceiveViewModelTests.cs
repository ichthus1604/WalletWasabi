using System.Reactive.Linq;
using System.Threading.Tasks;
using WalletWasabi.Fluent.ViewModels.Wallets.Receive;
using WalletWasabi.Tests.Gui.TestDoubles;
using Xunit;

namespace WalletWasabi.Tests.Gui;

public class ReceiveViewModelTests
{
	[Fact]
	public async Task Test()
	{
		var addresses = new[]
		{
			new TestAddress("A") { IsUsed = false },
		};

		var sut = new ReceiveViewModel(new TestWallet(addresses));
		var lists = await sut.IsExistingAddressesButtonVisible.ToList();
	}

	[Fact]
	public async Task Test2()
	{
		var addresses = new[]
		{
			new TestAddress("A") { IsUsed = true },
		};

		var sut = new ReceiveViewModel(new TestWallet(addresses));
		var lists = await sut.IsExistingAddressesButtonVisible.ToList();
	}
}
