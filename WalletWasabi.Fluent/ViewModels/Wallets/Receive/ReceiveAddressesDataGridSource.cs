using System.Collections.ObjectModel;
using System.Runtime.InteropServices;
using Avalonia.Controls;
using Avalonia.Controls.Models.TreeDataGrid;
using Avalonia.Controls.Templates;
using WalletWasabi.Fluent.Views.Wallets.Receive.Columns;

namespace WalletWasabi.Fluent.ViewModels.Wallets.Receive;

public class ReceiveAddressesDataGridSource
{
	public static FlatTreeDataGridSource<AddressViewModel> Create(ReadOnlyObservableCollection<AddressViewModel> addresses)
	{
		return new FlatTreeDataGridSource<AddressViewModel>(addresses)
		{
			Columns =
			{
				ActionsColumn(),
				AddressColumn(),
				LabelsColumn()
			}
		};
	}

	public static IColumn<AddressViewModel> ActionsColumn()
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

	public static IColumn<AddressViewModel> AddressColumn()
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

	public static IColumn<AddressViewModel> LabelsColumn()
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
}
