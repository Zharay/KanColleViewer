using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using Livet;

namespace Grabacr07.KanColleViewer.Models
{
	public class WindowOrientaionMode : NotificationObject, IOrientationMode, IDisposable
	{
		#region static members

		static public bool EventSetted { get; private set; }

		#endregion
		
        public OrientationType[] SupportedModes
		{
			get {
                OrientationType[] r = { OrientationType.Auto, OrientationType.Horizontal, OrientationType.Vertical };
                return r;
            }
		}

		#region Mode 変更通知プロパティ

		private Orientation _Mode;

        public Orientation Mode
		{
			get { return this._Mode; }
			private set
			{
                if (this._Mode != value)
				{
					this.ChangeWindowSize(value);
                    this._Mode = value;
                    this.RaisePropertyChanged();
                }
			}
		}

		#endregion

		#region CurrentMode 変更通知プロパティ

		private OrientationType _CurrentMode;

		public OrientationType CurrentMode
		{
			get { return this._CurrentMode; }
			set
			{
				if (this._CurrentMode != value)
				{
					this._CurrentMode = value;
					this.Refresh();
					this.RaisePropertyChanged();
				}
			}
		}

		#endregion

		private void UpdateOrientationMode()
		{
			this.Mode = System.Windows.SystemParameters.FullPrimaryScreenWidth >= System.Windows.SystemParameters.FullPrimaryScreenHeight ? Orientation.Horizontal : Orientation.Vertical;
		}

	    private void SystemParameters_StaticPropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName.Equals("FullPrimaryScreenHeight") || e.PropertyName.Equals("FullPrimaryScreenWidth"))
            {
                this.UpdateOrientationMode();
            }
        }

		private void ChangeWindowSize(Orientation type, System.Windows.Window window)
		{
			if (type == Orientation.Horizontal)
			{
				if (window != null && this.Mode != Orientation.Horizontal)
				{
					if (window.WindowState == System.Windows.WindowState.Normal)
					{
						Settings.Current.VerticalSize = new System.Windows.Point(window.Width, window.Height);
					}

					window.Height = Settings.Current.HorizontalSize.Y;
					window.Width = Settings.Current.HorizontalSize.X;
				}
			}
			else
			{
				if (window != null && this.Mode != Orientation.Vertical)
				{
					if (window.WindowState == System.Windows.WindowState.Normal)
					{
						Settings.Current.HorizontalSize = new System.Windows.Point(window.Width, window.Height);
					}

					window.Height = Settings.Current.VerticalSize.Y;
					window.Width = Settings.Current.VerticalSize.X;
				}
			}
		}

		private void ChangeWindowSize(Orientation type)
		{
			try
			{
				this.ChangeWindowSize(type, System.Windows.Application.Current.MainWindow);
			}
			catch
			{
				// ignored
			}
		}

		public void Dispose()
		{
			System.Windows.SystemParameters.StaticPropertyChanged -= this.SystemParameters_StaticPropertyChanged;
		}

		public void Refresh()
		{
			if (this.CurrentMode == OrientationType.Auto)
			{
				if (!EventSetted)
					System.Windows.SystemParameters.StaticPropertyChanged += this.SystemParameters_StaticPropertyChanged;
				EventSetted = true;
				this.UpdateOrientationMode();
			}
			else
			{
				System.Windows.SystemParameters.StaticPropertyChanged -= this.SystemParameters_StaticPropertyChanged;
				EventSetted = false;
				this.Mode = this.CurrentMode == OrientationType.Horizontal ? Orientation.Horizontal : Orientation.Vertical;
			}
		}
	}
}
