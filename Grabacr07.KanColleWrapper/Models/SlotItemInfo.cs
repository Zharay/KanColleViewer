using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Grabacr07.KanColleWrapper.Globalization;
using Grabacr07.KanColleWrapper.Internal;
using Grabacr07.KanColleWrapper.Models.Raw;

namespace Grabacr07.KanColleWrapper.Models
{
	/// <summary>
	/// 装備アイテムの種類に基づく情報を表します。
	/// </summary>
	public class SlotItemInfo : RawDataWrapper<kcsapi_mst_slotitem>, IIdentifiable
	{
		private SlotItemType? type;
		private SlotItemIconType? iconType;
		private int? categoryId;

		public int Id
		{
			get { return this.RawData.api_id; }
		}

		public string Name
		{
            get { return KanColleClient.Current.Translations.GetTranslation(this.RawData.api_name, TranslationType.Equipment, this.RawData); }
        }

		public string UntranslatedName
		{
			get { return this.RawData.api_name; }
		}

		public SlotItemType Type
		{
			get { return this.type ?? (SlotItemType)(this.type = (SlotItemType)(this.RawData.api_type.Get(2) ?? 0)); }
		}

		public SlotItemIconType IconType
		{
			get { return this.iconType ?? (SlotItemIconType)(this.iconType = (SlotItemIconType)(this.RawData.api_type.Get(3) ?? 0)); }
		}

		public int CategoryId
		{
			get { return this.categoryId ?? (int)(this.categoryId = this.RawData.api_type.Get(2) ?? int.MaxValue); }
		}

		/// <summary>
		/// 対空値を取得します。
		/// </summary>
		public int AA
		{
			get { return this.RawData.api_tyku; }
		}

		/// <summary>
		/// 制空戦に参加できる戦闘機または水上機かどうかを示す値を取得します。
		/// </summary>
		public bool IsAirSuperiorityFighter
		{
			get
			{
				return this.Type == SlotItemType.艦上戦闘機
					|| this.Type == SlotItemType.艦上攻撃機
					|| this.Type == SlotItemType.艦上爆撃機
					|| this.Type == SlotItemType.水上爆撃機;
			}
		}

		public int Firepower
		{
			get { return this.RawData.api_houg; }
		}

		public int Torpedo
		{
			get { return this.RawData.api_raig; }
		}

		public int AntiSub
		{
			get { return this.RawData.api_tais; }
		}

		public int SightRange
		{
			get { return this.RawData.api_saku; }
		}

		public int Speed
		{
			get { return this.RawData.api_soku; }
		}

		public int Armor
		{
			get { return this.RawData.api_souk; }
		}

		public int Health
		{
			get { return this.RawData.api_taik; }
		}

		public int Luck
		{
			get { return this.RawData.api_luck; }
		}

		public int Evasion
		{
			get { return this.RawData.api_houk; }
		}

		public int Accuracy
		{
			get { return this.RawData.api_houm; }
		}

		public int DiveBomb
		{
			get { return this.RawData.api_baku; }
		}

		public int AttackRange
		{
			get { return this.RawData.api_leng; }
		}

		public string AllStats
		{
			get
			{
				List<string> details = new List<string>();

				if (this.Firepower != 0) details.Add(this.StatFormat(this.Firepower, Resources.Stats_Firepower));
				if (this.AA != 0) details.Add(this.StatFormat(this.AA, Resources.Stats_AntiAir));
				if (this.Torpedo != 0) details.Add(this.StatFormat(this.Torpedo, Resources.Stats_Torpedo));
				if (this.AntiSub != 0) details.Add(this.StatFormat(this.AntiSub, Resources.Stats_AntiSub));
				if (this.SightRange != 0) details.Add(this.StatFormat(this.SightRange, Resources.Stats_SightRange));
				if (this.Speed != 0) details.Add(this.StatFormat(this.Speed, Resources.Stats_Speed));
				if (this.Armor != 0) details.Add(this.StatFormat(this.Armor, Resources.Stats_Armor));
				if (this.Health != 0) details.Add(this.StatFormat(this.Health, Resources.Stats_Health));
				if (this.Luck != 0) details.Add(this.StatFormat(this.Luck, Resources.Stats_Luck));
				if (this.Evasion != 0) details.Add(this.StatFormat(this.Evasion, Resources.Stats_Evasion));
				if (this.Accuracy != 0) details.Add(this.StatFormat(this.Accuracy, Resources.Stats_Accuracy));
				if (this.DiveBomb != 0) details.Add(this.StatFormat(this.DiveBomb, Resources.Stats_DiveBomb));
				if (this.AttackRange > 0) details.Add(string.Format(" {1}({0})", this.AttackRange, Resources.Stats_AttackRange));

				return String.Join("\n", details);
			}
		}

		public bool IsNumerable
		{
			get
			{
				return this.Type == SlotItemType.艦上偵察機
					|| this.Type == SlotItemType.艦上戦闘機
					|| this.Type == SlotItemType.艦上攻撃機
					|| this.Type == SlotItemType.艦上爆撃機
					|| this.Type == SlotItemType.水上偵察機
					|| this.Type == SlotItemType.水上爆撃機
					|| this.Type == SlotItemType.オートジャイロ
					|| this.Type == SlotItemType.対潜哨戒機
					|| this.Type == SlotItemType.大型飛行艇;
			}
		}

		internal SlotItemInfo(kcsapi_mst_slotitem rawData) : base(rawData) { }

		public override string ToString()
		{
			return string.Format("ID = {0}, Name = \"{1}\", Type = {{{2}}}", this.Id, this.Name, this.RawData.api_type.ToString(", "));
		}

		private string StatFormat(int stat, string name)
		{
			return String.Format(" {0:+#;-#} {1}", stat, name);
		}

		#region static members

		private static SlotItemInfo dummy = new SlotItemInfo(new kcsapi_mst_slotitem()
		{
			api_id = 0,
			api_name = "？？？",
		});

		public static SlotItemInfo Dummy
		{
			get { return dummy; }
		}

		#endregion
	}
}
