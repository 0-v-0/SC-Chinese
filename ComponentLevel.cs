using Engine;
using Game;
using System;
using System.Collections.Generic;

namespace ZHCN
{
	public class ComponentLevel : Game.ComponentLevel, IUpdateable
	{
		public new float CalculateStrengthFactor(ICollection<Factor> factors)
		{
			float num = (m_componentPlayer.PlayerData.PlayerClass == PlayerClass.Female) ? 0.8f : 1f;
			float num2 = 1f * num;
			Factor item;
			if (factors != null)
			{
				item = new Factor
				{
					Value = num,
					Description = m_componentPlayer.PlayerData.PlayerClass.ToString()
				};
				factors.Add(item);
			}
			float level = m_componentPlayer.PlayerData.Level;
			float num3 = 1f + 0.05f * MathUtils.Floor(MathUtils.Clamp(level, 1f, 21f) - 1f);
			float num4 = num2 * num3;
			if (factors != null)
			{
				item = new Factor
				{
					Value = num3,
					Description = "等级 " + MathUtils.Floor(level).ToString()
				};
				factors.Add(item);
			}
			float stamina = m_componentPlayer.ComponentVitalStats.Stamina;
			float num5 = MathUtils.Lerp(0.5f, 1f, MathUtils.Saturate(4f * stamina)) * MathUtils.Lerp(0.9f, 1f, MathUtils.Saturate(stamina));
			float num6 = num4 * num5;
			if (factors != null)
			{
				item = new Factor
				{
					Value = num5,
					Description = $"{stamina * 100f:0}% 耐力"
				};
				factors.Add(item);
			}
			float num7 = m_componentPlayer.ComponentSickness.IsSick ? 0.75f : 1f;
			float num8 = num6 * num7;
			if (factors != null)
			{
				item = new Factor
				{
					Value = num7,
					Description = m_componentPlayer.ComponentSickness.IsSick ? "疾病" : "无疾病"
				};
				factors.Add(item);
			}
			float num9 = (!m_componentPlayer.ComponentSickness.IsPuking) ? 1 : 0;
			float num10 = num8 * num9;
			if (factors != null)
			{
				item = new Factor
				{
					Value = num9,
					Description = m_componentPlayer.ComponentSickness.IsPuking ? "呕吐" : "无呕吐"
				};
				factors.Add(item);
			}
			float num11 = m_componentPlayer.ComponentFlu.HasFlu ? 0.75f : 1f;
			float num12 = num10 * num11;
			if (factors != null)
			{
				item = new Factor
				{
					Value = num11,
					Description = m_componentPlayer.ComponentFlu.HasFlu ? "感冒" : "无感冒"
				};
				factors.Add(item);
			}
			float num13 = (!m_componentPlayer.ComponentFlu.IsCoughing) ? 1 : 0;
			float num14 = num12 * num13;
			if (factors != null)
			{
				item = new Factor
				{
					Value = num13,
					Description = m_componentPlayer.ComponentFlu.IsCoughing ? "咳嗽" : "无咳嗽"
				};
				factors.Add(item);
			}
			float num15 = (m_subsystemGameInfo.WorldSettings.GameMode == GameMode.Harmless) ? 1.25f : 1f;
			float result = num14 * num15;
			if (factors != null)
			{
				item = new Factor
				{
					Value = num15,
					Description = m_subsystemGameInfo.WorldSettings.GameMode.ToString() + " 模式"
				};
				factors.Add(item);
			}
			return result;
		}

		public new float CalculateResilienceFactor(ICollection<Factor> factors)
		{
			float num = (m_componentPlayer.PlayerData.PlayerClass == PlayerClass.Female) ? 0.8f : 1f;
			float num2 = 1f * num;
			Factor item;
			if (factors != null)
			{
				item = new Factor
				{
					Value = num,
					Description = m_componentPlayer.PlayerData.PlayerClass.ToString()
				};
				factors.Add(item);
			}
			float level = m_componentPlayer.PlayerData.Level;
			float num3 = 1f + 0.05f * MathUtils.Floor(MathUtils.Clamp(level, 1f, 21f) - 1f);

			float num4 = num2 * num3;
			if (factors != null)
			{
				item = new Factor
				{
					Value = num3,
					Description = "等级 " + MathUtils.Floor(level).ToString()
				};
				factors.Add(item);
			}
			float num5 = m_componentPlayer.ComponentSickness.IsSick ? 0.75f : 1f;
			float num6 = num4 * num5;
			if (factors != null)
			{
				item = new Factor
				{
					Value = num5,
					Description = m_componentPlayer.ComponentSickness.IsSick ? "疾病" : "无疾病"
				};
				factors.Add(item);
			}
			float num7 = m_componentPlayer.ComponentFlu.HasFlu ? 0.75f : 1f;
			float num8 = num6 * num7;
			if (factors != null)
			{
				item = new Factor
				{
					Value = num7,
					Description = m_componentPlayer.ComponentFlu.HasFlu ? "感冒" : "无感冒"
				};
				factors.Add(item);
			}
			float num9 = 1f;
			if (m_subsystemGameInfo.WorldSettings.GameMode == GameMode.Harmless)
			{
				num9 = 1.5f;
			}
			if (m_subsystemGameInfo.WorldSettings.GameMode == GameMode.Creative)
			{
				num9 = float.PositiveInfinity;
			}
			float result = num8 * num9;
			if (factors != null)
			{
				item = new Factor
				{
					Value = num9,
					Description = m_subsystemGameInfo.WorldSettings.GameMode.ToString() + " 模式"
				};
				factors.Add(item);
			}
			return result;
		}

		public new float CalculateSpeedFactor(ICollection<Factor> factors)
		{
			float num = 1f;
			float num2 = (m_componentPlayer.PlayerData.PlayerClass == PlayerClass.Female) ? 1.05f : 1f;
			num *= num2;
			Factor item;
			if (factors != null)
			{
				item = new Factor
				{
					Value = num2,
					Description = m_componentPlayer.PlayerData.PlayerClass.ToString()
				};
				factors.Add(item);
			}
			float level = m_componentPlayer.PlayerData.Level;
			float num3 = 1f + 0.02f * MathUtils.Floor(MathUtils.Clamp(level, 1f, 21f) - 1f);
			num *= num3;
			if (factors != null)
			{
				item = new Factor
				{
					Value = num3,
					Description = "等级 " + MathUtils.Floor(level).ToString()
				};
				factors.Add(item);
			}
			float clothingFactor = 1f;
			ReadOnlyList<int> clothes = m_componentPlayer.ComponentClothing.GetClothes(ClothingSlot.Head);
			ReadOnlyList<int>.Enumerator enumerator = clothes.GetEnumerator();
			try
			{
				while (enumerator.MoveNext())
				{
					AddClothingFactor(enumerator.Current, ref clothingFactor, factors);
				}
			}
			finally
			{
				((IDisposable)enumerator).Dispose();
			}
			clothes = m_componentPlayer.ComponentClothing.GetClothes(ClothingSlot.Torso);
			enumerator = clothes.GetEnumerator();
			try
			{
				while (enumerator.MoveNext())
				{
					AddClothingFactor(enumerator.Current, ref clothingFactor, factors);
				}
			}
			finally
			{
				((IDisposable)enumerator).Dispose();
			}
			clothes = m_componentPlayer.ComponentClothing.GetClothes(ClothingSlot.Legs);
			enumerator = clothes.GetEnumerator();
			try
			{
				while (enumerator.MoveNext())
				{
					AddClothingFactor(enumerator.Current, ref clothingFactor, factors);
				}
			}
			finally
			{
				((IDisposable)enumerator).Dispose();
			}
			clothes = m_componentPlayer.ComponentClothing.GetClothes(ClothingSlot.Feet);
			enumerator = clothes.GetEnumerator();
			try
			{
				while (enumerator.MoveNext())
				{
					AddClothingFactor(enumerator.Current, ref clothingFactor, factors);
				}
			}
			finally
			{
				((IDisposable)enumerator).Dispose();
			}
			num *= clothingFactor;
			float stamina = m_componentPlayer.ComponentVitalStats.Stamina;
			float num4 = MathUtils.Lerp(0.5f, 1f, MathUtils.Saturate(4f * stamina)) * MathUtils.Lerp(0.9f, 1f, MathUtils.Saturate(stamina));
			num *= num4;
			if (factors != null)
			{
				item = new Factor
				{
					Value = num4,
					Description = $"{stamina * 100f:0}% 耐力"
				};
				factors.Add(item);
			}
			float num5 = m_componentPlayer.ComponentSickness.IsSick ? 0.75f : 1f;
			num *= num5;
			if (factors != null)
			{
				item = new Factor
				{
					Value = num5,
					Description = m_componentPlayer.ComponentSickness.IsSick ? "疾病" : "无疾病"
				};
				factors.Add(item);
			}
			float num6 = (!m_componentPlayer.ComponentSickness.IsPuking) ? 1 : 0;
			num *= num6;
			if (factors != null)
			{
				item = new Factor
				{
					Value = num6,
					Description = m_componentPlayer.ComponentSickness.IsPuking ? "呕吐" : "无呕吐"
				};
				factors.Add(item);
			}
			float num7 = m_componentPlayer.ComponentFlu.HasFlu ? 0.75f : 1f;
			num *= num7;
			if (factors != null)
			{
				item = new Factor
				{
					Value = num7,
					Description = m_componentPlayer.ComponentFlu.HasFlu ? "感冒" : "无感冒"
				};
				factors.Add(item);
			}
			float num8 = (!m_componentPlayer.ComponentFlu.IsCoughing) ? 1 : 0;
			num *= num8;
			if (factors != null)
			{
				item = new Factor
				{
					Value = num8,
					Description = m_componentPlayer.ComponentFlu.IsCoughing ? "咳嗽" : "无咳嗽"
				};
				factors.Add(item);
			}
			return num;
		}

		public new float CalculateHungerFactor(ICollection<Factor> factors)
		{
			float num = (m_componentPlayer.PlayerData.PlayerClass == PlayerClass.Female) ? 0.7f : 1f;
			float num2 = 1f * num;
			Factor item;
			if (factors != null)
			{
				item = new Factor
				{
					Value = num,
					Description = m_componentPlayer.PlayerData.PlayerClass.ToString()
				};
				factors.Add(item);
			}
			float level = m_componentPlayer.PlayerData.Level;
			float num3 = 1f - 0.01f * MathUtils.Floor(MathUtils.Clamp(level, 1f, 21f) - 1f);
			float num4 = num2 * num3;
			if (factors != null)
			{
				item = new Factor
				{
					Value = num3,
					Description = "等级 " + MathUtils.Floor(level).ToString()
				};
				factors.Add(item);
			}
			float num5 = 1f;
			if (m_subsystemGameInfo.WorldSettings.GameMode == GameMode.Harmless)
			{
				num5 = 0.66f;
			}
			if (m_subsystemGameInfo.WorldSettings.GameMode == GameMode.Creative)
			{
				num5 = 0f;
			}
			float result = num4 * num5;
			if (factors != null)
			{
				item = new Factor
				{
					Value = num5,
					Description = m_subsystemGameInfo.WorldSettings.GameMode.ToString() + " 模式"
				};
				factors.Add(item);
			}
			return result;
		}

		public new void Update(float dt)
		{
			if (m_subsystemTime.PeriodicGameTimeEvent(180.0, 179.0))
				AddExperience(1, false);
			StrengthFactor = CalculateStrengthFactor(null);
			SpeedFactor = CalculateSpeedFactor(null);
			HungerFactor = CalculateHungerFactor(null);
			ResilienceFactor = CalculateResilienceFactor(null);
			if (!m_lastLevelTextValue.HasValue || m_lastLevelTextValue.Value != MathUtils.Floor(m_componentPlayer.PlayerData.Level))
			{
				m_componentPlayer.ComponentGui.LevelLabelWidget.Text = "等级 " + MathUtils.Floor(m_componentPlayer.PlayerData.Level).ToString();
				m_lastLevelTextValue = MathUtils.Floor(m_componentPlayer.PlayerData.Level);
			}
			m_componentPlayer.PlayerStats.HighestLevel = MathUtils.Max(m_componentPlayer.PlayerStats.HighestLevel, m_componentPlayer.PlayerData.Level);
		}
	}
	public class ComponentSleep : Game.ComponentSleep, IUpdateable
	{
		public new void Update(float dt)
		{
			if (IsSleeping && m_componentPlayer.ComponentHealth.Health > 0f)
			{
				m_sleepFactor = MathUtils.Min(m_sleepFactor + 0.33f * Time.FrameDuration, 1f);
				m_minWetness = MathUtils.Min(m_minWetness, m_componentPlayer.ComponentVitalStats.Wetness);
				m_componentPlayer.PlayerStats.TimeSlept += m_subsystemGameInfo.TotalElapsedGameTimeDelta;
				if ((m_componentPlayer.ComponentVitalStats.Sleep >= 1f || m_subsystemGameInfo.WorldSettings.GameMode == GameMode.Creative) && m_subsystemTimeOfDay.TimeOfDay > 0.3f && m_subsystemTimeOfDay.TimeOfDay < 0.599999964f && m_sleepStartTime.HasValue && m_subsystemGameInfo.TotalElapsedGameTime > m_sleepStartTime + 180.0)
					WakeUp();
				if (m_componentPlayer.ComponentHealth.HealthChange < 0f && (m_componentPlayer.ComponentHealth.Health < 0.5f || m_componentPlayer.ComponentVitalStats.Sleep > 0.5f))
					WakeUp();
				if (m_componentPlayer.ComponentVitalStats.Wetness > m_minWetness + 0.05f && m_componentPlayer.ComponentVitalStats.Sleep > 0.2f)
				{
					WakeUp();
					m_subsystemTime.QueueGameTimeDelayedExecution(m_subsystemTime.GameTime + 1.0, Update_b__21_0);
				}
				if (m_sleepStartTime.HasValue)
				{
					float num = (float)(m_subsystemGameInfo.TotalElapsedGameTime - m_sleepStartTime.Value);
					if (m_allowManualWakeUp && num > 10f)
					{
						if (m_componentPlayer.View.Input.Any && !DialogsManager.HasDialogs(m_componentPlayer.View.GameWidget))
						{
							m_componentPlayer.View.Input.Clear();
							WakeUp();
							m_subsystemTime.QueueGameTimeDelayedExecution(m_subsystemTime.GameTime + 2.0, Update_b__21_1);
						}
						m_messageFactor = MathUtils.Min(m_messageFactor + 0.5f * Time.FrameDuration, 1f);
						m_componentPlayer.ComponentScreenOverlays.Message = "轻触以起床";
						m_componentPlayer.ComponentScreenOverlays.MessageFactor = m_messageFactor;
					}
					if (!m_allowManualWakeUp && num > 5f)
					{
						m_messageFactor = MathUtils.Min(m_messageFactor + 1f * Time.FrameDuration, 1f);
						m_componentPlayer.ComponentScreenOverlays.Message = "你睡眠不足而不省人事";
						m_componentPlayer.ComponentScreenOverlays.MessageFactor = m_messageFactor;
					}
				}
			}
			else
				m_sleepFactor = MathUtils.Max(m_sleepFactor - 1f * Time.FrameDuration, 0f);
			m_componentPlayer.ComponentScreenOverlays.BlackoutFactor = MathUtils.Max(m_componentPlayer.ComponentScreenOverlays.BlackoutFactor, m_sleepFactor);
			if (m_sleepFactor > 0.01f)
			{
				m_componentPlayer.ComponentScreenOverlays.FloatingMessage = "Zzz...";
				m_componentPlayer.ComponentScreenOverlays.FloatingMessageFactor = MathUtils.Saturate(10f * (m_sleepFactor - 0.9f));
			}
		}
	}
}