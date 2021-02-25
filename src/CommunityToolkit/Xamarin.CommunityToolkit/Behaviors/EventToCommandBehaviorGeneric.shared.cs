﻿using System;

namespace Xamarin.CommunityToolkit.Behaviors
{
	/// <summary>
	/// This <see cref="EventToCommandBehavior"/> cast the sender object to a specific type defined by the user. 
	/// </summary>
	/// <typeparam name="TType">The type that you want to receive in your <see cref="Xamarin.Forms.Command"/> </typeparam>
	public class EventToCommandBehavior<TType> : EventToCommandBehavior
	{
		protected override void OnTriggerHandled(object sender = null, object eventArgs = null)
		{
			var parameter = CommandParameter
				?? EventArgsConverter?.Convert(eventArgs, typeof(object), null, null)
				?? eventArgs;

			if (parameter.GetType() != typeof(TType))
			{
				// nulling it to avoid any cast exception
				parameter = null;
				parameter = Convert.ChangeType(parameter, typeof(TType));
			}

			var command = Command;
			if (command?.CanExecute(parameter) ?? false)
				command.Execute(parameter);
		}
	}
}