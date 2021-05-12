using System;
using System.Collections.Generic;
using System.Drawing;

namespace Game.Model
{
    public class Scheduler
    {


        public readonly LinkedList<StepAction> Actions = new LinkedList<StepAction>();

        public virtual void AddMovement(Action<Direction> movement, Direction direction) => Actions.AddLast(new StepAction(null,() => movement(direction), ActionType.Move));

        public virtual void AddAttack(Entity entity, Point position , Point? initialPosition=null)
        {
            if(initialPosition==null)
                initialPosition = entity.Position;
            if (!entity.Attack.IsRanged)
                Actions.AddLast(new StepAction(entity,() => entity.MoveTo(position), ActionType.NotPreviewable));
            Actions.AddLast(new StepAction(entity,() => entity.AttackPosition(position), ActionType.Attack));
            if (!entity.Attack.IsRanged)
                Actions.AddLast(new StepAction(entity,() => entity.MoveTo(initialPosition.Value), ActionType.NotPreviewable));
        }

        public virtual bool Commit()
        {
            if (Actions.Count <= 0)
                return false;
            Actions.First.Value.Action();
            Actions.RemoveFirst();
            return Actions.Count > 0;
        }

        public virtual bool Unschedule()
        {
            Actions.RemoveLast();
            return Actions.Count > 0;
        }


    }
}