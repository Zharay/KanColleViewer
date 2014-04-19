using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Documents;
using System.Net;
using System.Xml.Linq;
using Grabacr07.KanColleViewer.Models;
using Grabacr07.KanColleViewer.Properties;
using Grabacr07.KanColleViewer.ViewModels.Messages;
using Grabacr07.KanColleWrapper;
using Grabacr07.KanColleWrapper.Models;
using Livet;
using Livet.EventListeners;
using Livet.Messaging;
using Livet.Messaging.IO;
using MetroRadiance;
using Settings = Grabacr07.KanColleViewer.Models.Settings;

namespace Grabacr07.KanColleViewer.ViewModels
{
	public class SettingsViewModel : TabItemViewModel, INotifyDataErrorInfo
	{
		public override string Name
		{
			get { return Resources.Settings; }
			protected set { throw new NotImplementedException(); }
		}

        public override string Icon
        {
            get { return "🔧"; }
            protected set { throw new NotImplementedException(); }
        }

		public IEnumerable<string> FlashQualityList {get; private set;}
		public string[] FlashQualities = { "Best", "High", "AutoHigh",  "Medium", "AutoLow", "Low" };

		public IEnumerable<string> FlashWindowList { get; private set; }
		public string[] FlashWindows = { "Opaque", "Direct", "GPU" };

		#region ScreenshotFolder 変更通知プロパティ

		public string ScreenshotFolder
		{
			get { return Settings.Current.ScreenshotFolder; }
			set
			{
				if (Settings.Current.ScreenshotFolder != value)
				{
					Settings.Current.ScreenshotFolder = value;
					this.RaisePropertyChanged();
					this.RaisePropertyChanged("CanOpenScreenshotFolder");
				}
			}
		}

		#endregion

		#region CanOpenScreenshotFolder 変更通知プロパティ

		public bool CanOpenScreenshotFolder
		{
			get { return Directory.Exists(this.ScreenshotFolder); }
		}

		#endregion

		#region ScreenshotImageFormat 変更通知プロパティ

		public SupportedImageFormat ScreenshotImageFormat
		{
			get { return Settings.Current.ScreenshotImageFormat; }
			set
			{
				if (Settings.Current.ScreenshotImageFormat != value)
				{
					Settings.Current.ScreenshotImageFormat = value;
					this.RaisePropertyChanged();
				}
			}
		}

		#endregion

		#region UseProxy 変更通知プロパティ

		public string UseProxy
		{
			get { return Settings.Current.EnableProxy.ToString(); }
			set
			{
				bool booleanValue;
				if (Boolean.TryParse(value, out booleanValue))
				{
					Settings.Current.EnableProxy = booleanValue;
					KanColleClient.Current.Proxy.UseProxyOnConnect = booleanValue;
					this.RaisePropertyChanged();
				}
			}
		}

		#endregion

		#region UseProxyForSSL 変更通知プロパティ

		public bool UseProxyForSSL
		{
			get { return Settings.Current.EnableSSLProxy; }
			set
			{
				if (Settings.Current.EnableSSLProxy != value)
				{
					Settings.Current.EnableSSLProxy = value;
					KanColleClient.Current.Proxy.UseProxyOnSSLConnect = value;
					this.RaisePropertyChanged();
				}
			}
		}

		#endregion

		#region ProxyHost 変更通知プロパティ

		public string ProxyHost
		{
			get { return Settings.Current.ProxyHost; }
			set
			{
				if (Settings.Current.ProxyHost != value)
				{
					Settings.Current.ProxyHost = value;
					KanColleClient.Current.Proxy.UpstreamProxyHost = value;
					this.RaisePropertyChanged();
				}
			}
		}

		#endregion

		#region ProxyPort 変更通知プロパティ

		public string ProxyPort
		{
			get { return Settings.Current.ProxyPort.ToString(); }
			set
			{
				UInt16 numberPort;
				if (UInt16.TryParse(value, out numberPort))
				{
					Settings.Current.ProxyPort = numberPort;
					KanColleClient.Current.Proxy.UpstreamProxyPort = numberPort;
					this.RaisePropertyChanged();
				}
			}
		}

		#endregion

		#region ReSortieCondition 変更通知プロパティ

		private string _ReSortieCondition = Settings.Current.ReSortieCondition.ToString(CultureInfo.InvariantCulture);
		private string reSortieConditionError;

		public string ReSortieCondition
		{
			get { return this._ReSortieCondition; }
			set
			{
				if (this._ReSortieCondition != value)
				{
					ushort cond;
					if (ushort.TryParse(value, out cond) && cond <= 49)
					{
						Settings.Current.ReSortieCondition = cond;
						this.reSortieConditionError = null;
					}
					else
					{
						this.reSortieConditionError = "Please enter a condition value between 0 and 49.";
					}

					this._ReSortieCondition = value;
					this.RaisePropertyChanged();
				}
			}
		}

		#endregion

		#region EnableLogging 変更通知プロパティ

		public bool EnableLogging
		{
			get { return Settings.Current.EnableLogging; }
			set
			{
				if (Settings.Current.EnableLogging != value)
				{
					Settings.Current.EnableLogging = value;
					KanColleClient.Current.Homeport.Logger.EnableLogging = value;
					this.RaisePropertyChanged();
				}
			}
		}

		#endregion

		#region Libraries 変更通知プロパティ

		private IEnumerable<BindableTextViewModel> _Libraries;

		public IEnumerable<BindableTextViewModel> Libraries
		{
			get { return this._Libraries; }
			set
			{
				if (!Equals(this._Libraries, value))
				{
					this._Libraries = value;
					this.RaisePropertyChanged();
				}
			}
		}

		#endregion

		#region IsDarkTheme 変更通知プロパティ

		private bool _IsDarkTheme;

		public bool IsDarkTheme
		{
			get { return this._IsDarkTheme; }
			set
			{
				if (this._IsDarkTheme != value)
				{
					this._IsDarkTheme = value;
					this.RaisePropertyChanged();
					if (value) ThemeService.Current.ChangeTheme(Theme.Dark);
				}
			}
		}

		#endregion

		#region IsLightTheme 変更通知プロパティ

		private bool _IsLightTheme;

		public bool IsLightTheme
		{
			get { return this._IsLightTheme; }
			set
			{
				if (this._IsLightTheme != value)
				{
					this._IsLightTheme = value;
					this.RaisePropertyChanged();
					if (value) ThemeService.Current.ChangeTheme(Theme.Light);
				}
			}
		}

		#endregion

		#region Cultures 変更通知プロパティ

		private IReadOnlyCollection<CultureViewModel> _Cultures;

		public IReadOnlyCollection<CultureViewModel> Cultures
		{
			get { return this._Cultures; }
			set
			{
				if (!Equals(this._Cultures, value))
				{
					this._Cultures = value;
					this.RaisePropertyChanged();
				}
			}
		}

		#endregion

		#region Culture 変更通知プロパティ

		/// <summary>
		/// カルチャを取得または設定します。
		/// </summary>
		public string Culture
		{
			get { return Settings.Current.Culture; }
			set
			{
				if (Settings.Current.Culture != value)
				{
					ResourceService.Current.ChangeCulture(value);
					KanColleClient.Current.Homeport.Translations.ChangeCulture(value);

					KanColleClient.Current.Homeport.Translations.Firepower =Properties.Resources.Stats_Firepower;
					KanColleClient.Current.Homeport.Translations.AntiAir = Properties.Resources.Stats_AntiAir;
					KanColleClient.Current.Homeport.Translations.Accuracy = Properties.Resources.Stats_Accuracy;
					KanColleClient.Current.Homeport.Translations.Torpedo = Properties.Resources.Stats_Torpedo;
					KanColleClient.Current.Homeport.Translations.AntiSub = Properties.Resources.Stats_AntiSub;
					KanColleClient.Current.Homeport.Translations.Evasion = Properties.Resources.Stats_Evasion;
					KanColleClient.Current.Homeport.Translations.SightRange = Properties.Resources.Stats_SightRange;
					KanColleClient.Current.Homeport.Translations.Luck = Properties.Resources.Stats_Luck;
					KanColleClient.Current.Homeport.Translations.Speed = Properties.Resources.Stats_Speed;
					KanColleClient.Current.Homeport.Translations.Armor = Properties.Resources.Stats_Armor;
					KanColleClient.Current.Homeport.Translations.Health = Properties.Resources.Stats_Health;
					KanColleClient.Current.Homeport.Translations.AttackRange = Properties.Resources.Stats_AttackRange;
					KanColleClient.Current.Homeport.Translations.DiveBomb = Properties.Resources.Stats_DiveBomb;

					this.RaisePropertyChanged();
				}
			}
		}

		#endregion

		#region EnableTranslations 変更通知プロパティ

		public bool EnableTranslations
		{
			get { return Settings.Current.EnableTranslations; }
			set
			{
				if (Settings.Current.EnableTranslations != value)
				{
					Settings.Current.EnableTranslations = value;
					KanColleClient.Current.Homeport.Translations.EnableTranslations = value;
					this.RaisePropertyChanged();
				}
			}
		}

		#endregion

		#region EnableAddUntranslated 変更通知プロパティ

		public bool EnableAddUntranslated
		{
			get { return Settings.Current.EnableAddUntranslated; }
			set
			{
				if (Settings.Current.EnableAddUntranslated != value)
				{
					Settings.Current.EnableAddUntranslated = value;
					KanColleClient.Current.Homeport.Translations.EnableAddUntranslated = value;
					this.RaisePropertyChanged();
				}
			}
		}

		#endregion

		#region BrowserZoomFactor 変更通知プロパティ

		private BrowserZoomFactor _BrowserZoomFactor;

		public BrowserZoomFactor BrowserZoomFactor
		{
			get { return this._BrowserZoomFactor; }
			private set
			{
				if (this._BrowserZoomFactor != value)
				{
					this._BrowserZoomFactor = value;
					this.RaisePropertyChanged();
				}
			}
		}

		#endregion

		#region EnableCriticalNotify 変更通知プロパティ

		public bool EnableCriticalNotify
		{
			get { return Settings.Current.EnableCriticalNotify; }
			set
			{
				if (Settings.Current.EnableCriticalNotify != value)
				{
					Settings.Current.EnableCriticalNotify = value;
					this.RaisePropertyChanged();
				}
			}
		}

		#endregion

		#region EnableCriticalAccent 変更通知プロパティ

		public bool EnableCriticalAccent
		{
			get { return Settings.Current.EnableCriticalAccent; }
			set
			{
				if (Settings.Current.EnableCriticalAccent != value)
				{
					Settings.Current.EnableCriticalAccent = value;
					if (!Settings.Current.EnableCriticalAccent && App.ViewModelRoot.Mode == Mode.CriticalCondition)
						App.ViewModelRoot.Mode = Mode.Started;
					this.RaisePropertyChanged();
				}
			}
		}

		#endregion

		#region EquipmentVersion 変更通知プロパティ

		public string EquipmentVersion
		{
			get { return KanColleClient.Current.Homeport.Translations.EquipmentVersion; }
			set
			{
				if (KanColleClient.Current.Homeport.Translations.EquipmentVersion != value)
				{
					KanColleClient.Current.Homeport.Translations.EquipmentVersion = value;
					this.RaisePropertyChanged();
				}
			}
		}

		#endregion

		#region OperationsVersion 変更通知プロパティ

		public string OperationsVersion
		{
			get { return KanColleClient.Current.Homeport.Translations.OperationsVersion; }
			set
			{
				if (KanColleClient.Current.Homeport.Translations.OperationsVersion != value)
				{
					KanColleClient.Current.Homeport.Translations.OperationsVersion = value;
					this.RaisePropertyChanged();
				}
			}
		}

		#endregion

		#region QuestsVersion 変更通知プロパティ

		public string QuestsVersion
		{
			get { return KanColleClient.Current.Homeport.Translations.QuestsVersion; }
			set
			{
				if (KanColleClient.Current.Homeport.Translations.QuestsVersion != value)
				{
					KanColleClient.Current.Homeport.Translations.QuestsVersion = value;
					this.RaisePropertyChanged();
				}
			}
		}

		#endregion

		#region ShipsVersion 変更通知プロパティ

		public string ShipsVersion
		{
			get { return KanColleClient.Current.Homeport.Translations.ShipsVersion; }
			set
			{
				if (KanColleClient.Current.Homeport.Translations.ShipsVersion != value)
				{
					KanColleClient.Current.Homeport.Translations.ShipsVersion = value;
					this.RaisePropertyChanged();
				}
			}
		}

		#endregion

		#region ShipTypesVersion 変更通知プロパティ

		public string ShipTypesVersion
		{
			get { return KanColleClient.Current.Homeport.Translations.ShipTypesVersion; }
			set
			{
				if (KanColleClient.Current.Homeport.Translations.ShipTypesVersion != value)
				{
					KanColleClient.Current.Homeport.Translations.ShipTypesVersion = value;
					this.RaisePropertyChanged();
				}
			}
		}

		#endregion

		#region AppOnlineVersion 変更通知プロパティ

		private string _AppOnlineVersion;
		public string AppOnlineVersionURL { get; set; }

		public string AppOnlineVersion
		{
			get { return _AppOnlineVersion; }
			set
			{
				if (_AppOnlineVersion != value)
				{
					_AppOnlineVersion = value;
					this.RaisePropertyChanged();
					this.RaisePropertyChanged("AppOnlineVersionURL");
				}
			}
		}

		#endregion

		#region EquipmentOnlineVersion 変更通知プロパティ

		private string _EquipmentOnlineVersion;
		public string EquipmentOnlineVersionURL { get; set; }

		public string EquipmentOnlineVersion
		{
			get { return _EquipmentOnlineVersion; }
			set
			{
				if (_EquipmentOnlineVersion != value)
				{
					_EquipmentOnlineVersion = value;
					this.RaisePropertyChanged();
					this.RaisePropertyChanged("EquipmentOnlineVersionURL");
				}
			}
		}

		#endregion

		#region OperationsOnlineVersion 変更通知プロパティ

		private string _OperationsOnlineVersion;
		public string OperationsOnlineVersionURL { get; set; }

		public string OperationsOnlineVersion
		{
			get { return _OperationsOnlineVersion; }
			set
			{
				if (_OperationsOnlineVersion != value)
				{
					_OperationsOnlineVersion = value;
					this.RaisePropertyChanged();
					this.RaisePropertyChanged("OperationsOnlineVersionURL");
				}
			}
		}

		#endregion

		#region QuestsOnlineVersion 変更通知プロパティ

		private string _QuestsOnlineVersion;
		public string QuestsOnlineVersionURL {get; set;}

		public string QuestsOnlineVersion
		{
			get { return _QuestsOnlineVersion; }
			set
			{
				if (_QuestsOnlineVersion != value)
				{
					_QuestsOnlineVersion = value;
					this.RaisePropertyChanged();
					this.RaisePropertyChanged("QuestsOnlineVersionURL");
				}
			}
		}

		#endregion

		#region ShipsOnlineVersion 変更通知プロパティ

		private string _ShipsOnlineVersion;
		public string ShipsOnlineVersionURL { get; set; }

		public string ShipsOnlineVersion
		{
			get { return _ShipsOnlineVersion; }
			set
			{
				if (_ShipsOnlineVersion != value)
				{
					_ShipsOnlineVersion = value;
					this.RaisePropertyChanged();
					this.RaisePropertyChanged("ShipsOnlineVersionURL");
				}
			}
		}

		#endregion

		#region ShipTypesOnlineVersion 変更通知プロパティ

		private string _ShipTypesOnlineVersion;
		public string ShipTypesOnlineVersionURL { get; set; }

		public string ShipTypesOnlineVersion
		{
			get { return _ShipTypesOnlineVersion; }
			set
			{
				if (_ShipTypesOnlineVersion != value)
				{
					_ShipTypesOnlineVersion = value;
					this.RaisePropertyChanged();
					this.RaisePropertyChanged("ShipTypesOnlineVersionURL");
				}
			}
		}

		#endregion

		#region EnableUpdateNotification 変更通知プロパティ

		public bool EnableUpdateNotification
		{
			get { return Settings.Current.EnableUpdateNotification; }
			set
			{
				if (Settings.Current.EnableUpdateNotification != value)
				{
					Settings.Current.EnableUpdateNotification = value;
					this.RaisePropertyChanged();
				}
			}
		}

		#endregion

		#region EnableUpdateTransOnStart 変更通知プロパティ

		public bool EnableUpdateTransOnStart
		{
			get { return Settings.Current.EnableUpdateTransOnStart; }
			set
			{
				if (Settings.Current.EnableUpdateTransOnStart != value)
				{
					Settings.Current.EnableUpdateTransOnStart = value;
					this.RaisePropertyChanged();
				}
			}
		}

		#endregion

		#region FlashQuality 変更通知プロパティ

		public string FlashQuality
		{
			get { return Settings.Current.FlashQuality; }
			set
			{
				if (Settings.Current.FlashQuality != value)
				{
					Settings.Current.FlashQuality = value;
					this.RaisePropertyChanged();
				}
			}
		}

		#endregion

		#region FlashWindow 変更通知プロパティ

		public string FlashWindow
		{
			get { return Settings.Current.FlashWindow; }
			set
			{
				if (Settings.Current.FlashWindow != value)
				{
					Settings.Current.FlashWindow = value;
					this.RaisePropertyChanged();
				}
			}
		}

		#endregion

		public bool HasErrors
		{
			get { return this.reSortieConditionError != null; }
		}

		public event EventHandler<DataErrorsChangedEventArgs> ErrorsChanged;


		public SettingsViewModel()
		{
			if (Helper.IsInDesignMode) return;

			this.FlashQualityList = FlashQualities.ToList();
			this.FlashWindowList = FlashWindows.ToList();

			this.Libraries = App.ProductInfo.Libraries.Aggregate(
				new List<BindableTextViewModel>(),
				(list, lib) =>
				{
					list.Add(new BindableTextViewModel { Text = list.Count == 0 ? "Build with " : ", " });
					list.Add(new HyperlinkViewModel { Text = lib.Name.Replace(' ', Convert.ToChar(160)), Uri = lib.Url });
					// プロダクト名の途中で改行されないように、space を non-break space に置き換えてあげてるんだからねっっ
					return list;
				});

			this.Cultures = new[] { new CultureViewModel { DisplayName = "(Auto)" } }
				.Concat(ResourceService.Current.SupportedCultures
					.Select(x => new CultureViewModel { DisplayName = x.EnglishName, Name = x.Name })
					.OrderBy(x => x.DisplayName))
				.ToList();

			this.CompositeDisposable.Add(new PropertyChangedEventListener(Settings.Current)
			{
				(sender, args) => this.RaisePropertyChanged(args.PropertyName),
			});

			this._IsDarkTheme = ThemeService.Current.Theme == Theme.Dark;
			this._IsLightTheme = ThemeService.Current.Theme == Theme.Light;

			var zoomFactor = new BrowserZoomFactor { Current = Settings.Current.BrowserZoomFactor };
			this.CompositeDisposable.Add(new PropertyChangedEventListener(zoomFactor)
			{
				{ "Current", (sender, args) => Settings.Current.BrowserZoomFactor = zoomFactor.Current },
			});
			this.BrowserZoomFactor = zoomFactor;

			this.CompositeDisposable.Add(new PropertyChangedEventListener(KanColleClient.Current.Homeport.Translations)
			{
				(sender, args) => this.RaisePropertyChanged(args.PropertyName),
			});
		}


		public void OpenScreenshotFolderSelectionDialog()
		{
			var message = new FolderSelectionMessage("OpenFolderDialog/Screenshot")
			{
				Title = Resources.Settings_Screenshot_FolderSelectionDialog_Title,
				DialogPreference = Helper.IsWindows8OrGreater
					? FolderSelectionDialogPreference.CommonItemDialog
					: FolderSelectionDialogPreference.FolderBrowser,
				SelectedPath = this.CanOpenScreenshotFolder
					? this.ScreenshotFolder
					: ""
			};
			this.Messenger.Raise(message);

			if (Directory.Exists(message.Response))
			{
				this.ScreenshotFolder = message.Response;
			}
		}

		public void OpenScreenshotFolder()
		{
			if (this.CanOpenScreenshotFolder)
			{
				try
				{
					Process.Start(this.ScreenshotFolder);
				}
				catch (Exception ex)
				{
					Debug.WriteLine(ex);
				}
			}
		}

		public void ClearZoomFactor()
		{
			App.ViewModelRoot.Messenger.Raise(new InteractionMessage { MessageKey = "WebBrowser/Zoom" });
		}

		public void SetLocationLeft()
		{
			App.ViewModelRoot.Messenger.Raise(new SetWindowLocationMessage { MessageKey = "Window/Location", Left = 0.0 });
		}


		public IEnumerable GetErrors(string propertyName)
		{
			var errors = new List<string>();

			switch (propertyName)
			{
				case "ReSortieCondition":
					if (this.reSortieConditionError != null)
					{
						errors.Add(this.reSortieConditionError);
					}
					break;
			}

			return errors.HasItems() ? errors : null;
		}

		protected void RaiseErrorsChanged([CallerMemberName]string propertyName = "")
		{
			if (this.ErrorsChanged != null)
			{
				this.ErrorsChanged(this, new DataErrorsChangedEventArgs(propertyName));
			}
		}

		public void CheckForUpdates()
		{
			if (KanColleClient.Current.Updater.LoadVersion(Properties.Settings.Default.KCVUpdateUrl.AbsoluteUri))
			{
				AppOnlineVersionURL = KanColleClient.Current.Updater.GetOnlineVersion(TranslationType.App, true);
				EquipmentOnlineVersionURL = KanColleClient.Current.Updater.GetOnlineVersion(TranslationType.Equipment, true);
				OperationsOnlineVersionURL = KanColleClient.Current.Updater.GetOnlineVersion(TranslationType.Operations, true);
				QuestsOnlineVersionURL = KanColleClient.Current.Updater.GetOnlineVersion(TranslationType.Quests, true);
				ShipsOnlineVersionURL = KanColleClient.Current.Updater.GetOnlineVersion(TranslationType.Ships, true);
				ShipTypesOnlineVersionURL = KanColleClient.Current.Updater.GetOnlineVersion(TranslationType.ShipTypes, true);

				AppOnlineVersion = KanColleClient.Current.Updater.GetOnlineVersion(TranslationType.App);
				EquipmentOnlineVersion = KanColleClient.Current.Updater.GetOnlineVersion(TranslationType.Equipment);
				OperationsOnlineVersion = KanColleClient.Current.Updater.GetOnlineVersion(TranslationType.Operations);
				QuestsOnlineVersion = KanColleClient.Current.Updater.GetOnlineVersion(TranslationType.Quests);
				ShipsOnlineVersion = KanColleClient.Current.Updater.GetOnlineVersion(TranslationType.Ships);
				ShipTypesOnlineVersion = KanColleClient.Current.Updater.GetOnlineVersion(TranslationType.ShipTypes);
			}
			else
			{
				WindowsNotification.Notifier.Show(
					Resources.Updater_Notification_Title,
					Resources.Updater_Notification_CheckFailed,
					() => App.ViewModelRoot.Activate());
			}
		}

		public void UpdateTranslations()
		{
			int UpdateStatus = KanColleClient.Current.Updater.UpdateTranslations(Properties.Settings.Default.XMLTransUrl.AbsoluteUri, Culture, KanColleClient.Current.Homeport.Translations);
			
			if (UpdateStatus > 0)
			{
				WindowsNotification.Notifier.Show(
					Resources.Updater_Notification_Title,
					Resources.Updater_Notification_TransUpdate_Success,
					() => App.ViewModelRoot.Activate());
			}
			else if (UpdateStatus < 0)
			{
				WindowsNotification.Notifier.Show(
					Resources.Updater_Notification_Title,
					Resources.Updater_Notification_TransUpdate_Fail,
					() => App.ViewModelRoot.Activate());
			}
			else
			{
				WindowsNotification.Notifier.Show(
					Resources.Updater_Notification_Title,
					Resources.Updater_Notification_TransUpdate_Same,
					() => App.ViewModelRoot.Activate());
			}
				
			KanColleClient.Current.Homeport.Translations.ChangeCulture(Culture);
		}
	}
}
