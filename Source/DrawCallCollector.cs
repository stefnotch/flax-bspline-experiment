using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FlaxEngine;

namespace BezierCurve
{
	public static class EditorDrawCallCollector
	{
		//TODO: Use RenderView
		public static readonly Stack<Action<Transform>> DrawCalls = new Stack<Action<Transform>>();
	}
}
