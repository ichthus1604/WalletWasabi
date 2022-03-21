using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive;
using ReactiveUI;
using WalletWasabi.Blockchain.TransactionBuilding;
using WalletWasabi.Fluent.ViewModels.Dialogs.Base;
using WalletWasabi.Fluent.ViewModels.Wallets.Graph;

namespace WalletWasabi.Fluent.ViewModels.Wallets.Send;

[NavigationMetaData(
	Title = "Transaction Graph",
	Caption = "",
	IconName = "wallet_action_send",
	NavBarPosition = NavBarPosition.None,
	Searchable = false,
	NavigationTarget = NavigationTarget.DialogScreen)]
public partial class TransactionGraphViewModel : DialogViewModelBase<Unit>
{
	public TransactionGraphViewModel(BuildTransactionResult transaction)
	{
		SetupCancel(false, true, false);
		EnableBack = true;

		NextCommand = ReactiveCommand.Create(() => Close());

		var centerNode = new TransactionGraphTransaction { X = 250 };
		Nodes.Add(centerNode);

		var itemHeight = 50;

		var inputsColumnHeight = transaction.Transaction.WalletInputs.Count * itemHeight;
		var currentY = 
			inputsColumnHeight > 400
			? 20
			: (400 - inputsColumnHeight) / 2;

		foreach (var input in transaction.Transaction.WalletInputs)
		{
			var node = new TransactionGraphInput(input) { X = 20, Y = currentY };
			Nodes.Add(node);
			Connect(centerNode.Input, node.Pins.First());
			currentY += itemHeight;
		}

		var outputsColumnHeight = transaction.Transaction.WalletOutputs.Count * itemHeight;
		currentY =
			outputsColumnHeight > 400
			? 20
			: (400 - outputsColumnHeight) / 2;

		foreach (var output in transaction.Transaction.WalletOutputs)
		{
			var node = new TransactionGraphOutput(output) { X = 500, Y = currentY };
			Nodes.Add(node);
			Connect(centerNode.Output, node.Pins.First());
			currentY += itemHeight;
		}

		centerNode.Y = (Math.Max(Math.Max(inputsColumnHeight, outputsColumnHeight), 400) - itemHeight) / 2;
	}

	public GraphConnector Connect(GraphPin start, GraphPin end)
	{
		var connector = new GraphConnector
		{
			Start = start,
			End = end
		};
		Connectors.Add(connector);
		return connector;
	}

	public ObservableCollection<GraphNode>      Nodes      { get; } = new();
	public ObservableCollection<GraphConnector> Connectors { get; } = new();
}