using System.Reactive.Linq;
using ReactiveUI;
using WalletWasabi.Blockchain.Analysis.Clustering;
using WalletWasabi.Blockchain.Keys;
using WalletWasabi.Fluent.Models;
using WalletWasabi.Fluent.ViewModels.Navigation;
using WalletWasabi.Fluent.ViewModels.Wallets.Labels;
using WalletWasabi.Wallets;

namespace WalletWasabi.Fluent.ViewModels.Wallets.Receive;

[NavigationMetaData(Title = "Edit Labels")]
public partial class AddressLabelEditViewModel : RoutableViewModel
{
	[AutoNotify] private bool _isCurrentTextValid;

	public AddressLabelEditViewModel(ReceiveAddressesViewModel owner, IUiWallet wallet, Address address)
	{
		SuggestionLabels = new SuggestionLabelsViewModel(wallet, Intent.Receive, 3, address.Labels);

		SetupCancel(enableCancel: true, enableCancelOnEscape: true, enableCancelOnPressed: true);

		var canExecute =
			this.WhenAnyValue(x => x.SuggestionLabels.Labels.Count, x => x.IsCurrentTextValid)
				.Select(tup =>
				{
					var (labelsCount, isCurrentTextValid) = tup;
					return labelsCount > 0 || isCurrentTextValid;
				});

		NextCommand = ReactiveCommand.Create(
			() =>
			{
				address.SetLabels(SuggestionLabels.Labels);
				owner.InitializeAddresses();
				Navigate().Back();
			},
			canExecute);
	}

	public SuggestionLabelsViewModel SuggestionLabels { get; }
}
