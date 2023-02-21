using System.Threading.Tasks;
using Avalonia.Controls;
using DynamicData;
using WalletWasabi.Fluent.Models.Wallets;
using WalletWasabi.Fluent.UIServices;
using WalletWasabi.Fluent.ViewModels.Dialogs.Base;
using WalletWasabi.Fluent.ViewModels.Navigation;

namespace WalletWasabi.Fluent.ViewModels.Wallets.Receive;

[NavigationMetaData(Title = "Receive Addresses")]
public partial class ReceiveAddressesViewModel : RoutableViewModel
{
	public ReceiveAddressesViewModel(IWalletModel wallet, UIContext context)
	{
		Wallet = wallet;
		Context = context;

		Wallet
			.UnusedAddresses()
			.Transform(CreateAddressViewModel)
			.Bind(out var addresses)
			.Subscribe();
		Source = ReceiveAddressesDataGridSource.Create(addresses);
		Source.RowSelection!.SingleSelect = true;
		EnableBack = true;
		SetupCancel(true, true, true);
	}

	private IWalletModel Wallet { get; }
	private UIContext Context { get; }

	public FlatTreeDataGridSource<AddressViewModel> Source { get; }

	private AddressViewModel CreateAddressViewModel(IAddress address)
	{
		return new AddressViewModel(NavigateToAddressEditAsync, NavigateToAddressAsync, UIContext.Default, address);
	}

	private async Task NavigateToAddressEditAsync(IAddress address)
	{
		var result = await NavigateDialogAsync(new AddressLabelEditViewModel(Wallet, address), NavigationTarget.CompactDialogScreen);
		if (result is { Kind: DialogResultKind.Normal, Result: { } })
		{
			address.SetLabels(result.Result);
		}
	}

	private async Task NavigateToAddressAsync(IAddress address)
	{
		Context.NavigationService.Go(new ReceiveAddressViewModel(Wallet, address, UIContext.Default, Services.UiConfig.Autocopy));
	}
}
