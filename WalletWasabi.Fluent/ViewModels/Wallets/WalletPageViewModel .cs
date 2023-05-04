using System.Linq;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using ReactiveUI;
using WalletWasabi.Fluent.Models.Wallets;
using WalletWasabi.Fluent.ViewModels.Login;
using WalletWasabi.Fluent.ViewModels.Navigation;
using WalletWasabi.Wallets;

namespace WalletWasabi.Fluent.ViewModels.Wallets;

public partial class WalletPageViewModel : ViewModelBase
{
	[AutoNotify] private bool _isLoggedIn;
	[AutoNotify] private bool _isSelected;
	[AutoNotify] private bool _isLoading;
	[AutoNotify] private string? _iconName;
	[AutoNotify] private string? _iconNameFocused;
	[AutoNotify] private WalletViewModel? _walletViewModel;
	[AutoNotify] private RoutableViewModel? _currentPage;

	private WalletPageViewModel(IWalletModel walletModel)
	{
		WalletModel = walletModel;

		// TODO: Finish partial refactor
		// Wallet property must be removed
		Wallet = Services.WalletManager.GetWallets(false).First(x => x.WalletName == walletModel.Name);

		this.WhenAnyValue(x => x.IsLoggedIn)
		.Do(isLoggedIn =>
		{
			if (!isLoggedIn && CurrentPage is not { })
			{
				CurrentPage = new LoginViewModel(UiContext, WalletModel, Wallet);
			}
			else if (isLoggedIn && CurrentPage is LoginViewModel)
			{
				CurrentPage = new LoadingViewModel(Wallet);
				IsLoading = true;
			}
		})
		.Subscribe();

		this.WhenAnyValue(x => x.WalletModel.IsLoggedIn)
			.BindTo(this, x => x.IsLoggedIn);

		this.WhenAnyObservable(x => x.WalletModel.State)
			.Where(x => x == WalletState.Started)
			.Do(_ => ShowWallet())
			.Subscribe();

		SetIcon();
	}

	public IWalletModel WalletModel { get; }
	public Wallet Wallet { get; set; }

	public string Title => WalletModel.Name;

	protected void OnActivated(CompositeDisposable disposables)
	{
		IsSelected = true;

		//this.WhenAnyValue(x => x.CurrentPage)
		//	.WhereNotNull()
		//	.Do(x => UiContext.Navigate().To(x, NavigationTarget.HomeScreen, NavigationMode.Clear))
		//	.Subscribe()
		//	.DisposeWith(disposables);
	}

	protected void OnDeactivated()
	{
		IsSelected = false;
	}

	private void ShowWallet()
	{
		WalletViewModel = WalletViewModel.Create(UiContext, this);
		CurrentPage = WalletViewModel;
		IsLoading = false;
	}

	private void SetIcon()
	{
		var walletType = WalletModel.WalletType;

		var baseResourceName = walletType switch
		{
			WalletType.Coldcard => "coldcard_24",
			WalletType.Trezor => "trezor_24",
			WalletType.Ledger => "ledger_24",
			_ => "wallet_24"
		};

		IconName = $"nav_{baseResourceName}_regular";
		IconNameFocused = $"nav_{baseResourceName}_filled";
	}
}
