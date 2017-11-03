namespace Xamarin.Forms
{
	static class PaddingElement
	{
		public static readonly BindableProperty PaddingProperty =
			BindableProperty.Create(nameof(IPaddingElement.Padding), typeof(Thickness), typeof(IPaddingElement), default(Thickness),
									propertyChanged: OnPaddingPropertyChanged,
									defaultValueCreator: PaddingDefaultValueCreator);

		static void OnPaddingPropertyChanged(BindableObject bindable, object oldValue, object newValue)
		{
			((IPaddingElement)bindable).OnPaddingPropertyChanged((Thickness)oldValue, (Thickness)newValue);
		}

		static object PaddingDefaultValueCreator(BindableObject bindable)
		{
			return ((IPaddingElement)bindable).PaddingDefaultValueCreator();
		}
	}
}