using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Grabacr07.KanColleWrapper;
using Grabacr07.KanColleViewer.Properties;
using Livet.EventListeners;

namespace Grabacr07.KanColleViewer.ViewModels.Contents
{
	public class ShipyardViewModel : TabItemViewModel
	{
		public override string Name
		{
			get { return Resources.Shipyard; }
			protected set { throw new NotImplementedException(); }
		}

		#region RepairingDocks 変更通知プロパティ

		private RepairingDockViewModel[] _RepairingDocks;

		public RepairingDockViewModel[] RepairingDocks
		{
			get { return this._RepairingDocks; }
			set
			{
				if (!Equals(this._RepairingDocks, value))
				{
					this._RepairingDocks = value;
					this.RaisePropertyChanged();
				}
			}
		}

		#endregion

		#region BuildingDocks 変更通知プロパティ

		private BuildingDockViewModel[] _BuildingDocks;

		public BuildingDockViewModel[] BuildingDocks
		{
			get { return this._BuildingDocks; }
			set
			{
				if (!Equals(this._BuildingDocks, value))
				{
					this._BuildingDocks = value;
					this.RaisePropertyChanged();
				}
			}
		}

		#endregion

        #region HalfBuildingDocks1 変更通知プロパティ
        private BuildingDockViewModel[] _HalfBuildingDocks1;
        public BuildingDockViewModel[] HalfBuildingDocks1
        {
            get { return this._HalfBuildingDocks1; }
            set
            {
                if (!Equals(this._HalfBuildingDocks1, value))
                {
                    this._HalfBuildingDocks1 = value;
                    this.RaisePropertyChanged();
                }
            }
        }
        #endregion
        #region HalfBuildingDocks2 変更通知プロパティ
        private BuildingDockViewModel[] _HalfBuildingDocks2;
        public BuildingDockViewModel[] HalfBuildingDocks2
        {
            get { return this._HalfBuildingDocks2; }
            set
            {
                if (!Equals(this._HalfBuildingDocks2, value))
                {
                    this._HalfBuildingDocks2 = value;
                    this.RaisePropertyChanged();
                }
            }
        }
        #endregion

		public CreatedSlotItemViewModel CreatedSlotItem { get; private set; }
		public DroppedShipViewModel DroppedShip { get; private set; }

		#region NewItem 変更通知プロパティ

		private NewItemViewModel _NewItem;

		public NewItemViewModel NewItem
		{
			get { return this._NewItem; }
			set
			{
				if (!Equals(this._NewItem, value))
				{
					this._NewItem = value;
					this.RaisePropertyChanged();
				}
			}
		}

		#endregion

		public ShipyardViewModel()
		{
			this.CreatedSlotItem = new CreatedSlotItemViewModel();
			this.DroppedShip = new DroppedShipViewModel();
			this.CompositeDisposable.Add(new CollectionChangedEventListener(KanColleClient.Current.Homeport.Organization.DroppedShips)
			{
				(sender, args) => this.UpdateDroppedShip()
			});

			this.CompositeDisposable.Add(new PropertyChangedEventListener(KanColleClient.Current.Homeport.Repairyard)
			{
				{ "Docks", (sender, args) => this.UpdateRepairingDocks() },
			});
			this.UpdateRepairingDocks();

			this.CompositeDisposable.Add(new PropertyChangedEventListener(KanColleClient.Current.Homeport.Dockyard)
			{
				{ "Docks", (sender, args) => this.UpdateBuildingDocks() },
				{ "CreatedSlotItem", (sender, args) => this.UpdateSlotItem() },
			});
			this.UpdateBuildingDocks();
			this.NewItem = this.CreatedSlotItem;
		}


		private void UpdateRepairingDocks()
		{
			this.RepairingDocks = KanColleClient.Current.Homeport.Repairyard.Docks.Select(kvp => new RepairingDockViewModel(kvp.Value)).ToArray();
		}

		private void UpdateBuildingDocks()
		{
			this.BuildingDocks = KanColleClient.Current.Homeport.Dockyard.Docks.Select(kvp => new BuildingDockViewModel(kvp.Value)).ToArray();
            this.HalfBuildingDocks1 = this.BuildingDocks.Where(x => x.Id % 2 != 0).ToArray();
			this.HalfBuildingDocks2 = this.BuildingDocks.Where(x => x.Id % 2 == 0).ToArray();
		}

		private void UpdateSlotItem()
		{
			this.CreatedSlotItem.Update(KanColleClient.Current.Homeport.Dockyard.CreatedSlotItem);
			this.NewItem = this.CreatedSlotItem;
		}

		private void UpdateDroppedShip()
		{
			var droppedShip = KanColleClient.Current.Homeport.Organization.DroppedShips.LastOrDefault();
			if (droppedShip != null)
			{
				this.DroppedShip.Update(droppedShip);
				this.NewItem = this.DroppedShip;
			}
			else
			{
				this.NewItem = new DroppedShipViewModel();
			}
		}
	}
}
