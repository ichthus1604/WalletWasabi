using NBitcoin;
using System.Collections.Generic;
using System.Linq;
using WalletWasabi.Blockchain.Analysis.Clustering;
using WalletWasabi.Blockchain.Keys;

namespace WalletWasabi.Wallets;

public record Address
{
	public Address(Wallet wallet, HdPubKey hdPubKey)
	{
		Wallet = wallet;
		HdPubKey = hdPubKey;
		Network = wallet.Network;
		HdFingerprint = Wallet.KeyManager.MasterFingerprint.Value;
		BitcoinAddress = HdPubKey.GetAddress(wallet.Network);
	}

	internal Wallet Wallet { get; }
	internal HdPubKey HdPubKey { get; }
	internal Network Network { get; }
	internal HDFingerprint HdFingerprint { get; }
	internal BitcoinAddress BitcoinAddress { get; }

	internal SmartLabel Label => HdPubKey.Label;
	internal PubKey PubKey => HdPubKey.PubKey;
	internal KeyPath FullKeyPath => HdPubKey.FullKeyPath;

	public string Text => BitcoinAddress.ToString();
	public IEnumerable<string> Labels => Label;

	public void SetLabels(IEnumerable<string> labels)
	{
		HdPubKey.SetLabel(new SmartLabel(labels.ToList()), Wallet.KeyManager);
	}
}
