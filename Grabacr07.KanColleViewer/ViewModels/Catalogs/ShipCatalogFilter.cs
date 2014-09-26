using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Grabacr07.KanColleWrapper;
using Grabacr07.KanColleWrapper.Models;
using Livet;
using Settings = Grabacr07.KanColleViewer.Models.Settings;

namespace Grabacr07.KanColleViewer.ViewModels.Catalogs
{
	public abstract class ShipCatalogFilter : NotificationObject
	{
		private readonly Action action;

		public abstract bool Predicate(Ship ship);

		protected ShipCatalogFilter(Action updateAction)
		{
			this.action = updateAction;
		}

		protected void Update()
		{
			if (this.action != null) this.action();
		}
	}

	public class ShipLevelFilter : ShipCatalogFilter
	{
		#region Both 変更通知プロパティ

		public bool Both
		{
			get { return Settings.Current.ShipCatalog_LevelFilter_Both; }
			set
			{
				if (Settings.Current.ShipCatalog_LevelFilter_Both != value)
				{
					Settings.Current.ShipCatalog_LevelFilter_Both = value;
					this.RaisePropertyChanged();
					this.Update();
				}
			}
		}

		#endregion

		#region Level1 変更通知プロパティ

		public bool Level1
		{
			get { return Settings.Current.ShipCatalog_LevelFilter_Level1; }
			set
			{
				if (Settings.Current.ShipCatalog_LevelFilter_Level1 != value)
				{
					Settings.Current.ShipCatalog_LevelFilter_Level1 = value;
					this.RaisePropertyChanged();
					this.Update();
				}
			}
		}

		#endregion

		#region Level2OrMore 変更通知プロパティ

		public bool Level2OrMore
		{
			get { return Settings.Current.ShipCatalog_LevelFilter_Level2OrMore; }
			set
			{
				if (Settings.Current.ShipCatalog_LevelFilter_Level2OrMore != value)
				{
					Settings.Current.ShipCatalog_LevelFilter_Level2OrMore = value;
					this.RaisePropertyChanged();
					this.Update();
				}
			}
		}

		#endregion

		public ShipLevelFilter(Action updateAction)
			: base(updateAction)
		{
			//this._Level2OrMore = true;
		}

		public override bool Predicate(Ship ship)
		{
			if (this.Both) return true;
			if (this.Level2OrMore && ship.Level >= 2) return true;
			if (this.Level1 && ship.Level == 1) return true;

			return false;
		}
	}

	public class ShipLockFilter : ShipCatalogFilter
	{
		#region Both 変更通知プロパティ

		public bool Both
		{
			get { return Settings.Current.ShipCatalog_LockFilter_Both; }
			set
			{
				if (Settings.Current.ShipCatalog_LockFilter_Both != value)
				{
					Settings.Current.ShipCatalog_LockFilter_Both = value;
					this.RaisePropertyChanged();
					this.Update();
				}
			}
		}

		#endregion

		#region Locked 変更通知プロパティ

		public bool Locked
		{
			get { return Settings.Current.ShipCatalog_LockFilter_Locked; }
			set
			{
				if (Settings.Current.ShipCatalog_LockFilter_Locked != value)
				{
					Settings.Current.ShipCatalog_LockFilter_Locked = value;
					this.RaisePropertyChanged();
					this.Update();
				}
			}
		}

		#endregion

		#region Unlocked 変更通知プロパティ

		public bool Unlocked
		{
			get { return Settings.Current.ShipCatalog_LockFilter_Unlocked; }
			set
			{
				if (Settings.Current.ShipCatalog_LockFilter_Unlocked != value)
				{
					Settings.Current.ShipCatalog_LockFilter_Unlocked = value;
					this.RaisePropertyChanged();
					this.Update();
				}
			}
		}

		#endregion

		public ShipLockFilter(Action updateAction)
			: base(updateAction)
		{
			//this._Locked = true;
		}

		public override bool Predicate(Ship ship)
		{
			if (this.Both) return true;
			if (this.Locked && ship.IsLocked) return true;
			if (this.Unlocked && !ship.IsLocked) return true;

			return false;
		}
	}

	public class ShipSpeedFilter : ShipCatalogFilter
	{
		#region Both 変更通知プロパティ

		public bool Both
		{
			get { return Settings.Current.ShipCatalog_SpeedFilter_Both; }
			set
			{
				if (Settings.Current.ShipCatalog_SpeedFilter_Both != value)
				{
					Settings.Current.ShipCatalog_SpeedFilter_Both = value;
					this.RaisePropertyChanged();
					this.Update();
				}
			}
		}

		#endregion

		#region Fast 変更通知プロパティ

		public bool Fast
		{
			get { return Settings.Current.ShipCatalog_SpeedFilter_Fast; }
			set
			{
				if (Settings.Current.ShipCatalog_SpeedFilter_Fast != value)
				{
					Settings.Current.ShipCatalog_SpeedFilter_Fast = value;
					this.RaisePropertyChanged();
					this.Update();
				}
			}
		}

		#endregion

		#region Low 変更通知プロパティ

		public bool Low
		{
			get { return Settings.Current.ShipCatalog_SpeedFilter_Low; }
			set
			{
				if (Settings.Current.ShipCatalog_SpeedFilter_Low != value)
				{
					Settings.Current.ShipCatalog_SpeedFilter_Low = value;
					this.RaisePropertyChanged();
				}
			}
		}

		#endregion


		public ShipSpeedFilter(Action updateAction)
			: base(updateAction)
		{
			//this._Both = true;
		}

		public override bool Predicate(Ship ship)
		{
			if (this.Both) return true;
			if (this.Fast && ship.Info.Speed == Speed.Fast) return true;
			if (this.Low && ship.Info.Speed == Speed.Low) return true;

			return false;
		}
	}

	public class ShipModernizeFilter : ShipCatalogFilter
	{
		#region Both 変更通知プロパティ

		public bool Both
		{
			get { return Settings.Current.ShipCatalog_ModernFilter_Both; }
			set
			{
				if (Settings.Current.ShipCatalog_ModernFilter_Both != value)
				{
					Settings.Current.ShipCatalog_ModernFilter_Both = value;
					this.RaisePropertyChanged();
					this.Update();
				}
			}
		}

		#endregion

		#region MaxModernized 変更通知プロパティ

		public bool MaxModernized
		{
			get { return Settings.Current.ShipCatalog_ModernFilter_MaxModernized; }
			set
			{
				if (Settings.Current.ShipCatalog_ModernFilter_MaxModernized != value)
				{
					Settings.Current.ShipCatalog_ModernFilter_MaxModernized = value;
					this.RaisePropertyChanged();
					this.Update();
				}
			}
		}

		#endregion

		#region NotMaxModernized 変更通知プロパティ

		public bool NotMaxModernized
		{
			get { return Settings.Current.ShipCatalog_ModernFilter_NotMaxModernized; }
			set
			{
				if (Settings.Current.ShipCatalog_ModernFilter_NotMaxModernized != value)
				{
					Settings.Current.ShipCatalog_ModernFilter_NotMaxModernized = value;
					this.RaisePropertyChanged();
					this.Update();
				}
			}
		}

		#endregion

		public ShipModernizeFilter(Action updateAction)
			: base(updateAction)
		{
			//this._Both = true;
		}

		public override bool Predicate(Ship ship)
		{
			if (this.Both) return true;
			if (this.MaxModernized && ship.IsMaxModernized) return true;
			if (this.NotMaxModernized && !ship.IsMaxModernized) return true;

			return false;
		}
	}

	public class ShipRemodelingFilter : ShipCatalogFilter
	{
		#region Both 変更通知プロパティ

		public bool Both
		{
			get { return Settings.Current.ShipCatalog_RemodelFilter_Both; }
			set
			{
				if (Settings.Current.ShipCatalog_RemodelFilter_Both != value)
				{
					Settings.Current.ShipCatalog_RemodelFilter_Both = value;
					this.RaisePropertyChanged();
					this.Update();
				}
			}
		}

		#endregion

		#region AlreadyRemodeling 変更通知プロパティ

		public bool AlreadyRemodeling
		{
			get { return Settings.Current.ShipCatalog_RemodelFilter_AlreadyRemodeling; }
			set
			{
				if (Settings.Current.ShipCatalog_RemodelFilter_AlreadyRemodeling != value)
				{
					Settings.Current.ShipCatalog_RemodelFilter_AlreadyRemodeling = value;
					this.RaisePropertyChanged();
					this.Update();
				}
			}
		}

		#endregion

		#region NotAlreadyRemodeling 変更通知プロパティ

		public bool NotAlreadyRemodeling
		{
			get { return Settings.Current.ShipCatalog_RemodelFilter_NotAlreadyRemodeling; }
			set
			{
				if (Settings.Current.ShipCatalog_RemodelFilter_NotAlreadyRemodeling != value)
				{
					Settings.Current.ShipCatalog_RemodelFilter_NotAlreadyRemodeling = value;
					this.RaisePropertyChanged();
					this.Update();
				}
			}
		}

		#endregion

		public ShipRemodelingFilter(Action updateAction)
			: base(updateAction)
		{
			//this._Both = true;
		}

		public override bool Predicate(Ship ship)
		{
			if (this.Both) return true;
			if (this.AlreadyRemodeling && !ship.Info.NextRemodelingLevel.HasValue) return true;
			if (this.NotAlreadyRemodeling && ship.Info.NextRemodelingLevel.HasValue) return true;

			return false;
		}
	}

	public class ShipExpeditionFilter : ShipCatalogFilter
	{
		private HashSet<int> shipIds = new HashSet<int>();

		#region WithoutExpedition 変更通知プロパティ

		public bool WithoutExpedition
		{
			get { return Settings.Current.ShipCatalog_ExpeditionFilter_WithoutExpedition; }
			set
			{
				if (Settings.Current.ShipCatalog_ExpeditionFilter_WithoutExpedition != value)
				{
					Settings.Current.ShipCatalog_ExpeditionFilter_WithoutExpedition = value;
					this.RaisePropertyChanged();
					this.Update();
				}
			}
		}

		#endregion

		public ShipExpeditionFilter(Action updateAction) : base(updateAction) { }

		public override bool Predicate(Ship ship)
		{
			return !this.WithoutExpedition || !this.shipIds.Contains(ship.Id);
		}

		public void SetFleets(MemberTable<Fleet> fleets)
		{
			if (fleets == null) return;

			this.shipIds = new HashSet<int>(fleets
				.Where(x => x.Value.Expedition.IsInExecution)
				.SelectMany(x => x.Value.Ships.Select(s => s.Id)));
		}
	}

	public class ShipSallyAreaFilter : ShipCatalogFilter
	{
		#region None 変更通知プロパティ

		private bool _None = true;

		public bool None
		{
			get { return this._None; }
			set
			{
				if (this._None != value)
				{
					this._None = value;
					this.RaisePropertyChanged();
					this.Update();
				}
			}
		}

		#endregion

		#region Aleutian 変更通知プロパティ

		private bool _Aleutian = true;

		public bool Aleutian
		{
			get { return this._Aleutian; }
			set
			{
				if (this._Aleutian != value)
				{
					this._Aleutian = value;
					this.RaisePropertyChanged();
					this.Update();
				}
			}
		}

		#endregion

		#region Midway 変更通知プロパティ

		private bool _Midway = true;

		public bool Midway
		{
			get { return this._Midway; }
			set
			{
				if (this._Midway != value)
				{
					this._Midway = value;
					this.RaisePropertyChanged();
					this.Update();
				}
			}
		}

		#endregion


		public ShipSallyAreaFilter(Action updateAction) : base(updateAction) { }

		public override bool Predicate(Ship ship)
		{
			//if (this.None && ship.SallyArea == 0) return true;
			//if (this.Aleutian && ship.SallyArea == 1) return true;
			//if (this.Midway && ship.SallyArea == 2) return true;

			//return false;

			return true;
		}
	}
}
