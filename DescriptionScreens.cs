using Engine;
using Engine.Media;
using Game;
using System.Collections.Generic;

namespace ZHCN
{
	public class BestiaryDescriptionScreen : Game.BestiaryDescriptionScreen
	{
		public override void Enter(object[] parameters)
		{
			BestiaryCreatureInfo item = (BestiaryCreatureInfo)parameters[0];
			m_infoList = (IList<BestiaryCreatureInfo>)parameters[1];
			m_index = m_infoList.IndexOf(item);
			UpdateCreatureProperties();
		}

		public override void Update()
		{
			m_leftButtonWidget.IsEnabled = m_index > 0;
			m_rightButtonWidget.IsEnabled = m_index < m_infoList.Count - 1;
			if (m_leftButtonWidget.IsClicked || Input.Left)
			{
				m_index = MathUtils.Max(m_index - 1, 0);
				UpdateCreatureProperties();
			}
			if (m_rightButtonWidget.IsClicked || Input.Right)
			{
				m_index = MathUtils.Min(m_index + 1, m_infoList.Count - 1);
				UpdateCreatureProperties();
			}
			if (Input.Back || Input.Cancel || Children.Find<ButtonWidget>("TopBar.Back", true).IsClicked)
				ScreensManager.SwitchScreen(ScreensManager.PreviousScreen);
		}

		public new void UpdateCreatureProperties()
		{
			if (m_index >= 0 && m_index < m_infoList.Count)
			{
				BestiaryCreatureInfo bestiaryCreatureInfo = m_infoList[m_index];
				m_modelWidget.AutoRotationVector = new Vector3(0f, 1f, 0f);
				BestiaryScreen.SetupBestiaryModelWidget(bestiaryCreatureInfo, m_modelWidget, new Vector3(-1f, 0f, -1f), true, true);
				m_nameWidget.Text = bestiaryCreatureInfo.DisplayName;
				m_descriptionWidget.Text = bestiaryCreatureInfo.Description;
				m_propertyNames1Widget.Text = string.Empty;
				m_propertyValues1Widget.Text = string.Empty;
				m_propertyNames1Widget.Text += "血量：\n";
				LabelWidget propertyValues1Widget = m_propertyValues1Widget;
				propertyValues1Widget.Text = propertyValues1Widget.Text + bestiaryCreatureInfo.AttackResilience.ToString() + "\n";
				m_propertyNames1Widget.Text += "攻击力：\n";
				LabelWidget propertyValues1Widget2 = m_propertyValues1Widget;
				propertyValues1Widget2.Text = propertyValues1Widget2.Text + ((bestiaryCreatureInfo.AttackPower > 0f) ? bestiaryCreatureInfo.AttackPower.ToString("0.0") : "无") + "\n";
				m_propertyNames1Widget.Text += "群聚行为：\n";
				LabelWidget propertyValues1Widget3 = m_propertyValues1Widget;
				propertyValues1Widget3.Text = propertyValues1Widget3.Text + (bestiaryCreatureInfo.IsHerding ? "是" : "否") + "\n";
				m_propertyNames1Widget.Text += "可否骑乘：\n";
				LabelWidget propertyValues1Widget4 = m_propertyValues1Widget;
				propertyValues1Widget4.Text = propertyValues1Widget4.Text + (bestiaryCreatureInfo.CanBeRidden ? "是" : "否") + "\n";
				m_propertyNames1Widget.Text = m_propertyNames1Widget.Text.TrimEnd();
				m_propertyValues1Widget.Text = m_propertyValues1Widget.Text.TrimEnd();
				m_propertyNames2Widget.Text = string.Empty;
				m_propertyValues2Widget.Text = string.Empty;
				m_propertyNames2Widget.Text += "速度：\n";
				LabelWidget propertyValues2Widget = m_propertyValues2Widget;
				propertyValues2Widget.Text = propertyValues2Widget.Text + (bestiaryCreatureInfo.MovementSpeed * 3.6).ToString("0") + " km/h\n";
				LabelWidget propertyNames2Widget2 = m_propertyNames2Widget;
				propertyNames2Widget2.Text += "跳跃高度：\n";
				LabelWidget propertyValues2Widget2 = m_propertyValues2Widget;
				propertyValues2Widget2.Text = propertyValues2Widget2.Text + bestiaryCreatureInfo.JumpHeight.ToString("0.0") + " 米\n";
				m_propertyNames2Widget.Text += "重量：\n";
				LabelWidget propertyValues2Widget3 = m_propertyValues2Widget;
				propertyValues2Widget3.Text = propertyValues2Widget3.Text + bestiaryCreatureInfo.Mass.ToString() + " kg\n";
				m_propertyNames2Widget.Text += "是否有蛋：\n";
				LabelWidget propertyValues2Widget4 = m_propertyValues2Widget;
				propertyValues2Widget4.Text = propertyValues2Widget4.Text + (bestiaryCreatureInfo.HasSpawnerEgg ? "是" : "否") + "\n";
				m_propertyNames2Widget.Text = m_propertyNames2Widget.Text.TrimEnd();
				m_propertyValues2Widget.Text = m_propertyValues2Widget.Text.TrimEnd();
				m_dropsPanel.Children.Clear();
				if (bestiaryCreatureInfo.Loot.Count > 0)
				{
					foreach (ComponentLoot.Loot item in bestiaryCreatureInfo.Loot)
					{
						string text = (item.MinCount >= item.MaxCount) ? $"{item.MinCount}" : $"{item.MinCount} 到 {item.MaxCount}";
						if (item.Probability < 1f)
						{
							text += $" ({item.Probability * 100f:0}% of time)";
						}
						m_dropsPanel.Children.Add(new StackPanelWidget
						{
							Margin = new Vector2(20f, 0f),
							Children =
							{
								new BlockIconWidget
								{
									Size = new Vector2(32f),
									Scale = 1.2f,
									VerticalAlignment = WidgetAlignment.Center,
									Value = item.Value
								},
								new CanvasWidget
								{
									Size = new Vector2(10f, 0f)
								},
								new LabelWidget
								{
									Font = ContentManager.Get<BitmapFont>("Fonts/Pericles18"),
									VerticalAlignment = WidgetAlignment.Center,
									Text = text
								}
							}
						});
					}
				}
				else
				{
					m_dropsPanel.Children.Add(new LabelWidget
					{
						Margin = new Vector2(20f, 0f),
						Font = ContentManager.Get<BitmapFont>("Fonts/Pericles18"),
						Text = "没有"
					});
				}
			}
		}
	}
	public class RecipaediaDescriptionScreen : Game.RecipaediaDescriptionScreen
	{
		public override void Enter(object[] parameters)
		{
			m_valuesList = (IList<int>)parameters[1];
			m_index = m_valuesList.IndexOf((int)parameters[0]);
			UpdateBlockProperties();
		}

		public override void Update()
		{
			m_leftButtonWidget.IsEnabled = m_index > 0;
			m_rightButtonWidget.IsEnabled = m_index < m_valuesList.Count - 1;
			if (m_leftButtonWidget.IsClicked || Input.Left)
			{
				m_index = MathUtils.Max(m_index - 1, 0);
				UpdateBlockProperties();
			}
			if (m_rightButtonWidget.IsClicked || Input.Right)
			{
				m_index = MathUtils.Min(m_index + 1, m_valuesList.Count - 1);
				UpdateBlockProperties();
			}
			if (Input.Back || Input.Cancel || Children.Find<ButtonWidget>("TopBar.Back", true).IsClicked)
			{
				ScreensManager.SwitchScreen(ScreensManager.PreviousScreen);
			}
		}

		public new Dictionary<string, string> GetBlockProperties(int value)
		{
			Dictionary<string, string> dictionary = new Dictionary<string, string>();
			int num = Terrain.ExtractContents(value);
			Block block = BlocksManager.Blocks[num];
			if (block.EmittedLightAmount > 0)
				dictionary.Add("发光强度", block.EmittedLightAmount.ToString());
			if (block.FuelFireDuration > 0f)
				dictionary.Add("燃烧值", block.FuelFireDuration.ToString());
			dictionary.Add("可堆叠", (block.MaxStacking > 1) ? ("是 (最高 " + block.MaxStacking.ToString() + ")") : "否");
			dictionary.Add("可燃烧", (block.FireDuration > 0f) ? "是" : "否");
			float num2;
			if (block.GetNutritionalValue(value) > 0f)
			{
				num2 = block.GetNutritionalValue(value);
				dictionary.Add("营养值", num2.ToString());
			}
			if (block.GetRotPeriod(value) > 0)
				dictionary.Add("最大保存时间", $"{2 * block.GetRotPeriod(value) * 60f / 1200f:0.0} 天");
			if (block.DigMethod != 0)
			{
				dictionary.Add("挖掘方法", ZHCN.BlockDigMethodNames[(int)block.DigMethod]);
				dictionary.Add("挖掘难度", block.DigResilience.ToString());
			}
			if (block.ExplosionResilience > 0f)
				dictionary.Add("抗爆能力", block.ExplosionResilience.ToString());
			if (block.GetExplosionPressure(value) > 0f)
			{
				num2 = block.GetExplosionPressure(value);
				dictionary.Add("爆炸威力", num2.ToString());
			}
			bool flag = false;
			if (block.GetMeleePower(value) > 1f)
			{
				num2 = block.GetMeleePower(value);
				dictionary.Add("近战攻击", num2.ToString());
				flag = true;
			}
			if (block.GetMeleePower(value) > 1f)
			{
				dictionary.Add("近战命中率", $"{100f * block.GetMeleeHitProbability(value):0}%");
				flag = true;
			}
			if (block.GetProjectilePower(value) > 1f)
			{
				num2 = block.GetProjectilePower(value);
				dictionary.Add("弹射伤害", num2.ToString());
				flag = true;
			}
			if (block.ShovelPower > 1f)
			{
				dictionary.Add("铲", block.ShovelPower.ToString());
				flag = true;
			}
			if (block.HackPower > 1f)
			{
				dictionary.Add("劈", block.HackPower.ToString());
				flag = true;
			}
			if (block.QuarryPower > 1f)
			{
				dictionary.Add("挖", block.QuarryPower.ToString());
				flag = true;
			}
			if (flag && block.Durability > 0)
				dictionary.Add("耐久", block.Durability.ToString());
			if (block.DefaultExperienceCount > 0f)
				dictionary.Add("掉落经验", block.DefaultExperienceCount.ToString());
			if (block is ClothingBlock)
			{
				ClothingData clothingData = Game.ClothingBlock.GetClothingData(Terrain.ExtractData(value));
				dictionary.Add("可染色", clothingData.CanBeDyed ? "是" : "否");
				dictionary.Add("护甲防御", $"{(int)(clothingData.ArmorProtection * 100f)}%");
				dictionary.Add("护甲耐久", clothingData.Sturdiness.ToString());
				dictionary.Add("绝缘", $"{clothingData.Insulation:0.0} clo");
				dictionary.Add("移动速度", $"{clothingData.MovementSpeedFactor * 100f:0}%");
			}
			return dictionary;
		}

		public new void UpdateBlockProperties()
		{
			if (m_index >= 0 && m_index < m_valuesList.Count)
			{
				int value = m_valuesList[m_index];
				int num = Terrain.ExtractContents(value);
				Block block = BlocksManager.Blocks[num];
				m_blockIconWidget.Value = value;
				m_nameWidget.Text = block.GetDisplayName(null, value);
				m_descriptionWidget.Text = block.GetDescription(value);
				m_propertyNames1Widget.Text = string.Empty;
				m_propertyValues1Widget.Text = string.Empty;
				m_propertyNames2Widget.Text = string.Empty;
				m_propertyValues2Widget.Text = string.Empty;
				Dictionary<string, string> blockProperties = GetBlockProperties(value);
				int num2 = 0;
				foreach (KeyValuePair<string, string> item in blockProperties)
				{
					LabelWidget lw;
					if (num2 < blockProperties.Count - blockProperties.Count / 2)
					{
						lw = m_propertyNames1Widget;
						lw.Text = lw.Text + item.Key + ":\n";
						lw = m_propertyValues1Widget;
						lw.Text = lw.Text + item.Value + "\n";
					}
					else
					{
						lw = m_propertyNames2Widget;
						lw.Text = lw.Text + item.Key + ":\n";
						lw = m_propertyValues2Widget;
						lw.Text = lw.Text + item.Value + "\n";
					}
					num2++;
				}
			}
		}
	}
}