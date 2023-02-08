using NBitcoin;
using System.Collections.Generic;
using WalletWasabi.Blockchain.Analysis.Clustering;
using WalletWasabi.Blockchain.Keys;

namespace WalletWasabi.Wallets;

public record Address
{
	internal Address(HdPubKey hdPubKey, HDFingerprint hdFingerprint, Network network)
	{
		HdPubKey = hdPubKey;
		Network = network;
		HdFingerprint = hdFingerprint;
		BitcoinAddress = HdPubKey.GetAddress(network);
	}

	internal HdPubKey HdPubKey { get; }
	internal Network Network { get; }
	internal HDFingerprint HdFingerprint { get; }
	internal BitcoinAddress BitcoinAddress { get; }

	internal SmartLabel Label => HdPubKey.Label;
	internal PubKey PubKey => HdPubKey.PubKey;
	internal KeyPath FullKeyPath => HdPubKey.FullKeyPath;

	public string Text => BitcoinAddress.ToString();
	public IEnumerable<string> Labels => Label;
}
