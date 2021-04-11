using System;
using System.Drawing;
using System.Windows.Forms;

namespace Game
{
	public class ScaledViewPanel : Panel
	{
		//private readonly ScenePainter painter;
		private GameModel GameModel;

        public ScaledViewPanel(GameModel gameModel):this() => GameModel = gameModel;

        private PointF centerLogicalPos;
		private bool dragInProgress;
		private Point dragStart;
		private PointF dragStartCenter;
		private PointF mouseLogicalPos;
		private float zoomScale;
		private readonly int tileSize = 70;
		private readonly int whiteSpace = 10;
		private readonly float tileBorderWidth = 0.5f;

		public ScaledViewPanel()
		{
			FitToWindow = true;
			zoomScale = 1f;
		}

		public PointF MouseLogicalPos => mouseLogicalPos;

		public PointF CenterLogicalPos
		{
			get { return centerLogicalPos; }
			set
			{
				centerLogicalPos = value;
				FitToWindow = false;
			}
		}

		public float ZoomScale
		{
			get { return zoomScale; }
			set
			{
				zoomScale = Math.Min(1000f, Math.Max(0.001f, value));
				FitToWindow = false;
			}
		}

		public bool FitToWindow { get; set; }

		protected override void InitLayout()
		{
			base.InitLayout();
			ResizeRedraw = true;
			DoubleBuffered = true;
		}

		protected override void OnMouseClick(MouseEventArgs e)
		{
			base.OnMouseClick(e);
			if (e.Button == MouseButtons.Middle)
				FitToWindow = true;
		}

		protected override void OnMouseDown(MouseEventArgs e)
		{
			base.OnMouseDown(e);
			if (e.Button == MouseButtons.Right)
			{
				dragInProgress = true;
				dragStart = e.Location;
				dragStartCenter = CenterLogicalPos;
			}
			else if (e.Button == MouseButtons.Left)
			{
				GameModel.OnMouseDown(Point.Truncate(mouseLogicalPos));
			}
		}

		protected override void OnMouseUp(MouseEventArgs e)
		{
			base.OnMouseUp(e);
			dragInProgress = false;

		}

		protected override void OnMouseMove(MouseEventArgs e)
		{
			base.OnMouseMove(e);
			mouseLogicalPos = ToLogical(e.Location);
			if (dragInProgress)
			{
				var loc = e.Location;
				var dx = (loc.X - dragStart.X) / ZoomScale;
				var dy = (loc.Y - dragStart.Y) / ZoomScale;
				CenterLogicalPos = new PointF(dragStartCenter.X - dx, dragStartCenter.Y - dy);
				Invalidate();
			}
		}

		protected override void OnMouseWheel(MouseEventArgs e)
		{
			base.OnMouseWheel(e);
			const float zoomChangeStep = 1.1f;
			if (e.Delta > 0)
				ZoomScale *= zoomChangeStep;
			if (e.Delta < 0)
				ZoomScale /= zoomChangeStep;
			Invalidate();
		}

		private PointF ToLogical(Point p)
		{
			var shift = GetShift();
			return new PointF(
				(p.X - shift.X) / zoomScale,
				(p.Y - shift.Y) / zoomScale);
		}

		private PointF GetShift()
		{
			return new PointF(
				ClientSize.Width / 2f - CenterLogicalPos.X * ZoomScale,
				ClientSize.Height / 2f - CenterLogicalPos.Y * ZoomScale);
		}

		protected override void OnPaint(PaintEventArgs e)
		{
			base.OnPaint(e);
			e.Graphics.Clear(Color.Black);
			var sceneSize = new SizeF(800,800);
			if (FitToWindow)
			{
				var vMargin = sceneSize.Height * ClientSize.Width < ClientSize.Height * sceneSize.Width;
				zoomScale = vMargin
					? ClientSize.Width / sceneSize.Width
					: ClientSize.Height / sceneSize.Height;
				centerLogicalPos = new PointF(sceneSize.Width / 2, sceneSize.Height / 2);
			}

			var shift = GetShift();
			e.Graphics.ResetTransform();
			e.Graphics.TranslateTransform(shift.X, shift.Y);
			e.Graphics.ScaleTransform(ZoomScale, ZoomScale);
			var g = e.Graphics;
			foreach (var tile in GameModel.currentLevel)
			{
				DrawWithBorder(g, tile);
			}
		}
		
		private (float x, float y) TileCoordinatesToWindow((int x, int y) coords)
		{
			var newX = whiteSpace + tileBorderWidth + coords.x * (tileSize + tileBorderWidth);
			var newY = whiteSpace + tileBorderWidth + coords.y * (tileSize + tileBorderWidth);
			return (newX, newY);
		}

		private void DrawWithBorder(Graphics g, Tile tile)
		{
			var coords = TileCoordinatesToWindow((tile.x, tile.y));
			g.DrawRectangle(Pens.Black, coords.x - tileBorderWidth, coords.y - tileBorderWidth, tileSize + tileBorderWidth, tileSize + tileBorderWidth);
			g.DrawImage(tile.GetDrawer().GetView(), coords.x, coords.y, tileSize, tileSize);			
		}
	}
}