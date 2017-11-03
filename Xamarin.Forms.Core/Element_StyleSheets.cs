using System.Collections.Generic;
using System.IO;
using System.Reflection;

using Xamarin.Forms.Internals;
using Xamarin.Forms.StyleSheets;

namespace Xamarin.Forms
{
	public partial class Element : IStyleSelectable
	{
		IEnumerable<IStyleSelectable> IStyleSelectable.Children => LogicalChildrenInternal;

		IList<string> IStyleSelectable.Classes => null;

		string IStyleSelectable.Id => StyleId;

		string _styleSelectableName;
		string IStyleSelectable.Name => _styleSelectableName ?? (_styleSelectableName = GetType().Name);

		IStyleSelectable IStyleSelectable.Parent => Parent;
	}
}