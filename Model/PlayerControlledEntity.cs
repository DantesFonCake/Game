using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace Game.Model
{
    public abstract class PlayerControlledEntity : Entity
    {
        public Attack EAttack { get; protected set; }
        public Attack QAttack { get; protected set; }
        public Ghost Ghost { get; private set; }
        public IReadOnlyDictionary<KeyedAttack, int> Cooldowns => cooldowns.ToDictionary(x => x.Key, x => x.Value);

        private readonly Dictionary<KeyedAttack, int> cooldowns = new Dictionary<KeyedAttack, int>();
        private KeyedAttack selectedAttack = KeyedAttack.None;
        public void SelectAttack(KeyedAttack attack)
        {
            selectedAttack = attack;
            Attack = attack switch
            {
                KeyedAttack.EAttack => EAttack,
                KeyedAttack.QAttack => QAttack,
                _=>null,
            };
        }

        public PlayerControlledEntity(GameModel game, Point position, int health, Dictionary<AttackType, int> resistances)
            : base(game, position, health, resistances)
        {
            IsPlayerControlled = true;
            Attack = QAttack;
            Ghost = new Ghost(this);
            RemoveOnDeath = false;
        }

        public override void AttackPosition(Point position, Direction attackDirection = Direction.None)
        {
            if (Attack.Cooldown > 0)
            {
                if (cooldowns.ContainsKey(selectedAttack))

                    cooldowns[selectedAttack] += Attack.Cooldown;
                else
                    cooldowns[selectedAttack] = Attack.Cooldown;
            }
            base.AttackPosition(position, attackDirection);
        }

        public void ReduceCooldowns()
        {
            foreach (var type in Cooldowns.Keys)
            {
                cooldowns[type] = Cooldowns[type] == 0 ? 0 : Cooldowns[type] - 1;
            }
        }
    }
}