using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Grabacr07.KanColleWrapper.Models;
using Grabacr07.KanColleViewer.Properties;

namespace Grabacr07.KanColleViewer.ViewModels.Catalogs
{
	public class SlotItemCounter
	{
		// Key:   装備レベル (★)
		// Value: レベル別の装備数カウンター
		private readonly Dictionary<int, SlotItemCounterByLevel> itemsByLevel;

		public SlotItemInfo Target { get; private set; }

		public IReadOnlyCollection<SlotItemCounterByLevel> Levels
		{
			get { return this.itemsByLevel.OrderBy(x => x.Key).Select(x => x.Value).ToList(); }
		}

		public int Count
		{
			get { return this.itemsByLevel.Sum(x => x.Value.Count); }
		}


		public SlotItemCounter(SlotItemInfo target, IEnumerable<SlotItem> items)
		{
			this.Target = target;

			this.itemsByLevel = items
				.GroupBy(x => x.Level)
				.ToDictionary(x => x.Key, x => new SlotItemCounterByLevel { Level = x.Key, Count = x.Count(), });
		}

		public void AddShip(Ship ship, int itemLevel)
		{
			this.itemsByLevel[itemLevel].AddShip(ship);
		}

		public string Detail
		{
			get
			{
				string AddDetail = "";

				if (this.Target.Firepower > 0) AddDetail += " +" + this.Target.Firepower + " " + Resources.Stats_Firepower;
				if (this.Target.AA > 0) AddDetail += (AddDetail != "" ? "\n" : "") + " +" + this.Target.AA + " " + Resources.Stats_AntiAir;
				if (this.Target.Torpedo > 0) AddDetail += (AddDetail != "" ? "\n" : "") + " +" + this.Target.Torpedo + " " + Resources.Stats_Torpedo;
				if (this.Target.AntiSub > 0) AddDetail += (AddDetail != "" ? "\n" : "") + " +" + this.Target.AntiSub + " " + Resources.Stats_AntiSub;
				if (this.Target.SightRange > 0) AddDetail += (AddDetail != "" ? "\n" : "") + " +" + this.Target.SightRange + " " + Resources.Stats_SightRange;
				if (this.Target.Speed > 0) AddDetail += (AddDetail != "" ? "\n" : "") + " +" + this.Target.Speed + " " + Resources.Stats_Speed;
				if (this.Target.Armor > 0) AddDetail += (AddDetail != "" ? "\n" : "") + " +" + this.Target.Armor + " " + Resources.Stats_Armor;
				if (this.Target.Health > 0) AddDetail += (AddDetail != "" ? "\n" : "") + " +" + this.Target.Health + " " + Resources.Stats_Health;
				if (this.Target.Luck > 0) AddDetail += (AddDetail != "" ? "\n" : "") + " +" + this.Target.Luck + " " + Resources.Stats_Luck;
				if (this.Target.Evasion > 0) AddDetail += (AddDetail != "" ? "\n" : "") + " +" + this.Target.Evasion + " " + Resources.Stats_Evasion;
				if (this.Target.Accuracy > 0) AddDetail += (AddDetail != "" ? "\n" : "") + " +" + this.Target.Accuracy + " " + Resources.Stats_Accuracy;
				if (this.Target.DiveBomb > 0) AddDetail += (AddDetail != "" ? "\n" : "") + " +" + this.Target.DiveBomb + " " + Resources.Stats_DiveBomb;
				if (this.Target.AttackRange > 0) AddDetail += (AddDetail != "" ? "\n" : "") + " " + Resources.Stats_AttackRange + " (" + this.Target.AttackRange + ")";
				//if (this.Target.RawData.api_raik > 0) AddDetail += (AddDetail != "" ? "\n" : "") + " +" + this.Target.RawData.api_raik + " api_raik";
				//if (this.Target.RawData.api_raim > 0) AddDetail += (AddDetail != "" ? "\n" : "") + " +" + this.Target.RawData.api_raim + " api_raim";
				//if (this.Target.RawData.api_sakb > 0) AddDetail += (AddDetail != "" ? "\n" : "") + " +" + this.Target.RawData.api_sakb + " api_sakb";
				//if (this.Target.RawData.api_atap > 0) AddDetail += (AddDetail != "" ? "\n" : "") + " +" + this.Target.RawData.api_atap + " api_atap";
				//if (this.Target.RawData.api_rare > 0) AddDetail += (AddDetail != "" ? "\n" : "") + " +" + this.Target.RawData.api_rare + " api_rare";
				//if (this.Target.RawData.api_bakk > 0) AddDetail += (AddDetail != "" ? "\n" : "") + " +" + this.Target.RawData.api_bakk + " api_bakk";

				return AddDetail;
			}
		}
	}


	public class SlotItemCounterByLevel
	{
		// Key:   艦娘の ID
		// Value: 艦娘別の装備カウンター
		private readonly Dictionary<int, SlotItemCounterByShip> itemsByShip;
		private int count;

		public int Level { get; set; }

		public IReadOnlyCollection<SlotItemCounterByShip> Ships
		{
			get { return this.itemsByShip.Values.OrderByDescending(x => x.Ship.Level).ThenBy(x => x.Ship.SortNumber).ToList(); }
		}

		public int Count
		{
			get { return this.count; }
			set { this.count = this.Remainder = value; }
		}

		// 余り
		public int Remainder { get; private set; }


		public SlotItemCounterByLevel()
		{
			this.itemsByShip = new Dictionary<int, SlotItemCounterByShip>();
		}

		public void AddShip(Ship ship)
		{
			SlotItemCounterByShip target;
			if (this.itemsByShip.TryGetValue(ship.Id, out target))
			{
				target.Count++;
			}
			else
			{
				this.itemsByShip.Add(ship.Id, new SlotItemCounterByShip { Ship = ship, Count = 1 });
			}

			this.Remainder--;
		}
	}

	public class SlotItemCounterByShip
	{
		public Ship Ship { get; set; }

		public int Count { get; set; }

		public string ShipName
		{
			get { return this.Ship.Info.Name; }
		}

		public string ShipLevel
		{
			get { return "Lv." + this.Ship.Level; }
		}

		public string CountString
		{
			get { return this.Count == 1 ? "" : " x " + this.Count + " "; }
		}
	}
}
