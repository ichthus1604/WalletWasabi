using System.Collections.Generic;
using System.Reactive;
using System.Threading.Tasks;
using System.Windows.Input;
using Avalonia;
using ReactiveUI;
using WalletWasabi.Fluent.Models.Wallets;
using WalletWasabi.Fluent.UIServices;
using WalletWasabi.Fluent.ViewModels.Dialogs;
using AddressAction = System.Func<WalletWasabi.Fluent.Models.Wallets.IAddress, System.Threading.Tasks.Task>;

namespace WalletWasabi.Fluent.ViewModels.Wallets.Receive;

public partial class AddressViewModel
{
	private readonly UIContext _context;
	private readonly IAddress _model;
	[AutoNotify] private string _address;
	[AutoNotify] private IEnumerable<string> _label;

	public AddressViewModel(AddressAction onEdit, AddressAction onShow, UIContext context, IAddress model)
	{
		_context = context;
		_model = model;
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
			ReactiveCommand.CreateFromTask(PromptHideAddress);

		EditLabelCommand =
			ReactiveCommand.CreateFromTask(() => onEdit(model));

		NavigateCommand =
			ReactiveCommand.CreateFromTask(() => onShow(model));
	}

	private async Task PromptHideAddress()
	{
		var result = await _context.DialogService.Show(new ConfirmHideAddressViewModel(_model));

		if (result.Result == false)
		{
			return;
		}

		_model.Hide();
		
		var isAddressCopied = await _context.Clipboard.GetTextAsync() == _model.Text;

		if (isAddressCopied)
		{
			await _context.Clipboard.ClearAsync();
		}
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
