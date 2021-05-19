﻿using System;
using System.Collections.Generic;
using System.Drawing;

namespace Game.Model
{
    public abstract class PlayerControlledEntity : Entity
    {
        public Attack EAttack { get; protected set; }
        public Attack QAttack { get; protected set; }
        public Ghost Ghost { get; private set; }
        public Dictionary<KeyedAttack, int> Cooldowns { get; protected set; } = new Dictionary<KeyedAttack, int>();
        public bool CanQAttack => Cooldowns.GetValueOrDefault(KeyedAttack.QAttack, 0) <= 0;
        public bool CanEAttack => Cooldowns.GetValueOrDefault(KeyedAttack.EAttack, 0) <= 0;
        private KeyedAttack selectedAttack = KeyedAttack.None;
        public void SelectAttack(KeyedAttack attack)
        {
            selectedAttack = attack;
            Attack = attack switch
            {
                KeyedAttack.EAttack => EAttack,
                KeyedAttack.QAttack => QAttack,
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
                    Cooldowns[selectedAttack] = Attack.Cooldown;
                base.AttackPosition(position, attackDirection);
        }
    }
}