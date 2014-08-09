using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Fiddler;
using Grabacr07.KanColleWrapper.Internal;
using Grabacr07.KanColleWrapper.Models.Raw;

namespace Grabacr07.KanColleWrapper.Models
{
	/// <summary>
	/// 母港に所属している艦娘を表します。
	/// </summary>
	public class Ship : RawDataWrapper<kcsapi_ship2>, IIdentifiable
	{
		private readonly Homeport homeport;

		private int[] BaseRepairTime = { 0,  10,  20,  30,  40,  50,  60,  70,  80,  90, 100, 110, 120, 125, 130, 145, 150, 155, 160, 165, 180,
											185, 190, 195, 200, 205, 210, 225, 230, 235, 240, 245, 250, 255, 260, 265, 280, 285, 290, 295, 300,
											305, 310, 315, 320, 325, 330, 345, 350, 355, 360, 365, 370, 375, 380, 385, 390, 395, 400, 405, 420,
											425, 430, 435, 440, 445, 450, 455, 460, 465, 470, 475, 480, 485, 490, 505, 510, 515, 520, 525, 530,
											535, 540, 545, 550, 555, 560, 565, 570, 575, 580, 585, 600, 605, 610, 615, 620, 652, 630, 635, 635,
											650, 655, 660, 665, 670, 675, 680, 685, 690, 695, 700, 705, 710, 715, 720, 725, 730, 735, 740, 745,
											750, 755, 760, 765, 770, 775, 780, 785, 790, 795, 800, 805, 810, 815, 820, 825, 830, 835, 840, 845,
											850, 855, 860, 865, 870, 875, 880, 885, 890, 895, 900, 905, 910, 915};

		/// <summary>
		/// この艦娘を識別する ID を取得します。
		/// </summary>
		public int Id
		{
			get { return this.RawData.api_id; }
		}

		/// <summary>
		/// 艦娘の種類に基づく情報を取得します。
		/// </summary>
		public ShipInfo Info { get; private set; }

		public int SortNumber
		{
			get { return this.RawData.api_sortno; }
		}

		/// <summary>
		/// 艦娘の現在のレベルを取得します。
		/// </summary>
		public int Level
		{
			get { return this.RawData.api_lv; }
		}

		/// <summary>
		/// 艦娘がロックされているかどうかを示す値を取得します。
		/// </summary>
		public bool IsLocked
		{
			get { return this.RawData.api_locked == 1; }
		}

		/// <summary>
		/// 艦娘の現在の累積経験値を取得します。
		/// </summary>
		public int Exp
		{
			get { return this.RawData.api_exp.Get(0) ?? 0; }
		}

		/// <summary>
		/// この艦娘が次のレベルに上がるために必要な経験値を取得します。
		/// </summary>
		public int ExpForNextLevel
		{
			get { return this.RawData.api_exp.Get(1) ?? 0; }
		}

		#region HP 変更通知プロパティ

		private LimitedValue _HP;

		/// <summary>
		/// 耐久値を取得します。
		/// </summary>
		public LimitedValue HP
		{
			get { return this._HP; }
			private set
			{
				this._HP = value;
				this.RaisePropertyChanged();
			}
		}

		#endregion

		#region Fuel 変更通知プロパティ

		private LimitedValue _Fuel;

		/// <summary>
		/// 燃料を取得します。
		/// </summary>
		public LimitedValue Fuel
		{
			get { return this._Fuel; }
			private set
			{
				this._Fuel = value;
				this.RaisePropertyChanged();
			}
		}

		#endregion

		#region Bull 変更通知プロパティ

		private LimitedValue _Bull;

		public LimitedValue Bull
		{
			get { return this._Bull; }
			private set
			{
				this._Bull = value;
				this.RaisePropertyChanged();
			}
		}

		#endregion


		#region Firepower 変更通知プロパティ

		private ModernizableStatus _Firepower;

		/// <summary>
		/// 火力ステータス値を取得します。
		/// </summary>
		public ModernizableStatus Firepower
		{
			get { return this._Firepower; }
			private set
			{
				this._Firepower = value;
				this.RaisePropertyChanged();

			}
		}

		#endregion

		#region Torpedo 変更通知プロパティ

		private ModernizableStatus _Torpedo;

		/// <summary>
		/// 雷装ステータス値を取得します。
		/// </summary>
		public ModernizableStatus Torpedo
		{
			get { return this._Torpedo; }
			private set
			{
				this._Torpedo = value;
				this.RaisePropertyChanged();

			}
		}

		#endregion

		#region AA 変更通知プロパティ

		private ModernizableStatus _AA;

		/// <summary>
		/// 対空ステータス値を取得します。
		/// </summary>
		public ModernizableStatus AA
		{
			get { return this._AA; }
			private set
			{
				this._AA = value;
				this.RaisePropertyChanged();
			}

		}

		#endregion

		#region Armer 変更通知プロパティ

		private ModernizableStatus _Armer;

		/// <summary>
		/// 装甲ステータス値を取得します。
		/// </summary>
		public ModernizableStatus Armer
		{
			get { return this._Armer; }
			private set
			{
				this._Armer = value;
				this.RaisePropertyChanged();

			}
		}

		#endregion

		#region Luck 変更通知プロパティ

		private ModernizableStatus _Luck;

		/// <summary>
		/// 運のステータス値を取得します。
		/// </summary>
		public ModernizableStatus Luck
		{
			get { return this._Luck; }
			private set
			{
				this._Luck = value;
				this.RaisePropertyChanged();
			}
		}

		#endregion


		/// <summary>
		/// 装備によるボーナスを含めた索敵ステータス値を取得します。
		/// </summary>
		public int ViewRange
		{
			get { return this.RawData.api_sakuteki.Get(0) ?? 0; }
		}

		/// <summary>
		/// Anti-Submarine stat with and without equipment.
		/// </summary>
		public LimitedValue AntiSub { get; private set; }

		/// <summary>
		/// Line of Sight stat with and without equipment.
		/// </summary>
		public LimitedValue LineOfSight { get; private set; }

		/// <summary>
		/// Evasion stat with and without equipment.
		/// </summary>
		public LimitedValue Evasion { get; private set; }

		/// <summary>
		/// 火力・雷装・対空・装甲のすべてのステータス値が最大値に達しているかどうかを示す値を取得します。
		/// </summary>
		public bool IsMaxModernized
		{
			get { return this.Firepower.IsMax && this.Torpedo.IsMax && this.AA.IsMax && this.Armer.IsMax; }
		}

		/// <summary>
		/// 現在のコンディション値を取得します。
		/// </summary>
		public int Condition
		{
			get { return this.RawData.api_cond; }
		}

		/// <summary>
		/// コンディションの種類を示す <see cref="ConditionType" /> 値を取得します。
		/// </summary>
		public ConditionType ConditionType
		{
			get { return ConditionTypeHelper.ToConditionType(this.RawData.api_cond); }
		}

		/// <summary>
		/// For visually generated elements. "[Lv.00]   Name"
		/// </summary>
		public string LvName
		{
			get { return "[Lv." + this.Level + "]  \t" + this.Info.Name; }
		}

		/// <summary>
		/// For visually generated elements. 
		/// "Name           [Lv.00]"
		/// "Long Name      [Lv.00]"    
		/// </summary>
		public string NameLv
		{
			get { return string.Format("{0, -20} [Lv.{1}]", this.Info.Name, this.Level); }
		}

		/// <summary>
		/// Repair time taking in consideration HP, LV, and Ship Type.
		/// </summary>
		public string RepairDockTime
		{
			get
			{
				return TimeSpan.FromSeconds(Math.Floor((this.HP.Maximum - this.HP.Current) * BaseRepairTime[Math.Min(this.Level, 150)] * this.Info.ShipType.RepairMultiplier) + 30).ToString();
			}
		}

		public string RepairFacilityTime
		{
			get
			{
				// Time it takes to heal 1HP
				double MinDockTime = Math.Floor(BaseRepairTime[Math.Min(this.Level, 150)] * this.Info.ShipType.RepairMultiplier) + 30;

				if (MinDockTime < 1200)
					return RepairDockTime;
					
				return TimeSpan.FromMinutes((this.HP.Maximum - this.HP.Current) * 20).ToString();
			}
		}

		public bool IsDamaged
		{
			get { return (this.HP.Maximum - this.HP.Current) > 0; }
		}

		public bool IsLightlyDamaged
		{
			get { return this.IsDamaged && (this.HP.Current / (double)this.HP.Maximum) > 0.5; }
		}

		public bool IsBadlyDamaged
		{
			get { return (this.HP.Current / (double)this.HP.Maximum) <= 0.25; }
		}

		public SlotItem[] SlotItems { get; private set; }

		#region OnSlot 変更通知プロパティ

		private int[] _OnSlot;

		/// <summary>
		/// 各装備スロットの艦載機数を取得します。
		/// </summary>
		public int[] OnSlot
		{
			get { return this._OnSlot; }
			private set
			{
				if (this._OnSlot != value)
				{
					this._OnSlot = value;
					this.RaisePropertyChanged();
				}
			}
		}

		#endregion

		#region IsInRepairing 変更通知プロパティ

		private bool _IsInRepairing;

		/// <summary>
		/// この艦が入渠中かどうかを示す値を取得します。
		/// </summary>
		public bool IsInRepairing
		{
			get { return this._IsInRepairing; }
			internal set
			{
				if (this._IsInRepairing != value)
				{
					this._IsInRepairing = value;
					this.RaisePropertyChanged();
				}
			}
		}

		#endregion

		/// <summary>
		/// この艦が出撃した海域を識別する整数値を取得します。
		/// </summary>
		public int SallyArea
		{
			get { return this.RawData.api_sally_area; }
		}


		internal Ship(Homeport parent, kcsapi_ship2 rawData)
			: base(rawData)
		{
			this.homeport = parent;
			this.Update(rawData);
		}

		internal void Update(kcsapi_ship2 rawData)
		{
			this.UpdateRawData(rawData);

			this.Info = KanColleClient.Current.Master.Ships[rawData.api_ship_id] ?? ShipInfo.Dummy;
			this.HP = new LimitedValue(this.RawData.api_nowhp, this.RawData.api_maxhp, 0);
			this.Fuel = new LimitedValue(this.RawData.api_fuel, this.Info.RawData.api_fuel_max, 0);
			this.Bull = new LimitedValue(this.RawData.api_bull, this.Info.RawData.api_bull_max, 0);

			if (this.RawData.api_kyouka.Length >= 5)
			{
				this.Firepower = new ModernizableStatus(this.Info.RawData.api_houg, this.RawData.api_kyouka[0]);
				this.Torpedo = new ModernizableStatus(this.Info.RawData.api_raig, this.RawData.api_kyouka[1]);
				this.AA = new ModernizableStatus(this.Info.RawData.api_tyku, this.RawData.api_kyouka[2]);
				this.Armer = new ModernizableStatus(this.Info.RawData.api_souk, this.RawData.api_kyouka[3]);
				this.Luck = new ModernizableStatus(this.Info.RawData.api_luck, this.RawData.api_kyouka[4]);
			}

			this.SlotItems = this.RawData.api_slot.Select(id => this.homeport.Itemyard.SlotItems[id]).Where(x => x != null).ToArray();
			this.OnSlot = this.RawData.api_onslot;

			// Minimum removes equipped values.
			int EqAntiSub = 0, EqEvasion = 0, EqLineOfSight = 0;

			foreach (SlotItem item in this.SlotItems)
			{
				if (item == null)
					continue;

				EqAntiSub += item.Info.RawData.api_tais;
				EqEvasion += item.Info.RawData.api_houk;
				EqLineOfSight += item.Info.RawData.api_saku;
			}

			this.AntiSub = new LimitedValue(this.RawData.api_taisen[0], this.RawData.api_taisen[1], this.RawData.api_taisen[0] - EqAntiSub);
			this.Evasion = new LimitedValue(this.RawData.api_kaihi[0], this.RawData.api_kaihi[1], this.RawData.api_kaihi[0] - EqEvasion);
			this.LineOfSight = new LimitedValue(this.RawData.api_sakuteki[0], this.RawData.api_sakuteki[1], this.RawData.api_sakuteki[0] - EqLineOfSight);
		}


		internal void Charge(int fuel, int bull, int[] onslot)
		{
			this.Fuel = this.Fuel.Update(fuel);
			this.Bull = this.Bull.Update(bull);
			this.OnSlot = onslot;
		}

		internal void Repair()
		{
			var max = this.HP.Maximum;
			this.HP = this.HP.Update(max);
		}

		public override string ToString()
		{
			return string.Format("ID = {0}, Name = \"{1}\", ShipType = \"{2}\", Level = {3}", this.Id, this.Info.Name, this.Info.ShipType.Name, this.Level);
		}
	}
}
