using Engine;
using Game;
using System.Collections.Generic;
using System.Linq;

namespace ZHCN
{
	public class PumpkinBlock : Game.PumpkinBlock
	{
		public new const int Index = 131;

		public override string GetDisplayName(SubsystemTerrain subsystemTerrain, int value)
		{
			int size = GetSize(Terrain.ExtractData(value));
			if (size >= 7)
			{
				return "南瓜";
			}
			return "青南瓜";
		}
	}

	public class RottenPumpkinBlock : BasePumpkinBlock
	{
		public const int Index = 244;

		public RottenPumpkinBlock() : base(true)
		{
		}

		public override string GetDisplayName(SubsystemTerrain subsystemTerrain, int value)
		{
			int size = GetSize(Terrain.ExtractData(value));
			if (size >= 7)
			{
				return "烂南瓜";
			}
			return "烂青南瓜";
		}
	}

	public class ClothingBlock : Game.ClothingBlock
	{
		public new const int Index = 203;

		public override string GetDisplayName(SubsystemTerrain subsystemTerrain, int value)
		{
			int data = Terrain.ExtractData(value);
			ClothingData clothingData = GetClothingData(data);
			int clothingColor = GetClothingColor(data);
			if (clothingColor != 0)
			{
				return SubsystemPalette.GetName(subsystemTerrain, clothingColor, "染色 " + clothingData.DisplayName);
			}
			return clothingData.DisplayName;
		}

		public override CraftingRecipe GetAdHocCraftingRecipe(SubsystemTerrain terrain, string[] ingredients, float heatLevel)
		{
			if (heatLevel < 1f)
			{
				return null;
			}
			List<string> list = (from i in ingredients
								 where !string.IsNullOrEmpty(i)
								 select i).ToList();
			if (list.Count == 2)
			{
				int num = 0;
				int num2 = 0;
				int num3 = 0;
				foreach (string item in list)
				{
					CraftingRecipesManager.DecodeIngredient(item, out string craftingId, out int? data);
					if (craftingId == BlocksManager.Blocks[203].CraftingId)
					{
						num3 = Terrain.MakeBlockValue(203, 0, data ?? 0);
					}
					else if (craftingId == BlocksManager.Blocks[129].CraftingId)
					{
						num = Terrain.MakeBlockValue(129, 0, data ?? 0);
					}
					else if (craftingId == BlocksManager.Blocks[128].CraftingId)
					{
						num2 = Terrain.MakeBlockValue(128, 0, data ?? 0);
					}
				}
				if (num != 0 && num3 != 0)
				{
					int data2 = Terrain.ExtractData(num3);
					int clothingColor = GetClothingColor(data2);
					int clothingIndex = GetClothingIndex(data2);
					bool canBeDyed = GetClothingData(data2).CanBeDyed;
					int damage = BlocksManager.Blocks[203].GetDamage(num3);
					int color = PaintBucketBlock.GetColor(Terrain.ExtractData(num));
					int damage2 = BlocksManager.Blocks[129].GetDamage(num);
					Block block = BlocksManager.Blocks[129];
					Block block2 = BlocksManager.Blocks[203];
					if (!canBeDyed)
					{
						return null;
					}
					int num4 = PaintBucketBlock.CombineColors(clothingColor, color);
					if (num4 != clothingColor)
					{
						return new CraftingRecipe
						{
							ResultCount = 1,
							ResultValue = block2.SetDamage(Terrain.MakeBlockValue(203, 0, SetClothingIndex(SetClothingColor(0, num4), clothingIndex)), damage),
							RemainsCount = 1,
							RemainsValue = BlocksManager.DamageItem(Terrain.MakeBlockValue(129, 0, color), damage2 + MathUtils.Max(block.Durability / 4, 1)),
							RequiredHeatLevel = 1f,
							Description = $"染色服装 {SubsystemPalette.GetName(terrain, color, null)}",
							Ingredients = (string[])ingredients.Clone()
						};
					}
				}
				if (num2 != 0 && num3 != 0)
				{
					int data3 = Terrain.ExtractData(num3);
					int clothingColor2 = GetClothingColor(data3);
					int clothingIndex2 = GetClothingIndex(data3);
					bool canBeDyed2 = GetClothingData(data3).CanBeDyed;
					int damage3 = BlocksManager.Blocks[203].GetDamage(num3);
					int damage4 = BlocksManager.Blocks[128].GetDamage(num2);
					Block block3 = BlocksManager.Blocks[128];
					Block block4 = BlocksManager.Blocks[203];
					if (!canBeDyed2)
					{
						return null;
					}
					if (clothingColor2 != 0)
					{
						return new CraftingRecipe
						{
							ResultCount = 1,
							ResultValue = block4.SetDamage(Terrain.MakeBlockValue(203, 0, SetClothingIndex(SetClothingColor(0, 0), clothingIndex2)), damage3),
							RemainsCount = 1,
							RemainsValue = BlocksManager.DamageItem(Terrain.MakeBlockValue(128, 0, 0), damage4 + MathUtils.Max(block3.Durability / 4, 1)),
							RequiredHeatLevel = 1f,
							Description = "未染色服装",
							Ingredients = (string[])ingredients.Clone()
						};
					}
				}
			}
			return null;
		}
	}

	public class CottonBlock : Game.CottonBlock
	{
		public new const int Index = 204;

		public override string GetDisplayName(SubsystemTerrain subsystemTerrain, int value)
		{
			if (!GetIsWild(Terrain.ExtractData(value)))
			{
				return "棉花";
			}
			return "野棉花";
		}
	}

	public class EggBlock : Game.EggBlock
	{
		public new const int Index = 118;

		public override string GetDisplayName(SubsystemTerrain subsystemTerrain, int value)
		{
			EggType eggType = GetEggType(Terrain.ExtractData(value));
			int data = Terrain.ExtractData(value);
			bool isCooked = GetIsCooked(data);
			bool isLaid = GetIsLaid(data);
			if (isCooked)
			{
				return "熟的 " + eggType.DisplayName;
			}
			if (!isLaid)
			{
				return eggType.DisplayName;
			}
			return "生的 " + eggType.DisplayName;
		}
	}

	public class FireworksBlock : Game.FireworksBlock
	{
		public new const int Index = 215;

		public new static readonly string[] ShapeDisplayNames = new string[8]
		{
			"小型爆炸",
			"大型爆炸",
			"圆环",
			"圆盘",
			"球形",
			"短轨迹",
			"长轨迹",
			"平轨迹"
		};

		public new static readonly string[] FireworksColorDisplayNames = new string[8]
		{
			"白色",
			"青色",
			"红色",
			"蓝色",
			"黄色",
			"绿色",
			"橙色",
			"紫色"
		};

		public override string GetDisplayName(SubsystemTerrain subsystemTerrain, int value)
		{
			int data = Terrain.ExtractData(value);
			int color = GetColor(data);
			Shape shape = GetShape(data);
			int altitude = GetAltitude(data);
			bool flickering = GetFlickering(data);
			return string.Format("烟花 {0} {1}{2} ({3})", FireworksColorDisplayNames[color], flickering ? "闪烁 " : null, ShapeDisplayNames[(int)shape], (altitude == 0) ? "低" : "高");
		}
	}

	public class LightbulbBlock : Game.LightbulbBlock
	{
		public new const int Index = 139;

		public override string GetDisplayName(SubsystemTerrain subsystemTerrain, int value)
		{
			int? color = GetColor(Terrain.ExtractData(value));
			return SubsystemPalette.GetName(subsystemTerrain, color, "电灯");
		}
	}

	public class PaintBucketBlock : Game.PaintBucketBlock
	{
		public new const int Index = 129;

		public override string GetDisplayName(SubsystemTerrain subsystemTerrain, int value)
		{
			int color = GetColor(Terrain.ExtractData(value));
			return SubsystemPalette.GetName(subsystemTerrain, color, "油漆桶");
		}
	}

	public class SaplingBlock : Game.SaplingBlock
	{
		public new const int Index = 119;

		public override string GetDisplayName(SubsystemTerrain subsystemTerrain, int value)
		{
			switch (Terrain.ExtractData(value))
			{
				case 0:
					return "橡树树苗";
				case 1:
					return "白桦树苗";
				case 2:
					return "云杉树苗";
				case 3:
					return "高大云杉树苗";
				default:
					return "树苗";
			}
		}
	}

	public class SeedsBlock : Game.SeedsBlock
	{
		public new const int Index = 173;

		public override string GetDisplayName(SubsystemTerrain subsystemTerrain, int value)
		{
			switch (Terrain.ExtractData(value))
			{
				case 0:
					return "高草种子";
				case 1:
					return "红花种子";
				case 2:
					return "紫花种子";
				case 3:
					return "白花种子";
				case 4:
					return "野黑麦种子";
				case 5:
					return "黑麦种子";
				case 6:
					return "棉花种子";
				case 7:
					return "南瓜种子";
				default:
					return string.Empty;
			}
		}
	}

	/*public class FourLedBlock : Game.FourLedBlock
	{
		public new const int Index = 182;

		public override string GetDisplayName(SubsystemTerrain subsystemTerrain, int value)
		{
			int color = GetColor(Terrain.ExtractData(value));
			return LedBlock.LedColorDisplayNames[color] + " 4-LED";
		}
	}

	public class LedBlock : Game.LedBlock
	{
		public new const int Index = 152;

		public new static readonly string[] LedColorDisplayNames = new string[8]
		{
			"白色",
			"青色",
			"红色",
			"蓝色",
			"黄色",
			"绿色",
			"橙色",
			"紫色"
		};

		public override string GetDisplayName(SubsystemTerrain subsystemTerrain, int value)
		{
			int color = GetColor(Terrain.ExtractData(value));
			return LedColorDisplayNames[color] + " LED";
		}
	}*/

	public class SevenSegmentDisplayBlock : Game.SevenSegmentDisplayBlock
	{
		public new const int Index = 185;

		public override string GetDisplayName(SubsystemTerrain subsystemTerrain, int value)
		{
			int color = GetColor(Terrain.ExtractData(value));
			return LedBlock.LedColorDisplayNames[color] + " 七段显示器";
		}
	}
}