﻿using Xamarin.CommunityToolkit.UI.Views;
using Xamarin.CommunityToolkit.Sample.ViewModels.Views;
using Xamarin.Forms;
using System.Collections;
using System.Collections.Generic;

namespace Xamarin.CommunityToolkit.Sample.Pages.Views
{
	public partial class SegmentedViewPage
	{
		double all = 0.0;
		double topLeft = 0.0;
		double topRight = 0.0;
		double bottomLeft = 0.0;
		double bottomRight = 0.0;

		readonly Color MyRed = Color.FromHex("#97DC91");
		readonly Color MyGreen = Color.FromHex("#688ff4");
		readonly Color MyBlue = Color.FromHex("#FFA7A3");

		public SegmentedViewPage()
		{
			InitializeComponent();
		}

		void Slider_ValueChanged(System.Object sender, Xamarin.Forms.ValueChangedEventArgs e)
		{
			if (!double.TryParse(e.NewValue.ToString(), out var r))
				return;
			all = r;
			TextSegments.CornerRadius = new CornerRadius(all);
		}

		void TopLeftSlider_ValueChanged(System.Object sender, Xamarin.Forms.ValueChangedEventArgs e)
		{
			if (!double.TryParse(e.NewValue.ToString(), out var r))
				return;
			topLeft = r;
			UpdateCorners();
		}

		void TopRightSlider_ValueChanged(System.Object sender, Xamarin.Forms.ValueChangedEventArgs e)
		{
			if (!double.TryParse(e.NewValue.ToString(), out var r))
				return;
			topRight = r;
			UpdateCorners();
		}

		void BottomLeftSlider_ValueChanged(System.Object sender, Xamarin.Forms.ValueChangedEventArgs e)
		{
			if (!double.TryParse(e.NewValue.ToString(), out var r))
				return;
			bottomLeft = r;
			UpdateCorners();
		}

		void BottomRightSlider_ValueChanged(System.Object sender, Xamarin.Forms.ValueChangedEventArgs e)
		{
			if (!double.TryParse(e.NewValue.ToString(), out var r))
				return;
			bottomRight = r;
			UpdateCorners();
		}

		void UpdateCorners()
		{
			TextSegments.CornerRadius = new CornerRadius(topLeft, topRight, bottomLeft, bottomRight);
		}

		void Picker_SelectedIndexChanged(System.Object sender, System.EventArgs e)
		{
			if (!(sender is Picker picker))
				return;

			var color = Color.Default;

			color = picker.SelectedIndex switch
			{
				1 => Color.Green,
				2 => Color.Blue,
				_ => Color.Red,
			};
			TextSegments.Color = color;
		}

		void BG_Picker_SelectedIndexChanged(System.Object sender, System.EventArgs e)
		{
			if (!(sender is Picker picker))
				return;

			var color = Color.Default;

			color = picker.SelectedIndex switch
			{
				1 => MyRed,
				2 => MyGreen,
				_ => MyBlue,
			};
			TextSegments.BackgroundColor = color;
		}

		void DisplayMode_SelectedIndexChanged(System.Object sender, System.EventArgs e)
		{
			if (!(sender is Picker picker))
				return;

			switch (picker.SelectedIndex)
			{
				case 1:
					TextSegments.DisplayMode = SegmentMode.Image;
					TextSegments.ItemsSource = (BindingContext as ViewModels.Views.SegmentedViewModel).IconOptions;
					(BindingContext as SegmentedViewModel).SegmentMode = SegmentMode.Image;
					break;
				case 0:
				default:
					TextSegments.DisplayMode = SegmentMode.Text;
					TextSegments.ItemsSource = (BindingContext as ViewModels.Views.SegmentedViewModel).Options;
					(BindingContext as SegmentedViewModel).SegmentMode = SegmentMode.Text;
					break;
			}
		}

		void Text_Picker_SelectedIndexChanged(System.Object sender, System.EventArgs e)
		{
			if (!(sender is Picker picker))
				return;

			TextSegments.NormalTextColor = picker.SelectedIndex switch
			{
				1 => MyRed,
				2 => MyGreen,
				_ => MyBlue,
			};
		}

		void Selected_Text_Picker_SelectedIndexChanged(System.Object sender, System.EventArgs e)
		{
			if (!(sender is Picker picker))
				return;

			TextSegments.SelectedTextColor = picker.SelectedIndex switch
			{
				1 => MyRed,
				2 => MyGreen,
				_ => MyBlue,
			};
		}

		void Add_Clicked(object sender, System.EventArgs e)
		{
			(BindingContext as SegmentedViewModel).AddCommand.Execute(int.Parse((sender as Button).CommandParameter.ToString()));
		}

		void Delete_Clicked(object sender, System.EventArgs e)
		{
			(BindingContext as SegmentedViewModel).RemoveCommand.Execute(int.Parse((sender as Button).CommandParameter.ToString()));
		}
	}
}