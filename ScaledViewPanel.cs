using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Windows.Forms;

namespace Game
{
    public class ScaledViewPanel : Panel
    {
        //private readonly ScenePainter painter;
        private readonly GameModel GameModel;

        public ScaledViewPanel(GameModel gameModel) : this() => GameModel = gameModel;

        private PointF centerLogicalPos;
        private Point mouseLogicalPos;
        private float zoomScale;
        private readonly int tileSize = 64;
        private readonly int whiteSpace = 10;
        private readonly float tileBorderWidth = 0.5f;

        public ScaledViewPanel()
        {
            FitToWindow = true;
            zoomScale = 1f;
        }

        public Point MouseLogicalPos => mouseLogicalPos;

        public PointF CenterLogicalPos
        {
            get => centerLogicalPos;
            set
            {
                centerLogicalPos = value;
                FitToWindow = false;
            }
        }

        public float ZoomScale
        {
            get => zoomScale;
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
            else if (e.Button == MouseButtons.Left)
            {
                if (!GameModel.ReadyToAttack)
                    GameModel.SelectEntity(mouseLogicalPos);
                else
                    GameModel.AttackPosition(mouseLogicalPos);
            }
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);
            mouseLogicalPos = WindowCoordinatesToTile(ToLogical(e.Location));
        }


        protected override void OnMouseWheel(MouseEventArgs e)
        {
            base.OnMouseWheel(e);
            const float zoomChangeStep = 1.1f;
            if (e.Delta > 0)
                ZoomScale = ZoomScale <= 1.5 ? ZoomScale * zoomChangeStep : ZoomScale;
            if (e.Delta < 0)
                ZoomScale = ZoomScale >= 0.7 ? ZoomScale / zoomChangeStep : ZoomScale;
            Invalidate();
        }

        private PointF ToLogical(Point p)
        {
            var shift = GetShift();
            return new PointF(
                (p.X - shift.X) / zoomScale,
                (p.Y - shift.Y) / zoomScale);
        }

        private PointF GetShift() => new PointF(
                ClientSize.Width / 2f - CenterLogicalPos.X * ZoomScale,
                ClientSize.Height / 2f - CenterLogicalPos.Y * ZoomScale);

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            var g = e.Graphics;
            g.Clear(Color.Black);
            var sceneSize = new SizeF(800, 800);
            if (FitToWindow)
            {
                var vMargin = sceneSize.Height * ClientSize.Width < ClientSize.Height * sceneSize.Width;
                zoomScale = vMargin
                    ? ClientSize.Width / sceneSize.Width
                    : ClientSize.Height / sceneSize.Height;
                centerLogicalPos = new PointF(sceneSize.Width / 2, sceneSize.Height / 2);
            }

            var shift = GetShift();
            g.ResetTransform();
            g.TranslateTransform(shift.X, shift.Y);
            g.ScaleTransform(ZoomScale, ZoomScale);

            foreach (var tile in GameModel.CurrentLevel)
            {
                DrawWithBorder(g, tile);
            }

            if (GameModel.ReadyToAttack)
                foreach (var point in GameModel.SelectedEntity.Attack.GetPositions(mouseLogicalPos, GameModel.SelectedEntity.Direction))
                {
                    var coords = TileCoordinatesToWindow(point);
                    g.DrawRectangle(new Pen(Brushes.Red, 5), coords.X, coords.Y, tileSize, tileSize);
                }
            DrawPath(g, Color.Black, GameModel.Step.GetPreview(GameModel.Kaba.Position));

        }

        private PointF TileCoordinatesToWindow(Point coords)
        {
            var newX = whiteSpace + tileBorderWidth + coords.X * (tileSize + tileBorderWidth);
            var newY = whiteSpace + tileBorderWidth + coords.Y * (tileSize + tileBorderWidth);
            return new PointF(newX, newY);
        }

        private Point WindowCoordinatesToTile(PointF coords)
        {
            var newX = (coords.X - whiteSpace - tileBorderWidth) / (tileSize + tileBorderWidth);
            var newY = (coords.Y - whiteSpace - tileBorderWidth) / (tileSize + tileBorderWidth);
            return new Point((int)newX, (int)newY);
        }

        private void DrawWithBorder(Graphics g, Tile tile)
        {
            var coords = TileCoordinatesToWindow(tile.Position);
            g.DrawRectangle(Pens.Black, coords.X - tileBorderWidth, coords.Y - tileBorderWidth, tileSize + tileBorderWidth, tileSize + tileBorderWidth);
            g.DrawImage(tile.GetDrawer().GetView(), coords.X, coords.Y, tileSize, tileSize);
        }

        private void DrawPath(Graphics graphics, Color color, IEnumerable<Point> path)
        {
            var points = path.Select(x => TileCoordinatesToWindow(x)).Select(x => x + new SizeF(tileSize / 2, tileSize / 2)).ToArray();
            var pen = new Pen(color, 3f)
            {
                DashStyle = DashStyle.Dash,
            };
            for (var i = 0; i < points.Length - 1; i++)
                graphics.DrawLine(pen, points[i], points[i + 1]);
        }
    }
}