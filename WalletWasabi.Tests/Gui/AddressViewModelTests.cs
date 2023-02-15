using System.Threading.Tasks;
using Moq;
using WalletWasabi.Fluent.Models.Wallets;
using WalletWasabi.Fluent.ViewModels.Wallets.Receive;
using WalletWasabi.Tests.Gui.TestDoubles;
using Xunit;

namespace WalletWasabi.Tests.Gui;

public class ReceiveAddressesViewModelTests
{
	[Fact]
	public void Hide_command_should_invoke_correct_method()
	{
		var address = (IAddress)new TestAddress("Add");

		var onHide = Mock.Of<Func<IAddress, Task>>(func => func(address) == Task.CompletedTask);

		var sut = new AddressViewModel(
			onHide,
			_ => Task.CompletedTask,
			_ => Task.CompletedTask,
			address);
		
		sut.HideAddressCommand.Execute(null);

		Mock.Get(onHide).Verify(func => func(address), Times.Once);
	}
}
