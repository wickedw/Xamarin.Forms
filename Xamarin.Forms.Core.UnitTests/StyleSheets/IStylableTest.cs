using System;

using NUnit.Framework;

using Xamarin.Forms.Core.UnitTests;

namespace Xamarin.Forms.StyleSheets.UnitTests
{
	[TestFixture]
	public class IStylableTest
	{
		[SetUp]
		public void SetUp()
		{
			Device.PlatformServices = new MockPlatformServices();
			Internals.Registrar.RegisterAll(new Type[0]);
		}

		[TestCase]
		public void GetBackgroundColor()
		{
			var label = new Label();
			var bp = ((IStylable)label).GetProperty("background-color");
			Assert.AreSame(VisualElement.BackgroundColorProperty, bp);
		}

		[TestCase]
		public void GetLabelColor()
		{
			var label = new Label();
			var bp = ((IStylable)label).GetProperty("color");
			Assert.AreSame(Label.TextColorProperty, bp);
		}

		[TestCase]
		public void GetEntryColor()
		{
			var entry = new Entry();
			var bp = ((IStylable)entry).GetProperty("color");
			Assert.AreSame(Entry.TextColorProperty, bp);
		}

		[TestCase]
		public void GetGridColor()
		{
			var grid = new Grid();
			var bp = ((IStylable)grid).GetProperty("color");
			Assert.Null(bp);
		}
	}
}