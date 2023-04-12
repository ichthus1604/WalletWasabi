using System.Collections.Generic;
using System.Linq;
using System.Reactive;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using Avalonia;
using NBitcoin;
using ReactiveUI;
using WalletWasabi.Fluent.Helpers;
using WalletWasabi.Fluent.ViewModels.Navigation;

namespace WalletWasabi.Fluent.ViewModels.AddWallet.Create;

[NavigationMetaData(Title = "Recovery Words")]
public partial class RecoveryWordsViewModel : RoutableViewModel
{
	private const int ClipboardAutoCleanTimeInSeconds = 30;

	public RecoveryWordsViewModel(Mnemonic mnemonic, string walletName)
	{
		MnemonicWords = mnemonic.Words.Select((w, i) => new RecoveryWordViewModel(i + 1, w)).ToList();
		
		EnableBack = true;

		NextCommand = ReactiveCommand.Create(() => OnNext(mnemonic, walletName));
		CancelCommand = ReactiveCommand.Create(OnCancel);
		var copyCommand = ReactiveCommand.CreateFromTask(OnCopyToClipboardAsync);
		CopyToClipboardCommand = copyCommand;
		
		ClipboardAutocleaner(copyCommand, TimeSpan.FromSeconds(ClipboardAutoCleanTimeInSeconds)).Subscribe();
	}

	private IObservable<Unit> ClipboardAutocleaner(IObservable<Unit> trigger, TimeSpan autocleanTime)
	{
		if (Application.Current?.Clipboard is null)
		{
			return Observable.Empty<Unit>();
		}

		return trigger
			.Throttle(autocleanTime, RxApp.MainThreadScheduler)
			.WithLatestFrom(ApplicationHelper.ClipboardTextChanged(), (_, clipboardText) => clipboardText == FormattedMnemonicWords)
			.Where(isEqual => isEqual)
			.SelectMany(_ => Observable.FromAsync(() => Application.Current.Clipboard.ClearAsync(), RxApp.MainThreadScheduler));
	}

	public ICommand CopyToClipboardCommand { get; }

	public List<RecoveryWordViewModel> MnemonicWords { get; set; }

	private string FormattedMnemonicWords => string.Join(", ", MnemonicWords.Select(x => x.Word));

	private void OnNext(Mnemonic mnemonic, string walletName)
	{
		Navigate().To(new ConfirmRecoveryWordsViewModel(MnemonicWords, mnemonic, walletName));
	}

	private void OnCancel()
	{
		Navigate().Clear();
	}

	private async Task OnCopyToClipboardAsync()
	{
		if (Application.Current?.Clipboard is null)
		{
			return;
		}
		
		await Application.Current.Clipboard.SetTextAsync(FormattedMnemonicWords);
	}

	protected override void OnNavigatedTo(bool isInHistory, CompositeDisposable disposables)
	{
		var enableCancel = Services.WalletManager.HasWallet();
		SetupCancel(enableCancel: enableCancel, enableCancelOnEscape: enableCancel, enableCancelOnPressed: false);

		base.OnNavigatedTo(isInHistory, disposables);
	}
}
