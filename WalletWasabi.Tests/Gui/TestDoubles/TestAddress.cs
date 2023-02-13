using System.Collections.Generic;
using System.Threading.Tasks;
using WalletWasabi.Fluent.Models.Wallets;

namespace WalletWasabi.Tests.Gui.TestDoubles;

public class TestAddress : IAddress
{
	public TestAddress(string address)
	{
		Text = address;
	}

	public string Text { get; }
	public IEnumerable<string> Labels { get; }
	public bool IsUsed { get; set; }
	public void Hide()
	{
	}

	public void SetLabels(IEnumerable<string> labels)
	{
	}

	public Task ShowOnHwWalletAsync()
	{
		return Task.CompletedTask;
	}
}
