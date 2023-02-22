using System.Linq;
using System.Reactive.Linq;
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
	private readonly IWalletModel _wallet;

	public ReceiveAddressesViewModel(IWalletModel wallet, UIContext context)
	{
		_wallet = wallet;
		_context = context;

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
		return new AddressViewModel(NavigateToAddressEditAsync, NavigateToAddressAsync, UIContext.Default, address);
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
		_context.NavigationService.Go(new ReceiveAddressViewModel(_wallet, address, UIContext.Default, Services.UiConfig.Autocopy));
	}
}
