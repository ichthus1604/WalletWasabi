using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Windows.Input;
using DynamicData;
using ReactiveUI;
using WalletWasabi.Fluent.Extensions;
using WalletWasabi.Fluent.Models.Wallets;
using WalletWasabi.Fluent.ViewModels.Navigation;
using WalletWasabi.Fluent.ViewModels.Wallets.Labels;

namespace WalletWasabi.Fluent.ViewModels.Wallets.Receive;

[NavigationMetaData(
	Title = "Receive",
	Caption = "Displays wallet receive dialog",
	IconName = "wallet_action_receive",
	Order = 6,
	Category = "Wallet",
	Keywords = new[] { "Wallet", "Receive", "Action", },
	NavBarPosition = NavBarPosition.None,
	NavigationTarget = NavigationTarget.DialogScreen)]
public partial class ReceiveViewModel : RoutableViewModel
{
	private readonly IWalletModel _wallet;

	public ReceiveViewModel(IWalletModel wallet)
	{
		_wallet = wallet;
		SetupCancel(enableCancel: true, enableCancelOnEscape: true, enableCancelOnPressed: true);

		EnableBack = false;

		SuggestionLabels = new SuggestionLabelsViewModel(wallet, Intent.Receive, 3);

		var nextCommandCanExecute =
			SuggestionLabels
				.WhenAnyValue(x => x.Labels.Count).ToSignal()
				.Merge(SuggestionLabels.WhenAnyValue(x => x.IsCurrentTextValid).ToSignal())
				.Select(_ => SuggestionLabels.Labels.Count > 0 || SuggestionLabels.IsCurrentTextValid);

		NextCommand = ReactiveCommand.Create(OnNext, nextCommandCanExecute);

		ShowExistingAddressesCommand = ReactiveCommand.Create(OnShowExistingAddresses);

		HasUnusedAddresses =
			_wallet
				.GetUnusedAddresses()
				.ToCollection()
				.Select(x => x.Any())
				.StartWith(false);
	}

	public SuggestionLabelsViewModel SuggestionLabels { get; }

	public ICommand ShowExistingAddressesCommand { get; }
	public IObservable<bool> HasUnusedAddresses { get; }

	private void OnNext()
	{
		var address = _wallet.CreateReceiveAddress(SuggestionLabels.Labels);
		SuggestionLabels.Labels.Clear();

		Navigate().To(new ReceiveAddressViewModel(_wallet, address));
	}

	private void OnShowExistingAddresses()
	{
		Navigate().To(new ReceiveAddressesViewModel(_wallet));
	}
}
