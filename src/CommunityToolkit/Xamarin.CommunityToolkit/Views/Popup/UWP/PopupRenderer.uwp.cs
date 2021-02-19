﻿using System;
using System.ComponentModel;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Xamarin.CommunityToolkit.UI.Views;
using Xamarin.Forms;
using Xamarin.Forms.Internals;
using Xamarin.Forms.Platform.UWP;
using UWPThickness = Windows.UI.Xaml.Thickness;
using XamlStyle = Windows.UI.Xaml.Style;

[assembly: ExportRenderer(typeof(BasePopup), typeof(PopupRenderer))]

namespace Xamarin.CommunityToolkit.UI.Views
{
	public class PopupRenderer : Flyout, IVisualElementRenderer
	{
		const double defaultBorderThickness = 2;
		const double defaultSize = 600;
		bool isDisposed = false;
		XamlStyle flyoutStyle;
		XamlStyle panelStyle;

		public BasePopup Element { get; private set; }

		internal ViewToRendererConverter.WrapperControl Control { get; private set; }

		FrameworkElement IVisualElementRenderer.ContainerElement => null;

		VisualElement IVisualElementRenderer.Element => Element;

		public event EventHandler<VisualElementChangedEventArgs> ElementChanged;

		public event EventHandler<PropertyChangedEventArgs> ElementPropertyChanged;

		public PopupRenderer()
		{
		}

		void IVisualElementRenderer.SetElement(VisualElement element)
		{
			if (element == null)
				throw new ArgumentNullException(nameof(element));

			if (!(element is BasePopup popup))
				throw new ArgumentNullException("Element is not of type " + typeof(BasePopup), nameof(element));

			var oldElement = Element;
			Element = popup;
			CreateControl();

			if (oldElement != null)
				oldElement.PropertyChanged -= OnElementPropertyChanged;

			element.PropertyChanged += OnElementPropertyChanged;

			OnElementChanged(new ElementChangedEventArgs<BasePopup>(oldElement, Element));
		}

		void CreateControl()
		{
			if (Control == null)
			{
				Control = new Xamarin.CommunityToolkit.UI.Views.ViewToRendererConverter.WrapperControl(Element.Content);
				Content = Control;
			}
		}

		void InitializeStyles()
		{
			flyoutStyle = new XamlStyle { TargetType = typeof(FlyoutPresenter) };
			panelStyle = new XamlStyle { TargetType = typeof(Panel) };
		}

		protected virtual void OnElementChanged(ElementChangedEventArgs<BasePopup> e)
		{
			if (e.NewElement != null && !isDisposed)
			{
				ConfigureControl();
				Show();
			}

			ElementChanged?.Invoke(this, new VisualElementChangedEventArgs(e.OldElement, e.NewElement));
		}

		protected virtual void OnElementPropertyChanged(object sender, PropertyChangedEventArgs args)
		{
			if (args.PropertyName == BasePopup.VerticalOptionsProperty.PropertyName ||
				args.PropertyName == BasePopup.HorizontalOptionsProperty.PropertyName ||
				args.PropertyName == BasePopup.SizeProperty.PropertyName ||
				args.PropertyName == BasePopup.ColorProperty.PropertyName)
			{
				ConfigureControl();
			}

			ElementPropertyChanged?.Invoke(this, args);
		}

		void ConfigureControl()
		{
			InitializeStyles();
			SetEvents();
			SetColor();
			SetBorderColor();
			SetSize();
			SetLayout();
			ApplyStyles();
		}

		void SetEvents()
		{
			if (Element.IsLightDismissEnabled)
				Closing += OnClosing;

			Element.Dismissed += OnDismissed;
		}

		void SetSize()
		{
			var standardSize = new Size { Width = defaultSize, Height = defaultSize / 2 };

			var currentSize = Element.Size != default(Size) ? Element.Size : standardSize;
			Control.Width = currentSize.Width;
			Control.Height = currentSize.Height;

			flyoutStyle.Setters.Add(new Windows.UI.Xaml.Setter(FlyoutPresenter.MinHeightProperty, currentSize.Height + (defaultBorderThickness * 2)));
			flyoutStyle.Setters.Add(new Windows.UI.Xaml.Setter(FlyoutPresenter.MinWidthProperty, currentSize.Width + (defaultBorderThickness * 2)));
			flyoutStyle.Setters.Add(new Windows.UI.Xaml.Setter(FlyoutPresenter.MaxHeightProperty, currentSize.Height + (defaultBorderThickness * 2)));
			flyoutStyle.Setters.Add(new Windows.UI.Xaml.Setter(FlyoutPresenter.MaxWidthProperty, currentSize.Width + (defaultBorderThickness * 2)));
		}

		void SetLayout()
		{
			LightDismissOverlayMode = LightDismissOverlayMode.On;
			SetDialogPosition(Element.VerticalOptions, Element.HorizontalOptions);
		}

		void SetBorderColor()
		{
			flyoutStyle.Setters.Add(new Windows.UI.Xaml.Setter(FlyoutPresenter.PaddingProperty, 0));
			flyoutStyle.Setters.Add(new Windows.UI.Xaml.Setter(FlyoutPresenter.BorderThicknessProperty, new UWPThickness(defaultBorderThickness)));

			var borderColor = Views.WindowsSpecific.Popup.GetBorderColor(Element);
			if (borderColor == default(Color))
				flyoutStyle.Setters.Add(new Windows.UI.Xaml.Setter(FlyoutPresenter.BorderBrushProperty, Color.FromHex("#2e6da0").ToWindowsColor()));
			else
				flyoutStyle.Setters.Add(new Windows.UI.Xaml.Setter(FlyoutPresenter.BorderBrushProperty, borderColor.ToWindowsColor()));
		}

		void SetColor()
		{
			if (Element.Content.BackgroundColor == default(Color))
				panelStyle.Setters.Add(new Windows.UI.Xaml.Setter(Panel.BackgroundProperty, Element.Color.ToWindowsColor()));

			flyoutStyle.Setters.Add(new Windows.UI.Xaml.Setter(FlyoutPresenter.BackgroundProperty, Element.Color.ToWindowsColor()));

			if (ApiInformation.IsPropertyPresent(typeof(FlyoutPresenter).FullName, nameof(FlyoutPresenter.IsDefaultShadowEnabledProperty)))
			{
				if (Element.Color == Color.Transparent)
					flyoutStyle.Setters.Add(new Windows.UI.Xaml.Setter(FlyoutPresenter.IsDefaultShadowEnabledProperty, false));
			}
		}

		void ApplyStyles()
		{
			Control.Style = panelStyle;
			FlyoutPresenterStyle = flyoutStyle;
		}

		void Show()
		{
			if (Element.Anchor != null)
			{
				var anchor = Platform.GetRenderer(Element.Anchor).ContainerElement;
				FlyoutBase.SetAttachedFlyout(anchor, this);
				FlyoutBase.ShowAttachedFlyout(anchor);
			}
			else
			{
				var frameworkElement = Platform.GetRenderer(Element.Parent as VisualElement).ContainerElement;
				FlyoutBase.SetAttachedFlyout(frameworkElement, this);
				FlyoutBase.ShowAttachedFlyout(frameworkElement);
			}

			Element.OnOpened();
		}

		void SetDialogPosition(LayoutOptions verticalOptions, LayoutOptions horizontalOptions)
		{
			if (IsTopLeft())
				Placement = FlyoutPlacementMode.TopEdgeAlignedLeft;
			else if (IsTop())
				Placement = FlyoutPlacementMode.Top;
			else if (IsTopRight())
				Placement = FlyoutPlacementMode.TopEdgeAlignedRight;
			else if (IsRight())
				Placement = FlyoutPlacementMode.Right;
			else if (IsBottomRight())
				Placement = FlyoutPlacementMode.BottomEdgeAlignedRight;
			else if (IsBottom())
				Placement = FlyoutPlacementMode.Bottom;
			else if (IsBottomLeft())
				Placement = FlyoutPlacementMode.BottomEdgeAlignedLeft;
			else if (IsLeft())
				Placement = FlyoutPlacementMode.Left;
			else if (Element.Anchor == null)
				Placement = FlyoutPlacementMode.Full;
			else
				Placement = FlyoutPlacementMode.Top;

			bool IsTopLeft() => verticalOptions.Alignment == LayoutAlignment.Start && horizontalOptions.Alignment == LayoutAlignment.Start;
			bool IsTop() => verticalOptions.Alignment == LayoutAlignment.Start && horizontalOptions.Alignment == LayoutAlignment.Center;
			bool IsTopRight() => verticalOptions.Alignment == LayoutAlignment.Start && horizontalOptions.Alignment == LayoutAlignment.End;
			bool IsRight() => verticalOptions.Alignment == LayoutAlignment.Center && horizontalOptions.Alignment == LayoutAlignment.End;
			bool IsBottomRight() => verticalOptions.Alignment == LayoutAlignment.End && horizontalOptions.Alignment == LayoutAlignment.End;
			bool IsBottom() => verticalOptions.Alignment == LayoutAlignment.End && horizontalOptions.Alignment == LayoutAlignment.Center;
			bool IsBottomLeft() => verticalOptions.Alignment == LayoutAlignment.End && horizontalOptions.Alignment == LayoutAlignment.Start;
			bool IsLeft() => verticalOptions.Alignment == LayoutAlignment.Center && horizontalOptions.Alignment == LayoutAlignment.Start;
		}

		SizeRequest IVisualElementRenderer.GetDesiredSize(double widthConstraint, double heightConstraint)
		{
			if (isDisposed || Control == null)
				return default(SizeRequest);

			var constraint = new Windows.Foundation.Size(widthConstraint, heightConstraint);
			Control.Measure(constraint);

			var size = new Size(Math.Ceiling(Control.DesiredSize.Width), Math.Ceiling(Control.DesiredSize.Height));
			return new SizeRequest(size);
		}

		UIElement IVisualElementRenderer.GetNativeElement() => Control;

		void OnDismissed(object sender, PopupDismissedEventArgs e)
		{
			Hide();
		}

		void OnClosing(object sender, object e)
		{
			if (IsOpen && Element.IsLightDismissEnabled)
				Element.LightDismiss();
		}

		public void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}

		protected virtual void Dispose(bool disposing)
		{
			if (!isDisposed && disposing)
			{
				Element.Dismissed -= OnDismissed;
				Element = null;

				Control.CleanUp();
				Control = null;

				Closed -= OnClosing;
			}

			isDisposed = true;
		}
	}
}
