﻿using System.Windows.Input;
using Xamarin.CommunityToolkit.ObjectModel;
using Xamarin.Forms.Xaml;

namespace Xamarin.CommunityToolkit.Sample.Pages.Effects
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class TouchEffectPage
	{
		public TouchEffectPage()
		{
			Command = CommandFactory.Create(() =>
			{
				TouchCount++;
				OnPropertyChanged(nameof(TouchCount));
			});
			LongPressCommand = CommandFactory.Create(() =>
			{
				LongPressCount++;
				OnPropertyChanged(nameof(LongPressCount));
			});

			InitializeComponent();
		}

		public ICommand Command { get; }

		public ICommand LongPressCommand { get; }

		public int TouchCount { get; private set; }

		public int LongPressCount { get; private set; }
	}
}