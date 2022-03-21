using ReactiveUI;
using WalletWasabi.Fluent.Models;

namespace WalletWasabi.Fluent.ViewModels.Wallets.Graph;

public partial class GraphPin : ReactiveObject
{
	[AutoNotify] private GraphNode? _parent;
	[AutoNotify] private double _x;
	[AutoNotify] private double _y;
	[AutoNotify] private double _width;
	[AutoNotify] private double _height;
	[AutoNotify] private GraphPinAlignment _alignment;
}
