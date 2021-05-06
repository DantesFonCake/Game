using System.Collections.Generic;
using System.Drawing;

namespace Game.Model
{
    public abstract class PlayerControlledEntity : Entity
    {
        public Attack EAttack { get; protected set; }
        public Attack QAttack { get; protected set; }
        public override Attack Attack { get; protected set; }
        public Ghost Ghost { get; private set; }
        public void SelectAttack(KeyedAttack attack) => Attack = attack switch
        {
            KeyedAttack.EAttack => EAttack,
            KeyedAttack.QAttack => QAttack,
        };

        public PlayerControlledEntity(Point position, int health, Dictionary<AttackType, int> resistances)
            : base(position, health, resistances)
        {
            IsPlayerControlled = true;
            Attack = QAttack;
            Ghost = new Ghost(this);
        }
    }
}