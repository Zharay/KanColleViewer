using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Grabacr07.Desktop.Metro.Controls;
using Grabacr07.KanColleWrapper.Models;
using Livet;

namespace Grabacr07.KanColleViewer.ViewModels.Catalogs
{
	public abstract class SortableColumnViewModel : ViewModel
	{
		public ShipCatalogSortTarget Target { get; private set; }

		#region Direction 変更通知プロパティ

		private SortDirection _Direction = SortDirection.None;

		public SortDirection Direction
		{
			get { return this._Direction; }
			set
			{
				if (this._Direction != value)
				{
					this._Direction = value;
					this.RaisePropertyChanged();
				}
			}
		}

		#endregion

		protected SortableColumnViewModel(ShipCatalogSortTarget target)
		{
			this.Target = target;
		}

		public abstract IEnumerable<Ship> Sort(IEnumerable<Ship> list);
	}

	public class NoneColumnViewModel : SortableColumnViewModel
	{
		public NoneColumnViewModel() : base(ShipCatalogSortTarget.None) { }

		public override IEnumerable<Ship> Sort(IEnumerable<Ship> list)
		{
			return list;
		}
	}

	public class IdColumnViewModel : SortableColumnViewModel
	{
		public IdColumnViewModel() : base(ShipCatalogSortTarget.Id) { }

		public override IEnumerable<Ship> Sort(IEnumerable<Ship> list)
		{
			if (this.Direction == SortDirection.Ascending)
			{
				return list.OrderBy(x => x.Id);
			}
			if (this.Direction == SortDirection.Descending)
			{
				return list.OrderByDescending(x => x.Id);
			}
			return list;
		}
	}

	public class TypeColumnViewModel : SortableColumnViewModel
	{
		public TypeColumnViewModel() : base(ShipCatalogSortTarget.Type) { }

		public override IEnumerable<Ship> Sort(IEnumerable<Ship> list)
		{
			if (this.Direction == SortDirection.Ascending)
			{
				return list.OrderBy(x => x.Info.ShipType.Id)
					.ThenBy(x => x.Info.Name)
					.ThenBy(x => x.Id);
			}
			if (this.Direction == SortDirection.Descending)
			{
				return list.OrderByDescending(x => x.Info.ShipType.Id)
					.ThenByDescending(x => x.Info.Name)
					.ThenByDescending(x => x.Id);
			}
			return list;
		}
	}

	public class NameColumnViewModel : SortableColumnViewModel
	{
		public NameColumnViewModel() : base(ShipCatalogSortTarget.Name) { }

		public override IEnumerable<Ship> Sort(IEnumerable<Ship> list)
		{
			if (this.Direction == SortDirection.Ascending)
			{
				return list.OrderBy(x => x.Info.Name)
					.ThenBy(x => x.Id);
			}
			if (this.Direction == SortDirection.Descending)
			{
				return list.OrderByDescending(x => x.Info.Name)
					.ThenByDescending(x => x.Id);
			}
			return list;
		}
	}

	public class LevelColumnViewModel : SortableColumnViewModel
	{
		public LevelColumnViewModel() : base(ShipCatalogSortTarget.Level) { }

		public override IEnumerable<Ship> Sort(IEnumerable<Ship> list)
		{
			if (this.Direction == SortDirection.Ascending)
			{
				return list.OrderBy(x => x.Level)
					.ThenByDescending(x => x.ExpForNextLevel)
					.ThenBy(x => x.Info.Name)
					.ThenBy(x => x.Id);
			}
			if (this.Direction == SortDirection.Descending)
			{
				return list.OrderByDescending(x => x.Level)
					.ThenBy(x => x.ExpForNextLevel)
					.ThenByDescending(x => x.Info.Name)
					.ThenByDescending(x => x.Id);
			}
			return list;
		}
	}

	public class ConditionColumnViewModel : SortableColumnViewModel
	{
		public ConditionColumnViewModel() : base(ShipCatalogSortTarget.Cond) { }

		public override IEnumerable<Ship> Sort(IEnumerable<Ship> list)
		{
			if (this.Direction == SortDirection.Ascending)
			{
				return list.OrderBy(x => x.Condition)
					.ThenBy(x => x.Info.ShipType.Id)
					.ThenBy(x => x.Level)
					.ThenByDescending(x => x.ExpForNextLevel)
					.ThenBy(x => x.Info.Name);
			}
			if (this.Direction == SortDirection.Descending)
			{
				return list.OrderByDescending(x => x.Condition)
					.ThenBy(x => x.Info.ShipType.Id)
					.ThenBy(x => x.Level)
					.ThenByDescending(x => x.ExpForNextLevel)
					.ThenBy(x => x.Info.Name);
			}
			return list;
		}
	}

	public class FirepowerColumnViewModel : SortableColumnViewModel
	{
		public FirepowerColumnViewModel() : base(ShipCatalogSortTarget.Firepower) { }

		public override IEnumerable<Ship> Sort(IEnumerable<Ship> list)
		{
			if (this.Direction == SortDirection.Ascending)
			{
				return list.OrderBy(x => x.Firepower.Current)
					.ThenBy(x => x.Info.ShipType.Id)
					.ThenBy(x => x.Level)
					.ThenBy(x => x.Info.Name);
			}
			if (this.Direction == SortDirection.Descending)
			{
				return list.OrderByDescending(x => x.Firepower.Current)
					.ThenBy(x => x.Info.ShipType.Id)
					.ThenBy(x => x.Level)
					.ThenBy(x => x.Info.Name);
			}
			return list;
		}
	}

	public class TorpedoColumnViewModel : SortableColumnViewModel
	{
		public TorpedoColumnViewModel() : base(ShipCatalogSortTarget.Torpedo) { }

		public override IEnumerable<Ship> Sort(IEnumerable<Ship> list)
		{
			if (this.Direction == SortDirection.Ascending)
			{
				return list.OrderBy(x => x.Torpedo.Current)
					.ThenBy(x => x.Info.ShipType.Id)
					.ThenBy(x => x.Level)
					.ThenBy(x => x.Info.Name);
			}
			if (this.Direction == SortDirection.Descending)
			{
				return list.OrderByDescending(x => x.Torpedo.Current)
					.ThenBy(x => x.Info.ShipType.Id)
					.ThenBy(x => x.Level)
					.ThenBy(x => x.Info.Name);
			}
			return list;
		}
	}

	public class AntiAirColumnViewModel : SortableColumnViewModel
	{
		public AntiAirColumnViewModel() : base(ShipCatalogSortTarget.AntiAir) { }

		public override IEnumerable<Ship> Sort(IEnumerable<Ship> list)
		{
			if (this.Direction == SortDirection.Ascending)
			{
				return list.OrderBy(x => x.AA.Current)
					.ThenBy(x => x.Info.ShipType.Id)
					.ThenBy(x => x.Level)
					.ThenBy(x => x.Info.Name);
			}
			if (this.Direction == SortDirection.Descending)
			{
				return list.OrderByDescending(x => x.AA.Current)
					.ThenBy(x => x.Info.ShipType.Id)
					.ThenBy(x => x.Level)
					.ThenBy(x => x.Info.Name);
			}
			return list;
		}
	}

	public class ArmorColumnViewModel : SortableColumnViewModel
	{
		public ArmorColumnViewModel() : base(ShipCatalogSortTarget.Armor) { }

		public override IEnumerable<Ship> Sort(IEnumerable<Ship> list)
		{
			if (this.Direction == SortDirection.Ascending)
			{
				return list.OrderBy(x => x.Armer.Current)
					.ThenBy(x => x.Info.ShipType.Id)
					.ThenBy(x => x.Level)
					.ThenBy(x => x.Info.Name);
			}
			if (this.Direction == SortDirection.Descending)
			{
				return list.OrderByDescending(x => x.Armer.Current)
					.ThenBy(x => x.Info.ShipType.Id)
					.ThenBy(x => x.Level)
					.ThenBy(x => x.Info.Name);
			}
			return list;
		}
	}

	public class LuckColumnViewModel : SortableColumnViewModel
	{
		public LuckColumnViewModel() : base(ShipCatalogSortTarget.Luck) { }

		public override IEnumerable<Ship> Sort(IEnumerable<Ship> list)
		{
			if (this.Direction == SortDirection.Ascending)
			{
				return list.OrderBy(x => x.Luck.Current)
					.ThenBy(x => x.Info.ShipType.Id)
					.ThenBy(x => x.Level)
					.ThenBy(x => x.Info.Name);
			}
			if (this.Direction == SortDirection.Descending)
			{
				return list.OrderByDescending(x => x.Luck.Current)
					.ThenBy(x => x.Info.ShipType.Id)
					.ThenBy(x => x.Level)
					.ThenBy(x => x.Info.Name);
			}
			return list;
		}
	}

	public class ViewRangeColumnViewModel : SortableColumnViewModel
	{
		public ViewRangeColumnViewModel() : base(ShipCatalogSortTarget.ViewRange) { }

		public override IEnumerable<Ship> Sort(IEnumerable<Ship> list)
		{
			if (this.Direction == SortDirection.Ascending)
			{
				return list.OrderBy(x => x.ViewRange)
					.ThenBy(x => x.Info.ShipType.Id)
					.ThenBy(x => x.Level)
					.ThenByDescending(x => x.ExpForNextLevel)
					.ThenBy(x => x.Info.SortId);
			}
			if (this.Direction == SortDirection.Descending)
			{
				return list.OrderByDescending(x => x.ViewRange)
					.ThenBy(x => x.Info.ShipType.Id)
					.ThenBy(x => x.Level)
					.ThenBy(x => x.Info.SortId);
			}
			return list;
		}
	}

	public class EvasionColumnViewModel : SortableColumnViewModel
	{
		public EvasionColumnViewModel() : base(ShipCatalogSortTarget.Evasion) { }

		public override IEnumerable<Ship> Sort(IEnumerable<Ship> list)
		{
			if (this.Direction == SortDirection.Ascending)
			{
				return list.OrderBy(x => x.Evasion.Current)
					.ThenBy(x => x.Info.ShipType.Id)
					.ThenBy(x => x.Level)
					.ThenBy(x => x.Info.SortId);
			}
			if (this.Direction == SortDirection.Descending)
			{
				return list.OrderByDescending(x => x.Evasion.Current)
					.ThenBy(x => x.Info.ShipType.Id)
					.ThenBy(x => x.Level)
					.ThenBy(x => x.Info.SortId);
			}
			return list;
		}
	}

	public class AntiSubColumnViewModel : SortableColumnViewModel
	{
		public AntiSubColumnViewModel() : base(ShipCatalogSortTarget.AntiSub) { }

		public override IEnumerable<Ship> Sort(IEnumerable<Ship> list)
		{
			if (this.Direction == SortDirection.Ascending)
			{
				return list.OrderBy(x => x.AntiSub.Current)
					.ThenBy(x => x.Info.ShipType.Id)
					.ThenBy(x => x.Level)
					.ThenBy(x => x.Info.SortId);
			}
			if (this.Direction == SortDirection.Descending)
			{
				return list.OrderByDescending(x => x.AntiSub.Current)
					.ThenBy(x => x.Info.ShipType.Id)
					.ThenBy(x => x.Level)
					.ThenByDescending(x => x.ExpForNextLevel)
					.ThenBy(x => x.Info.SortId);
			}
			return list;
		}
	}
}
