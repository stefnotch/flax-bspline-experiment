using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using FlaxEngine;

namespace BezierCurve
{
	/// <summary>
	/// A B-Spline
	/// </summary>
	[ExecuteInEditMode]
	public class BSpline : Script
	{
		/// <summary>
		/// The prefab that will get spawned for every point of the B-Spline
		/// </summary>
		public Prefab PointActor { get; set; }

		/// <summary>
		/// Step size of the B-Spline
		/// </summary>
		public float Step = 0.01f;

		/// <summary>
		/// Material of the B-Spline
		/// </summary>
		public MaterialBase Material;

		// Could be replaced with an ObservableCollection 
		/// <summary>
		/// The points that define the B-Spline
		/// </summary>
		public Vector3[] Points = new Vector3[]
		{
			new Vector3(-100.0f,  0.0f, 0f),
			new Vector3(-50f,  50f, 0f),
			new Vector3(50f, -50f, 0f),
			new Vector3(100f,  0.0f, 0f),
		};

		/// <summary>
		/// Degree of the B-Spline
		/// </summary>
		public int Degree = 2;

		private Actor _pointsContainer;
		private Mesh _mesh;
		private void OnEnable()
		{
			// Create dynamic model with a single LOD with one mesh
			var model = Content.CreateVirtualAsset<Model>();
			model.SetupLODs(1);
			_mesh = model.LODs[0].Meshes[0];
			UpdateMesh(_mesh);

			// Create or reuse child model actor
			var childModel = Actor.GetOrAddChild<ModelActor>();
			childModel.Model = model;
			childModel.Entries[0].Material = Material;
			childModel.HideFlags = HideFlags.DontSelect;

			// Create a new container actor for the points
			if (!_pointsContainer)
			{
				_pointsContainer = New<EmptyActor>();
				Actor.AddChild(_pointsContainer, false);
				_pointsContainer.LocalPosition = Vector3.Zero;
				_pointsContainer.HideFlags = HideFlags.DontSave;
			}
			else
			{
				_pointsContainer.DestroyChildren();
			}
			SpawnPoints(_pointsContainer);
		}

		private void OnDisable()
		{
			// Clean up after yourself
			Destroy(ref _pointsContainer);
		}

		private void Update()
		{
			// Here you can add code that needs to be called every frame
			if (_mesh != null)
			{
				UpdatePoints();
				UpdateMesh(_mesh);
			}
		}

		private void SpawnPoints(Actor parent)
		{
			if (PointActor)
			{
				foreach (Vector3 point in Points)
				{
					var spawnedPoint = PrefabManager.SpawnPrefab(PointActor, parent);
					spawnedPoint.LocalPosition = point;
				}
			}
		}

		private void UpdatePoints()
		{
			if (_pointsContainer)
			{
				if (_pointsContainer.ChildrenCount >= Points.Length)
				{
					for (int i = 0; i < Points.Length; i++)
					{
						Points[i] = _pointsContainer.GetChild(i).LocalPosition;
					}
				}
			}
		}

		private void UpdateMesh(Mesh mesh)
		{
			if (mesh == null) return;

			float[] knots = new float[] { 0, 0, 0, 1, 2, 2, 2 };

			// 3 points => 1 triangle
			// 2 triangles => 1 quad
			// 4 quads ==> one "line-segment"
			// 3*2*4 = 24
			int len = (int)Math.Floor(1 / Step) * 24;

			Vector3[] vertices = new Vector3[len];
			int[] triangles = new int[len];

			Vector3 lastPoint = GetBSplinePoint(0, Degree, Points, knots);
			int counter = 0;
			for (float t = Step; t < 1; t += Step)
			{
				Vector3 point = GetBSplinePoint(t, Degree, Points, knots);

				Create3DLine(ref counter, ref lastPoint, ref point, vertices, triangles);
				//DebugDraw.DrawSphere(point + Actor.Position, 1f, Color.Red);
				lastPoint = point;
			}

			mesh.UpdateMesh(vertices, triangles);
		}

		private void Create3DLine(ref int counter, ref Vector3 lastPoint, ref Vector3 point, Vector3[] vertices, int[] triangles)
		{
			float radius = 1f;
			Vector3 line = Vector3.Normalize(point - lastPoint);
			// Upwards offset
			Vector3 offset1 = Vector3.Cross(line, Vector3.Up) * radius;
			// Right-offset
			Vector3 offset2 = Vector3.Cross(line, offset1) * radius;

			// The rectangle-points on one end
			Vector3[] startPoints = new Vector3[]
			{
				point + offset1,
				point + offset2,
				point - offset1,
				point - offset2
			};

			// The rectangle-points on the other end
			Vector3[] endPoints = new Vector3[]
			{
				lastPoint + offset1,
				lastPoint + offset2,
				lastPoint - offset1,
				lastPoint - offset2
			};

			// 3 points => 1 triangle
			// 2 triangles => 1 quad
			// 4 quads ==> one "line-segment"
			// 3*2*4 = 24
			for (int i = 0; i < 4; i++)
			{
				// 1st triangle
				vertices[counter] = startPoints[i];
				vertices[counter + 1] = endPoints[(i + 1) % 4];
				vertices[counter + 2] = startPoints[(i + 1) % 4];

				for (int j = 0; j < 3; j++)
				{
					triangles[counter] = counter;
					counter++;
				}

				// 2nd triangle
				vertices[counter] = startPoints[i];
				vertices[counter + 1] = endPoints[i];
				vertices[counter + 2] = endPoints[(i + 1) % 4];
				for (int j = 0; j < 3; j++)
				{
					triangles[counter] = counter;
					counter++;
				}
			}

		}

		private Vector3 GetBSplinePoint(float t, int degree, Vector3[] points, float[] knots = null, float[] weights = null/*, result*/)
		{
			/*
 The MIT License (MIT)

Copyright (c) 2015 Thibaut Séguy <thibaut.seguy@gmail.com>

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in
all copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
THE SOFTWARE.
 */

			var n = points.Length;    // points count

			if (degree < 1) throw new ArgumentException("degree must be at least 1 (linear)");
			if (degree > (n - 1)) throw new ArgumentException("degree must be less than or equal to point count - 1");

			if (weights == null)
			{
				// build weight vector of length [n]
				weights = new float[n];
				for (int i = 0; i < n; i++)
				{
					weights[i] = 1;
				}
			}

			if (knots == null)
			{
				// build knot vector of length [n + degree + 1]
				knots = new float[n + degree + 1];
				for (int i = 0; i < n + degree + 1; i++)
				{
					knots[i] = i;
				}
			}
			else
			{
				if (knots.Length != n + degree + 1) throw new ArgumentException("bad knot vector length");
			}


			// remap t to the domain where the spline is defined
			var low = knots[degree];
			var high = knots[knots.Length - 1 - degree];
			t = t * (high - low) + low;

			if (t < low || t > high) throw new ArgumentException("out of bounds");
			int s = 0;
			// find s (the spline segment) for the [t] value provided
			for (s = degree; s < knots.Length - 1 - degree; s++)
			{
				if (t >= knots[s] && t <= knots[s + 1])
				{
					break;
				}
			}

			// convert points to homogeneous coordinates
			var v = new Vector3[n];
			var vd = new float[n];
			for (int i = 0; i < n; i++)
			{
				v[i] = points[i] * weights[i];
				vd[i] = weights[i];
			}

			// l (level) goes from 1 to the curve degree + 1
			for (int l = 1; l <= degree + 1; l++)
			{
				// build level l of the pyramid
				for (int i = s; i > s - degree - 1 + l; i--)
				{
					float alpha = (t - knots[i]) / (knots[i + degree + 1 - l] - knots[i]);

					// interpolate each component
					v[i] = Vector3.Lerp(v[i - 1], v[i], alpha);

					vd[i] = Mathf.LerpUnclamped(vd[i - 1], vd[i], alpha);

				}
			}

			// convert back to cartesian and return
			return v[s] / vd[s];
		}
	}
}
