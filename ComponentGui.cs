using Engine;
using Game;

namespace ZHCN
{
	public class ComponentGui : Game.ComponentGui, IUpdateable
	{
		public new void Update(float dt)
		{
			HandleInput();
			UpdateWidgets();
		}

		public new void HandleInput()
		{
			WidgetInput input = m_componentPlayer.View.Input;
			PlayerInput playerInput = m_componentPlayer.ComponentInput.PlayerInput;
			ComponentRider componentRider = m_componentPlayer.ComponentRider;
			if (m_componentPlayer.View.ActiveCamera.IsEntityControlEnabled)
				if (!m_keyboardHelpMessageShown && (m_componentPlayer.PlayerData.InputDevice & WidgetInputDevice.Keyboard) != 0 && Time.PeriodicEvent(7.0, 0.0))
				{
					m_keyboardHelpMessageShown = true;
					DisplaySmallMessage("按 H 键查看键盘控制帮助\n(或看帮助)", blinking: true, playNotificationSound: true);
				}
				else if (!m_gamepadHelpMessageShown && (m_componentPlayer.PlayerData.InputDevice & WidgetInputDevice.Gamepads) != 0 && Time.PeriodicEvent(7.0, 0.0))
				{
					m_gamepadHelpMessageShown = true;
					DisplaySmallMessage("按 START/PAUSE 键查看手柄控制帮助\n(或看帮助)", blinking: true, playNotificationSound: true);
				}
			if (playerInput.KeyboardHelp)
			{
				if (m_keyboardHelpDialog == null)
					m_keyboardHelpDialog = new KeyboardHelpDialog();
				if (m_keyboardHelpDialog.ParentWidget != null)
					DialogsManager.HideDialog(m_keyboardHelpDialog);
				else
					DialogsManager.ShowDialog(m_componentPlayer.View.GameWidget, m_keyboardHelpDialog);
			}
			if (playerInput.GamepadHelp)
			{
				if (m_gamepadHelpDialog == null)
					m_gamepadHelpDialog = new GamepadHelpDialog();
				if (m_gamepadHelpDialog.ParentWidget != null)
					DialogsManager.HideDialog(m_gamepadHelpDialog);
				else
					DialogsManager.ShowDialog(m_componentPlayer.View.GameWidget, m_gamepadHelpDialog);
			}
			if (m_helpButtonWidget.IsClicked)
				ScreensManager.SwitchScreen("Help");
			if (playerInput.ToggleInventory || m_inventoryButtonWidget.IsClicked)
			{
				if (IsInventoryVisible())
					ModalPanelWidget = null;
				else if (m_componentPlayer.ComponentMiner.Inventory is ComponentCreativeInventory)
				{
					ModalPanelWidget = new CreativeInventoryWidget(m_componentPlayer.Entity);
				}
				else
				{
					ModalPanelWidget = new FullInventoryWidget(m_componentPlayer.ComponentMiner.Inventory, m_componentPlayer.Entity.FindComponent<ComponentCraftingTable>(throwOnError: true));
				}
			}
			if (playerInput.ToggleClothing || m_clothingButtonWidget.IsClicked)
			{
				ModalPanelWidget = IsClothingVisible() ? null : new ClothingWidget(m_componentPlayer);
			}
			if (m_sneakButtonWidget.IsClicked || playerInput.ToggleSneak)
			{
				bool isSneaking = m_componentPlayer.ComponentBody.IsSneaking;
				m_componentPlayer.ComponentBody.IsSneaking = !isSneaking;
				if (m_componentPlayer.ComponentBody.IsSneaking != isSneaking)
					if (m_componentPlayer.ComponentBody.IsSneaking)
						DisplaySmallMessage("潜行模式：开", false, false);
					else
						DisplaySmallMessage("潜行模式：关", false, false);
			}
			if (componentRider != null && (m_mountButtonWidget.IsClicked || playerInput.ToggleMount))
			{
				bool flag = componentRider.Mount != null;
				if (flag)
					componentRider.StartDismounting();
				else
				{
					ComponentMount componentMount = componentRider.FindNearestMount();
					if (componentMount != null)
						componentRider.StartMounting(componentMount);
				}
				if (componentRider.Mount != null != flag)
					if (componentRider.Mount != null)
						DisplaySmallMessage("上马", false, false);
					else
						DisplaySmallMessage("下马", false, false);
			}
			if ((m_editItemButton.IsClicked || playerInput.EditItem) && m_nearbyEditableCell.HasValue)
			{
				int cellValue = m_subsystemTerrain.Terrain.GetCellValue(m_nearbyEditableCell.Value.X, m_nearbyEditableCell.Value.Y, m_nearbyEditableCell.Value.Z);
				int contents = Terrain.ExtractContents(cellValue);
				SubsystemBlockBehavior[] blockBehaviors = m_subsystemBlockBehaviors.GetBlockBehaviors(contents);
				for (int i = 0; i < blockBehaviors.Length && !blockBehaviors[i].OnEditBlock(m_nearbyEditableCell.Value.X, m_nearbyEditableCell.Value.Y, m_nearbyEditableCell.Value.Z, cellValue, m_componentPlayer); i++)
				{
				}
			}
			else if ((m_editItemButton.IsClicked || playerInput.EditItem) && IsActiveSlotEditable())
			{
				IInventory inventory = m_componentPlayer.ComponentMiner.Inventory;
				if (inventory != null)
				{
					int activeSlotIndex = inventory.ActiveSlotIndex;
					int num = Terrain.ExtractContents(inventory.GetSlotValue(activeSlotIndex));
					if (BlocksManager.Blocks[num].IsEditable)
					{
						SubsystemBlockBehavior[] blockBehaviors2 = m_subsystemBlockBehaviors.GetBlockBehaviors(num);
						for (int j = 0; j < blockBehaviors2.Length && !blockBehaviors2[j].OnEditInventoryItem(inventory, activeSlotIndex, m_componentPlayer); j++)
						{
						}
					}
				}
			}
			if (m_subsystemGameInfo.WorldSettings.GameMode == GameMode.Creative && (m_creativeFlyButtonWidget.IsClicked || playerInput.ToggleCreativeFly) && componentRider.Mount == null)
			{
				bool isCreativeFlyEnabled = m_componentPlayer.ComponentLocomotion.IsCreativeFlyEnabled;
				m_componentPlayer.ComponentLocomotion.IsCreativeFlyEnabled = !isCreativeFlyEnabled;
				if (m_componentPlayer.ComponentLocomotion.IsCreativeFlyEnabled != isCreativeFlyEnabled)
				{
					if (m_componentPlayer.ComponentLocomotion.IsCreativeFlyEnabled)
					{
						m_componentPlayer.ComponentLocomotion.JumpOrder = 1f;
						DisplaySmallMessage("飞行模式：开", blinking: false, playNotificationSound: false);
					}
					else
						DisplaySmallMessage("飞行模式：关", blinking: false, playNotificationSound: false);
				}
			}
			if (m_cameraButtonWidget.IsClicked || playerInput.SwitchCameraMode)
			{
				View view = m_componentPlayer.View;
				if (view.ActiveCamera.GetType() == typeof(FppCamera))
				{
					view.ActiveCamera = view.FindCamera<TppCamera>(true);
					DisplaySmallMessage("第三人称视角", false, false);
				}
				else if (view.ActiveCamera.GetType() == typeof(TppCamera))
				{
					view.ActiveCamera = view.FindCamera<OrbitCamera>(true);
					DisplaySmallMessage("滑轨视角", false, false);
				}
				else if (view.ActiveCamera.GetType() == typeof(OrbitCamera))
				{
					view.ActiveCamera = view.FindCamera<FixedCamera>(true);
					DisplaySmallMessage("固定视角", false, false);
				}
				else
				{
					view.ActiveCamera = view.FindCamera<FppCamera>(true);
					DisplaySmallMessage("第一人称视角", false, false);
				}
			}
			if (m_photoButtonWidget.IsClicked || playerInput.TakeScreenshot)
			{
				ScreenCaptureManager.CapturePhoto();
				Time.QueueFrameCountDelayedExecution(Time.FrameIndex + 1, delegate
				{
					DisplaySmallMessage("照片已保存到图片相册", false, false);
				});
			}
			if (m_subsystemGameInfo.WorldSettings.GameMode == GameMode.Creative && (m_lightningButtonWidget.IsClicked || playerInput.Lighting))
			{
				var matrix = Matrix.CreateFromQuaternion(m_componentPlayer.ComponentCreatureModel.EyeRotation);
				Project.FindSubsystem<SubsystemWeather>(throwOnError: true).ManualLightingStrike(m_componentPlayer.ComponentCreatureModel.EyePosition, matrix.Forward);
			}
			if (m_subsystemGameInfo.WorldSettings.GameMode == GameMode.Creative && (m_timeOfDayButtonWidget.IsClicked || playerInput.TimeOfDay))
			{
				float num2 = MathUtils.Remainder(0.25f, 1f);
				float num3 = MathUtils.Remainder(0.5f, 1f);
				float num4 = MathUtils.Remainder(0.75f, 1f);
				float num5 = MathUtils.Remainder(1f, 1f);
				float num6 = MathUtils.Remainder(num2 - m_subsystemTimeOfDay.TimeOfDay, 1f);
				float num7 = MathUtils.Remainder(num3 - m_subsystemTimeOfDay.TimeOfDay, 1f);
				float num8 = MathUtils.Remainder(num4 - m_subsystemTimeOfDay.TimeOfDay, 1f);
				float num9 = MathUtils.Remainder(num5 - m_subsystemTimeOfDay.TimeOfDay, 1f);
				float num10 = MathUtils.Min(num6, num7, num8, num9);
				if (num6 == num10)
				{
					m_subsystemTimeOfDay.TimeOfDayOffset += num6;
					DisplaySmallMessage("黎明", false, false);
				}
				else if (num7 == num10)
				{
					m_subsystemTimeOfDay.TimeOfDayOffset += num7;
					DisplaySmallMessage("中午", false, false);
				}
				else if (num8 == num10)
				{
					m_subsystemTimeOfDay.TimeOfDayOffset += num8;
					DisplaySmallMessage("黄昏", false, false);
				}
				else if (num9 == num10)
				{
					m_subsystemTimeOfDay.TimeOfDayOffset += num9;
					DisplaySmallMessage("午夜", false, false);
				}
			}
			if (ModalPanelWidget != null)
			{
				if (input.Cancel || input.Back || m_backButtonWidget.IsClicked)
					ModalPanelWidget = null;
			}
			else if (input.Back || m_backButtonWidget.IsClicked)
				DialogsManager.ShowDialog(m_componentPlayer.View.GameWidget, new GameMenuDialog(m_componentPlayer));
		}

		public new void UpdateWidgets()
		{
			ComponentRider componentRider = m_componentPlayer.ComponentRider;
			var componentSleep = m_componentPlayer.ComponentSleep;
			ComponentInput componentInput = m_componentPlayer.ComponentInput;
			WorldSettings worldSettings = m_subsystemGameInfo.WorldSettings;
			GameMode gameMode = worldSettings.GameMode;
			UpdateSidePanelsAnimation();
			if (m_modalPanelAnimationData != null)
				UpdateModalPanelAnimation();
			if (m_message != null)
			{
				double realTime = Time.RealTime;
				m_largeMessageWidget.IsVisible = true;
				LabelWidget labelWidget = m_largeMessageWidget.Children.Find<LabelWidget>("LargeLabel");
				LabelWidget labelWidget2 = m_largeMessageWidget.Children.Find<LabelWidget>("SmallLabel");
				labelWidget.Text = m_message.LargeText ?? string.Empty;
				labelWidget2.Text = m_message.SmallText;
				labelWidget.IsVisible = !string.IsNullOrEmpty(m_message.LargeText);
				labelWidget2.IsVisible = !string.IsNullOrEmpty(m_message.SmallText);
				float num = (float)MathUtils.Min(MathUtils.Saturate(2.0 * (realTime - m_message.StartTime)), MathUtils.Saturate(2.0 * (m_message.StartTime + m_message.Duration - realTime)));
				labelWidget.Color = new Color(num, num, num, num);
				labelWidget2.Color = new Color(num, num, num, num);
				if (Time.RealTime > m_message.StartTime + m_message.Duration)
					m_message = null;
			}
			else
				m_largeMessageWidget.IsVisible = false;
			m_controlsContainerWidget.IsVisible = m_componentPlayer.PlayerData.IsReadyForPlaying && m_componentPlayer.View.ActiveCamera.IsEntityControlEnabled && componentSleep.SleepFactor <= 0f;
			m_moveRectangleContainerWidget.IsVisible = !SettingsManager.HideMoveLookPads && componentInput.IsControlledByTouch;
			m_lookRectangleContainerWidget.IsVisible = !SettingsManager.HideMoveLookPads && componentInput.IsControlledByTouch && (SettingsManager.LookControlMode != LookControlMode.EntireScreen || SettingsManager.MoveControlMode != MoveControlMode.Buttons);
			m_lookPadContainerWidget.IsVisible = SettingsManager.LookControlMode != LookControlMode.SplitTouch;
			MoveRoseWidget.IsVisible = componentInput.IsControlledByTouch;
			m_moreContentsWidget.IsVisible = m_moreButtonWidget.IsChecked;
			HealthBarWidget.IsVisible = gameMode != GameMode.Creative;
			FoodBarWidget.IsVisible = gameMode != 0 && worldSettings.AreAdventureSurvivalMechanicsEnabled;
			TemperatureBarWidget.IsVisible = gameMode != 0 && worldSettings.AreAdventureSurvivalMechanicsEnabled;
			LevelLabelWidget.IsVisible = gameMode != 0 && worldSettings.AreAdventureSurvivalMechanicsEnabled;
			m_creativeFlyButtonWidget.IsVisible = gameMode == GameMode.Creative;
			m_timeOfDayButtonWidget.IsVisible = gameMode == GameMode.Creative;
			m_lightningButtonWidget.IsVisible = gameMode == GameMode.Creative;
			m_moveButtonsContainerWidget.IsVisible = SettingsManager.MoveControlMode == MoveControlMode.Buttons;
			m_movePadContainerWidget.IsVisible = SettingsManager.MoveControlMode == MoveControlMode.Pad;
			if (SettingsManager.LeftHandedLayout)
			{
				m_moveContainerWidget.HorizontalAlignment = WidgetAlignment.Far;
				m_lookContainerWidget.HorizontalAlignment = WidgetAlignment.Near;
				m_moveRectangleWidget.FlipHorizontal = true;
				m_lookRectangleWidget.FlipHorizontal = false;
			}
			else
			{
				m_moveContainerWidget.HorizontalAlignment = WidgetAlignment.Near;
				m_lookContainerWidget.HorizontalAlignment = WidgetAlignment.Far;
				m_moveRectangleWidget.FlipHorizontal = false;
				m_lookRectangleWidget.FlipHorizontal = true;
			}
			m_nearbyEditableCell = FindNearbyEditableCell();
			m_sneakButtonWidget.IsChecked = m_componentPlayer.ComponentBody.IsSneaking;
			m_creativeFlyButtonWidget.IsChecked = m_componentPlayer.ComponentLocomotion.IsCreativeFlyEnabled;
			m_inventoryButtonWidget.IsChecked = IsInventoryVisible();
			m_clothingButtonWidget.IsChecked = IsClothingVisible();
			if (IsActiveSlotEditable() || m_nearbyEditableCell.HasValue)
			{
				m_sneakButtonWidget.IsVisible = false;
				m_mountButtonWidget.IsVisible = false;
				m_editItemButton.IsVisible = true;
			}
			else if (componentRider != null && componentRider.Mount != null)
			{
				m_sneakButtonWidget.IsVisible = false;
				m_mountButtonWidget.IsChecked = true;
				m_mountButtonWidget.IsVisible = true;
				m_editItemButton.IsVisible = false;
			}
			else
			{
				m_mountButtonWidget.IsChecked = false;
				if (componentRider != null && Time.FrameStartTime - m_lastMountableCreatureSearchTime > 0.5)
				{
					m_lastMountableCreatureSearchTime = Time.FrameStartTime;
					if (componentRider.FindNearestMount() != null)
					{
						m_sneakButtonWidget.IsVisible = false;
						m_mountButtonWidget.IsVisible = true;
						m_editItemButton.IsVisible = false;
					}
					else
					{
						m_sneakButtonWidget.IsVisible = true;
						m_mountButtonWidget.IsVisible = false;
						m_editItemButton.IsVisible = false;
					}
				}
			}
			if (!m_componentPlayer.IsAddedToProject || m_componentPlayer.ComponentHealth.Health == 0f || componentSleep.IsSleeping || m_componentPlayer.ComponentSickness.IsPuking)
				ModalPanelWidget = null;
			m_componentPlayer.ComponentGui.HealthBarWidget.LitBarColor = m_componentPlayer.ComponentSickness.IsSick
				? new Color(166, 175, 103)
				: m_componentPlayer.ComponentFlu.HasFlu ? new Color(0, 48, 255) : new Color(224, 24, 0);
		}
	}
}