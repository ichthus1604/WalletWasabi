using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using ReactiveUI;
using WalletWasabi.Blockchain.TransactionOutputs;
using WalletWasabi.Fluent.Models;
using WalletWasabi.Fluent.ViewModels.Wallets.Send;

namespace WalletWasabi.Fluent.ViewModels.Wallets.Graph;

public partial class GraphNode : ReactiveObject
{
	[AutoNotify] private string? _name;
	[AutoNotify] private TransactionGraphViewModel? _parent;
	[AutoNotify] private double _x;
	[AutoNotify] private double _y;
	[AutoNotify] private double _width;
	[AutoNotify] private double _height;
	[AutoNotify] private object? _content;
	[AutoNotify] private ObservableCollection<GraphPin> _pins = new();

	public GraphPin AddPin(double x, double y, double width, double height, GraphPinAlignment alignment = GraphPinAlignment.None)
	{
		var pin = new GraphPin
		{
			Parent = this,
			X = x,
			Y = y,
			Width = width,
			Height = height,
			Alignment = alignment
		};

		Pins.Add(pin);

		return pin;
	}

	public GraphPin AddLeftPin(double pinSize = 8) => AddPin(0, Height / 2, pinSize, pinSize, GraphPinAlignment.Left);

	public GraphPin AddRightPin(double pinSize = 8) => AddPin(Width, Height / 2, pinSize, pinSize, GraphPinAlignment.Right);
}

public class TransactionGraphInput : GraphNode
{
	public List<string> FilteredLabel { get; }

	public string? Label { get; }

	public TransactionGraphInput(SmartCoin smartCoin)
	{
		Width = 150;
		Height = 30;
		AddRightPin();

		Label = smartCoin.Transaction.Label.Labels.FirstOrDefault() ?? "Label";
	}
}

public class TransactionGraphOutput: GraphNode
{
	public string? Label { get; }

	public TransactionGraphOutput(SmartCoin smartCoin)
	{
		Width = 150;
		Height = 30;
		AddLeftPin();

		Label = smartCoin.Transaction.Label.Labels.FirstOrDefault() ?? "Label";
	}
}

public class TransactionGraphTransaction: GraphNode
{
	public TransactionGraphTransaction()
	{
		Width = 100;
		Height = 30;

		Input  = AddLeftPin();
		Output = AddRightPin();
	}

	public GraphPin Input { get; }
	public GraphPin Output { get; }
}
