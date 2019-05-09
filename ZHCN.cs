using Engine;
using Engine.Graphics;
using Game;
using GameEntitySystem;
using System;
using System.Collections.Generic;
using System.IO;

namespace ZHCN
{
	[PluginLoader("zh-cn", "", 0)]
	public class ZHCN
	{
		public static PlayerData PlayerData;

		/// <summary>
		/// 读取键值对文件
		/// </summary>
		/// <param name="dict">读取到的数据</param>
		/// <param name="stream">要读取的流</param>
		/// <param name="separator">分隔符</param>
		/// <param name="commentchar">注释符</param>
		public static void ReadKeyValueFile(Dictionary<string, string> dict, Stream stream, char separator = '=', char commentchar = '#')
		{
			var reader = new StreamReader(stream);
			while (true)
			{
				var line = reader.ReadLine();
				if (line == null) return;
				if (line[0] != commentchar)
				{
					int i = line.IndexOf(separator);
					if (i >= 0)
						dict[line.Substring(0, i).Replace("\\n", "\n")] = line.Substring(i + 1);
				}
			}
		}

		public static string GetEntryTypeDescription(ExternalContentType type)
		{
			switch (type)
			{
				case ExternalContentType.Directory: return "目录";
				case ExternalContentType.World: return "世界";
				case ExternalContentType.BlocksTexture: return "材质纹理";
				case ExternalContentType.CharacterSkin: return "角色皮肤";
				case ExternalContentType.FurniturePack: return "家具包";
				default: return string.Empty;
			}
		}

		public static void Init(PlayerData data, Project project)
		{
			PlayerData = data;
			data.m_stateMachine.m_states.Remove("PlayerDead");
			data.m_stateMachine.AddState("PlayerDead", Enter, Update, null);
		}

		public static void Enter()
		{
			PlayerData.View.ActiveCamera = PlayerData.View.FindCamera<DeathCamera>();
			if (PlayerData.ComponentPlayer != null)
			{
				string text = PlayerData.ComponentPlayer.ComponentHealth.CauseOfDeath;
				if (string.IsNullOrEmpty(text))
					text = "未知";
				string arg = "死因: " + text;
				if (PlayerData.m_subsystemGameInfo.WorldSettings.GameMode == GameMode.Cruel)
					PlayerData.ComponentPlayer.ComponentGui.DisplayLargeMessage("你死了", $"{arg}\n\n不能在残酷模式复活", 30f, 1.5f);
				else if (PlayerData.m_subsystemGameInfo.WorldSettings.GameMode == GameMode.Adventure && !PlayerData.m_subsystemGameInfo.WorldSettings.IsAdventureRespawnAllowed)
					PlayerData.ComponentPlayer.ComponentGui.DisplayLargeMessage("你死了", $"{arg}\n\n轻触以重置", 30f, 1.5f);
				else
					PlayerData.ComponentPlayer.ComponentGui.DisplayLargeMessage("你死了", $"{arg}\n\n轻触以复活", 30f, 1.5f);
			}
			PlayerData.Level = MathUtils.Max(MathUtils.Floor(PlayerData.Level / 2f), 1f);
		}

		public static void Update()
		{
			if (PlayerData.ComponentPlayer == null)
				PlayerData.m_stateMachine.TransitionTo("PrepareSpawn");
			else if ((PlayerData.m_playerDeathTime.HasValue ? Time.RealTime - PlayerData.m_playerDeathTime.Value : Time.RealTime) > 1.5 && !DialogsManager.HasDialogs(PlayerData.ComponentPlayer.View.GameWidget) && PlayerData.ComponentPlayer.View.Input.Any)
				if (PlayerData.m_subsystemGameInfo.WorldSettings.GameMode == GameMode.Cruel)
					DialogsManager.ShowDialog(PlayerData.ComponentPlayer.View.GameWidget, new GameMenuDialog(PlayerData.ComponentPlayer));
				else if (PlayerData.m_subsystemGameInfo.WorldSettings.GameMode == GameMode.Adventure && !PlayerData.m_subsystemGameInfo.WorldSettings.IsAdventureRespawnAllowed)
					ScreensManager.SwitchScreen("GameLoading", GameManager.WorldInfo, "AdventureRestart");
				else
					PlayerData.m_project.RemoveEntity(PlayerData.ComponentPlayer.Entity, true);
		}

		public static string[] BlockDigMethodNames;
		public static string[] CommunityContentModeNames;
		public static string[] ScreenshotSizeNames;
		public static string[] ResolutionModeNames;
		public static string[] GuiSizeNames;
		public static string[] SkyRenderingModeNames;
		public static string[] ViewAngleModeNames;
		public static string[] GameModeNames;
		public static string[] EnvironmentBehaviorModeNames;
		public static string[] TerrainGenerationModeNames;
		//public static string[] ExternalContentTypeNames;
		//public static string[] LookControlModeNames;
		//public static string[] MoveControlModeNames;
		public static string[] TimeOfDayModeNames;
		public static string[] PlayerClassNames;

		public static void Initialize()
		{
			BlockDigMethodNames = new[]{
				"无",
				"铲",
				"挖",
				"劈"
			};

			GameModeNames = new[]{
				"创造",
				"无害",
				"挑战",
				"残酷",
				"冒险"
			};

			EnvironmentBehaviorModeNames = new[]{
				"动态",
				"静态"
			};

			TerrainGenerationModeNames = new[]{
				"大陆",
				"岛屿",
				"平坦"
			};

			/*ExternalContentTypeNames = new[]{
				"未知",
				"目录",
				"方块纹理",
				"角色皮肤",
				"世界"
			};

			LookControlModeNames = new[]{
				"托盘",
				"全屏幕",
				"分离 触控"
			};

			MoveControlModeNames = new[]{
				"托盘",
				"按钮"
			};*/

			TimeOfDayModeNames = new[]{
				"变化",
				"白天",
				"晚上",
				"日出",
				"日落"
			};

			ViewAngleModeNames = new[]{
				"正常",
				"窄",
				"宽"
			};

			SkyRenderingModeNames = new[]{
				"完整",
				"无云",
				"禁用"
			};

			GuiSizeNames = new[]{
				"最小",
				"小",
				"正常",
				"大"
			};

			ResolutionModeNames = new[]{
				"低",
				"中",
				"高"
			};

			ScreenshotSizeNames = new[]{
				"屏幕大小",
				"全高清"
			};

			CommunityContentModeNames = new[]{
				"禁用",
				"严格",
				"普通",
				"全部显示"
			};

			PlayerClassNames = new[]{
				"男",
				"女"
			};
			var enumerator = ModsManager.GetEntries(".lng").GetEnumerator();
			while (enumerator.MoveNext())
				if (string.Equals(Path.GetFileNameWithoutExtension(enumerator.Current.Filename), "zh-cn", StringComparison.OrdinalIgnoreCase))
					ReadKeyValueFile(LabelWidget.Strings, enumerator.Current.Stream);
			ArrowBlock.m_displayNames = new[]
			{
				"木箭",
				"石箭",
				"铁箭",
				"钻石箭",
				"火箭",
				"铁弩箭",
				"钻石铁弩箭",
				"爆炸弩箭",
				"铜箭"
			};
			BulletBlock.m_displayNames = new[]
			{
				"步枪子弹",
				"散弹",
				"单颗散弹"
			};
			ExternalContentManager.GetEntryTypeDescription1 += GetEntryTypeDescription;
			PlayerData.ctor1 += Init;
			ScreensManager.Initialized += Init;
			WorldOptionsScreen.Update1 = Update;
			WorldOptionsScreen.Update_b__39_11 = U;
			//WorldPalette.DefaultNames
			BulletBlock.m_displayNames = new string[16]
			{
				"白色",
				"浅青色",
				"粉红色",
				"淡蓝色",
				"黄色",
				"浅绿色",
				"鲑红色",
				"浅灰色",
				"灰色",
				"青色",
				"紫色",
				"蓝色",
				"棕色",
				"绿色",
				"红色",
				"黑色"
			};
			//LedBlock.LedColorDisplayNames
			BulletBlock.m_displayNames = new string[8]
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
		}

		public static void Init()
		{
			((LoadingScreen)ScreensManager.CurrentScreen).AddLoadAction(ReplaceScreen);
		}

		public static void ReplaceScreen()
		{
			ScreensManager.m_screens["BestiaryDescription"] = new BestiaryDescriptionScreen();
			ScreensManager.m_screens["ExternalContent"] = new ExternalContentScreen();
			ScreensManager.m_screens["ModifyWorld"] = new ModifyWorldScreen();
			ScreensManager.m_screens["RecipaediaDescription"] = new RecipaediaDescriptionScreen();
			ScreensManager.m_screens["SettingsPerformance"] = new SettingsPerformanceScreen();
			ScreensManager.m_screens["SettingsControls"] = new SettingsControlsScreen();
			ScreensManager.m_screens["SettingsUi"] = new SettingsUiScreen();
		}
		public static void Update(WorldOptionsScreen s)
		{
			if (s.m_terrainGenerationButton.IsClicked && !s.m_isExistingWorld)
			{
				IList<int> enumValues = EnumUtils.GetEnumValues(typeof(TerrainGenerationMode));
				DialogsManager.ShowDialog(null, new ListSelectionDialog("选择世界类型", enumValues, 56f, (Func<object, string>)WorldOptionsScreen.c._.Update_b__39_0, s.Update_b__39_1));
			}
			if (s.m_islandSizeEW.IsSliding && !s.m_isExistingWorld)
				s.m_worldSettings.IslandSize.X = WorldOptionsScreen.m_islandSizes[MathUtils.Clamp((int)s.m_islandSizeEW.Value, 0, WorldOptionsScreen.m_islandSizes.Length - 1)];
			if (s.m_islandSizeNS.IsSliding && !s.m_isExistingWorld)
				s.m_worldSettings.IslandSize.Y = WorldOptionsScreen.m_islandSizes[MathUtils.Clamp((int)s.m_islandSizeNS.Value, 0, WorldOptionsScreen.m_islandSizes.Length - 1)];
			if (s.m_flatTerrainLevelSlider.IsSliding && !s.m_isExistingWorld)
			{
				s.m_worldSettings.TerrainLevel = (int)s.m_flatTerrainLevelSlider.Value;
				s.m_descriptionLabel.Text = StringsManager.GetString("FlatTerrainLevel.Description");
			}
			if (s.m_flatTerrainBlockButton.IsClicked && !s.m_isExistingWorld)
			{
				var items = new int[19]
				{
					8,
					2,
					7,
					3,
					67,
					66,
					4,
					5,
					26,
					73,
					21,
					46,
					47,
					15,
					62,
					68,
					126,
					71,
					1
				};
				DialogsManager.ShowDialog(null, new ListSelectionDialog("选择方块", items, 72f, (Func<object, Widget>)WorldOptionsScreen.c._.Update_b__39_2, s.Update_b__39_3));
			}
			if (s.m_flatTerrainMagmaOceanCheckbox.IsClicked)
			{
				s.m_worldSettings.TerrainOceanBlockIndex = (s.m_worldSettings.TerrainOceanBlockIndex == 18) ? 92 : 18;
				s.m_descriptionLabel.Text = StringsManager.GetString("FlatTerrainMagmaOcean.Description");
			}
			if (s.m_seaLevelOffsetSlider.IsSliding && !s.m_isExistingWorld)
			{
				s.m_worldSettings.SeaLevelOffset = (int)s.m_seaLevelOffsetSlider.Value;
				s.m_descriptionLabel.Text = StringsManager.GetString("SeaLevelOffset.Description");
			}
			if (s.m_temperatureOffsetSlider.IsSliding && !s.m_isExistingWorld)
			{
				s.m_worldSettings.TemperatureOffset = s.m_temperatureOffsetSlider.Value;
				s.m_descriptionLabel.Text = StringsManager.GetString("TemperatureOffset.Description");
			}
			if (s.m_humidityOffsetSlider.IsSliding && !s.m_isExistingWorld)
			{
				s.m_worldSettings.HumidityOffset = s.m_humidityOffsetSlider.Value;
				s.m_descriptionLabel.Text = StringsManager.GetString("HumidityOffset.Description");
			}
			if (s.m_biomeSizeSlider.IsSliding && !s.m_isExistingWorld)
			{
				s.m_worldSettings.BiomeSize = WorldOptionsScreen.m_biomeSizes[MathUtils.Clamp((int)s.m_biomeSizeSlider.Value, 0, WorldOptionsScreen.m_biomeSizes.Length - 1)];
				s.m_descriptionLabel.Text = StringsManager.GetString("BiomeSize.Description");
			}
			if (s.m_blocksTextureButton.IsClicked)
			{
				BlocksTexturesManager.UpdateBlocksTexturesList();
				var dialog = new ListSelectionDialog("选择方块材质", BlocksTexturesManager.BlockTexturesNames, 64f, (Func<object, Widget>)s.Update_b__39_4, s.Update_b__39_5);
				DialogsManager.ShowDialog(null, dialog);
				s.m_descriptionLabel.Text = StringsManager.GetString("BlocksTexture.Description");
			}
			if (s.m_paletteButton.IsClicked)
				DialogsManager.ShowDialog(null, new EditPaletteDialog(s.m_worldSettings.Palette));
			if (s.m_supernaturalCreaturesButton.IsClicked)
			{
				s.m_worldSettings.AreSupernaturalCreaturesEnabled = !s.m_worldSettings.AreSupernaturalCreaturesEnabled;
				s.m_descriptionLabel.Text = StringsManager.GetString("SupernaturalCreatures." + s.m_worldSettings.AreSupernaturalCreaturesEnabled.ToString());
			}
			if (s.m_environmentBehaviorButton.IsClicked)
			{
				IList<int> enumValues2 = EnumUtils.GetEnumValues(typeof(EnvironmentBehaviorMode));
				s.m_worldSettings.EnvironmentBehaviorMode = (EnvironmentBehaviorMode)((enumValues2.IndexOf((int)s.m_worldSettings.EnvironmentBehaviorMode) + 1) % enumValues2.Count);
				s.m_descriptionLabel.Text = StringsManager.GetString("EnvironmentBehaviorMode." + s.m_worldSettings.EnvironmentBehaviorMode + ".Description");
			}
			if (s.m_timeOfDayButton.IsClicked)
			{
				IList<int> enumValues3 = EnumUtils.GetEnumValues(typeof(TimeOfDayMode));
				s.m_worldSettings.TimeOfDayMode = (TimeOfDayMode)((enumValues3.IndexOf((int)s.m_worldSettings.TimeOfDayMode) + 1) % enumValues3.Count);
				s.m_descriptionLabel.Text = StringsManager.GetString("TimeOfDayMode." + s.m_worldSettings.TimeOfDayMode + ".Description");
			}
			if (s.m_weatherEffectsButton.IsClicked)
			{
				s.m_worldSettings.AreWeatherEffectsEnabled = !s.m_worldSettings.AreWeatherEffectsEnabled;
				s.m_descriptionLabel.Text = StringsManager.GetString("WeatherMode." + s.m_worldSettings.AreWeatherEffectsEnabled.ToString());
			}
			if (s.m_adventureRespawnButton.IsClicked)
			{
				s.m_worldSettings.IsAdventureRespawnAllowed = !s.m_worldSettings.IsAdventureRespawnAllowed;
				s.m_descriptionLabel.Text = StringsManager.GetString("AdventureRespawnMode." + s.m_worldSettings.IsAdventureRespawnAllowed.ToString());
			}
			if (s.m_adventureSurvivalMechanicsButton.IsClicked)
			{
				s.m_worldSettings.AreAdventureSurvivalMechanicsEnabled = !s.m_worldSettings.AreAdventureSurvivalMechanicsEnabled;
				s.m_descriptionLabel.Text = StringsManager.GetString("AdventureSurvivalMechanics." + s.m_worldSettings.AreAdventureSurvivalMechanicsEnabled.ToString());
			}
			s.m_creativeModePanel.IsVisible = s.m_worldSettings.GameMode == GameMode.Creative;
			s.m_newWorldOnlyPanel.IsVisible = !s.m_isExistingWorld;
			s.m_continentTerrainPanel.IsVisible = s.m_worldSettings.TerrainGenerationMode == TerrainGenerationMode.Continent;
			s.m_islandTerrainPanel.IsVisible = s.m_worldSettings.TerrainGenerationMode == TerrainGenerationMode.Island;
			s.m_flatTerrainPanel.IsVisible = s.m_worldSettings.TerrainGenerationMode == TerrainGenerationMode.Flat;
			s.m_terrainGenerationLabel.Text = TerrainGenerationModeNames[(int)s.m_worldSettings.TerrainGenerationMode];
			s.m_islandSizeEW.Value = WorldOptionsScreen.FindNearestIndex(WorldOptionsScreen.m_islandSizes, s.m_worldSettings.IslandSize.X);
			s.m_islandSizeEW.Text = s.m_worldSettings.IslandSize.X.ToString();
			s.m_islandSizeNS.Value = WorldOptionsScreen.FindNearestIndex(WorldOptionsScreen.m_islandSizes, s.m_worldSettings.IslandSize.Y);
			s.m_islandSizeNS.Text = s.m_worldSettings.IslandSize.Y.ToString();
			s.m_flatTerrainLevelSlider.Value = s.m_worldSettings.TerrainLevel;
			s.m_flatTerrainLevelSlider.Text = s.m_worldSettings.TerrainLevel.ToString();
			s.m_flatTerrainBlock.Contents = s.m_worldSettings.TerrainBlockIndex;
			s.m_flatTerrainMagmaOceanCheckbox.IsChecked = s.m_worldSettings.TerrainOceanBlockIndex == 92;
			string text = (BlocksManager.Blocks[s.m_worldSettings.TerrainBlockIndex] != null) ? BlocksManager.Blocks[s.m_worldSettings.TerrainBlockIndex].GetDisplayName(null, Terrain.MakeBlockValue(s.m_worldSettings.TerrainBlockIndex)) : string.Empty;
			s.m_flatTerrainBlockLabel.Text = text.Length > 10 ? (text.Substring(0, 10) + "...") : text;
			Texture2D texture = s.m_blockTexturesCache.GetTexture(s.m_worldSettings.BlocksTextureName);
			s.m_blocksTextureIcon.Subtexture = new Subtexture(texture, Vector2.Zero, Vector2.One);
			s.m_blocksTextureLabel.Text = BlocksTexturesManager.GetDisplayName(s.m_worldSettings.BlocksTextureName);
			s.m_blocksTextureDetails.Text = string.Format("{0}x{1}", new object[2]
			{
				texture.Width,
				texture.Height
			});
			s.m_seaLevelOffsetSlider.Value = s.m_worldSettings.SeaLevelOffset;
			s.m_seaLevelOffsetSlider.Text = WorldOptionsScreen.FormatOffset(s.m_worldSettings.SeaLevelOffset);
			s.m_temperatureOffsetSlider.Value = s.m_worldSettings.TemperatureOffset;
			s.m_temperatureOffsetSlider.Text = WorldOptionsScreen.FormatOffset(s.m_worldSettings.TemperatureOffset);
			s.m_humidityOffsetSlider.Value = s.m_worldSettings.HumidityOffset;
			s.m_humidityOffsetSlider.Text = WorldOptionsScreen.FormatOffset(s.m_worldSettings.HumidityOffset);
			s.m_biomeSizeSlider.Value = WorldOptionsScreen.FindNearestIndex(WorldOptionsScreen.m_biomeSizes, s.m_worldSettings.BiomeSize);
			s.m_biomeSizeSlider.Text = s.m_worldSettings.BiomeSize.ToString() + "x";
			s.m_environmentBehaviorButton.Text = EnvironmentBehaviorModeNames[(int)s.m_worldSettings.EnvironmentBehaviorMode];
			s.m_timeOfDayButton.Text = TimeOfDayModeNames[(int)s.m_worldSettings.TimeOfDayMode];
			s.m_weatherEffectsButton.Text = s.m_worldSettings.AreWeatherEffectsEnabled ? "Enabled" : "Disabled";
			s.m_adventureRespawnButton.Text = s.m_worldSettings.IsAdventureRespawnAllowed ? "Allowed" : "Not Allowed";
			s.m_adventureSurvivalMechanicsButton.Text = s.m_worldSettings.AreAdventureSurvivalMechanicsEnabled ? "Enabled" : "Disabled";
			s.m_supernaturalCreaturesButton.Text = s.m_worldSettings.AreSupernaturalCreaturesEnabled ? "Enabled" : "Disabled";
			if (s.Input.Back || s.Input.Cancel || s.Children.Find<ButtonWidget>("TopBar.Back").IsClicked)
				ScreensManager.SwitchScreen(ScreensManager.PreviousScreen);
		}
		public static void U(WorldOptionsScreen s, object e)
		{
			if (s.m_worldSettings.GameMode != 0 && (TerrainGenerationMode)e == TerrainGenerationMode.Flat)
			{
				DialogsManager.ShowDialog(null, new MessageDialog("Unavailable", "平坦地形只能在创造模式下使用", "OK", null, null));
			}
			else
			{
				s.m_worldSettings.TerrainGenerationMode = (TerrainGenerationMode)e;
				s.m_descriptionLabel.Text = StringsManager.GetString("TerrainGenerationMode." + s.m_worldSettings.TerrainGenerationMode + ".Description");
			}
		}
	}
}