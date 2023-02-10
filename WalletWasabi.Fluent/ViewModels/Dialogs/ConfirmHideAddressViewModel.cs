using ReactiveUI;
using System.Collections.Generic;
using WalletWasabi.Fluent.ViewModels.Dialogs.Base;
using WalletWasabi.Wallets;

namespace WalletWasabi.Fluent.ViewModels.Dialogs;

public class ConfirmHideAddressViewModel : DialogViewModelBase<bool>
{
	private string _title;

	public ConfirmHideAddressViewModel(Address address)
	{
		Label = address.Labels;
		_title = "Hide Address";

		NextCommand = ReactiveCommand.Create(() => Close(result: true));
		CancelCommand = ReactiveCommand.Create(() => Close(DialogResultKind.Cancel));

		SetupCancel(enableCancel: false, enableCancelOnEscape: true, enableCancelOnPressed: true);
	}

	public IEnumerable<string> Label { get; }

	public override string Title
	{
		get => _title;
		protected set => this.RaiseAndSetIfChanged(ref _title, value);
	}
}
