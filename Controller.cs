using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace Game.Model
{
    public class Controller
    {
        private static readonly Keys[] movingKeys = new[] { Keys.W, Keys.A, Keys.D, Keys.S, Keys.Up, Keys.Right, Keys.Left, Keys.Down };
        public Controller(GameModel game) => Game = game;

        public GameModel Game { get; }

        private void HandleMovingKeyPress(Keys keyCode)
        {
            if (Game.ReadyToAttack)
                Game.SelectedEntity.Rotate(keyCode.KeyCodeToDirection());
            else
                Game.TrySheduleMove(keyCode.KeyCodeToDirection());
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
                        Game.SelectAttackE();
                        break;
                    case Keys.Q:
                        Game.SelectAttackQ();
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
                        Game.SelectEntity(logicalPosition);
                    else
                        Game.TryScheduleAttack(logicalPosition);
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
