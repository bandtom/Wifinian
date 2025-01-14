﻿using System;
using System.Linq;
using System.Reactive.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;
using Reactive.Bindings.Extensions;

using StartupAgency;
using Wifinian.Common;
using Wifinian.Models;

namespace Wifinian;

public class AppKeeper : DisposableBase
{
	public StartupAgent StartupAgent { get; }

	public AppKeeper(StartupEventArgs e)
	{
		StartupAgent = new StartupAgent();
	}

	public bool Start()
	{
		#region Exceptions

		var dispatcherUnhandled = Observable.FromEventPattern<DispatcherUnhandledExceptionEventHandler, DispatcherUnhandledExceptionEventArgs>(
			h => Application.Current.DispatcherUnhandledException += h,
			h => Application.Current.DispatcherUnhandledException -= h)
			.Select(x => x.EventArgs.Exception);
		var taskUnobserved = Observable.FromEventPattern<UnobservedTaskExceptionEventArgs>(
			h => TaskScheduler.UnobservedTaskException += h,
			h => TaskScheduler.UnobservedTaskException -= h)
			.SelectMany(x => x.EventArgs.Exception.InnerExceptions);
		var appDomainUnhandled = Observable.FromEventPattern<UnhandledExceptionEventHandler, UnhandledExceptionEventArgs>(
			h => AppDomain.CurrentDomain.UnhandledException += h,
			h => AppDomain.CurrentDomain.UnhandledException -= h)
			.Select(x => (Exception)x.EventArgs.ExceptionObject);
		Observable.Merge(dispatcherUnhandled, taskUnobserved, appDomainUnhandled)
			.Take(1)
			.Subscribe(x =>
			{
				if (LocationInfo.PromptOpenLocation(x))
					return;

				Logger.RecordException(x);
			})
			.AddTo(this.Subscription);

		#endregion

		var (success, _) = StartupAgent.Start(ProductInfo.Product, ProductInfo.StartupTaskId, null);
		return success;
	}
}