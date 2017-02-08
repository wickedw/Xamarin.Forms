using System;
using System.ComponentModel;

using System.Drawing;
using UIKit;
using Xamarin.Forms.PlatformConfiguration.iOSSpecific;

namespace Xamarin.Forms.Platform.iOS
{
	public class EntryRenderer : ViewRenderer<Entry, UITextField>
	{
		UIColor _defaultTextColor;

		// Placeholder default color is 70% gray
		// https://developer.apple.com/library/prerelease/ios/documentation/UIKit/Reference/UITextField_Class/index.html#//apple_ref/occ/instp/UITextField/placeholder
		readonly Color _defaultPlaceholderColor = ColorExtensions.SeventyPercentGrey.ToColor();

		bool _useLegacyColorManagement;
		bool _disposed;

		public EntryRenderer()
		{
			Frame = new RectangleF(0, 20, 320, 40);
		}

		public override SizeRequest GetDesiredSize(double widthConstraint, double heightConstraint)
		{
			//with borderStyle set to RoundedRect, iOS always returns a height of 30
			//https://stackoverflow.com/a/36569247/1063783
			//we get the current value, and restor it, to allow custom renderers to change the border style
			var borderStyle = Control.BorderStyle;
			Control.BorderStyle = UITextBorderStyle.None;
			var size = Control.GetSizeRequest(widthConstraint, double.PositiveInfinity);
			Control.BorderStyle = borderStyle;
			return size;
		}

		IElementController ElementController => Element as IElementController;

		protected override void Dispose(bool disposing)
		{
			if (_disposed)
				return;

			_disposed = true;

			if (disposing)
			{
				_defaultTextColor = null;

				if (Control != null)
				{
					Control.EditingDidBegin -= OnEditingBegan;
					Control.EditingChanged -= OnEditingChanged;
					Control.EditingDidEnd -= OnEditingEnded;
				}
			}

			base.Dispose(disposing);
		}

		protected override void OnElementChanged(ElementChangedEventArgs<Entry> e)
		{
			base.OnElementChanged(e);

			if (e.NewElement == null)
				return;

			if (Control == null)
			{
				var textField = new UITextField(RectangleF.Empty);
				SetNativeControl(textField);

				// Cache the default text color
				_defaultTextColor = textField.TextColor;

				// Determine whether we're letting the VSM handle the colors or doing it the old way
				_useLegacyColorManagement = VisualStateManager.GetVisualStateGroups(e.NewElement) == null;

				textField.BorderStyle = UITextBorderStyle.RoundedRect;
				textField.ClipsToBounds = true;

				textField.EditingChanged += OnEditingChanged;

				textField.ShouldReturn = OnShouldReturn;

				textField.EditingDidBegin += OnEditingBegan;
				textField.EditingDidEnd += OnEditingEnded;
			}

			UpdatePlaceholder();
			UpdatePassword();
			UpdateText();
			UpdateColor();
			UpdateFont();
			UpdateKeyboard();
			UpdateAlignment();
			UpdateAdjustsFontSizeToFitWidth();
		}

		protected override void OnElementPropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			if (e.PropertyName == Entry.PlaceholderProperty.PropertyName || e.PropertyName == Entry.PlaceholderColorProperty.PropertyName)
				UpdatePlaceholder();
			else if (e.PropertyName == Entry.IsPasswordProperty.PropertyName)
				UpdatePassword();
			else if (e.PropertyName == Entry.TextProperty.PropertyName)
				UpdateText();
			else if (e.PropertyName == Entry.TextColorProperty.PropertyName)
				UpdateColor();
			else if (e.PropertyName == Xamarin.Forms.InputView.KeyboardProperty.PropertyName)
				UpdateKeyboard();
			else if (e.PropertyName == Entry.HorizontalTextAlignmentProperty.PropertyName)
				UpdateAlignment();
			else if (e.PropertyName == Entry.FontAttributesProperty.PropertyName)
				UpdateFont();
			else if (e.PropertyName == Entry.FontFamilyProperty.PropertyName)
				UpdateFont();
			else if (e.PropertyName == Entry.FontSizeProperty.PropertyName)
				UpdateFont();
			else if (e.PropertyName == VisualElement.IsEnabledProperty.PropertyName)
			{
				UpdateColor();
				UpdatePlaceholder();
			}
			else if (e.PropertyName == PlatformConfiguration.iOSSpecific.Entry.AdjustsFontSizeToFitWidthProperty.PropertyName)
				UpdateAdjustsFontSizeToFitWidth();

			base.OnElementPropertyChanged(sender, e);
		}

		void OnEditingBegan(object sender, EventArgs e)
		{
			ElementController.SetValueFromRenderer(VisualElement.IsFocusedPropertyKey, true);
		}

		void OnEditingChanged(object sender, EventArgs eventArgs)
		{
			ElementController.SetValueFromRenderer(Entry.TextProperty, Control.Text);
		}

		void OnEditingEnded(object sender, EventArgs e)
		{
			// Typing aid changes don't always raise EditingChanged event
			if (Control.Text != Element.Text)
			{
				ElementController.SetValueFromRenderer(Entry.TextProperty, Control.Text);
			}

			ElementController.SetValueFromRenderer(VisualElement.IsFocusedPropertyKey, false);
		}

		protected virtual bool OnShouldReturn(UITextField view)
		{
			Control.ResignFirstResponder();
			((IEntryController)Element).SendCompleted();
			return false;
		}

		void UpdateAlignment()
		{
			Control.TextAlignment = Element.HorizontalTextAlignment.ToNativeTextAlignment();
		}

		void UpdateColor()
		{
			var textColor = Element.TextColor;

			if (_useLegacyColorManagement)
			{
				Control.TextColor = textColor.IsDefault || !Element.IsEnabled ? _defaultTextColor : textColor.ToUIColor();
			}
			else
			{
				Control.TextColor = textColor.IsDefault ? _defaultTextColor : textColor.ToUIColor();
			}
		}

		void UpdateAdjustsFontSizeToFitWidth()
		{
			Control.AdjustsFontSizeToFitWidth = Element.OnThisPlatform().AdjustsFontSizeToFitWidth();
		}

		void UpdateFont()
		{
			Control.Font = Element.ToUIFont();
		}

		void UpdateKeyboard()
		{
			Control.ApplyKeyboard(Element.Keyboard);
			Control.ReloadInputViews();
		}

		void UpdatePassword()
		{
			if (Element.IsPassword && Control.IsFirstResponder)
			{
				Control.Enabled = false;
				Control.SecureTextEntry = true;
				Control.Enabled = Element.IsEnabled;
				Control.BecomeFirstResponder();
			}
			else
				Control.SecureTextEntry = Element.IsPassword;
		}

		void UpdatePlaceholder()
		{
			var formatted = (FormattedString)Element.Placeholder;

			if (formatted == null)
				return;

			var targetColor = Element.PlaceholderColor;

			if (_useLegacyColorManagement)
			{
				var color = targetColor.IsDefault || !Element.IsEnabled ? _defaultPlaceholderColor : targetColor;
				Control.AttributedPlaceholder = formatted.ToAttributed(Element, color);
			}
			else
			{
				// Using VSM color management; take whatever is in Element.PlaceholderColor
				var color = targetColor.IsDefault ? _defaultPlaceholderColor : targetColor;
				Control.AttributedPlaceholder = formatted.ToAttributed(Element, color);
			}
		}

		void UpdateText()
		{
			// ReSharper disable once RedundantCheckBeforeAssignment
			if (Control.Text != Element.Text)
				Control.Text = Element.Text;
		}
	}
}
