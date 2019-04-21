using Engine;
using Engine.Graphics;
using Engine.Media;
using Game;
using GameEntitySystem;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace ZHCN
{
	public class GameMenuDialog : Dialog
	{
		private static bool m_increaseDetailDialogShown;

		private static bool m_decreaseDetailDialogShown;

		private readonly bool m_adventureRestartExists;

		private StackPanelWidget m_statsPanel;

		private ComponentPlayer m_componentPlayer;

		public GameMenuDialog(ComponentPlayer componentPlayer)
		{
			XElement node = ContentManager.Get<XElement>("Dialogs/GameMenuDialog");
			WidgetsManager.LoadWidgetContents(this, this, node);
			m_statsPanel = Children.Find<StackPanelWidget>("StatsPanel", true);
			m_componentPlayer = componentPlayer;
			m_adventureRestartExists = WorldsManager.SnapshotExists(GameManager.WorldInfo.DirectoryName, "AdventureRestart");
			if (!m_increaseDetailDialogShown && PerformanceManager.LongTermAverageFrameTime.HasValue && PerformanceManager.LongTermAverageFrameTime.Value * 1000f < 25f && (SettingsManager.VisibilityRange <= 64 || SettingsManager.ResolutionMode == ResolutionMode.Low))
			{
				m_increaseDetailDialogShown = true;
				DialogsManager.ShowDialog(null, new MessageDialog("您的设备性能充足", "可以通过增加视距或分辨率来获得更好的图形效果。请到性能设置做这些更改。", "确定", null, null));
				AnalyticsManager.LogEvent("[GameMenuScreen] IncreaseDetailDialog Shown");
			}
			if (!m_decreaseDetailDialogShown && PerformanceManager.LongTermAverageFrameTime.HasValue && PerformanceManager.LongTermAverageFrameTime.Value * 1000f > 50f && (SettingsManager.VisibilityRange >= 64 || SettingsManager.ResolutionMode == ResolutionMode.High))
			{
				m_decreaseDetailDialogShown = true;
				DialogsManager.ShowDialog(null, new MessageDialog("您的设备性能不足", "可以通过降低视距或分辨率来获得更好的游戏性能。请到性能设置做这些更改。", "确定", null, null));
				AnalyticsManager.LogEvent("[GameMenuScreen] DecreaseDetailDialog Shown");
			}
			m_statsPanel.Children.Clear();
			Project project = componentPlayer.Project;
			PlayerData playerData = componentPlayer.PlayerData;
			PlayerStats playerStats = componentPlayer.PlayerStats;
			SubsystemGameInfo subsystemGameInfo = project.FindSubsystem<SubsystemGameInfo>(true);
			SubsystemFurnitureBlockBehavior subsystemFurnitureBlockBehavior = project.FindSubsystem<SubsystemFurnitureBlockBehavior>(true);
			BitmapFont font = ContentManager.Get<BitmapFont>("Fonts/Pericles24");
			BitmapFont font2 = ContentManager.Get<BitmapFont>("Fonts/Pericles18");
			Color white = Color.White;
			var stackPanelWidget = new StackPanelWidget
			{
				Direction = LayoutDirection.Vertical,
				HorizontalAlignment = WidgetAlignment.Center
			};
			m_statsPanel.Children.Add(stackPanelWidget);
			stackPanelWidget.Children.Add(new LabelWidget
			{
				Text = "游戏统计",
				Font = font,
				HorizontalAlignment = WidgetAlignment.Center,
				Margin = new Vector2(0f, 10f),
				Color = white
			});
			AddStat(stackPanelWidget, "游戏模式", ZHCN.GameModeNames[(int)subsystemGameInfo.WorldSettings.GameMode] + ", " + ZHCN.EnvironmentBehaviorModeNames[(int)subsystemGameInfo.WorldSettings.EnvironmentBehaviorMode], "");
			AddStat(stackPanelWidget, "地形类型", ZHCN.TerrainGenerationModeNames[(int)subsystemGameInfo.WorldSettings.TerrainGenerationMode], "");
			string seed = subsystemGameInfo.WorldSettings.Seed;
			AddStat(stackPanelWidget, "世界种子", !string.IsNullOrEmpty(seed) ? seed : "(无)", "");
			AddStat(stackPanelWidget, "海平面", WorldOptionsScreen.FormatOffset(subsystemGameInfo.WorldSettings.SeaLevelOffset), "");
			AddStat(stackPanelWidget, "温度", WorldOptionsScreen.FormatOffset(subsystemGameInfo.WorldSettings.TemperatureOffset), "");
			AddStat(stackPanelWidget, "湿度", WorldOptionsScreen.FormatOffset(subsystemGameInfo.WorldSettings.HumidityOffset), "");
			AddStat(stackPanelWidget, "生物群落大小", subsystemGameInfo.WorldSettings.BiomeSize + "x", "");
			int num = 0;
			for (int i = 0; i < 1024; i++)
			{
				if (subsystemFurnitureBlockBehavior.GetDesign(i) != null)
				{
					num++;
				}
			}
			AddStat(stackPanelWidget, "家具设计使用", $"{num}/{1024}", "");
			stackPanelWidget.Children.Add(new LabelWidget
			{
				Text = "玩家统计",
				Font = font,
				HorizontalAlignment = WidgetAlignment.Center,
				Margin = new Vector2(0f, 10f),
				Color = white
			});
			AddStat(stackPanelWidget, "名称", playerData.Name, "");
			AddStat(stackPanelWidget, "性别", ZHCN.PlayerClassNames[(int)playerData.PlayerClass], "");
			object obj;
			double num2;
			if (!(playerData.FirstSpawnTime >= 0.0))
			{
				obj = "还未出生";
			}
			else
			{
				num2 = (subsystemGameInfo.TotalElapsedGameTime - playerData.FirstSpawnTime) / 1200.0;
				obj = num2.ToString("N1") + " 天前";
			}
			string value = (string)obj;
			AddStat(stackPanelWidget, "首次出生", value, "");
			object obj2;
			if (!(playerData.LastSpawnTime >= 0.0))
			{
				obj2 = "还未出生";
			}
			else
			{
				num2 = (subsystemGameInfo.TotalElapsedGameTime - playerData.LastSpawnTime) / 1200.0;
				obj2 = num2.ToString("N1") + " 天";
			}
			AddStat(stackPanelWidget, "存活天数", (string)obj2, "");
			AddStat(stackPanelWidget, "重生", MathUtils.Max(playerData.SpawnsCount - 1, 0).ToString("N0") + " 次", "");
			AddStat(stackPanelWidget, "达到最高等级", "等级 " + ((int)MathUtils.Floor(playerStats.HighestLevel)).ToString("N0"), "");
			if (componentPlayer != null)
			{
				Vector3 position = componentPlayer.ComponentBody.Position;
				if (subsystemGameInfo.WorldSettings.GameMode == GameMode.Creative)
				{
					AddStat(stackPanelWidget, "位置", $"{position.X:0}, {position.Z:0} 在海拔 {position.Y:0}", "");
				}
				else
				{
					AddStat(stackPanelWidget, "位置", "(在 " + ZHCN.GameModeNames[(int)subsystemGameInfo.WorldSettings.GameMode] + "模式不可用)", "");
				}
			}
			if (string.CompareOrdinal(subsystemGameInfo.WorldSettings.OriginalSerializationVersion, "1.29") > 0)
			{
				stackPanelWidget.Children.Add(new LabelWidget
				{
					Text = "战斗统计",
					Font = font,
					HorizontalAlignment = WidgetAlignment.Center,
					Margin = new Vector2(0f, 10f),
					Color = white
				});
				AddStat(stackPanelWidget, "杀死玩家", playerStats.PlayerKills.ToString("N0"), "");
				AddStat(stackPanelWidget, "杀死陆生生物", playerStats.LandCreatureKills.ToString("N0"), "");
				AddStat(stackPanelWidget, "杀死水生生物", playerStats.WaterCreatureKills.ToString("N0"), "");
				AddStat(stackPanelWidget, "杀死空中生物", playerStats.AirCreatureKills.ToString("N0"), "");
				AddStat(stackPanelWidget, "近战攻击", playerStats.MeleeAttacks.ToString("N0"), "");
				AddStat(stackPanelWidget, "近战重击", playerStats.MeleeHits.ToString("N0"), $"({((playerStats.MeleeHits == 0L) ? 0.0 : (playerStats.MeleeHits / (double)playerStats.MeleeAttacks * 100.0)):0}%)");
				AddStat(stackPanelWidget, "远程攻击", playerStats.RangedAttacks.ToString("N0"), "");
				AddStat(stackPanelWidget, "远程重击", playerStats.RangedHits.ToString("N0"), $"({((playerStats.RangedHits == 0L) ? 0.0 : (playerStats.RangedHits / (double)playerStats.RangedAttacks * 100.0)):0}%)");
				AddStat(stackPanelWidget, "受到重击", playerStats.HitsReceived.ToString("N0"), "");
				stackPanelWidget.Children.Add(new LabelWidget
				{
					Text = "工作统计",
					Font = font,
					HorizontalAlignment = WidgetAlignment.Center,
					Margin = new Vector2(0f, 10f),
					Color = white
				});
				AddStat(stackPanelWidget, "挖掘方块", playerStats.BlocksDug.ToString("N0"), "");
				AddStat(stackPanelWidget, "放置方块", playerStats.BlocksPlaced.ToString("N0"), "");
				AddStat(stackPanelWidget, "交互方块", playerStats.BlocksInteracted.ToString("N0"), "");
				AddStat(stackPanelWidget, "工艺/冶炼", playerStats.ItemsCrafted.ToString("N0"), "");
				AddStat(stackPanelWidget, "家具制造", playerStats.FurnitureItemsMade.ToString("N0"), "");
				stackPanelWidget.Children.Add(new LabelWidget
				{
					Text = "移动统计",
					Font = font,
					HorizontalAlignment = WidgetAlignment.Center,
					Margin = new Vector2(0f, 10f),
					Color = white
				});
				AddStat(stackPanelWidget, "总共旅行距离", FormatDistance(playerStats.DistanceTravelled), "");
				AddStat(stackPanelWidget, "行走距离", FormatDistance(playerStats.DistanceWalked), $"({((playerStats.DistanceTravelled > 0.0) ? (playerStats.DistanceWalked / playerStats.DistanceTravelled * 100.0) : 0.0):0.0}%)");
				AddStat(stackPanelWidget, "跌落距离", FormatDistance(playerStats.DistanceFallen), $"({((playerStats.DistanceTravelled > 0.0) ? (playerStats.DistanceFallen / playerStats.DistanceTravelled * 100.0) : 0.0):0.0}%)");
				AddStat(stackPanelWidget, "攀爬距离", FormatDistance(playerStats.DistanceClimbed), $"({((playerStats.DistanceTravelled > 0.0) ? (playerStats.DistanceClimbed / playerStats.DistanceTravelled * 100.0) : 0.0):0.0}%)");
				AddStat(stackPanelWidget, "飞行距离", FormatDistance(playerStats.DistanceFlown), $"({((playerStats.DistanceTravelled > 0.0) ? (playerStats.DistanceFlown / playerStats.DistanceTravelled * 100.0) : 0.0):0.0}%)");
				AddStat(stackPanelWidget, "游泳距离", FormatDistance(playerStats.DistanceSwam), $"({((playerStats.DistanceTravelled > 0.0) ? (playerStats.DistanceSwam / playerStats.DistanceTravelled * 100.0) : 0.0):0.0}%)");
				AddStat(stackPanelWidget, "骑行距离", FormatDistance(playerStats.DistanceRidden), $"({((playerStats.DistanceTravelled > 0.0) ? (playerStats.DistanceRidden / playerStats.DistanceTravelled * 100.0) : 0.0):0.0}%)");
				AddStat(stackPanelWidget, "最低海拔", FormatDistance(playerStats.LowestAltitude), "");
				AddStat(stackPanelWidget, "最高海拔", FormatDistance(playerStats.HighestAltitude), "");
				AddStat(stackPanelWidget, "最深潜水", playerStats.DeepestDive.ToString("N1") + "m", "");
				AddStat(stackPanelWidget, "跳跃", playerStats.Jumps.ToString("N0"), "");
				stackPanelWidget.Children.Add(new LabelWidget
				{
					Text = "身体统计",
					Font = font,
					HorizontalAlignment = WidgetAlignment.Center,
					Margin = new Vector2(0f, 10f),
					Color = white
				});
				StackPanelWidget containerWidget = stackPanelWidget;
				num2 = playerStats.TotalHealthLost * 100.0;
				AddStat(containerWidget, "总共损失生命", num2.ToString("N0") + "%", "");
				AddStat(stackPanelWidget, "进食次数", playerStats.FoodItemsEaten.ToString("N0") + " 次", "");
				AddStat(stackPanelWidget, "睡觉次数", playerStats.TimesWentToSleep.ToString("N0") + " 次", "");
				StackPanelWidget containerWidget2 = stackPanelWidget;
				num2 = playerStats.TimeSlept / 1200.0;
				AddStat(containerWidget2, "总共睡眠时间", num2.ToString("N1") + " 天", "");
				AddStat(stackPanelWidget, "生病次数", playerStats.TimesWasSick.ToString("N0") + " 次", "");
				AddStat(stackPanelWidget, "呕吐次数", playerStats.TimesPuked.ToString("N0") + " 次", "");
				AddStat(stackPanelWidget, "感冒次数", playerStats.TimesHadFlu.ToString("N0") + " 次", "");
				stackPanelWidget.Children.Add(new LabelWidget
				{
					Text = "其他统计",
					Font = font,
					HorizontalAlignment = WidgetAlignment.Center,
					Margin = new Vector2(0f, 10f),
					Color = white
				});
				AddStat(stackPanelWidget, "闪电袭击", playerStats.StruckByLightning.ToString("N0") + " 次", "");
				StackPanelWidget containerWidget3 = stackPanelWidget;
				AddStat(containerWidget3, "最简单游戏模式", ZHCN.GameModeNames[(int)playerStats.EasiestModeUsed], "");
				ReadOnlyList<PlayerStats.DeathRecord> deathRecords = playerStats.DeathRecords;
				if (deathRecords.Count > 0)
				{
					stackPanelWidget.Children.Add(new LabelWidget
					{
						Text = "死亡记录",
						Font = font,
						HorizontalAlignment = WidgetAlignment.Center,
						Margin = new Vector2(0f, 10f),
						Color = white
					});
					deathRecords = playerStats.DeathRecords;
					foreach (PlayerStats.DeathRecord item in deathRecords)
					{
						float num3 = (float)MathUtils.Remainder(item.Day, 1.0);
						string arg = (!(num3 < 0.2f) && !(num3 >= 0.8f)) ? ((!(num3 >= 0.7f)) ? ((!(num3 >= 0.5f)) ? "早晨，" : "下午，") : "夜晚，") : "白天，";
						AddStat(stackPanelWidget, string.Format("{1}第{0:0}", MathUtils.Floor(item.Day) + 1.0, arg), "", item.Cause);
					}
				}
			}
			else
			{
				stackPanelWidget.Children.Add(new LabelWidget
				{
					Text = "没有玩家统计资料，因为是旧版游戏中的世界。",
					WordWrap = true,
					Font = font2,
					HorizontalAlignment = WidgetAlignment.Center,
					TextAnchor = TextAnchor.HorizontalCenter,
					Margin = new Vector2(20f, 10f),
					Color = white
				});
			}
		}

		public override void Update()
		{
			if (Children.Find<ButtonWidget>("More", true).IsClicked)
			{
				var list = new List<Tuple<string, Action>>();
				if (m_adventureRestartExists && GameManager.WorldInfo.WorldSettings.GameMode == GameMode.Adventure)
				{
					list.Add(new Tuple<string, Action>("重启冒险", delegate
					{
						DialogsManager.ShowDialog(null, new MessageDialog("重置冒险？", "冒险将从头开始。", "是", "否", delegate (MessageDialogButton result)
						{
							if (result == MessageDialogButton.Button1)
							{
								ScreensManager.SwitchScreen("GameLoading", GameManager.WorldInfo, "AdventureRestart");
							}
						}));
					}));
				}
				if (GetRateableItems().FirstOrDefault() != null && UserManager.ActiveUser != null)
				{
					list.Add(new Tuple<string, Action>("评价", delegate
					{
						DialogsManager.ShowDialog(null, new ListSelectionDialog("选择内容进行评价", GetRateableItems(), 60f, (object o) => ((ActiveExternalContentInfo)o).DisplayName, delegate (object o)
						{
							var activeExternalContentInfo = (ActiveExternalContentInfo)o;
							DialogsManager.ShowDialog(null, new RateCommunityContentDialog(activeExternalContentInfo.Address, activeExternalContentInfo.DisplayName, UserManager.ActiveUser.UniqueId));
						}));
					}));
				}
				list.Add(new Tuple<string, Action>("编辑玩家", delegate
				{
					ScreensManager.SwitchScreen("Players", m_componentPlayer.Project.FindSubsystem<SubsystemPlayers>(true));
				}));
				list.Add(new Tuple<string, Action>("设置", delegate
				{
					ScreensManager.SwitchScreen("Settings");
				}));
				list.Add(new Tuple<string, Action>("帮助", delegate
				{
					ScreensManager.SwitchScreen("Help");
				}));
				if ((Input.Devices & (WidgetInputDevice.Keyboard | WidgetInputDevice.Mouse)) != 0)
				{
					list.Add(new Tuple<string, Action>("键盘控制", delegate
					{
						DialogsManager.ShowDialog(ParentWidget, new KeyboardHelpDialog());
					}));
				}
				if ((Input.Devices & WidgetInputDevice.Gamepads) != 0)
				{
					list.Add(new Tuple<string, Action>("游戏手柄控制", delegate
					{
						DialogsManager.ShowDialog(ParentWidget, new GamepadHelpDialog());
					}));
				}
				var dialog = new ListSelectionDialog("更多操作", list, 60f, (object t) => ((Tuple<string, Action>)t).Item1, delegate (object t)
				{
					((Tuple<string, Action>)t).Item2();
				});
				DialogsManager.ShowDialog(this, dialog);
			}
			if (Input.Back || Input.Cancel || Children.Find<ButtonWidget>("Resume", true).IsClicked)
			{
				DialogsManager.HideDialog(this);
			}
			if (Children.Find<ButtonWidget>("Quit", true).IsClicked)
			{
				DialogsManager.HideDialog(this);
				GameManager.SaveProject(true, true);
				GameManager.DisposeProject();
				ScreensManager.SwitchScreen("MainMenu");
			}
		}

		private IEnumerable<ActiveExternalContentInfo> GetRateableItems()
		{
			if (GameManager.Project != null && UserManager.ActiveUser != null)
			{
				SubsystemGameInfo subsystemGameInfo = GameManager.Project.FindSubsystem<SubsystemGameInfo>(true);
				foreach (ActiveExternalContentInfo item in subsystemGameInfo.GetActiveExternalContent())
				{
					if (!CommunityContentManager.IsContentRated(item.Address, UserManager.ActiveUser.UniqueId))
					{
						yield return item;
					}
				}
			}
		}

		private static string FormatDistance(double value)
		{
			if (value < 1000.0)
			{
				return $"{value:0}m";
			}
			return $"{value / 1000.0:N2}km";
		}

		private void AddStat(ContainerWidget containerWidget, string title, string value1, string value2 = "")
		{
			BitmapFont font = ContentManager.Get<BitmapFont>("Fonts/Pericles18");
			Color white = Color.White;
			Color gray = Color.Gray;
			containerWidget.Children.Add(new UniformSpacingPanelWidget
			{
				Direction = LayoutDirection.Horizontal,
				HorizontalAlignment = WidgetAlignment.Center,
				Children =
				{
					new LabelWidget
					{
						Text = title + ":",
						HorizontalAlignment = WidgetAlignment.Far,
						Font = font,
						Color = gray,
						Margin = new Vector2(5f, 1f)
					},
					new StackPanelWidget
					{
						Direction = LayoutDirection.Horizontal,
						HorizontalAlignment = WidgetAlignment.Near,
						Children =
						{
							new LabelWidget
							{
								Text = value1,
								Font = font,
								Color = white,
								Margin = new Vector2(5f, 1f)
							},
							new LabelWidget
							{
								Text = value2,
								Font = font,
								Color = gray,
								Margin = new Vector2(5f, 1f)
							}
						}
					}
				}
			});
		}
	}
}