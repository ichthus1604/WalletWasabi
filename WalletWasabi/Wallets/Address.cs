using NBitcoin;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using WalletWasabi.Blockchain.Analysis.Clustering;
using WalletWasabi.Blockchain.Keys;
using WalletWasabi.Extensions;
using WalletWasabi.Hwi;
using WalletWasabi.Logging;

namespace WalletWasabi.Wallets;

public interface IAddress
{
	string Text { get; }
	IEnumerable<string> Labels { get; }

	void Hide();

	void SetLabels(Address address, IEnumerable<string> labels);

	Task ShowOnHwWalletAsync();
}

public class Address : IAddress
{
	public Address(Wallet wallet, HdPubKey hdPubKey)
	{
		Wallet = wallet;
		HdPubKey = hdPubKey;
		Network = wallet.Network;
		HdFingerprint = Wallet.KeyManager.MasterFingerprint;
		BitcoinAddress = HdPubKey.GetAddress(wallet.Network);
	}

	public Wallet Wallet { get; }
	public HdPubKey HdPubKey { get; }
	public Network Network { get; }
	public HDFingerprint? HdFingerprint { get; }
	public BitcoinAddress BitcoinAddress { get; }
	public SmartLabel Label => HdPubKey.Label;
	public PubKey PubKey => HdPubKey.PubKey;
	public KeyPath FullKeyPath => HdPubKey.FullKeyPath;

	public string Text => BitcoinAddress.ToString();
	public IEnumerable<string> Labels => Label;

	public void SetLabels(Address address, IEnumerable<string> labels)
	{
		HdPubKey.SetLabel(new SmartLabel(labels.ToList()), Wallet.KeyManager);
	}

	public void Hide()
	{
		Wallet.KeyManager.SetKeyState(KeyState.Locked, HdPubKey);
	}

	public async Task ShowOnHwWalletAsync()
	{
		if (HdFingerprint is null)
		{
			return;
		}

		using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(60));
		try
		{
			var client = new HwiClient(Network);
			await client.DisplayAddressAsync(HdFingerprint.Value, FullKeyPath, cts.Token).ConfigureAwait(false);
		}
		catch (FormatException ex) when (ex.Message.Contains("network") && Network == Network.TestNet)
		{
			// This exception happens everytime on TestNet because of Wasabi Keypath handling.
			// The user doesn't need to know about it.
		}
		catch (Exception ex)
		{
			Logger.LogError(ex);
			var exMessage = cts.IsCancellationRequested ? "User response didn't arrive in time." : ex.ToUserFriendlyString();
			throw;
		};
	}
}
