using System;
using System.Collections.Generic;
using FlaxEngine;

namespace BezierCurve
{
	[ExecuteInEditMode]
	public class Test : Script
	{
		public BezierCurve Curve = new BezierCurve();
		private void Start()
		{
			Curve.Start = Vector3.Zero;
			Curve.End = Vector3.One * 100f;
			// Here you can add code that needs to be called when script is created
		}

		private void Update()
		{
			// Here you can add code that needs to be called every frame
		}

		private void OnDebugDrawSelected()
		{
			Curve.DebugDraw(Color.Red);
		}
	}
}
