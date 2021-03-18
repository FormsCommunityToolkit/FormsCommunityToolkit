﻿using System;
using System.Reflection;
using Xamarin.CommunityToolkit.Behaviors;
using Xamarin.CommunityToolkit.Converters;
using Xamarin.CommunityToolkit.UnitTests.Mocks;
using Xamarin.Forms;
using NUnit.Framework;

namespace Xamarin.CommunityToolkit.UnitTests.Behaviors
{
	public class EventToCommandBehaviorGeneric_Tests
	{
		[SetUp]
		public void Setup() => Device.PlatformServices = new MockPlatformServices();

		[Test]
		public void ArgumentExceptionIfSpecifiedEventDoesNotExist()
		{
			var listView = new ListView();
			var behavior = new EventToCommandBehavior<Coffee>
			{
				EventName = "Wrong Event Name"
			};
			Assert.Throws<ArgumentException>(() => listView.Behaviors.Add(behavior));
		}

		[Test]
		public void NoExceptionIfSpecifiedEventExists()
		{
			var listView = new ListView();
			var behavior = new EventToCommandBehavior<Coffee>
			{
				EventName = nameof(ListView.ItemTapped)
			};
			listView.Behaviors.Add(behavior);
		}

		[Test]
		public void NoExceptionIfAttachedToPage()
		{
			var page = new ContentPage();
			var behavior = new EventToCommandBehavior<Coffee>
			{
				EventName = nameof(Page.Appearing)
			};
			page.Behaviors.Add(behavior);
		}

		[Test]
		public void NoExceptionWhenTheEventArgsAreNotNull()
		{
			var vm = new ViewModelCoffe();
			var behavior = new EventToCommandBehavior<Coffee>
			{
				EventName = nameof(ListView.ItemTapped),
				EventArgsConverter = new ItemSelectedEventArgsConverter(),
				Command = vm.SelectedCommand
			};

			Assert.Null(vm.CoffeeName);
			var coffe = new Coffee { Id = 1, Name = "Café" };
			var eventArgs = new SelectedItemChangedEventArgs(coffe, 1);

			var notNullArgs = new object?[] { null, eventArgs };

			TriggerEventToCommandBehavior(behavior, notNullArgs);

			Assert.AreEqual(coffe.Name, vm.CoffeeName);
		}

		[Test]
		public void ParameterOfTypeInt()
		{
			var vm = new ViewModelCoffe();
			var behavior = new EventToCommandBehavior<int>
			{
				EventName = nameof(ListView.ItemTapped),
				Command = vm.SelectedCommand,
				CommandParameter = 2
			};

			var nullArgs = new object?[] { null, null };

			TriggerEventToCommandBehavior(behavior, nullArgs);
		}

		[Test]
		public void NoExceptionWhenTheSelectedItemIsNull()
		{
			var vm = new ViewModelCoffe();
			var behavior = new EventToCommandBehavior<Coffee>
			{
				EventName = nameof(ListView.ItemTapped),
				EventArgsConverter = new ItemSelectedEventArgsConverter(),
				Command = vm.SelectedCommand
			};

			Assert.Null(vm.CoffeeName);
			var coffeNull = default(Coffee);
			var notNullArgs = new object?[] { null, new SelectedItemChangedEventArgs(coffeNull, -1) };

			TriggerEventToCommandBehavior(behavior, notNullArgs);

			Assert.Null(vm.CoffeeName);
		}

		void TriggerEventToCommandBehavior<T>(EventToCommandBehavior<T> eventToCommand, object?[] args)
		{
			var method = eventToCommand.GetType().GetMethod("OnTriggerHandled", BindingFlags.Instance | BindingFlags.NonPublic);
			method?.Invoke(eventToCommand, args);
		}

		class Coffee
		{
			public int Id { get; set; }

			public string Roaster { get; set; } = string.Empty;

			public string? Name { get; set; }

			public string Image { get; set; } = string.Empty;
		}

		class ViewModelCoffe
		{
			public Command<Coffee> SelectedCommand { get; set; }

			public string? CoffeeName { get; set; }

			public ViewModelCoffe()
			{
				SelectedCommand = new Command<Coffee>(Selected);
			}

			void Selected(Coffee coffee)
			{
				if (coffee == null)
					return;

				CoffeeName = coffee?.Name;
			}
		}
	}
}
