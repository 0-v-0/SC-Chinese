using Engine;
using Game;
using GameEntitySystem;
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
				case ExternalContentType.Directory:
					return "目录";
				case ExternalContentType.World:
					return "世界";
				case ExternalContentType.BlocksTexture:
					return "材质纹理";
				case ExternalContentType.CharacterSkin:
					return "角色皮肤";
				case ExternalContentType.FurniturePack:
					return "家具包";
				default:
					return string.Empty;
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
			else if (Time.RealTime - PlayerData.m_playerDeathTime.Value > 1.5 && !DialogsManager.HasDialogs(PlayerData.ComponentPlayer.View.GameWidget) && PlayerData.ComponentPlayer.View.Input.Any)
				if (PlayerData.m_subsystemGameInfo.WorldSettings.GameMode == GameMode.Cruel)
					DialogsManager.ShowDialog(PlayerData.ComponentPlayer.View.GameWidget, new GameMenuDialog(PlayerData.ComponentPlayer));
				else if (PlayerData.m_subsystemGameInfo.WorldSettings.GameMode == GameMode.Adventure && !PlayerData.m_subsystemGameInfo.WorldSettings.IsAdventureRespawnAllowed)
					ScreensManager.SwitchScreen("GameLoading", GameManager.WorldInfo, "AdventureRestart");
				else
					PlayerData.m_project.RemoveEntity(PlayerData.ComponentPlayer.Entity, true);
		}

		public static string[] BlockDigMethodNames;
		/*public static string[] CommunityContentModeNames;
		public static string[] ScreenshotSizeNames;
		public static string[] TextureAnimationModeNames;
		public static string[] ResolutionModeNames;
		public static string[] GuiSizeNames;
		public static string[] SkyRenderingModeNames;
		public static string[] ViewAngleModeNames;
		public static string[] BlockDigMethodNames;
		public static string[] GameModeNames;
		public static string[] EnvironmentBehaviorModeNames;
		public static string[] TerrainGenerationModeNames;
		public static string[] ExternalContentTypeNames;
		public static string[] LookControlModeNames;
		public static string[] MoveControlModeNames;
		public static string[] TimeOfDayModeNames;*/

		public static void Initialize()
		{
			BlockDigMethodNames = new[]{
				"无",
				"铲",
				"挖",
				"劈"
			};

			/*GameModeNames = new[]{
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
				"正常",
				"平坦"
			};

			ExternalContentTypeNames = new[]{
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
			};

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
				"完全",
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

			TextureAnimationModeNames = new[]{
				"完全",
				"简化"
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
			};*/
			var enumerator = ModsManager.GetEntries(".lng").GetEnumerator();
			while (enumerator.MoveNext())
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
		}
	}
}