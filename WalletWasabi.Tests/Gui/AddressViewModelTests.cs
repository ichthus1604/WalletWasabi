using System.Threading.Tasks;
using Avalonia.Input.Platform;
using FluentAssertions;
using Moq;
using WalletWasabi.Fluent;
using WalletWasabi.Fluent.Models.Wallets;
using WalletWasabi.Fluent.ViewModels.Dialogs.Base;
using WalletWasabi.Fluent.ViewModels.Wallets.Receive;
using WalletWasabi.Tests.Gui.TestDoubles;
using Xunit;

namespace WalletWasabi.Tests.Gui;

public class AddressViewModelTests
{
	[Fact]
	public void Hide_command_should_invoke_correct_method()
	{
		var address = Mock.Of<IAddress>(MockBehavior.Loose);
		var dialogService = CreateDialogReturning(true, DialogResultKind.Normal);
		var context = new UIContext(Mock.Of<IQrCodeGenerator>(), Mock.Of<IClipboard>(), dialogService);
		var sut = new AddressViewModel(
			_ => Task.CompletedTask,
			_ => Task.CompletedTask,
			context,
			address);

		sut.HideAddressCommand.Execute(null);

		Mock.Get(address).Verify(x => x.Hide(), Times.Once);
	}

	[Fact]
	public void Properties_are_mapped()
	{
		var testAddress = new TestAddress("ad");
		var labels = new[] { "Label 1", "Label 2" };
		testAddress.SetLabels(labels);
		var sut = new AddressViewModel(_ => Task.CompletedTask, _ => Task.CompletedTask, new UIContext(null, null, null), testAddress);

		sut.Address.Should().Be(testAddress.Text);
		sut.Label.Should().BeEquivalentTo(labels);
	}

	private static IDialogService CreateDialogReturning<T>(T result, DialogResultKind dialogResultKind)
	{
		return Mock.Of<IDialogService>(x => x.Show(It.IsAny<DialogViewModelBase<T>>()) == Task.FromResult(new DialogResult<T>(result, dialogResultKind)));
	}
}
