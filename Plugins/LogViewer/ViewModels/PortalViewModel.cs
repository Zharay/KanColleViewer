using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Grabacr07.KanColleWrapper;
using Livet;

namespace Grabacr07.KanColleViewer.Plugins.ViewModels
{
    public class PortalViewModel : ViewModel
    {
        #region LogCollection 変更通知プロパティ

        private LogItemCollection logCollection;

        public LogItemCollection LogCollection
        {
            get { return this.logCollection; }
            set
            {
                if (this.logCollection != value)
                {
                    this.logCollection = value;
                    this.RaisePropertyChanged();
                }
            }
        }

        #endregion

        #region IsReloading 変更通知プロパティ

        private bool isReloading;

        public bool IsReloading
        {
            get { return this.isReloading; }
            set
            {
                if (this.isReloading != value)
                {
                    this.isReloading = value;
                    this.RaisePropertyChanged();
                }
            }
        }

        #endregion

        #region SelectorBuildItem 変更通知プロパティ

        private bool selectorBuildItem;

        public bool SelectorBuildItem
        {
            get { return this.selectorBuildItem; }
            set
            {
                if (this.selectorBuildItem != value)
                {
                    this.selectorBuildItem = value;
                    if (value)
                        this.CurrentLogType = Logger.LogType.BuildItem;
                    this.RaisePropertyChanged();
                }
            }
        }

        #endregion

        #region SelectorBuildShip 変更通知プロパティ

        private bool selectorBuildShip;

        public bool SelectorBuildShip
        {
            get { return this.selectorBuildShip; }
            set
            {
                if (this.selectorBuildShip != value)
                {
                    this.selectorBuildShip = value;
                    if (value)
                        this.CurrentLogType = Logger.LogType.BuildShip;
                    this.RaisePropertyChanged();
                }
            }
        }

        #endregion

        #region SelectorShipDrop 変更通知プロパティ

        private bool selectorShipDrop = true;

        public bool SelectorShipDrop
        {
            get { return this.selectorShipDrop; }
            set
            {
                if (this.selectorShipDrop != value)
                {
                    this.selectorShipDrop = value;
                    if (value)
                        this.CurrentLogType = Logger.LogType.ShipDrop;
                    this.RaisePropertyChanged();
                }
            }
        }

        #endregion

		#region SelectorMaterials 変更通知プロパティ

		private bool selectorMaterials;

		public bool SelectorMaterials
		{
			get { return this.selectorMaterials; }
			set
			{
				if (this.selectorMaterials != value)
				{
					this.selectorMaterials = value;
					if (value)
						this.CurrentLogType = Logger.LogType.Materials;
					this.RaisePropertyChanged();
				}
			}
		}

		#endregion

        #region CurrentLogType

        private Logger.LogType currentLogType;

        private Logger.LogType CurrentLogType
        {
            get { return this.currentLogType; }
            set
            {
                if (this.currentLogType != value)
                {
                    this.currentLogType = value;

	                this._watcher.Filter = Logger.LogParameters[value].FileName;

					this.CurrentPage = 1;
                    this.Update();
                }
            }
        }

		#endregion

		#region HasPreviousPage 変更通知プロパティ

		public bool HasPreviousPage
		{
			get { return this.TotalPage > 1 && this.CurrentPage > 1; }
		}

		#endregion

		#region HasNextPage 変更通知プロパティ

		public bool HasNextPage
		{
			get { return this.TotalPage > 1 && this.CurrentPage < this.TotalPage; }
		}

		#endregion

		#region CurrentPage 変更通知プロパティ

		private int _CurrentPage;

		public int CurrentPage
		{
			get { return this._CurrentPage; }
			set
			{
				if (this._CurrentPage != value)
				{
					this._CurrentPage = value;
					this.RaisePropertyChanged("HasPreviousPage");
					this.RaisePropertyChanged("HasNextPage");
					this.RaisePropertyChanged();
				}
			}
		}

		#endregion

		#region TotalPage 変更通知プロパティ

		private int _TotalPage;

		public int TotalPage
		{
			get { return this._TotalPage; }
			set
			{
				if (this._TotalPage != value)
				{
					this._TotalPage = value;
					this.RaisePropertyChanged("HasPreviousPage");
					this.RaisePropertyChanged("HasNextPage");
					this.RaisePropertyChanged();
				}
			}
		}

		#endregion

        private FileSystemWatcher _watcher;

	    private int logPerPage = 10;

        public PortalViewModel()
        {
            this.SelectorShipDrop = true;
            this.currentLogType = Logger.LogType.ShipDrop;
			this.CurrentPage = 1;

            this.Update();

            try
            {

                this._watcher = new FileSystemWatcher(Directory.GetParent(Logger.LogFolder).ToString())
                {
                    IncludeSubdirectories = true,
                    NotifyFilter = NotifyFilters.LastWrite | NotifyFilters.CreationTime | NotifyFilters.FileName,
                    Filter = Logger.LogParameters[this.currentLogType].FileName,
                    EnableRaisingEvents = true
                };

                this._watcher.Changed += (sender, e) => { this.Update(); };
                this._watcher.Created += (sender, e) => { this.Update(); };
                this._watcher.Deleted += (sender, e) => { this.Update(); };
                this._watcher.Renamed += (sender, e) => { this.Update(); };
            }
            catch (Exception)
            {
                if (this._watcher != null)
                    this._watcher.EnableRaisingEvents = false;
            }
        }

        public async void Update()
        {
            this.IsReloading = true;
            this.LogCollection = await this.UpdateCore();
            this.IsReloading = false;
        }

        private Task<LogItemCollection> UpdateCore()
        {
            LogItemCollection items = new LogItemCollection();

            return Task.Factory.StartNew(() =>
            {
                try
                {
                    string file = Path.Combine(Logger.LogFolder,
                        Logger.LogParameters[this.CurrentLogType].FileName);
                    if (!File.Exists(file))
                        return items;

                    IEnumerable<string> lines = File.ReadLines(file);

	                this.TotalPage = (lines.Count() - 1)/20 + 1;

	                // ReSharper disable once PossibleMultipleEnumeration
                    lines.Take(1).First().Split(',').ToList().ForEach((col => items.Columns.Add(col)));

	                // ReSharper disable once PossibleMultipleEnumeration
					lines.Skip(1).Reverse().Skip((this.CurrentPage - 1) * this.logPerPage).Take(this.logPerPage).ToList().ForEach(line =>
                    {
                        string[] elements = line.Split(',');

                        if (elements.Length < 5)
                            return;

                        items.Rows.Add(elements
                            .Take(items.Columns.Count)
                            .ToArray());
                    });

                    return items;
                }
                catch (Exception)
                {
                    return items;
                }
            });
        }

	    public void ToPreviousPage()
	    {
		    --this.CurrentPage;
		    this.Update();
	    }

		public void ToNextPage()
		{
			++this.CurrentPage;
			this.Update();
		}
    }
}
