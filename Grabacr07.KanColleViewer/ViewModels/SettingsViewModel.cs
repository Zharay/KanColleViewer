using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Grabacr07.KanColleViewer.Composition;
using Grabacr07.KanColleViewer.Models;
using Grabacr07.KanColleViewer.Properties;
using Grabacr07.KanColleViewer.ViewModels.Composition;
using Grabacr07.KanColleViewer.ViewModels.Messages;
using Grabacr07.KanColleWrapper;
using Grabacr07.KanColleWrapper.Models;
using Livet.EventListeners;
using Livet.Messaging;
using Livet.Messaging.IO;
using MetroRadiance;
using Settings = Grabacr07.KanColleViewer.Models.Settings;
using Livet;

namespace Grabacr07.KanColleViewer.ViewModels
{
	public class SettingsViewModel : TabItemViewModel
	{
		public override string Name
		{
			get { return Resources.Settings; }
			protected set { throw new NotImplementedException(); }
		}

		public IEnumerable<string> FlashQualityList {get; private set;}
		public string[] FlashQualities = { "Best", "High", "AutoHigh",  "Medium", "AutoLow", "Low" };

		public IEnumerable<string> FlashWindowList { get; private set; }
		public string[] FlashWindows = { "Opaque", "Direct", "GPU" };

        public IEnumerable<string> BrowserVerticalPositionList { get; private set; }
        public string[] BrowserVerticalPositions = { "Top", "Bottom"};

        public IEnumerable<string> BrowserHorizontalPositionList { get; private set; }
        public string[] BrowserHorizontalPositions = { "Left", "Right" };

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
					KanColleClient.Current.Translations.ChangeCulture(value);
					if (KanColleClient.Current != null && KanColleClient.Current.Homeport != null && KanColleClient.Current.Homeport.Admiral != null)
					{
						KanColleClient.Current.Homeport.Admiral.Update();
					}

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
                    KanColleClient.Current.Translations.EnableTranslations = value;
                    KanColleClient.Current.Translations.ChangeCulture(this.Culture);
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
                    KanColleClient.Current.Translations.EnableAddUntranslated = value;
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
        
		#region EnableLogging 変更通知プロパティ

		public bool EnableLogging
		{
			get { return Settings.Current.KanColleClientSettings.EnableLogging; }
			set
			{
				if (Settings.Current.KanColleClientSettings.EnableLogging != value)
				{
					Settings.Current.KanColleClientSettings.EnableLogging = value;
					KanColleClient.Current.Homeport.Logger.EnableLogging = value;
					this.RaisePropertyChanged();
				}
			}
		}

		#endregion

		#region EquipmentVersion 変更通知プロパティ

		public string EquipmentVersion
		{
			get { return KanColleClient.Current.Translations.EquipmentVersion; }
			set
			{
				if (KanColleClient.Current.Translations.EquipmentVersion != value)
				{
					KanColleClient.Current.Translations.EquipmentVersion = value;
					this.RaisePropertyChanged();
				}
			}
		}

		#endregion

		#region OperationsVersion 変更通知プロパティ

		public string OperationsVersion
		{
			get { return KanColleClient.Current.Translations.OperationsVersion; }
			set
			{
				if (KanColleClient.Current.Translations.OperationsVersion != value)
				{
					KanColleClient.Current.Translations.OperationsVersion = value;
					this.RaisePropertyChanged();
				}
			}
		}

		#endregion

		#region QuestsVersion 変更通知プロパティ

		public string QuestsVersion
		{
			get { return KanColleClient.Current.Translations.QuestsVersion; }
			set
			{
				if (KanColleClient.Current.Translations.QuestsVersion != value)
				{
					KanColleClient.Current.Translations.QuestsVersion = value;
					this.RaisePropertyChanged();
				}
			}
		}

		#endregion

		#region ShipsVersion 変更通知プロパティ

		public string ShipsVersion
		{
			get { return KanColleClient.Current.Translations.ShipsVersion; }
			set
			{
				if (KanColleClient.Current.Translations.ShipsVersion != value)
				{
					KanColleClient.Current.Translations.ShipsVersion = value;
					this.RaisePropertyChanged();
				}
			}
		}

		#endregion

		#region ShipTypesVersion 変更通知プロパティ

		public string ShipTypesVersion
		{
			get { return KanColleClient.Current.Translations.ShipTypesVersion; }
			set
			{
				if (KanColleClient.Current.Translations.ShipTypesVersion != value)
				{
					KanColleClient.Current.Translations.ShipTypesVersion = value;
					this.RaisePropertyChanged();
				}
			}
		}

		#endregion

        #region ExpeditionsVersion 変更通知プロパティ

        public string ExpeditionsVersion
        {
            get { return KanColleClient.Current.Translations.ExpeditionsVersion; }
            set
            {
                if (KanColleClient.Current.Translations.ExpeditionsVersion != value)
                {
                    KanColleClient.Current.Translations.ExpeditionsVersion = value;
                    this.RaisePropertyChanged();
                }
            }
        }

        #endregion

		// ReSharper disable InconsistentNaming
		#region AppOnlineVersion 変更通知プロパティ

		private string _AppOnlineVersion;
		public string AppOnlineVersionURL { get; set; }

		public string AppOnlineVersion
		{
			get { return this._AppOnlineVersion; }
			set
			{
				if (this._AppOnlineVersion != value)
				{
					this._AppOnlineVersion = value;
					this.RaisePropertyChanged();
					this.RaisePropertyChanged("AppOnlineVersionURL");
					this.RaisePropertyChanged("IsUpdateAvailable");
				}
			}
		}

		#endregion

		#region EquipmentOnlineVersion 変更通知プロパティ

		private string _EquipmentOnlineVersion;

		public string EquipmentOnlineVersionURL { get; set; }

		public string EquipmentOnlineVersion
		{
			get { return this._EquipmentOnlineVersion; }
			set
			{
				if (this._EquipmentOnlineVersion != value)
				{
					this._EquipmentOnlineVersion = value;
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
			get { return this._OperationsOnlineVersion; }
			set
			{
				if (this._OperationsOnlineVersion != value)
				{
					this._OperationsOnlineVersion = value;
					this.RaisePropertyChanged();
					this.RaisePropertyChanged("OperationsOnlineVersionURL");
				}
			}
		}

		#endregion

		#region QuestsOnlineVersion 変更通知プロパティ

		private string _QuestsOnlineVersion;

		public string QuestsOnlineVersionURL { get; set; }

		public string QuestsOnlineVersion
		{
			get { return this._QuestsOnlineVersion; }
			set
			{
				if (this._QuestsOnlineVersion != value)
				{
					this._QuestsOnlineVersion = value;
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
			get { return this._ShipsOnlineVersion; }
			set
			{
				if (this._ShipsOnlineVersion != value)
				{
					this._ShipsOnlineVersion = value;
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
			get { return this._ShipTypesOnlineVersion; }
			set
			{
				if (this._ShipTypesOnlineVersion != value)
				{
					this._ShipTypesOnlineVersion = value;
					this.RaisePropertyChanged();
					this.RaisePropertyChanged("ShipTypesOnlineVersionURL");
				}
			}
		}

		#endregion

        #region ExpeditionsOnlineVersion 変更通知プロパティ

        private string _ExpeditionsOnlineVersion;
        public string ExpeditionsOnlineVersionURL { get; set; }

        public string ExpeditionsOnlineVersion
        {
            get { return this._ExpeditionsOnlineVersion; }
            set
            {
                if (this._ExpeditionsOnlineVersion != value)
                {
	                this._ExpeditionsOnlineVersion = value;
                    this.RaisePropertyChanged();
                    this.RaisePropertyChanged("ExpeditionsOnlineVersionURL");
                }
            }
        }

        #endregion
		
		// ReSharper restore InconsistentNaming

		#region NotifierPlugins 変更通知プロパティ

		private List<NotifierViewModel> _NotifierPlugins;

		public List<NotifierViewModel> NotifierPlugins
		{
			get { return this._NotifierPlugins; }
			set
			{
				if (this._NotifierPlugins != value)
				{
					this._NotifierPlugins = value;
					this.RaisePropertyChanged();
				}
			}
		}

		#endregion

		#region ToolPlugins 変更通知プロパティ

		private List<ToolViewModel> _ToolPlugins;

		public List<ToolViewModel> ToolPlugins
		{
			get { return this._ToolPlugins; }
			set
			{
				if (this._ToolPlugins != value)
				{
					this._ToolPlugins = value;
					this.RaisePropertyChanged();
				}
			}
		}

		#endregion

		#region ViewRangeSettingsCollection 変更通知プロパティ

		private List<ViewRangeSettingsViewModel> _ViewRangeSettingsCollection;

		public List<ViewRangeSettingsViewModel> ViewRangeSettingsCollection
		{
			get { return this._ViewRangeSettingsCollection; }
			set
			{
				if (this._ViewRangeSettingsCollection != value)
				{
					this._ViewRangeSettingsCollection = value;
					this.RaisePropertyChanged();
				}
			}
		}

		#endregion

		#region IsUpdateAvailable 変更通知プロパティ

		public bool IsUpdateAvailable
		{
			get { return KanColleClient.Current.Updater.IsOnlineVersionGreater(0, App.ProductInfo.Version.ToString()); }
		}

		#endregion

		public SettingsViewModel()
		{
			if (Helper.IsInDesignMode) return;

			this.FlashQualityList = this.FlashQualities.ToList();
			this.FlashWindowList = this.FlashWindows.ToList();
            this.BrowserVerticalPositionList = this.BrowserVerticalPositions.ToList();
            this.BrowserHorizontalPositionList = this.BrowserHorizontalPositions.ToList();

			this.Libraries = App.ProductInfo.Libraries.Aggregate(
				new List<BindableTextViewModel>(),
				(list, lib) =>
				{
					list.Add(new BindableTextViewModel { Text = list.Count == 0 ? "Build with " : ", " });
					list.Add(new HyperlinkViewModel { Text = lib.Name.Replace(' ', Convert.ToChar(160)), Uri = lib.Url });
					// プロダクト名の途中で改行されないように、space を non-break space に置き換えてあげてるんだからねっっ
					return list;
				});

			this.Cultures = new[] { new CultureViewModel { DisplayName = "(auto)" } }
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

			var orientationMode = new WindowOrientaionMode { CurrentMode = Settings.Current.OrientationMode };
			orientationMode.Refresh();
			this.CompositeDisposable.Add(orientationMode);
			this.CompositeDisposable.Add(new PropertyChangedEventListener(orientationMode)
			{
                { "CurrentMode", (sender, args) => Settings.Current.OrientationMode = orientationMode.CurrentMode },
			});
			Settings.Current.Orientation = orientationMode;

			this.CompositeDisposable.Add(new PropertyChangedEventListener(KanColleClient.Current.Translations)
			{
				(sender, args) => this.RaisePropertyChanged(args.PropertyName),
			});

			if (Settings.Current.EnableUpdateNotification)
			{
				this.CheckForUpdates();
			}

			this.ViewRangeSettingsCollection = ViewRangeCalcLogic.Logics
				.Select(x => new ViewRangeSettingsViewModel(x))
				.ToList();

			this.ReloadPlugins();
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

		public void CheckForUpdates()
		{
			Task.Factory.StartNew(
				() =>
				{
					if (KanColleClient.Current.Updater.LoadVersion(Properties.Settings.Default.KCVUpdateUrl.AbsoluteUri, Properties.Settings.Default.KCVUpdateTransUrl.AbsoluteUri))
					{
						this.AppOnlineVersionURL = KanColleClient.Current.Updater.GetOnlineVersion(TranslationType.App, true);
						this.EquipmentOnlineVersionURL = KanColleClient.Current.Updater.GetOnlineVersion(TranslationType.Equipment, true);
						this.OperationsOnlineVersionURL = KanColleClient.Current.Updater.GetOnlineVersion(TranslationType.Operations, true);
						this.QuestsOnlineVersionURL = KanColleClient.Current.Updater.GetOnlineVersion(TranslationType.Quests, true);
						this.ShipsOnlineVersionURL = KanColleClient.Current.Updater.GetOnlineVersion(TranslationType.Ships, true);
						this.ShipTypesOnlineVersionURL = KanColleClient.Current.Updater.GetOnlineVersion(TranslationType.ShipTypes, true);
						this.ExpeditionsOnlineVersionURL = KanColleClient.Current.Updater.GetOnlineVersion(TranslationType.Expeditions, true);

						this.AppOnlineVersion = KanColleClient.Current.Updater.GetOnlineVersion(TranslationType.App);
						this.EquipmentOnlineVersion = KanColleClient.Current.Updater.GetOnlineVersion(TranslationType.Equipment);
						this.OperationsOnlineVersion = KanColleClient.Current.Updater.GetOnlineVersion(TranslationType.Operations);
						this.QuestsOnlineVersion = KanColleClient.Current.Updater.GetOnlineVersion(TranslationType.Quests);
						this.ShipsOnlineVersion = KanColleClient.Current.Updater.GetOnlineVersion(TranslationType.Ships);
						this.ShipTypesOnlineVersion = KanColleClient.Current.Updater.GetOnlineVersion(TranslationType.ShipTypes);
						this.ExpeditionsOnlineVersion = KanColleClient.Current.Updater.GetOnlineVersion(TranslationType.Expeditions);
					}
					else
					{
						PluginHost.Instance.GetNotifier().Show(NotifyType.Other,
							Resources.Updater_Notification_Title,
							Resources.Updater_Notification_CheckFailed,
							() => App.ViewModelRoot.Activate());
					}
				});
		}

		public void UpdateTranslations()
		{
			TaskScheduler ts = TaskScheduler.FromCurrentSynchronizationContext();
			Task.Factory.StartNew(
				() =>
				{
					int updateStatus = KanColleClient.Current.Updater.UpdateTranslations(KanColleClient.Current.Translations);
					this.NotifyUpdate(updateStatus);
				}).ContinueWith(t => KanColleClient.Current.Translations.ChangeCulture(this.Culture), ts);

		}

		public void DownloadTranslations()
		{
			TaskScheduler ts = TaskScheduler.FromCurrentSynchronizationContext();
			Task.Factory.StartNew(
				() =>
				{
					int updateStatus = KanColleClient.Current.Updater.UpdateTranslations(KanColleClient.Current.Translations, false);
					this.NotifyUpdate(updateStatus);
				}).ContinueWith(t => KanColleClient.Current.Translations.ChangeCulture(this.Culture), ts);

		}

		private void NotifyUpdate(int updateStatus) 
		{
			if (updateStatus > 0)
			{
				PluginHost.Instance.GetNotifier().Show(NotifyType.Other,
					Resources.Updater_Notification_Title,
					Resources.Updater_Notification_TransUpdate_Success,
					() => App.ViewModelRoot.Activate());
			}
			else if (updateStatus < 0)
			{
				PluginHost.Instance.GetNotifier().Show(NotifyType.Other,
					Resources.Updater_Notification_Title,
					Resources.Updater_Notification_TransUpdate_Fail,
					() => App.ViewModelRoot.Activate());
			}
			else
			{
				PluginHost.Instance.GetNotifier().Show(NotifyType.Other,
					Resources.Updater_Notification_Title,
					Resources.Updater_Notification_TransUpdate_Same,
					() => App.ViewModelRoot.Activate());
			}
		}

		// ReSharper disable once InconsistentNaming
		public void OpenKCVLink()
		{
			try
			{
				Process.Start(this.AppOnlineVersionURL);
			}
			catch (Exception ex)
			{
				Debug.WriteLine(ex);
			}
		}

		public void ReloadPlugins()
		{
			this.NotifierPlugins = new List<NotifierViewModel>(PluginHost.Instance.Notifiers.Select(x => new NotifierViewModel(x)));
			this.ToolPlugins = new List<ToolViewModel>(PluginHost.Instance.Tools.Select(x => new ToolViewModel(x)));
		}


		public class ViewRangeSettingsViewModel : ViewModel
		{
			private bool selected;

			public ICalcViewRange Logic { get; set; }

			public bool Selected
			{
				get { return this.selected; }
				set
				{
					this.selected = value;
					if (value)
					{
						Settings.Current.KanColleClientSettings.ViewRangeCalcType = this.Logic.Id;
					}
				}
			}

			public ViewRangeSettingsViewModel(ICalcViewRange logic)
			{
				this.Logic = logic;
				this.selected = Settings.Current.KanColleClientSettings.ViewRangeCalcType == logic.Id;

				this.CompositeDisposable.Add(new PropertyChangedEventListener(ResourceService.Current)
				{
					(sender, args) =>
					{
						this.RaisePropertyChanged("Logic");
					},
				});
			}
		}
	}
}
