using System.Collections.Generic;
using System.Reactive;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using Avalonia;
using ReactiveUI;
using WalletWasabi.Extensions;
using WalletWasabi.Fluent.Models;
using WalletWasabi.Fluent.Models.Wallets;
using WalletWasabi.Fluent.ViewModels.Navigation;
using WalletWasabi.Logging;
using WalletWasabi.Wallets;

namespace WalletWasabi.Fluent.ViewModels.Wallets.Receive;

[NavigationMetaData(Title = "Receive Address")]
public partial class ReceiveAddressViewModel : RoutableViewModel
{
	public ReceiveAddressViewModel(IUiWallet wallet, IAddress model)
	{
		Model = model;
		Address = model.Text;
		Labels = model.Labels;
		IsHardwareWallet = wallet.IsHardwareWallet();
		IsAutoCopyEnabled = Services.UiConfig.Autocopy;

		GenerateQrCode();

		SetupCancel(enableCancel: false, enableCancelOnEscape: true, enableCancelOnPressed: true);

		EnableBack = true;

		CopyAddressCommand = ReactiveCommand.CreateFromTask(async () =>
		{
			if (Application.Current is { Clipboard: { } clipboard })
			{
				await clipboard.SetTextAsync(Address);
			}
		});

		ShowOnHwWalletCommand = ReactiveCommand.CreateFromTask(OnShowOnHwWalletAsync);

		SaveQrCodeCommand = ReactiveCommand.CreateFromTask(OnSaveQrCodeAsync);

		SaveQrCodeCommand.ThrownExceptions
			.ObserveOn(RxApp.TaskpoolScheduler)
			.Subscribe(ex => Logger.LogError(ex));

		NextCommand = CancelCommand;
	}

	private IAddress Model { get; }

	public ReactiveCommand<string, Unit>? QrCodeCommand { get; set; }

	public ICommand CopyAddressCommand { get; }

	public ReactiveCommand<Unit, Unit> SaveQrCodeCommand { get; }

	public ICommand ShowOnHwWalletCommand { get; }

	public string Address { get; }

	public IEnumerable<string> Labels { get; }

	public bool[,]? QrCode { get; set; }

	public bool IsHardwareWallet { get; }

	public bool IsAutoCopyEnabled { get; }

	private async Task OnShowOnHwWalletAsync()
	{
		try
		{
			await Model.ShowOnHwWalletAsync();
		}
		catch (Exception ex)
		{
			await ShowErrorAsync(Title, ex.ToUserFriendlyString(), "Unable to send the address to the device");
		}
	}

	private async Task OnSaveQrCodeAsync()
	{
		if (QrCodeCommand is { } cmd)
		{
			await cmd.Execute(Address);
		}
	}

	protected override void OnNavigatedTo(bool isInHistory, CompositeDisposable disposables)
	{
		base.OnNavigatedTo(isInHistory, disposables);

		this.WhenAnyValue(x => x.Model.IsUsed)
			.Subscribe(_ => Navigate().Back());
	}

	private async Task<bool[,]> GenerateQrCode()
	{
		try
		{
			return await UIContext.QrCodeGenerator.Generate(Address.ToUpperInvariant());
		}
		catch (Exception ex)
		{
			Logger.LogError(ex);
			return new bool[0, 0];
		}
	}
}
