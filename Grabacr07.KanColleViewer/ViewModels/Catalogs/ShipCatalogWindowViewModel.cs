using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Threading.Tasks;
using Grabacr07.KanColleViewer.ViewModels.Contents;
using Grabacr07.KanColleWrapper;
using Livet.EventListeners;
using Settings = Grabacr07.KanColleViewer.Models.Settings;

namespace Grabacr07.KanColleViewer.ViewModels.Catalogs
{
	public class ShipCatalogWindowViewModel : WindowViewModel
	{
		private readonly Subject<Unit> updateSource = new Subject<Unit>();
		private readonly Homeport homeport = KanColleClient.Current.Homeport;

		public ShipCatalogSortWorker SortWorker { get; }
		public IReadOnlyCollection<ShipTypeViewModel> ShipTypes { get; }

		public ShipLevelFilter ShipLevelFilter { get; }
		public ShipLockFilter ShipLockFilter { get; }
		public ShipSpeedFilter ShipSpeedFilter { get; }
		public ShipModernizeFilter ShipModernizeFilter { get; }
		public ShipRemodelingFilter ShipRemodelingFilter { get; }
		public ShipExpeditionFilter ShipExpeditionFilter { get; }
		public ShipSallyAreaFilter ShipSallyAreaFilter { get; }

		public bool CheckAllShipTypes
		{
			get { return this.ShipTypes.All(x => x.IsSelected); }
			set
			{
				this.ShipTypes.ForEach(x => x.Set(value));
				this.Update();
			}
		}

		#region Ships 変更通知プロパティ

		private IReadOnlyCollection<ShipViewModel> _Ships;

		public IReadOnlyCollection<ShipViewModel> Ships
		{
			get { return this._Ships; }
			set
			{
				if (this._Ships != value)
				{
					this._Ships = value;
					this.RaisePropertyChanged();
				}
			}
		}

		#endregion

		#region IsOpenSettings 変更通知プロパティ

		private bool _IsOpenSettings;

		public bool IsOpenSettings
		{
			get { return this._IsOpenSettings; }
			set
			{
				if (this._IsOpenSettings != value)
				{
					this._IsOpenSettings = value;
					this.RaisePropertyChanged();
				}
			}
		}

		#endregion

		#region IsReloading 変更通知プロパティ

		private bool _IsReloading;

		public bool IsReloading
		{
			get { return this._IsReloading; }
			set
			{
				if (this._IsReloading != value)
				{
					this._IsReloading = value;
					this.RaisePropertyChanged();
				}
			}
		}

		#endregion

		#region SaveFilters 変更通知プロパティ

		public bool SaveFilters
		{
			get { return Settings.Current.ShipCatalog_SaveFilters; }
			set
			{
				if (Settings.Current.ShipCatalog_SaveFilters != value)
				{
					Settings.Current.ShipCatalog_SaveFilters = value;
					this.RaisePropertyChanged();
				}
			}
		}

		#endregion

		#region ShowMoreStats 変更通知プロパティ

		public bool ShowMoreStats
		{
			get { return Settings.Current.ShipCatalog_ShowMoreStats; }
			set
			{
				if (Settings.Current.ShipCatalog_ShowMoreStats != value)
				{
					Settings.Current.ShipCatalog_ShowMoreStats = value;
					this.RaisePropertyChanged();
				}
			}
		}

		#endregion

		public void ResetFilters ()
		{
			this.ShipLevelFilter.Level2OrMore = true;
			this.ShipLevelFilter.Both = this.ShipLevelFilter.Level1 = false;

			this.ShipLockFilter.Locked = true;
			this.ShipLockFilter.Both = this.ShipLockFilter.Unlocked = false;

			this.ShipSpeedFilter.Both = true;
			this.ShipSpeedFilter.Fast = this.ShipSpeedFilter.Low = false;

			this.ShipModernizeFilter.Both = true;
			this.ShipModernizeFilter.MaxModernized = this.ShipModernizeFilter.NotMaxModernized = false;

			this.ShipRemodelingFilter.Both = true;
			this.ShipRemodelingFilter.AlreadyRemodeling = this.ShipRemodelingFilter.NotAlreadyRemodeling = false;

			this.ShipExpeditionFilter.WithoutExpedition = false;

			this.CheckAllShipTypes = true;

			this.Update();
		}

		public ShipCatalogWindowViewModel()
		{
			this.Title = "Ship Girl Roster";
			this.IsOpenSettings = true;

			this.SortWorker = new ShipCatalogSortWorker();
			this.SortWorker.SetTarget(ShipCatalogSortTarget.Level, true);

			this.ShipTypes = KanColleClient.Current.Master.ShipTypes
				.Select(kvp => new ShipTypeViewModel(kvp.Value)
				{
					IsSelected = true,
					SelectionChangedAction = () => this.Update()
				})
				.ToList();

			this.ShipLevelFilter = new ShipLevelFilter(this.Update);
			this.ShipLockFilter = new ShipLockFilter(this.Update);
			this.ShipSpeedFilter = new ShipSpeedFilter(this.Update);
			this.ShipModernizeFilter = new ShipModernizeFilter(this.Update);
			this.ShipRemodelingFilter = new ShipRemodelingFilter(this.Update);
			this.ShipExpeditionFilter = new ShipExpeditionFilter(this.Update);
			this.ShipSallyAreaFilter = new ShipSallyAreaFilter(this.Update);

			this.updateSource
				.Do(_ => this.IsReloading = true)
				.Throttle(TimeSpan.FromMilliseconds(7.0))
				.Do(_ => this.UpdateCore())
				.Subscribe(_ => this.IsReloading = false);
			this.CompositeDisposable.Add(this.updateSource);

			this.CompositeDisposable.Add(new PropertyChangedEventListener(this.homeport.Organization)
			{
				{ "Ships", (sender, args) => this.Update() },
			});

			this.Update();
		}


		public void Update()
		{
			this.ShipExpeditionFilter.SetFleets(this.homeport.Organization.Fleets);

			this.RaisePropertyChanged("CheckAllShipTypes");
			this.updateSource.OnNext(Unit.Default);
		}

		public void Update(ShipCatalogSortTarget sortTarget)
		{
			this.SortWorker.SetTarget(sortTarget, false);
			this.Update();
		}
		public void UpdateReverse(ShipCatalogSortTarget sortTarget)
		{
			this.SortWorker.SetTarget(sortTarget, true);
			this.Update();
		}

		private void UpdateCore()
		{
			var list = this.homeport.Organization.Ships
				.Select(kvp => kvp.Value)
				.Where(x => this.ShipTypes.Where(t => t.IsSelected).Any(t => x.Info.ShipType.Id == t.Id))
				.Where(this.ShipLevelFilter.Predicate)
				.Where(this.ShipLockFilter.Predicate)
				.Where(this.ShipSpeedFilter.Predicate)
				.Where(this.ShipModernizeFilter.Predicate)
				.Where(this.ShipRemodelingFilter.Predicate)
				.Where(this.ShipExpeditionFilter.Predicate)
				.Where(this.ShipSallyAreaFilter.Predicate);

			this.Ships = this.SortWorker.Sort(list)
				.Select((x, i) => new ShipViewModel(i + 1, x))
				.ToList();
		}


		public void SetShipType(int[] ids)
		{
			this.ShipTypes.ForEach(x => x.Set(ids.Any(id => x.Id == id)));
			this.Update();
		}

		public void AdditionalStats(bool bShow)
		{
			if (bShow)
			{

			}
		}
	}
}
