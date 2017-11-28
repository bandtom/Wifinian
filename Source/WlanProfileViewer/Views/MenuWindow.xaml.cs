﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;

using ScreenFrame.Movers;
using WlanProfileViewer.ViewModels;

namespace WlanProfileViewer.Views
{
	public partial class MenuWindow : Window
	{
		private readonly FloatWindowMover _mover;
		private MenuWindowViewModel ViewModel => (MenuWindowViewModel)this.DataContext;
		
		internal MenuWindow(MainController controller, Point pivot)
		{
			InitializeComponent();

			this.Topmost = true;
			this.ShowInTaskbar = false;
			
			this.DataContext = new MenuWindowViewModel(controller);

			_mover = new FloatWindowMover(this, pivot);
		}

		#region Close

		private bool _isClosing = false;

		protected override void OnDeactivated(EventArgs e)
		{
			base.OnDeactivated(e);

			if (!_isClosing)
				this.Close();
		}

		protected override void OnClosing(CancelEventArgs e)
		{
			if (!e.Cancel)
			{
				_isClosing = true;
				ViewModel.Dispose();
			}

			base.OnClosing(e);
		}

		#endregion
	}
}