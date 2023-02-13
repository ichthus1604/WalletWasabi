namespace WalletWasabi.Fluent.Models.Wallets;

public interface IQrCodeGenerator
{
	IObservable<bool[,]> Generate(string data);
}