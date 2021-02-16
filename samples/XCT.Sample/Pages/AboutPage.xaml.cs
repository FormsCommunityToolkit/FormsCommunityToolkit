﻿using System;
using Xamarin.CommunityToolkit.Sample.ViewModels;

namespace Xamarin.CommunityToolkit.Sample.Pages
{
	public partial class AboutPage : BasePage
	{
		public AboutPage()
			=> InitializeComponent();

		protected override async void OnAppearing()
		{
			base.OnAppearing();
			await ((AboutViewModel)BindingContext).OnAppearing();
		}

		async void OnCloseClicked(object sender, EventArgs e)
			=> await Navigation.PopModalAsync();
	}
}