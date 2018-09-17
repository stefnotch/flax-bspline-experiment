using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FlaxEditor.Viewport;
using FlaxEngine;
using FlaxEngine.GUI;

namespace BezierCurve.Editor
{
	public class EditorWindowRenderer : Control
	{
		public static EditorWindowRenderer Instance { get; private set; }
		//public Camera Camera { get; set; }
		public MainEditorGizmoViewport Transform { get; set; }

		public EditorWindowRenderer()
		{
			Instance = this;
		}

		public override void Draw()
		{
			base.Draw();
			if (Transform != null)
			{
				while (EditorDrawCallCollector.DrawCalls.Count > 0)
				{
					var drawAction = EditorDrawCallCollector.DrawCalls.Pop();
					drawAction.Invoke(Transform.ViewTransform);
				}
			}
		}

		public override void OnParentResized(ref Vector2 oldSize)
		{
			Size = Parent.Size;
		}
		public override void OnGotFocus()
		{
			Parent.Focus();
		}

		// To make this control a click-through control:
		public override bool IntersectsContent(ref Vector2 locationParent, out Vector2 location)
		{
			location = Vector2.Zero;
			return false;
		}

		public override void Dispose()
		{
			base.Dispose();
		}
	}
}
