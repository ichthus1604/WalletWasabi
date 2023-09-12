using System.Collections.Generic;
using System.Linq;
using System.Reactive.Concurrency;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using NBitcoin;
using ReactiveUI;
using WalletWasabi.Fluent.Models;
using WalletWasabi.Fluent.Models.UI;
using WalletWasabi.Fluent.Models.Wallets;
using WalletWasabi.Fluent.ViewModels.Navigation;
using WalletWasabi.Fluent.ViewModels.Wallets.Home.History;
using WalletWasabi.Fluent.ViewModels.Wallets.Home.Tiles;
using WalletWasabi.Fluent.ViewModels.Wallets.Send;
using WalletWasabi.Wallets;

namespace WalletWasabi.Fluent.ViewModels.Wallets;

public partial class WalletViewModel : RoutableViewModel, IWalletViewModel
{
	[AutoNotify] private double _widthSource;
	[AutoNotify] private double _heightSource;
	[AutoNotify] private bool _isPointerOver;

	[AutoNotify(SetterModifier = AccessModifier.Private)] private bool _isWalletBalanceZero;
	[AutoNotify(SetterModifier = AccessModifier.Private)] private bool _isTransactionHistoryEmpty;
	[AutoNotify(SetterModifier = AccessModifier.Private)] private bool _isSendButtonVisible;
	[AutoNotify(SetterModifier = AccessModifier.Private)] private WalletState _walletState;

	[AutoNotify(SetterModifier = AccessModifier.Protected)]
	private bool _isLoading;

	[AutoNotify(SetterModifier = AccessModifier.Protected)]
	private bool _isCoinJoining;

	public WalletViewModel(UiContext uiContext, IWalletModel wallet)
	{
		Wallet = wallet;
		UiContext = uiContext;

		_title = WalletName;

		Disposables = Disposables is null
			? new CompositeDisposable()
			: throw new NotSupportedException($"Cannot open {GetType().Name} before closing it.");

		Settings = new WalletSettingsViewModel(UiContext, Wallet);
		CoinJoinSettings = new CoinJoinSettingsViewModel(UiContext, Wallet);
		UiTriggers = new UiTriggers(this);
		History = new HistoryViewModel(UiContext, this);

		Wallet.Balances.HasBalance
					   .Subscribe(x => IsWalletBalanceZero = !x)
					   .DisposeWith(Disposables);

		Wallet.Coinjoin.IsCoinjoining
					   .BindTo(this, x => x.IsCoinJoining);

		this.WhenAnyValue(x => x.History.IsTransactionHistoryEmpty)
			.Subscribe(x => IsTransactionHistoryEmpty = x);

		this.WhenAnyValue(x => x.IsWalletBalanceZero)
			.Subscribe(_ => IsSendButtonVisible = !IsWalletBalanceZero && (!Wallet.IsWatchOnlyWallet || Wallet.IsHardwareWallet));

		IsMusicBoxVisible =
			this.WhenAnyValue(x => x.IsActive, x => x.IsWalletBalanceZero, x => x.CoinJoinStateViewModel.AreAllCoinsPrivate, x => x.IsPointerOver)
				.Throttle(TimeSpan.FromMilliseconds(200), RxApp.MainThreadScheduler)
				.Select(tuple =>
				{
					var (isSelected, isWalletBalanceZero, areAllCoinsPrivate, pointerOver) = tuple;
					return (isSelected && !isWalletBalanceZero && (!areAllCoinsPrivate || pointerOver)) && !Wallet.IsWatchOnlyWallet;
				});

		SendCommand = ReactiveCommand.Create(() => Navigate().To(new SendViewModel(UiContext, this)));

		ReceiveCommand = ReactiveCommand.Create(() => Navigate().To().Receive(Wallet));

		WalletInfoCommand = ReactiveCommand.CreateFromTask(ShowWalletInfoAsync);

		WalletStatsCommand = ReactiveCommand.Create(() => Navigate().To().WalletStats(this));

		WalletSettingsCommand = ReactiveCommand.Create(() => Navigate().To(Settings));

		WalletCoinsCommand = ReactiveCommand.Create(() => Navigate().To().WalletCoins(this));

		CoinJoinSettingsCommand = ReactiveCommand.Create(() => Navigate().To(CoinJoinSettings), Observable.Return(!Wallet.IsWatchOnlyWallet));

		CoinJoinStateViewModel = new CoinJoinStateViewModel(UiContext, Wallet);

		Tiles = GetTiles().ToList();

		this.WhenAnyValue(x => x.Settings.PreferPsbtWorkflow)
			.Do(x => this.RaisePropertyChanged(nameof(PreferPsbtWorkflow)))
			.Subscribe();
	}

	private string _title;

	public IWalletModel Wallet { get; }

	public string WalletName => Wallet.Name;

	public bool IsLoggedIn => Wallet.Auth.IsLoggedIn;

	public bool PreferPsbtWorkflow => Wallet.Settings.PreferPsbtWorkflow;

	public override string ToString() => WalletName;

	public UiTriggers UiTriggers { get; private set; }

	public CoinJoinSettingsViewModel CoinJoinSettings { get; private set; }

	public bool IsWatchOnly => Wallet.IsWatchOnlyWallet;

	public IObservable<bool> IsMusicBoxVisible { get; }

	internal CoinJoinStateViewModel CoinJoinStateViewModel { get; private set; }

	public WalletSettingsViewModel Settings { get; private set; }

	public HistoryViewModel History { get; }

	public IEnumerable<ActivatableViewModel> Tiles { get; }

	public ICommand SendCommand { get; private set; }

	public ICommand? BroadcastPsbtCommand { get; set; }

	public ICommand ReceiveCommand { get; private set; }

	public ICommand WalletInfoCommand { get; private set; }

	public ICommand WalletSettingsCommand { get; private set; }

	public ICommand WalletStatsCommand { get; private set; }

	public ICommand WalletCoinsCommand { get; private set; }

	public ICommand CoinJoinSettingsCommand { get; private set; }

	private CompositeDisposable Disposables { get; set; }

	public void NavigateAndHighlight(uint256 txid)
	{
		Navigate().To(this, NavigationMode.Clear);

		SelectTransaction(txid);
	}

	public static WalletViewModel Create(UiContext uiContext, WalletPageViewModel parent)
	{
		return parent.Wallet.KeyManager.IsHardwareWallet
			? new HardwareWalletViewModel(uiContext, parent)
			: new WalletViewModel(uiContext, parent.WalletModel);
	}

	public override string Title
	{
		get => _title;
		protected set => this.RaiseAndSetIfChanged(ref _title, value);
	}

	public void SelectTransaction(uint256 txid)
	{
		RxApp.MainThreadScheduler.Schedule(async () =>
		{
			await Task.Delay(500);
			History.SelectTransaction(txid);
		});
	}

	protected override void OnNavigatedTo(bool isInHistory, CompositeDisposable disposables)
	{
		History.Activate(disposables);

		foreach (var tile in Tiles)
		{
			tile.Activate(disposables);
		}

		Wallet.State
			  .BindTo(this, x => x.WalletState)
			  .DisposeWith(disposables);
	}

	private IEnumerable<ActivatableViewModel> GetTiles()
	{
		yield return new WalletBalanceTileViewModel(Wallet.Balances);

		if (!IsWatchOnly)
		{
			yield return new PrivacyControlTileViewModel(UiContext, this);
		}

		yield return new BtcPriceTileViewModel(Wallet.Balances);
	}

	private async Task ShowWalletInfoAsync()
	{
		if (Wallet.Auth.HasPassword)
		{
			var dialogResult = await Navigate().To().PasswordAuthDialog(Wallet).GetResultAsync();
			if (!dialogResult)
			{
				return;
			}
		}

		Navigate().To().WalletInfo(Wallet);
	}
}
