using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Models.TreeDataGrid;
using Avalonia.Controls.Templates;
using DynamicData;
using ReactiveUI;
using WalletWasabi.Fluent.Models;
using WalletWasabi.Fluent.ViewModels.Dialogs;
using WalletWasabi.Fluent.ViewModels.Dialogs.Base;
using WalletWasabi.Fluent.ViewModels.Navigation;
using WalletWasabi.Fluent.Views.Wallets.Receive.Columns;
using WalletWasabi.Wallets;

namespace WalletWasabi.Fluent.ViewModels.Wallets.Receive;

[NavigationMetaData(Title = "Receive Addresses")]
public partial class ReceiveAddressesViewModel : RoutableViewModel
{
	private ReadOnlyObservableCollection<AddressViewModel> _addresses;

	public ReceiveAddressesViewModel(IUiWallet wallet)
	{
		Wallet = wallet;
		_addresses = new(new ObservableCollection<AddressViewModel>());

		SetupCancel(enableCancel: true, enableCancelOnEscape: true, enableCancelOnPressed: true);

		EnableBack = true;

		// [Column]		[View]				[Header]	[Width]		[MinWidth]		[MaxWidth]	[CanUserSort]
		// Actions		ActionsColumnView	-			90			-				-			false
		// Address		AddressColumnView	Address		2*			-				-			true
		// Labels		LabelsColumnView	Labels		210			-				-			false

		Source = new FlatTreeDataGridSource<AddressViewModel>(_addresses)
		{
			Columns =
			{
				ActionsColumn(),
				AddressColumn(),
				LabelsColumn()
			}
		};

		Source.RowSelection!.SingleSelect = true;
	}

	public IUiWallet Wallet { get; }

	public FlatTreeDataGridSource<AddressViewModel> Source { get; }

	private static IColumn<AddressViewModel> ActionsColumn()
	{
		return new TemplateColumn<AddressViewModel>(
			null,
			new FuncDataTemplate<AddressViewModel>((node, ns) => new ActionsColumnView(), true),
			options: new ColumnOptions<AddressViewModel>
			{
				CanUserResizeColumn = false,
				CanUserSortColumn = false
			},
			width: new GridLength(90, GridUnitType.Pixel));
	}

	private static IColumn<AddressViewModel> AddressColumn()
	{
		return new TemplateColumn<AddressViewModel>(
			"Address",
			new FuncDataTemplate<AddressViewModel>((node, ns) => new AddressColumnView(), true),
			options: new ColumnOptions<AddressViewModel>
			{
				CanUserResizeColumn = false,
				CanUserSortColumn = true,
				CompareAscending = AddressViewModel.SortAscending(x => x.Address),
				CompareDescending = AddressViewModel.SortDescending(x => x.Address)
			},
			width: new GridLength(2, GridUnitType.Star));
	}

	private static IColumn<AddressViewModel> LabelsColumn()
	{
		return new TemplateColumn<AddressViewModel>(
			"Labels",
			new FuncDataTemplate<AddressViewModel>((node, ns) => new LabelsColumnView(), true),
			options: new ColumnOptions<AddressViewModel>
			{
				CanUserResizeColumn = false,
				CanUserSortColumn = true,
				CompareAscending = AddressViewModel.SortAscending(x => x.Label),
				CompareDescending = AddressViewModel.SortDescending(x => x.Label)
			},
			width: new GridLength(210, GridUnitType.Pixel));
	}

	protected override void OnNavigatedTo(bool isInHistory, CompositeDisposable disposables)
	{
		base.OnNavigatedTo(isInHistory, disposables);

		Wallet.UnusedAddresses
			  .Transform(CreateAddressViewModel)
			  .Bind(out _addresses)
			  .Subscribe()
			  .DisposeWith(disposables);
	}

	private AddressViewModel CreateAddressViewModel(IAddress address)
	{
		return new AddressViewModel(HideAddressAsync, NavigateToAddressEditAsync, NavigateToAddressAsync, address);
	}

	private async Task HideAddressAsync(IAddress address)
	{
		var result = await NavigateDialogAsync(new ConfirmHideAddressViewModel(address));

		if (result.Result == false)
		{
			return;
		}

		address.Hide();

		if (Application.Current is { Clipboard: { } clipboard })
		{
			var isAddressCopied = await clipboard.GetTextAsync() == address.Text;

			if (isAddressCopied)
			{
				await clipboard.ClearAsync();
			}
		}
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
		Navigate().To(new ReceiveAddressViewModel(Wallet, address));
	}
}
