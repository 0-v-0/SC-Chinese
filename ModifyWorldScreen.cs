using Game;
using System.Collections.Generic;

namespace ZHCN
{
	public class ModifyWorldScreen : Game.ModifyWorldScreen
	{
		public override void Update()
		{
			if (m_gameModeButton.IsClicked && m_worldSettings.GameMode != GameMode.Cruel)
			{
				IList<int> enumValues = EnumUtils.GetEnumValues(typeof(GameMode));
				do
				{
					m_worldSettings.GameMode = (GameMode)((enumValues.IndexOf((int)m_worldSettings.GameMode) + 1) % enumValues.Count);
				}
				while (m_worldSettings.GameMode == GameMode.Cruel);
				m_descriptionLabel.Text = StringsManager.GetString("GameMode." + m_worldSettings.GameMode + ".Description");
			}
			m_currentWorldSettingsData.Clear();
			m_worldSettings.Save(m_currentWorldSettingsData, true);
			bool flag = !CompareValueDictionaries(m_originalWorldSettingsData, m_currentWorldSettingsData);
			bool flag2 = WorldsManager.ValidateWorldName(m_worldSettings.Name);
			m_nameTextBox.Text = m_worldSettings.Name;
			m_seedLabel.Text = m_worldSettings.Seed;
			m_gameModeButton.Text = ZHCN.GameModeNames[(int)m_worldSettings.GameMode];
			m_gameModeButton.IsEnabled = m_worldSettings.GameMode != GameMode.Cruel;
			m_errorLabel.IsVisible = !flag2;
			m_descriptionLabel.IsVisible = flag2;
			m_uploadButton.IsEnabled = flag2 && !flag;
			m_applyButton.IsEnabled = flag2 & flag;
			m_descriptionLabel.Text = StringsManager.GetString("GameMode." + m_worldSettings.GameMode + ".Description");
			if (m_worldOptionsButton.IsClicked)
			{
				ScreensManager.SwitchScreen("WorldOptions", m_worldSettings, true);
			}
			if (m_deleteButton.IsClicked)
			{
				MessageDialog dialog = null;
				dialog = new MessageDialog("你确定？", "世界将被删除。", "是", "否", delegate (MessageDialogButton button)
				{
					if (button == MessageDialogButton.Button1)
					{
						WorldsManager.DeleteWorld(m_directoryName);
						ScreensManager.SwitchScreen("Play");
						DialogsManager.HideDialog(dialog);
					}
					else
					{
						DialogsManager.HideDialog(dialog);
					}
				})
				{
					AutoHide = false
				};
				DialogsManager.ShowDialog(null, dialog);
			}
			if ((m_uploadButton.IsClicked & flag2) && !flag)
			{
				ExternalContentManager.ShowUploadUi(ExternalContentType.World, m_directoryName);
			}
			if (m_applyButton.IsClicked & flag2 & flag)
			{
				if (m_worldSettings.GameMode != 0 && m_worldSettings.GameMode != GameMode.Adventure)
				{
					m_worldSettings.ResetOptionsForNonCreativeMode();
				}
				WorldsManager.ChangeWorld(m_directoryName, m_worldSettings);
				ScreensManager.SwitchScreen("Play");
			}
			if (Input.Back || Input.Cancel || Children.Find<ButtonWidget>("TopBar.Back", true).IsClicked)
			{
				if (flag)
				{
					DialogsManager.ShowDialog(null, new MessageDialog("放弃更改？", "你更改了世界，但还没有被应用。", "是", "否", delegate (MessageDialogButton button)
					{
						if (button == MessageDialogButton.Button1)
						{
							ScreensManager.SwitchScreen("Play");
						}
					}));
				}
				else
				{
					ScreensManager.SwitchScreen("Play");
				}
			}
		}
	}
}