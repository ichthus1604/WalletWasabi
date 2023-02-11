using ReactiveUI;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace WalletWasabi.Wallets;

public interface IAddress : IReactiveObject
{
	string Text { get; }
	IEnumerable<string> Labels { get; }

	bool IsUsed { get; }

	void Hide();

	void SetLabels(IEnumerable<string> labels);

	Task ShowOnHwWalletAsync();
}
