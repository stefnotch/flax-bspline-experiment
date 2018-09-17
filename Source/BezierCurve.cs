using System;
using System.Collections.Generic;
using FlaxEngine;

namespace BezierCurve
{
	public class BezierCurve
	{
		public Vector3 Start = Vector3.Zero;
		public Vector3 Middle1 = Vector3.Zero;
		public Vector3 Middle2 = Vector3.Zero;
		public Vector3 End = Vector3.Zero;

		public void Compute()
		{
			Vector3 uVector = Vector3.Zero;
			const float Increment = 0.0001f;

			for (float u = 0; u <= 1; u += Increment)
			{
				float a = Mathf.Pow(1 - u, 3);
				float b = Mathf.Pow(1 - u, 2) * 3 * u;
				float c = Mathf.Pow(u, 2) * 3 * (1 - u);
				float d = Mathf.Pow(u, 3);
				uVector = (a * Start) + (b * Middle1) + (c * Middle2) + (d * End);

				//Draw uVector (point)
			}
		}

		public void DebugDraw(Color color, float thickness = 1.0f)
		{
			EditorDrawCallCollector.DrawCalls.Push((transform) =>
			{
				Vector3 p1Screen = transform.WorldToLocal(Start);
				Vector3 p2Screen = transform.WorldToLocal(Middle1);
				Vector3 p3Screen = transform.WorldToLocal(Middle2);
				Vector3 p4Screen = transform.WorldToLocal(End);

				Render2D.DrawBezier(new Vector2(p1Screen), new Vector2(p2Screen), new Vector2(p3Screen), new Vector2(p4Screen), color, thickness);
			});

		}
	}
}
