namespace WalletWasabi.Fluent.UIServices;

public interface IQrCodeGenerator
{
	IObservable<bool[,]> Generate(string data);
}
