using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace Game.Model
{
    public class Controller
    {
        private static readonly Keys[] movingKeys = new[] { Keys.W, Keys.A, Keys.D, Keys.S, Keys.Up, Keys.Right, Keys.Left, Keys.Down };
        public Direction Direction { get; protected set; }
        public Controller(GameModel game) => Game = game;

        public GameModel Game { get; }

        private void HandleMovingKeyPress(Keys keyCode)
        {
            if (Game.ReadyToAttack)
                //Game.SelectedEntity.Rotate(keyCode.ToDirection());
                Direction = keyCode.ToDirection();
            else
            {
                Direction = Direction.None;
                Game.TrySheduleMove(keyCode.ToDirection());
            }
        }

        public void HandleKeyPress(Keys keyCode)
        {
            if (Game.IsAccessible)
            {
                if (movingKeys.Contains(keyCode))
                {
                    HandleMovingKeyPress(keyCode);
                    return;
                }
                switch (keyCode)
                {
                    case Keys.Space:
                        Game.CommitStep();
                        break;
                    case Keys.E:
                        Game.SelectAttack(KeyedAttack.EAttack);
                        break;
                    case Keys.Q:
                        Game.SelectAttack(KeyedAttack.QAttack);
                        break;
                    case Keys.F:
                        if (Game.HaveCollectable)
                        {
                            var items = Game.Snake.Heroes
                                .Where(x => x.IsAlive)
                                .Select(x => Game.CurrentLevel[x.Position])
                                .SelectMany(x => x.GameObjects)
                                .Where(x => x is ICollectable).Select(x => (ICollectable)x).ToArray();
                            foreach (var item in items)
                            {
                                item.Collect();
                            }
                        }
                        break;
                    case Keys.Z:
                        Game.TryUndoScheduled();
                        break;
                    case Keys.D1:
                        Game.SelectEntity(Game.Snake.Kaba);
                        break;
                    case Keys.D2:
                        Game.SelectEntity(Game.Snake.Hiro);
                        break;
                    case Keys.D3:
                        Game.SelectEntity(Game.Snake.Hana);
                        break;
                    case Keys.D4:
                        Game.SelectEntity(Game.Snake.Naru);
                        break;

                }
            }
        }

        public void HandleMouseClick(MouseButtons button, Point logicalPosition)
        {
            if (Game.IsAccessible)
            {
                if (button == MouseButtons.Left)
                {
                    if (!Game.ReadyToAttack)
                    {
                        Game.SelectEntity(logicalPosition);
                        Direction=Game.SelectedEntity?.Direction??Direction.None;
                    }
                    else
                        Game.TryScheduleAttack(logicalPosition, Direction);
                }
                else if (button == MouseButtons.Right)
                {
                    if (Game.ReadyToAttack)
                        Game.UnprepareForAttack();
                }
            }
        }
    }

}
