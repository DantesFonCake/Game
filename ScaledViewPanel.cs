using Game.Model;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Windows.Forms;

namespace Game
{
    public class ScaledViewPanel
    {
        private readonly GameModel GameModel;
        private readonly Controller Controller;
        private readonly GameWindow Parent;
        public ScaledViewPanel(GameWindow parent, GameModel gameModel, Controller controller)
        {
            GameModel = gameModel;
            Controller = controller;
            Parent = parent;
            FitToWindow = true;
            Parent.ClickPerformed += (_, e) => OnMouseClick(e);
            Parent.MouseMove += (_, e) => OnMouseMove(e);
            //Parent.MouseWheel += (_, e) => OnMouseWheel(e);
        }

        private PointF centerLogicalPos;
        private Point mouseLogicalPos;
        private readonly int tileSize = 64;
        private readonly int whiteSpace = 10;
        private readonly int tileBorderWidth = 1;

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


        public bool FitToWindow { get; set; }

        protected void OnMouseClick(CustomMouseEventArg e)
        {

            if (e.Button == MouseButtons.Middle)
                FitToWindow = true;
            if (!e.Handled)
                Controller.HandleMouseClick(e.Button, MouseLogicalPos);
        }

        protected void OnMouseMove(MouseEventArgs e) => mouseLogicalPos = WindowCoordinatesToTile(ToLogical(e.Location));


        private PointF ToLogical(Point p)
        {
            var shift = GetShift();
            return new PointF(
                p.X - shift.X,
                p.Y - shift.Y);
        }

        private PointF GetShift() => new PointF(
                Parent.ClientSize.Width / 2f - CenterLogicalPos.X,
                Parent.ClientSize.Height / 2f - CenterLogicalPos.Y);

        public Bitmap GetBitmap()
        {
            var map = new Bitmap(Parent.ClientSize.Width, Parent.ClientSize.Height);
            using (var g = Graphics.FromImage(map))
            {
                g.Clear(Color.LightGray);
                var sceneSize = Parent.ClientSize;
                if (FitToWindow)
                {
                    var vMargin = sceneSize.Height * Parent.ClientSize.Width < Parent.ClientSize.Height * sceneSize.Width;

                    centerLogicalPos = new PointF(sceneSize.Width / 2, sceneSize.Height / 2);
                }
                CenterLogicalPos = TileCoordinatesToWindow(GameModel.Snake.Position);
                var shift = GetShift();
                g.ResetTransform();
                g.TranslateTransform(shift.X, shift.Y);

                var previewRectangle = new RectangleF(Parent.ClientRectangle.Location-new Size(tileSize,tileSize), Parent.ClientRectangle.Size);
                previewRectangle.Inflate(new Size(tileSize * 2, tileSize * 2));
                foreach (var tile in GameModel.CurrentLevel.Where(x => PointInClipRegion(previewRectangle, x.Position, shift)))
                {
                    DrawWithBorder(g, tile);
                }
                if (GameModel.ReadyToAttack)
                {
                    var attack = GameModel.SelectedEntity.Attack;
                    var specificColor = GameModel.SelectedEntity.Drawer.SpecificColor;
                    if (GameModel.IsAccessible && GameModel.SelectedPosition != null)
                        FillRectangles(g, GameModel.AttackPositions.Where(x => PointInClipRegion(previewRectangle, x, shift)), Color.FromArgb(64, specificColor));
                    if (GameModel.AttackPositions
                    .Contains(MouseLogicalPos))
                        DrawRectangles(g, attack.GetPositions(mouseLogicalPos, Controller.Direction).Where(x => PointInClipRegion(previewRectangle, x, shift)), specificColor, 5);
                }
                DrawPath(g, Color.Black, GameModel.PlayerScheduler.PathPreview.Where(x => PointInClipRegion(previewRectangle, x, shift)));
                foreach (var hero in GameModel.Snake.Heroes)
                {
                    DrawRectangles(g, 
                        GameModel.PlayerScheduler.AttackPreview[hero]
                            .SelectMany(x => x.Where(y => PointInClipRegion(previewRectangle, y, shift))),
                        Color.FromArgb(128, hero.Drawer.SpecificColor), 4);
                }
                foreach (var ghost in GameModel.PlayerScheduler.Ghosts.Where(x => x.Value.Count > 1).Where(x => PointInClipRegion(previewRectangle, x.Key.Position, shift)))
                {
                    g.DrawImage(ghost.Key.Sprite, new RectangleF(TileCoordinatesToWindow(ghost.Key.Position), new Size(tileSize, tileSize)));
                }
            }
            return map;
        }

        private bool PointInClipRegion(RectangleF clipRectangle, Point x, PointF shift) => clipRectangle.Contains(TileCoordinatesToWindow(x) + new SizeF(shift));

        private void DrawRectangles(Graphics g, IEnumerable<Point> points, Color color, int borderWidth)
        {
            foreach (var point in points)
            {
                var coords = TileCoordinatesToWindow(point);
                g.DrawRectangle(new Pen(color, borderWidth), coords.X, coords.Y, tileSize, tileSize);
            }
        }
        private void FillRectangles(Graphics g, IEnumerable<Point> points, Color color)
        {
            foreach (var point in points)
            {
                var coords = TileCoordinatesToWindow(point);
                g.FillRectangle(new SolidBrush(color), coords.X, coords.Y, tileSize, tileSize);
            }
        }


        private PointF TileCoordinatesToWindow(Point coords)
        {
            var newX = whiteSpace - 2 * tileBorderWidth + coords.X * (tileSize + tileBorderWidth);
            var newY = whiteSpace - 2 * tileBorderWidth + coords.Y * (tileSize + tileBorderWidth);
            return new PointF(newX, newY);
        }

        private Point WindowCoordinatesToTile(PointF coords)
        {
            var newX = (coords.X - whiteSpace + 2 * tileBorderWidth - tileSize / 2) / (tileSize + tileBorderWidth);
            var newY = (coords.Y - whiteSpace + 2 * tileBorderWidth - tileSize / 2) / (tileSize + tileBorderWidth);
            return Point.Round(new PointF(newX, newY));
        }

        private void DrawWithBorder(Graphics g, Tile tile)
        {
            var coords = Point.Truncate(TileCoordinatesToWindow(tile.Position));
            var borderOffset = new Size(tileBorderWidth, tileBorderWidth);
            var tileRectangle = new Rectangle(coords, new Size(tileSize, tileSize));
            var fullRectangle = new Rectangle(coords - borderOffset, tileRectangle.Size + 2 * borderOffset);
            g.DrawRectangle(new Pen(Color.Black, tileBorderWidth), fullRectangle);

            g.DrawImage(tile.Drawer.GetView(), tileRectangle);
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