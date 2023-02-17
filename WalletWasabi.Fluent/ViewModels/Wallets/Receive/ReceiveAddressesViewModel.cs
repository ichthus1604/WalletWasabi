using System.Collections.ObjectModel;
using System.Threading.Tasks;
using Avalonia.Controls;
using DynamicData;
using WalletWasabi.Fluent.Models.Wallets;
using WalletWasabi.Fluent.ViewModels.Dialogs.Base;
using WalletWasabi.Fluent.ViewModels.Navigation;

namespace WalletWasabi.Fluent.ViewModels.Wallets.Receive;

[NavigationMetaData(Title = "Receive Addresses")]
public partial class ReceiveAddressesViewModel : RoutableViewModel
{
	private ReadOnlyObservableCollection<AddressViewModel> _addresses;

	public ReceiveAddressesViewModel(IWalletModel wallet)
	{
		Wallet = wallet;

		SetupCancel(enableCancel: true, enableCancelOnEscape: true, enableCancelOnPressed: true);

		EnableBack = true;

		// [Column]		[View]				[Header]	[Width]		[MinWidth]		[MaxWidth]	[CanUserSort]
		// Actions		ActionsColumnView	-			90			-				-			false
		// Address		AddressColumnView	Address		2*			-				-			true
		// Labels		LabelsColumnView	Labels		210			-				-			false

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
		Navigate().To(new ReceiveAddressViewModel(Wallet, address, UIContext.Default, Services.UiConfig.Autocopy));
	}
}
