using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Grabacr07.KanColleWrapper;
using Grabacr07.KanColleWrapper.Models;
using Grabacr07.KanColleViewer.Properties;
using Livet;

namespace Grabacr07.KanColleViewer.ViewModels.Contents
{
	public class ShipViewModel : ViewModel
	{
		public Ship Ship { get; private set; }
		public List<SlotItemViewModel> SlotItems {get; private set;}

		public string RepairToolTip
		{
			get
			{
				if (!this.Ship.IsDamaged)
					return "OK";

				// Only need to show Facility time when they are not the same time and if the ship is lightly damaged
				return string.Format(Resources.Ship_RepairDockToolTip, this.Ship.RepairDockTime) + (this.Ship.IsLightlyDamaged && this.Ship.RepairFacilityTime != this.Ship.RepairDockTime ? "\n" + string.Format(Resources.Ship_RepairFacilityToolTip, this.Ship.RepairFacilityTime) : "");
			}
		}

		public string StatsToolTip
		{
			get
			{
				string AddDetail = "";
				if (this.Ship.Info.UntranslatedName != "")
					AddDetail += this.Ship.Info.UntranslatedName + "\n";
				AddDetail += string.Format("{0}: {1} ({2})\n", Resources.Stats_Firepower, this.Ship.Firepower.Current, (this.Ship.Firepower.IsMax ? @"MAX" : "+" + (this.Ship.Firepower.Max - this.Ship.Firepower.Current).ToString()));
				AddDetail += string.Format("{0}: {1} ({2})\n", Resources.Stats_Torpedo, this.Ship.Torpedo.Current, (this.Ship.Torpedo.IsMax ? @"MAX" : "+" + (this.Ship.Torpedo.Max - this.Ship.Torpedo.Current).ToString()));
				AddDetail += string.Format("{0}: {1} ({2})\n", Resources.Stats_AntiAir, this.Ship.AA.Current, (this.Ship.AA.IsMax ? @"MAX" : "+" + (this.Ship.AA.Max - this.Ship.AA.Current).ToString()));
				AddDetail += string.Format("{0}: {1} ({2})\n", Resources.Stats_Armor, this.Ship.Armer.Current, (this.Ship.Armer.IsMax ? @"MAX" : "+" + (this.Ship.Armer.Max - this.Ship.Armer.Current).ToString()));
				AddDetail += string.Format("{0}: {1} ({2})", Resources.Stats_Luck, this.Ship.Luck.Current, (this.Ship.Luck.IsMax ? @"MAX" : "+" + (this.Ship.Luck.Max - this.Ship.Luck.Current).ToString()));

				return AddDetail;
			}
		}

		public ShipViewModel(Ship ship)
		{
			this.Ship = ship;
			// this.SlotItems = ship.SlotItems.Select(i => new SlotItemViewModel(i.Info)).ToList();
		}
	}
}
