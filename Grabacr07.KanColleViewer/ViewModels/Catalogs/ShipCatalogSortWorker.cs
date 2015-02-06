using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Grabacr07.Desktop.Metro.Controls;
using Grabacr07.KanColleViewer.ViewModels.Contents;
using Grabacr07.KanColleWrapper.Models;

namespace Grabacr07.KanColleViewer.ViewModels.Catalogs
{
	public class ShipCatalogSortWorker
	{
		private readonly List<SortableColumnViewModel> sortableColumns;
		private readonly NoneColumnViewModel noneColumn = new NoneColumnViewModel();
		private SortableColumnViewModel currentSortTarget;

		public IdColumnViewModel IdColumn { get; }
		public TypeColumnViewModel TypeColumn { get; }
		public NameColumnViewModel NameColumn { get; }
		public LevelColumnViewModel LevelColumn { get; }
		public ConditionColumnViewModel ConditionColumn { get; }
		public FirepowerColumnViewModel FirepowerColumn { get; }
		public TorpedoColumnViewModel TorpedoColumn { get; }
		public AntiAirColumnViewModel AntiAirColumn { get; }
		public ArmorColumnViewModel ArmorColumn { get; }
		public LuckColumnViewModel LuckColumn { get; }
		public ViewRangeColumnViewModel ViewRangeColumn { get; }
		public EvasionColumnViewModel EvasionColumn { get; }
		public AntiSubColumnViewModel AntiSubColumn { get; }

		public ShipCatalogSortWorker()
		{
			this.IdColumn = new IdColumnViewModel();
			this.TypeColumn = new TypeColumnViewModel();
			this.NameColumn = new NameColumnViewModel();
			this.LevelColumn = new LevelColumnViewModel();
			this.ConditionColumn = new ConditionColumnViewModel();
			this.FirepowerColumn = new FirepowerColumnViewModel();
			this.TorpedoColumn = new TorpedoColumnViewModel();
			this.AntiAirColumn = new AntiAirColumnViewModel();
			this.ArmorColumn = new ArmorColumnViewModel();
			this.LuckColumn = new LuckColumnViewModel();
			this.ViewRangeColumn = new ViewRangeColumnViewModel();
			this.EvasionColumn = new EvasionColumnViewModel();
			this.AntiSubColumn = new AntiSubColumnViewModel();

			this.sortableColumns = new List<SortableColumnViewModel>
			{
				this.noneColumn,
				this.IdColumn,
				this.TypeColumn,
				this.NameColumn,
				this.LevelColumn,
				this.ConditionColumn,
				this.FirepowerColumn,
				this.TorpedoColumn,
				this.AntiAirColumn,
				this.ArmorColumn,
				this.LuckColumn,
				this.ViewRangeColumn,
				this.EvasionColumn,
				this.AntiSubColumn,
			};

			this.currentSortTarget = this.noneColumn;
		}

		public void SetTarget(ShipCatalogSortTarget sortTarget, bool reverse)
		{
			var target = this.sortableColumns.FirstOrDefault(x => x.Target == sortTarget);
			if (target == null) return;

			if (reverse)
			{
				switch (target.Direction)
				{
					case SortDirection.None:
						target.Direction = SortDirection.Descending;
						break;
					case SortDirection.Descending:
						target.Direction = SortDirection.Ascending;
						break;
					case SortDirection.Ascending:
						target = this.noneColumn;
						break;
				}
			}
			else
			{
				switch (target.Direction)
				{
					case SortDirection.None:
						target.Direction = SortDirection.Ascending;
						break;
					case SortDirection.Ascending:
						target.Direction = SortDirection.Descending;
						break;
					case SortDirection.Descending:
						target = this.noneColumn;
						break;
				}
			}

			this.currentSortTarget = target;
			this.sortableColumns.Where(x => x.Target != target.Target).ForEach(x => x.Direction = SortDirection.None);
		}

		public IEnumerable<Ship> Sort(IEnumerable<Ship> shipList)
		{
			return this.currentSortTarget.Sort(shipList);
		}
	}
}
