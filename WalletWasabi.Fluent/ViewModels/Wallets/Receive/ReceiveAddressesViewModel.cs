using System.Collections.ObjectModel;
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
	private readonly UIContext _context;
	private ReadOnlyObservableCollection<AddressViewModel> _addresses;

	public ReceiveAddressesViewModel(IWalletModel wallet, UIContext context)
	{
		_context = context;
		Wallet = wallet;

		SetupCancel(enableCancel: true, enableCancelOnEscape: true, enableCancelOnPressed: true);

		EnableBack = true;

		Wallet.GetUnusedAddresses()
			.Transform(CreateAddressViewModel)
			.Bind(out _addresses)
			.Subscribe();

		Source = ReceiveAddressesDataGridSource.Create(_addresses);

		Source.RowSelection!.SingleSelect = true;
	}

	private IWalletModel Wallet { get; }

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
		_context.NavigationService.Go(new ReceiveAddressViewModel(Wallet, address, UIContext.Default, Services.UiConfig.Autocopy));
	}
}
