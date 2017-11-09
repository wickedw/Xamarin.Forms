using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Xamarin.Forms.Alias
{
	[ContentPropertyAttribute("Child")]
	public class Border : Frame
	{
		public static readonly BindableProperty BorderBrushProperty = OutlineColorProperty;
		public Color BorderBrush
		{
			get { return OutlineColor; }
			set { OutlineColor = value; }
		}
		
		public View Child
		{
			get { return Content; }
			set { Content = value; }
		}
	}

	public class Button : Xamarin.Forms.Button
	{
		public static readonly BindableProperty BorderBrushProperty = BorderColorProperty;

		public Color BorderBrush
		{
			get { return BorderColor; }
			set { BorderColor = value; }
		}


	}

	public class ComboBox : Picker
	{
		public ComboBox()
		{
			SelectedIndexChanged += (sender, args) => SelectionChanged?.Invoke(this, EventArgs.Empty);
		}

		public static readonly BindableProperty ForegroundProperty = TextColorProperty;

		public Color Foreground
		{
			get { return TextColor; }
			set { TextColor = value; }
		}

		public static readonly BindableProperty HeaderProperty = TitleProperty;

		public string Header
		{
			get { return Title; }
			set { Title = value; }
		}

		public event EventHandler SelectionChanged;
	}

	public class ProgressRing : ActivityIndicator
	{
		public static readonly BindableProperty IsActiveProperty = IsRunningProperty;

		public bool IsActive
		{
			get { return IsRunning; }
			set { IsRunning = value; }
		}
	}

	public class StackPanel : StackLayout
	{
		
	}

	[Flags]
	public enum FontStyle
	{
		Bold = 1 << 0,
		Italic = 1 << 1
	}

	public enum TextWrapping
	{
		NoWrap,
		Wrap,
		WrapWholeWords
	}

	public static class TextWrappingExtensions
	{
		public static LineBreakMode ToLineBreakMode(this TextWrapping self)
		{
			switch (self)
			{
				case TextWrapping.NoWrap:
					return LineBreakMode.NoWrap;
				case TextWrapping.Wrap:
					return LineBreakMode.CharacterWrap;
				case TextWrapping.WrapWholeWords:
					return LineBreakMode.WordWrap;
				default:
					throw new ArgumentOutOfRangeException(nameof(self), self, null);
			}
		}

		public static TextWrapping ToTextWrapping(this LineBreakMode self)
		{
			switch (self)
			{
				case LineBreakMode.NoWrap:
					return TextWrapping.NoWrap;
				case LineBreakMode.WordWrap:
					return TextWrapping.WrapWholeWords;
				case LineBreakMode.CharacterWrap:
					return TextWrapping.Wrap;
				case LineBreakMode.HeadTruncation:
					return TextWrapping.NoWrap;
				case LineBreakMode.TailTruncation:
					return TextWrapping.NoWrap;
				case LineBreakMode.MiddleTruncation:
					return TextWrapping.NoWrap;
				default:
					throw new ArgumentOutOfRangeException(nameof(self), self, null);
			}
		}
	}

	public class TextBlock : Label
	{
		public static readonly BindableProperty ForegroundProperty = TextColorProperty;

		public Color Foreground
		{
			get { return TextColor; }
			set { TextColor = value; }
		}

		public static readonly BindableProperty TextWrappingProperty = BindableProperty.Create(nameof(TextWrapping), typeof(TextWrapping), typeof(TextBlock), Alias.TextWrapping.NoWrap);

		public TextWrapping TextWrapping
		{
			get { return LineBreakMode.ToTextWrapping(); }
			set { LineBreakMode = value.ToLineBreakMode(); }
		}
	}

	public class TextBox : Entry
	{
		public static readonly BindableProperty ForegroundProperty = TextColorProperty;

		public Color Foreground
		{
			get { return TextColor; }
			set { TextColor = value; }
		}

		public static readonly BindableProperty PlaceholderForegroundProperty = PlaceholderColorProperty;

		public Color PlaceholderForeground
		{
			get { return PlaceholderColor; }
			set { PlaceholderColor = value; }
		}

		public static readonly BindableProperty PlaceholderTextProperty = PlaceholderProperty;

		public string PlaceholderText
		{
			get { return Placeholder; }
			set { Placeholder = value; }
		}
	}

	public class ToggleSwitch : Switch
	{
		public static readonly BindableProperty IsOnProperty = IsToggledProperty;

		public bool IsOn
		{
			get { return IsToggled; }
			set { IsToggled = value; }
		}
	}

	public class UserControl : ContentView
	{
		
	}

	public class ProgressBar : Xamarin.Forms.ProgressBar
	{
		public static readonly BindableProperty ValueProperty = ProgressProperty;

		public double Value
		{
			get { return Progress; }
			set { Progress = value; }
		}
	}
}
