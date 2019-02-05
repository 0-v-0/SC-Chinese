using Engine;
using Engine.Graphics;
using Game;
using System.Collections.Generic;
using System.Linq;

namespace ZHCN
{
	public class SubsystemSignBlockBehavior : Game.SubsystemSignBlockBehavior, IUpdateable
	{
		public new void RenderText(FontBatch2D fontBatch, FlatBatch2D flatBatch, TextData textData)
		{
			if (textData.TextureLocation.HasValue)
			{
				var list = new List<string>();
				var list2 = new List<Color>();
				for (int i = 0; i < textData.Lines.Length; i++)
				{
					if (!string.IsNullOrEmpty(textData.Lines[i]))
					{
						list.Add(textData.Lines[i].Replace("\\", "").ToUpper());
						list2.Add(textData.Colors[i]);
					}
				}
				if (list.Count > 0)
				{
					float num = list.Max((string l) => l.Length) * 16;
					float num2 = list.Count * 26;
					float num3 = 4f;
					float num4;
					float num5;
					if (num / num2 < num3)
					{
						num4 = num2 * num3;
						num5 = num2;
					}
					else
					{
						num4 = num;
						num5 = num / num3;
					}
					bool flag = !string.IsNullOrEmpty(textData.Url);
					for (int j = 0; j < list.Count; j++)
					{
						Vector2 vector = new Vector2(num4 / 2f, j * 26 + textData.TextureLocation.Value * 32 + (num5 - num2) / 2f);
						fontBatch.QueueText(list[j], vector, 0f, flag ? new Color(0, 0, 64) : list2[j], TextAnchor.HorizontalCenter);
						if (flag)
						{
							flatBatch.QueueLine(new Vector2(vector.X - list[j].Length * 26 / 2, vector.Y + 26f), new Vector2(vector.X + list[j].Length * 26 / 2, vector.Y + 26f), 0f, new Color(0, 0, 64));
						}
					}
					textData.UsedTextureWidth = num4;
					textData.UsedTextureHeight = num5;
				}
			}
		}

		public new void Update(float dt)
		{
			bool flag = false;
			foreach (View view in m_subsystemViews.Views)
			{
				bool flag2 = false;
				foreach (Vector3 lastUpdatePosition in m_lastUpdatePositions)
				{
					if (Vector3.DistanceSquared(view.ActiveCamera.ViewPosition, lastUpdatePosition) < 4f)
					{
						flag2 = true;
						break;
					}
				}
				if (!flag2)
				{
					flag = true;
					break;
				}
			}
			if (!flag)
				return;
			m_lastUpdatePositions.Clear();
			m_lastUpdatePositions.AddRange(m_subsystemViews.Views.Select(c._.UpdateRenderTarget_b__40_0));
			m_nearTexts.Clear();
			foreach (TextData value in m_textsByPoint.Values)
			{
				Point3 point = value.Point;
				float num = m_subsystemViews.CalculateSquaredDistanceFromNearestView(new Vector3(point));
				if (num <= 400f)
				{
					value.Distance = num;
					m_nearTexts.Add(value);
				}
			}
			m_nearTexts.Sort(c._.UpdateRenderTarget_b__40_1);
			if (m_nearTexts.Count > 32)
				m_nearTexts.RemoveRange(32, m_nearTexts.Count - 32);
			foreach (TextData nearText in m_nearTexts)
			{
				nearText.ToBeRenderedFrame = Time.FrameIndex;
			}
			bool flag3 = false;
			for (int i = 0; i < MathUtils.Min(m_nearTexts.Count, 32); i++)
			{
				TextData textData = m_nearTexts[i];
				if (textData.TextureLocation.HasValue)
					continue;
				int num2 = m_textureLocations.FirstIndex(c._.UpdateRenderTarget_b__40_2);
				if (num2 < 0)
					num2 = m_textureLocations.FirstIndex(c._.UpdateRenderTarget_b__40_3);
				if (num2 >= 0)
				{
					TextData textData2 = m_textureLocations[num2];
					if (textData2 != null)
					{
						textData2.TextureLocation = null;
						m_textureLocations[num2] = null;
					}
					m_textureLocations[num2] = textData;
					textData.TextureLocation = num2;
					flag3 = true;
				}
			}
			if (flag3)
			{
				RenderTarget2D renderTarget = Display.RenderTarget;
				Display.RenderTarget = m_renderTarget;
				try
				{
					Display.Clear(new Vector4(Color.Transparent));
					FlatBatch2D flatBatch = m_primitivesRenderer2D.FlatBatch(0, DepthStencilState.None, null, BlendState.Opaque);
					FontBatch2D fontBatch = m_primitivesRenderer2D.FontBatch(m_font, 1, DepthStencilState.None, null, BlendState.Opaque, SamplerState.PointClamp);
					for (int j = 0; j < m_textureLocations.Length; j++)
					{
						TextData textData3 = m_textureLocations[j];
						if (textData3 != null)
							RenderText(fontBatch, flatBatch, textData3);
					}
					m_primitivesRenderer2D.Flush();
				}
				finally
				{
					Display.RenderTarget = renderTarget;
				}
			}
		}
	}
}