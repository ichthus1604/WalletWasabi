using System.Collections.Generic;
using System.Reactive;
using System.Reactive.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using ReactiveUI;
using WalletWasabi.Extensions;
using WalletWasabi.Fluent.Models.Wallets;
using WalletWasabi.Fluent.UIServices;
using WalletWasabi.Fluent.ViewModels.Navigation;
using WalletWasabi.Logging;

namespace WalletWasabi.Fluent.ViewModels.Wallets.Receive;

[NavigationMetaData(Title = "Receive Address")]
public partial class ReceiveAddressViewModel : RoutableViewModel
{
	private readonly UIContext _context;
	private readonly ObservableAsPropertyHelper<bool[,]> _qrCode;

	public ReceiveAddressViewModel(IWalletModel wallet, IAddress model, UIContext context, bool isAutoCopyEnabled)
	{
		_context = context;
		Model = model;
		Address = model.Text;
		Labels = model.Labels;
		IsHardwareWallet = wallet.IsHardwareWallet();

		SetupCancel(enableCancel: false, enableCancelOnEscape: true, enableCancelOnPressed: true);

		EnableBack = true;

		CopyAddressCommand = ReactiveCommand.CreateFromTask(() => context.Clipboard.SetTextAsync(Address));

		ShowOnHwWalletCommand = ReactiveCommand.CreateFromTask(ShowOnHwWalletAsync);

		SaveQrCodeCommand = ReactiveCommand.CreateFromTask(OnSaveQrCodeAsync);

		SaveQrCodeCommand.ThrownExceptions
			.ObserveOn(RxApp.TaskpoolScheduler)
			.Subscribe(ex => Logger.LogError(ex));

		NextCommand = CancelCommand;

		GenerateQrCodeCommand = ReactiveCommand.CreateFromObservable(() => context.QrCodeGenerator.Generate(model.Text));
		_qrCode = GenerateQrCodeCommand.ToProperty(this, nameof(QrCode));

		if (isAutoCopyEnabled)
		{
			CopyAddressCommand.Execute(null);
		}

		GenerateQrCodeCommand
			.Execute()
			.Subscribe();
	}

	public ReactiveCommand<Unit, bool[,]> GenerateQrCodeCommand { get; }

	private IAddress Model { get; }

	public ReactiveCommand<string, Unit>? QrCodeCommand { get; set; }

	public ICommand CopyAddressCommand { get; }

	public ReactiveCommand<Unit, Unit> SaveQrCodeCommand { get; }

	public ICommand ShowOnHwWalletCommand { get; }

	public string Address { get; }

	public IEnumerable<string> Labels { get; }

	public bool IsHardwareWallet { get; }

	public bool[,]? QrCode => _qrCode.Value;

	private async Task ShowOnHwWalletAsync()
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
}
