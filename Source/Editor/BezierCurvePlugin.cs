using System;
using System.Collections.Generic;
using FlaxEditor;
using FlaxEngine;
using FlaxEngine.GUI;
using FlaxEngine.Rendering;

namespace BezierCurve.Editor
{
	public class BezierCurvePlugin : EditorPlugin
	{
		public override PluginDescription Description => new PluginDescription
		{
			Name = "Bezier Curve Plugin",
			Category = "Other",
			Author = "Stefnotch",
			AuthorUrl = null,
			HomepageUrl = null,
			//RepositoryUrl = "https://github.com/FlaxEngine/ExamplePlugin",
			Description = "This is an example plugin project.",
			Version = new Version(1, 0),
			SupportedPlatforms = new[]
			{
				PlatformType.Windows,
				PlatformType.WindowsStore,
				PlatformType.XboxOne,
			},
			IsAlpha = false,
			IsBeta = false,
		};

		private EditorWindowRenderer _rendererControl;

		public override void Initialize()
		{
			base.Initialize();
		}

		public override void InitializeEditor()
		{
			base.InitializeEditor();
			_rendererControl = Editor.Windows.EditWin.Viewport.AddChild<EditorWindowRenderer>();
			_rendererControl.Size = _rendererControl.Parent.Size;
			_rendererControl.Enabled = false;
			_rendererControl.Transform =
				Editor.Windows.EditWin.Viewport;
			//Camera.MainCamera;//Editor.Windows.EditWin.Viewport.Task.Camera;
			//Debug.Log(Editor.Windows.EditWin.Viewport.Task.Camera);

		}

		/// <inheritdoc />
		public override void Deinitialize()
		{
			if (_rendererControl != null)
			{
				_rendererControl.Dispose();
				_rendererControl = null;
			}

			base.Deinitialize();
		}
	}
}
