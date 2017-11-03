using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.Reflection;

using Xamarin.Forms.Internals;
using Xamarin.Forms.StyleSheets;
using Xamarin.Forms.Xaml;

namespace Xamarin.Forms
{
	public partial class VisualElement : IStyleSelectable, IStylable
	{
		IList<string> IStyleSelectable.Classes
			=> StyleClass;

		public static readonly BindableProperty StyleSheetProperty =
			BindableProperty.Create("StyleSheet", typeof(StyleSheet), typeof(VisualElement), default(StyleSheet),
				propertyChanged: (bp, o, n) => ((VisualElement)bp).OnStyleSheetChanged((StyleSheet)o, (StyleSheet)n));

		[TypeConverter(typeof(StyleSheetConverter))]
		public StyleSheet StyleSheet {
			get { return (StyleSheet)GetValue(StyleSheetProperty); }
			set { SetValue(StyleSheetProperty, value); }
		}

		void OnStyleSheetChanged(StyleSheet oldValue, StyleSheet newValue)
		{
			((IStyle)oldValue)?.UnApply(this);
			((IStyle)newValue)?.Apply(this);
		}

		BindableProperty IStylable.GetProperty(string key)
		{
			StylePropertyAttribute styleAttribute;
			if (!Internals.Registrar.StyleProperties.TryGetValue(key, out styleAttribute))
				return null;

			if (!styleAttribute.TargetType.GetTypeInfo().IsAssignableFrom(GetType().GetTypeInfo()))
				return null;

			if (styleAttribute.BindableProperty != null)
				return styleAttribute.BindableProperty;

			var bpField = GetType().GetField(styleAttribute.BindablePropertyName);
			if (bpField == null || !bpField.IsStatic)
				return null;

			return (styleAttribute.BindableProperty = bpField.GetValue(null) as BindableProperty);
		}

		void ApplyStyleSheetOnParentSet()
		{
			var parent = Parent;
			if (parent == null)
				return;
			var sheets = new List<StyleSheet>();
			while (parent != null) {
				var visualParent = parent as VisualElement;
				var sheet = visualParent?.StyleSheet;
				if (sheet != null)
					sheets.Add(sheet);
				parent = parent.Parent;
			}
			for (var i = sheets.Count - 1; i >= 0; i--)
				((IStyle)sheets[i]).Apply(this);
		}

		[Xaml.ProvideCompiled("Xamarin.Forms.Core.XamlC.StyleSheetConverter")]
		public class StyleSheetConverter : TypeConverter, IExtendedTypeConverter
		{
			object IExtendedTypeConverter.ConvertFromInvariantString(string value, IServiceProvider serviceProvider)
			{
				if (serviceProvider == null)
					throw new ArgumentNullException(nameof(serviceProvider));

				var rootObjectType = (serviceProvider.GetService(typeof(Xaml.IRootObjectProvider)) as Xaml.IRootObjectProvider)?.RootObject.GetType();
				if (rootObjectType == null)
					return null;

				var lineInfo = (serviceProvider.GetService(typeof(Xaml.IXmlLineInfoProvider)) as Xaml.IXmlLineInfoProvider)?.XmlLineInfo;
				var rootTargetPath = XamlResourceIdAttribute.GetPathForType(rootObjectType);
				var uri = new Uri(value, UriKind.Relative); //we don't want file:// uris, even if they start with '/'
				var resourceId = ResourceDictionary.RDSourceTypeConverter.GetResourceId(uri, rootTargetPath,
																						s => XamlResourceIdAttribute.GetResourceIdForPath(rootObjectType.GetTypeInfo().Assembly, s));

				return StyleSheet.Parse(rootObjectType.GetTypeInfo().Assembly, resourceId, lineInfo);
			}

			object IExtendedTypeConverter.ConvertFrom(CultureInfo culture, object value, IServiceProvider serviceProvider)
			{
				throw new NotImplementedException();
			}

			public override object ConvertFromInvariantString(string value)
			{
				throw new NotImplementedException();
			}
		}
	}
}