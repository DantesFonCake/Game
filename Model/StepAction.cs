using System;

namespace Game.Model
{
    public class StepAction
    {
        public readonly Entity Target;
        public readonly Action Action;
        public readonly ActionType ActionType;

        public StepAction(Entity target, Action action, ActionType actionType)
        {
            Target = target;
            Action = action;
            ActionType = actionType;
        }
    }
}