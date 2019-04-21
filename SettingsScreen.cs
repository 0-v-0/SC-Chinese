using Engine;
using Game;
using System.Collections.Generic;

namespace ZHCN
{
	public class SettingsControlsScreen : Game.SettingsControlsScreen
	{
		public override void Update()
		{
			if (m_moveControlModeButton.IsClicked)
				SettingsManager.MoveControlMode = (MoveControlMode)((int)(SettingsManager.MoveControlMode + 1) % EnumUtils.GetEnumValues(typeof(MoveControlMode)).Count);
			if (m_lookControlModeButton.IsClicked)
				SettingsManager.LookControlMode = (LookControlMode)((int)(SettingsManager.LookControlMode + 1) % EnumUtils.GetEnumValues(typeof(LookControlMode)).Count);
			if (m_leftHandedLayoutButton.IsClicked)
				SettingsManager.LeftHandedLayout = !SettingsManager.LeftHandedLayout;
			if (m_flipVerticalAxisButton.IsClicked)
				SettingsManager.FlipVerticalAxis = !SettingsManager.FlipVerticalAxis;
			if (m_autoJumpButton.IsClicked)
				SettingsManager.AutoJump = !SettingsManager.AutoJump;
			if (m_horizontalCreativeFlightButton.IsClicked)
				SettingsManager.HorizontalCreativeFlight = !SettingsManager.HorizontalCreativeFlight;
			if (m_moveSensitivitySlider.IsSliding)
				SettingsManager.MoveSensitivity = m_moveSensitivitySlider.Value;
			if (m_lookSensitivitySlider.IsSliding)
				SettingsManager.LookSensitivity = m_lookSensitivitySlider.Value;
			if (m_gamepadCursorSpeedSlider.IsSliding)
				SettingsManager.GamepadCursorSpeed = m_gamepadCursorSpeedSlider.Value;
			if (m_gamepadDeadZoneSlider.IsSliding)
				SettingsManager.GamepadDeadZone = m_gamepadDeadZoneSlider.Value;
			if (m_creativeDigTimeSlider.IsSliding)
				SettingsManager.CreativeDigTime = m_creativeDigTimeSlider.Value;
			if (m_creativeReachSlider.IsSliding)
				SettingsManager.CreativeReach = m_creativeReachSlider.Value;
			if (m_holdDurationSlider.IsSliding)
				SettingsManager.MinimumHoldDuration = m_holdDurationSlider.Value;
			if (m_dragDistanceSlider.IsSliding)
				SettingsManager.MinimumDragDistance = m_dragDistanceSlider.Value;
			m_moveControlModeButton.Text = StringsManager.GetString("MoveControlMode." + SettingsManager.MoveControlMode);
			m_lookControlModeButton.Text = StringsManager.GetString("LookControlMode." + SettingsManager.LookControlMode);
			m_leftHandedLayoutButton.Text = SettingsManager.LeftHandedLayout ? "On" : "Off";
			m_flipVerticalAxisButton.Text = SettingsManager.FlipVerticalAxis ? "On" : "Off";
			m_autoJumpButton.Text = SettingsManager.AutoJump ? "On" : "Off";
			m_horizontalCreativeFlightButton.Text = SettingsManager.HorizontalCreativeFlight ? "On" : "Off";
			m_moveSensitivitySlider.Value = SettingsManager.MoveSensitivity;
			SliderWidget moveSensitivitySlider = m_moveSensitivitySlider;
			float num = MathUtils.Round(SettingsManager.MoveSensitivity * 10f);
			moveSensitivitySlider.Text = num.ToString();
			m_lookSensitivitySlider.Value = SettingsManager.LookSensitivity;
			SliderWidget lookSensitivitySlider = m_lookSensitivitySlider;
			num = MathUtils.Round(SettingsManager.LookSensitivity * 10f);
			lookSensitivitySlider.Text = num.ToString();
			m_gamepadCursorSpeedSlider.Value = SettingsManager.GamepadCursorSpeed;
			m_gamepadCursorSpeedSlider.Text = $"{SettingsManager.GamepadCursorSpeed:0.0}x";
			m_gamepadDeadZoneSlider.Value = SettingsManager.GamepadDeadZone;
			m_gamepadDeadZoneSlider.Text = $"{SettingsManager.GamepadDeadZone * 100f:0}%";
			m_creativeDigTimeSlider.Value = SettingsManager.CreativeDigTime;
			m_creativeDigTimeSlider.Text = $"{MathUtils.Round(1000f * SettingsManager.CreativeDigTime)}ms";
			m_creativeReachSlider.Value = SettingsManager.CreativeReach;
			m_creativeReachSlider.Text = $"{SettingsManager.CreativeReach:0.0} 块";
			m_holdDurationSlider.Value = SettingsManager.MinimumHoldDuration;
			m_holdDurationSlider.Text = $"{MathUtils.Round(1000f * SettingsManager.MinimumHoldDuration)}ms";
			m_dragDistanceSlider.Value = SettingsManager.MinimumDragDistance;
			m_dragDistanceSlider.Text = $"{MathUtils.Round(SettingsManager.MinimumDragDistance)} pix";
			if (Input.Back || Input.Cancel || Children.Find<ButtonWidget>("TopBar.Back", true).IsClicked)
				ScreensManager.SwitchScreen(ScreensManager.PreviousScreen);
		}
	}
	public class SettingsPerformanceScreen : Game.SettingsPerformanceScreen
	{
		public override void Update()
		{
			if (m_resolutionButton.IsClicked)
			{
				IList<int> enumValues = EnumUtils.GetEnumValues(typeof(ResolutionMode));
				SettingsManager.ResolutionMode = (ResolutionMode)((enumValues.IndexOf((int)SettingsManager.ResolutionMode) + 1) % enumValues.Count);
			}
			if (m_visibilityRangeSlider.IsSliding)
				SettingsManager.VisibilityRange = m_visibilityRanges[MathUtils.Clamp((int)m_visibilityRangeSlider.Value, 0, m_visibilityRanges.Count - 1)];
			if (m_viewAnglesButton.IsClicked)
			{
				IList<int> enumValues2 = EnumUtils.GetEnumValues(typeof(ViewAngleMode));
				SettingsManager.ViewAngleMode = (ViewAngleMode)((enumValues2.IndexOf((int)SettingsManager.ViewAngleMode) + 1) % enumValues2.Count);
			}
			if (m_skyRenderingModeButton.IsClicked)
			{
				IList<int> enumValues3 = EnumUtils.GetEnumValues(typeof(SkyRenderingMode));
				SettingsManager.SkyRenderingMode = (SkyRenderingMode)((enumValues3.IndexOf((int)SettingsManager.SkyRenderingMode) + 1) % enumValues3.Count);
			}
			if (m_objectShadowsButton.IsClicked)
				SettingsManager.ObjectsShadowsEnabled = !SettingsManager.ObjectsShadowsEnabled;
			if (m_framerateLimitSlider.IsSliding)
				SettingsManager.PresentationInterval = m_presentationIntervals[MathUtils.Clamp((int)m_framerateLimitSlider.Value, 0, m_presentationIntervals.Count - 1)];
			if (m_displayFpsCounterButton.IsClicked)
				SettingsManager.DisplayFpsCounter = !SettingsManager.DisplayFpsCounter;
			m_resolutionButton.Text = ZHCN.ResolutionModeNames[(int)SettingsManager.ResolutionMode];
			m_visibilityRangeSlider.Value = (float)((m_visibilityRanges.IndexOf(SettingsManager.VisibilityRange) >= 0) ? m_visibilityRanges.IndexOf(SettingsManager.VisibilityRange) : 64);
			m_visibilityRangeSlider.Text = $"{SettingsManager.VisibilityRange} blocks";
			if (SettingsManager.VisibilityRange <= 48)
			{
				m_visibilityRangeWarningLabel.IsVisible = true;
				m_visibilityRangeWarningLabel.Text = "(good for slower devices)";
			}
			else if (SettingsManager.VisibilityRange <= 64)
			{
				m_visibilityRangeWarningLabel.IsVisible = false;
			}
			else if (SettingsManager.VisibilityRange <= 128)
			{
				m_visibilityRangeWarningLabel.IsVisible = true;
				m_visibilityRangeWarningLabel.Text = "(1GB RAM recommended)";
			}
			else if (SettingsManager.VisibilityRange <= 256)
			{
				m_visibilityRangeWarningLabel.IsVisible = true;
				m_visibilityRangeWarningLabel.Text = "(2GB RAM and a fast\ndevice recommended)";
			}
			else if (SettingsManager.VisibilityRange <= 512)
			{
				m_visibilityRangeWarningLabel.IsVisible = true;
				m_visibilityRangeWarningLabel.Text = "(4GB RAM and a very fast\ndevice recommended)";
			}
			else
			{
				m_visibilityRangeWarningLabel.IsVisible = true;
				m_visibilityRangeWarningLabel.Text = "(8GB RAM and an extremely fast\ndevice recommended)";
			}
			m_viewAnglesButton.Text = ZHCN.ViewAngleModeNames[(int)SettingsManager.ViewAngleMode];
			m_skyRenderingModeButton.Text = ZHCN.SkyRenderingModeNames[(int)SettingsManager.SkyRenderingMode];
			m_objectShadowsButton.Text = SettingsManager.ObjectsShadowsEnabled ? "Enabled" : "Disabled";
			m_framerateLimitSlider.Value = (float)((m_presentationIntervals.IndexOf(SettingsManager.PresentationInterval) >= 0) ? m_presentationIntervals.IndexOf(SettingsManager.PresentationInterval) : (m_presentationIntervals.Count - 1));
			m_framerateLimitSlider.Text = (SettingsManager.PresentationInterval != 0) ? $"{SettingsManager.PresentationInterval} vsync" : "Unlimited";
			m_displayFpsCounterButton.Text = SettingsManager.DisplayFpsCounter ? "Yes" : "No";
			if (Input.Back || Input.Cancel || Children.Find<ButtonWidget>("TopBar.Back", true).IsClicked)
			{
				if (SettingsManager.VisibilityRange > m_enterVisibilityRange && SettingsManager.VisibilityRange > 128)
					DialogsManager.ShowDialog(null, new MessageDialog("Large Visibility Range", "The game may crash randomly if your device does not have enough memory to handle the visibility range you selected.", "OK", "Back", c._.Update_b__13_0));
				else
					ScreensManager.SwitchScreen(ScreensManager.PreviousScreen);
			}
		}
	}
	public class SettingsUiScreen : Game.SettingsUiScreen
	{
		public override void Update()
		{
			if (m_uiSizeButton.IsClicked)
				SettingsManager.GuiSize = (GuiSize)((int)(SettingsManager.GuiSize + 1) % EnumUtils.GetEnumValues(typeof(GuiSize)).Count);
			if (m_upsideDownButton.IsClicked)
				SettingsManager.UpsideDownLayout = !SettingsManager.UpsideDownLayout;
			if (m_hideMoveLookPadsButton.IsClicked)
				SettingsManager.HideMoveLookPads = !SettingsManager.HideMoveLookPads;
			if (m_showGuiInScreenshotsButton.IsClicked)
				SettingsManager.ShowGuiInScreenshots = !SettingsManager.ShowGuiInScreenshots;
			if (m_showLogoInScreenshotsButton.IsClicked)
				SettingsManager.ShowLogoInScreenshots = !SettingsManager.ShowLogoInScreenshots;
			if (m_screenshotSizeButton.IsClicked)
				SettingsManager.ScreenshotSize = (ScreenshotSize)((int)(SettingsManager.ScreenshotSize + 1) % EnumUtils.GetEnumValues(typeof(ScreenshotSize)).Count);
			if (m_communityContentModeButton.IsClicked)
				SettingsManager.CommunityContentMode = (CommunityContentMode)((int)(SettingsManager.CommunityContentMode + 1) % EnumUtils.GetEnumValues(typeof(CommunityContentMode)).Count);
			m_uiSizeButton.Text = ZHCN.GuiSizeNames[(int)SettingsManager.GuiSize];
			m_upsideDownButton.Text = SettingsManager.UpsideDownLayout ? "Yes" : "No";
			m_hideMoveLookPadsButton.Text = SettingsManager.HideMoveLookPads ? "Yes" : "No";
			m_showGuiInScreenshotsButton.Text = SettingsManager.ShowGuiInScreenshots ? "Yes" : "No";
			m_showLogoInScreenshotsButton.Text = SettingsManager.ShowLogoInScreenshots ? "Yes" : "No";
			m_screenshotSizeButton.Text = ZHCN.ScreenshotSizeNames[(int)SettingsManager.ScreenshotSize];
			m_communityContentModeButton.Text = ZHCN.CommunityContentModeNames[(int)SettingsManager.CommunityContentMode];
			if (Input.Back || Input.Cancel || Children.Find<ButtonWidget>("TopBar.Back", true).IsClicked)
				ScreensManager.SwitchScreen(ScreensManager.PreviousScreen);
		}
	}
}