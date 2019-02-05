using Engine;
using Game;
using System;

namespace ZHCN
{
	public class EditAdjustableDelayGateDialog : Game.EditAdjustableDelayGateDialog
	{
		public EditAdjustableDelayGateDialog(int delay, Action<int> handler) : base(delay, handler)
		{
			UpdateControls();
		}

		public override void Update()
		{
			if (m_delaySlider.IsSliding)
				m_delay = (int)m_delaySlider.Value;
			if (m_minusButton.IsClicked)
				m_delay = MathUtils.Max(m_delay - 1, (int)m_delaySlider.MinValue);
			if (m_plusButton.IsClicked)
				m_delay = MathUtils.Min(m_delay + 1, (int)m_delaySlider.MaxValue);
			if (m_okButton.IsClicked)
				Dismiss(m_delay);
			if (Input.Cancel || m_cancelButton.IsClicked)
				Dismiss(null);
			UpdateControls();
		}

		public new void UpdateControls()
		{
			m_delaySlider.Value = m_delay;
			m_minusButton.IsEnabled = (m_delay > m_delaySlider.MinValue);
			m_plusButton.IsEnabled = (m_delay < m_delaySlider.MaxValue);
			m_delayLabel.Text = $"{(m_delay + 1) * 0.01f:0.00} 秒";
		}
	}

	public class EditBatteryDialog : Game.EditBatteryDialog
	{
		public EditBatteryDialog(int voltageLevel, Action<int> handler) : base(voltageLevel, handler)
		{
			UpdateControls();
		}

		public override void Update()
		{
			if (m_voltageSlider.IsSliding)
				m_voltageLevel = (int)m_voltageSlider.Value;
			if (m_okButton.IsClicked)
				Dismiss(m_voltageLevel);
			if (Input.Cancel || m_cancelButton.IsClicked)
				Dismiss(null);
			UpdateControls();
		}

		public new void UpdateControls()
		{
			m_voltageSlider.Text = string.Format("{0:0.0}V ({1})", new object[2]
			{
			1.5f * m_voltageLevel / 15f,
			m_voltageLevel < 8 ? "低电平" : "高电平"
			});
			m_voltageSlider.Value = m_voltageLevel;
		}
	}

	public class EditMemoryBankDialog : Game.EditMemoryBankDialog
	{
		public EditMemoryBankDialog(MemoryBankData memoryBankData, Action handler) : base(memoryBankData, handler)
		{
		}

		public override void Update()
		{
			m_ignoreTextChanges = true;
			try
			{
				string text = m_tmpMemoryBankData.SaveString(false);
				if (text.Length < 256)
				{
					text += new string('0', 256 - text.Length);
				}
				for (int i = 0; i < 16; i++)
					m_lineTextBoxes[i].Text = text.Substring(i * 16, 16);
				m_linearTextBox.Text = m_tmpMemoryBankData.SaveString(false);
			}
			finally
			{
				m_ignoreTextChanges = false;
			}
			if (m_linearPanel.IsVisible)
			{
				m_switchViewButton.Text = "表格";
				if (m_switchViewButton.IsClicked)
				{
					m_linearPanel.IsVisible = false;
					m_gridPanel.IsVisible = true;
				}
			}
			else
			{
				m_switchViewButton.Text = "行";
				if (m_switchViewButton.IsClicked)
				{
					m_linearPanel.IsVisible = true;
					m_gridPanel.IsVisible = false;
				}
			}
			if (m_okButton.IsClicked)
			{
				m_memoryBankData.Data = m_tmpMemoryBankData.Data;
				Dismiss(true);
			}
			if (Input.Cancel || m_cancelButton.IsClicked)
				Dismiss(false);
		}
	}

	public class EditTruthTableDialog : Game.EditTruthTableDialog
	{
		public EditTruthTableDialog(TruthTableData truthTableData, Action<bool> handler) : base(truthTableData, handler)
		{
		}

		public override void Update()
		{
			m_ignoreTextChanges = true;
			try
			{
				m_linearTextBox.Text = m_tmpTruthTableData.SaveBinaryString();
			}
			finally
			{
				m_ignoreTextChanges = false;
			}
			for (int i = 0; i < 16; i++)
			{
				if (m_lineCheckboxes[i].IsClicked)
					m_tmpTruthTableData.Data[i] = (byte)((m_tmpTruthTableData.Data[i] == 0) ? 15 : 0);
				m_lineCheckboxes[i].IsChecked = (m_tmpTruthTableData.Data[i] > 0);
			}
			if (m_linearPanel.IsVisible)
			{
				m_switchViewButton.Text = "表格";
				if (m_switchViewButton.IsClicked)
				{
					m_linearPanel.IsVisible = false;
					m_gridPanel.IsVisible = true;
				}
			}
			else
			{
				m_switchViewButton.Text = "行";
				if (m_switchViewButton.IsClicked)
				{
					m_linearPanel.IsVisible = true;
					m_gridPanel.IsVisible = false;
				}
			}
			if (m_okButton.IsClicked)
			{
				m_truthTableData.Data = m_tmpTruthTableData.Data;
				Dismiss(true);
			}
			if (Input.Cancel || m_cancelButton.IsClicked)
				Dismiss(false);
		}
	}

	public class SubsystemAdjustableDelayGateBlockBehavior : Game.SubsystemAdjustableDelayGateBlockBehavior
	{
		public override bool OnEditInventoryItem(IInventory inventory, int slotIndex, ComponentPlayer componentPlayer)
		{
			var c__DisplayClass2_ = new c__DisplayClass2_0
			{
				inventory = inventory,
				slotIndex = slotIndex
			};
			c__DisplayClass2_.value = c__DisplayClass2_.inventory.GetSlotValue(c__DisplayClass2_.slotIndex);
			c__DisplayClass2_.count = c__DisplayClass2_.inventory.GetSlotCount(c__DisplayClass2_.slotIndex);
			c__DisplayClass2_.data = Terrain.ExtractData(c__DisplayClass2_.value);
			int delay = AdjustableDelayGateBlock.GetDelay(c__DisplayClass2_.data);
			DialogsManager.ShowDialog(componentPlayer.View.GameWidget, new EditAdjustableDelayGateDialog(delay, c__DisplayClass2_.OnEditInventoryItem_b__0));
			return true;
		}

		public override bool OnEditBlock(int x, int y, int z, int value, ComponentPlayer componentPlayer)
		{
			var c__DisplayClass3_ = new c__DisplayClass3_0
			{
				__this = this,
				value = value,
				x = x,
				y = y,
				z = z
			};
			c__DisplayClass3_.data = Terrain.ExtractData(c__DisplayClass3_.value);
			int delay = AdjustableDelayGateBlock.GetDelay(c__DisplayClass3_.data);
			DialogsManager.ShowDialog(componentPlayer.View.GameWidget, new EditAdjustableDelayGateDialog(delay, c__DisplayClass3_.OnEditBlock_b__0));
			return true;
		}
	}

	public class SubsystemBatteryBlockBehavior : Game.SubsystemBatteryBlockBehavior
	{
		public override bool OnEditInventoryItem(IInventory inventory, int slotIndex, ComponentPlayer componentPlayer)
		{
			var c__DisplayClass2_ = new c__DisplayClass2_0
			{
				inventory = inventory,
				slotIndex = slotIndex
			};
			c__DisplayClass2_.value = c__DisplayClass2_.inventory.GetSlotValue(c__DisplayClass2_.slotIndex);
			c__DisplayClass2_.count = c__DisplayClass2_.inventory.GetSlotCount(c__DisplayClass2_.slotIndex);
			c__DisplayClass2_.data = Terrain.ExtractData(c__DisplayClass2_.value);
			int voltageLevel = BatteryBlock.GetVoltageLevel(c__DisplayClass2_.data);
			DialogsManager.ShowDialog(componentPlayer.View.GameWidget, new EditBatteryDialog(voltageLevel, c__DisplayClass2_.OnEditInventoryItem_b__0));
			return true;
		}

		public override bool OnEditBlock(int x, int y, int z, int value, ComponentPlayer componentPlayer)
		{
			var c__DisplayClass3_ = new c__DisplayClass3_0
			{
				__this = this,
				value = value,
				x = x,
				y = y,
				z = z
			};
			c__DisplayClass3_.data = Terrain.ExtractData(c__DisplayClass3_.value);
			int voltageLevel = BatteryBlock.GetVoltageLevel(c__DisplayClass3_.data);
			DialogsManager.ShowDialog(componentPlayer.View.GameWidget, new EditBatteryDialog(voltageLevel, c__DisplayClass3_.OnEditBlock_b__0));
			return true;
		}
	}

	public class SubsystemMemoryBankBlockBehavior : Game.SubsystemMemoryBankBlockBehavior
	{
		public override bool OnEditInventoryItem(IInventory inventory, int slotIndex, ComponentPlayer componentPlayer)
		{
			var c__DisplayClass3_ = new c__DisplayClass3_0
			{
				__this = this,
				inventory = inventory,
				slotIndex = slotIndex
			};
			c__DisplayClass3_.value = c__DisplayClass3_.inventory.GetSlotValue(c__DisplayClass3_.slotIndex);
			c__DisplayClass3_.count = c__DisplayClass3_.inventory.GetSlotCount(c__DisplayClass3_.slotIndex);
			int id = Terrain.ExtractData(c__DisplayClass3_.value);
			c__DisplayClass3_.memoryBankData = GetItemData(id);
			if (c__DisplayClass3_.memoryBankData != null)
				c__DisplayClass3_.memoryBankData = (MemoryBankData)c__DisplayClass3_.memoryBankData.Copy();
			else
				c__DisplayClass3_.memoryBankData = new MemoryBankData();
			DialogsManager.ShowDialog(componentPlayer.View.GameWidget, new EditMemoryBankDialog(c__DisplayClass3_.memoryBankData, c__DisplayClass3_.OnEditInventoryItem_b__0));
			return true;
		}

		public override bool OnEditBlock(int x, int y, int z, int value, ComponentPlayer componentPlayer)
		{
			var c__DisplayClass4_ = new c__DisplayClass4_0
			{
				__this = this,
				x = x,
				y = y,
				z = z,
				value = value
			};
			c__DisplayClass4_.memoryBankData = (GetBlockData(new Point3(c__DisplayClass4_.x, c__DisplayClass4_.y, c__DisplayClass4_.z)) ?? new MemoryBankData());
			DialogsManager.ShowDialog(componentPlayer.View.GameWidget, new EditMemoryBankDialog(c__DisplayClass4_.memoryBankData, c__DisplayClass4_.OnEditBlock_b__0));
			return true;
		}
	}

	public class SubsystemTruthTableCircuitBlockBehavior : Game.SubsystemTruthTableCircuitBlockBehavior
	{
		public override bool OnEditInventoryItem(IInventory inventory, int slotIndex, ComponentPlayer componentPlayer)
		{
			var c__DisplayClass3_ = new c__DisplayClass3_0
			{
				__this = this,
				inventory = inventory,
				slotIndex = slotIndex
			};
			c__DisplayClass3_.value = c__DisplayClass3_.inventory.GetSlotValue(c__DisplayClass3_.slotIndex);
			c__DisplayClass3_.count = c__DisplayClass3_.inventory.GetSlotCount(c__DisplayClass3_.slotIndex);
			int id = Terrain.ExtractData(c__DisplayClass3_.value);
			c__DisplayClass3_.truthTableData = GetItemData(id);
			if (c__DisplayClass3_.truthTableData != null)
				c__DisplayClass3_.truthTableData = (TruthTableData)c__DisplayClass3_.truthTableData.Copy();
			else
				c__DisplayClass3_.truthTableData = new TruthTableData();
			DialogsManager.ShowDialog(componentPlayer.View.GameWidget, new EditTruthTableDialog(c__DisplayClass3_.truthTableData, c__DisplayClass3_.OnEditInventoryItem_b__0));
			return true;
		}

		public override bool OnEditBlock(int x, int y, int z, int value, ComponentPlayer componentPlayer)
		{
			var c__DisplayClass4_ = new c__DisplayClass4_0
			{
				__this = this,
				x = x,
				y = y,
				z = z,
				value = value
			};
			c__DisplayClass4_.truthTableData = (GetBlockData(new Point3(c__DisplayClass4_.x, c__DisplayClass4_.y, c__DisplayClass4_.z)) ?? new TruthTableData());
			DialogsManager.ShowDialog(componentPlayer.View.GameWidget, new EditTruthTableDialog(c__DisplayClass4_.truthTableData, c__DisplayClass4_.OnEditBlock_b__0));
			return true;
		}
	}
}