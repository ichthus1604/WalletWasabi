using System.Collections.Generic;
using System.Reactive;
using System.Windows.Input;
using Avalonia;
using ReactiveUI;
using WalletWasabi.Fluent.Models.Wallets;
using WalletWasabi.Wallets;
using AddressAction = System.Func<WalletWasabi.Fluent.Models.Wallets.IAddress, System.Threading.Tasks.Task>;

namespace WalletWasabi.Fluent.ViewModels.Wallets.Receive;

public partial class AddressViewModel : ViewModelBase
{
	[AutoNotify] private string _address;
	[AutoNotify] private IEnumerable<string> _label;

	public AddressViewModel(AddressAction onHide, AddressAction onEdit, AddressAction onShow, IAddress model)
	{
		_address = model.Text;

		model.WhenAnyValue(x => x.Labels).BindTo(this, viewModel => viewModel.Label);

		CopyAddressCommand =
			ReactiveCommand.CreateFromTask(async () =>
			{
				if (Application.Current is { Clipboard: { } clipboard })
				{
					await clipboard.SetTextAsync(Address);
				}
			});

		HideAddressCommand =
			ReactiveCommand.CreateFromTask(() => onHide(model));

		EditLabelCommand =
			ReactiveCommand.CreateFromTask(() => onEdit(model));

		NavigateCommand =
			ReactiveCommand.CreateFromTask(() => onShow(model));
	}

	public ICommand CopyAddressCommand { get; }

	public ICommand HideAddressCommand { get; }

	public ICommand EditLabelCommand { get; }

	public ReactiveCommand<Unit, Unit> NavigateCommand { get; }

	public static Comparison<AddressViewModel?> SortAscending<T>(Func<AddressViewModel, T> selector)
	{
		return (x, y) =>
		{
			if (x is null && y is null)
			{
				return 0;
			}
			else if (x is null)
			{
				return -1;
			}
			else if (y is null)
			{
				return 1;
			}
			else
			{
				return Comparer<T>.Default.Compare(selector(x), selector(y));
			}
		};
	}

	public static Comparison<AddressViewModel?> SortDescending<T>(Func<AddressViewModel, T> selector)
	{
		return (x, y) =>
		{
			if (x is null && y is null)
			{
				return 0;
			}
			else if (x is null)
			{
				return 1;
			}
			else if (y is null)
			{
				return -1;
			}
			else
			{
				return Comparer<T>.Default.Compare(selector(y), selector(x));
			}
		};
	}
}
