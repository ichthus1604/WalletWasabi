using System.Linq;
using System.Net;
using System.Reactive.Linq;
using System.Threading.Tasks;
using Avalonia.Controls;
using DynamicData;
using WalletWasabi.Fluent.Models.Wallets;
using WalletWasabi.Fluent.UIServices;
using WalletWasabi.Fluent.ViewModels.Dialogs;
using WalletWasabi.Fluent.ViewModels.Dialogs.Base;
using WalletWasabi.Fluent.ViewModels.Navigation;

namespace WalletWasabi.Fluent.ViewModels.Wallets.Receive;

[NavigationMetaData(Title = "Receive Addresses")]
public partial class ReceiveAddressesViewModel : RoutableViewModel
{
	private readonly IWalletModel _wallet;

	public ReceiveAddressesViewModel(IWalletModel wallet)
	{
		_wallet = wallet;

		Source = CreateSource();

		Source.RowSelection!.SingleSelect = true;
		EnableBack = true;
		SetupCancel(true, true, true);
	}

	private FlatTreeDataGridSource<AddressViewModel> CreateSource()
	{
		_wallet
			.UnusedAddresses()
			.Transform(CreateAddressViewModel)
			.Bind(out var addresses)
			.Subscribe();

		var source = ReceiveAddressesDataGridSource.Create(addresses);
		return source;
	}

	public FlatTreeDataGridSource<AddressViewModel> Source { get; }

	private AddressViewModel CreateAddressViewModel(IAddress address)
	{
		return new AddressViewModel(NavigateToAddressEditAsync, NavigateToAddressAsync, address, UIContext);
	}

	private async Task NavigateToAddressEditAsync(IAddress address)
	{
		var result = await NavigateDialogAsync(new AddressLabelEditViewModel(_wallet, address), NavigationTarget.CompactDialogScreen);
		if (result is { Kind: DialogResultKind.Normal, Result: { } })
		{
			address.SetLabels(result.Result);
		}
	}

	private async Task NavigateToAddressAsync(IAddress address)
	{
		Navigate().To().ReceiveAddress(_wallet, address, Services.UiConfig.Autocopy);
		Navigate().To()
	}

	private async Task PromptHideAddress(IAddress address)
	{
		var result = await Navigate().To().ConfirmHideAddressDialog(address);

		if (result.Result == false)
		{
			return;
		}

		address.Hide();

		var isAddressCopied = await UIContext.Clipboard.GetTextAsync() == address.Text;

		if (isAddressCopied)
		{
			await UIContext.Clipboard.ClearAsync();
		}
	}
}
