﻿using System.Collections.Generic;
using Xamarin.CommunityToolkit.Sample.Models;
using Xamarin.CommunityToolkit.Sample.Pages.Views;
using Xamarin.CommunityToolkit.Sample.Resx;

namespace Xamarin.CommunityToolkit.Sample.ViewModels.Views
{
	public class ViewsGalleryViewModel : BaseViewModel
	{
		public IEnumerable<SectionModel> Items { get; } = new List<SectionModel> {
			new SectionModel(typeof(AvatarViewPage), AppResources.AvatarViewTitle, AppResources.AvatarViewDescription),
			new SectionModel(typeof(RangeSliderPage), AppResources.RangeSliderTitle, AppResources.RangeSliderDescription),
			new SectionModel(typeof(SideMenuViewPage), AppResources.SideMenuViewTitle, AppResources.SideMenuViewDescription),
			new SectionModel(typeof(CameraViewPage), AppResources.CameraViewTitle, AppResources.CameraViewDescription),
		};
	}
}