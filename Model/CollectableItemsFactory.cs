using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace Game.Model
{
    public static class CollectableItemsFactory
    {
        public static CollectableGameObject HermesBoots(GameModel game, Point posistion)
            => new CollectableGameObject(game, posistion,
                "Hermes Boots",
                "'No Plagiarism' Increases number of available steps",
                Properties.Resources.boots,
                () => game.PlayerScheduler.ChangeMaxActionCount(1));

        public static CollectableGameObject SharpeningStone(GameModel game, Point position)
            => new CollectableGameObject(game, position,
                  "Sharpening Stone",
                  "Increases Physical damage",
                  Properties.Resources.sharpening_stone,
                  () =>
                  {
                      foreach (var hero in game.Snake.Heroes)
                      {
                          var attack = hero.EAttack;
                          if (attack.Type == AttackType.Physical)
                              attack.ChangeDamage(10);
                          attack = hero.QAttack;
                          if (attack.Type == AttackType.Physical)
                              attack.ChangeDamage(10);
                      }
                  });

        public static CollectableGameObject NkhShield(GameModel game, Point position)
            => new CollectableGameObject(game, position,
                "Nkh Shield",
                "'Someone stole a letter' Increases resistance to all damage types",
                Properties.Resources.shield,
                () =>
                {
                    foreach (var hero in game.Snake.Heroes)
                        foreach (var attackType in Enum.GetValues(typeof(AttackType)).Cast<AttackType>())
                            hero.ChangeResistance(attackType, 10);
                });

        public static Dictionary<ItemTypes, Func<GameModel, Point, CollectableGameObject>> Items = new Dictionary<ItemTypes, Func<GameModel, Point, CollectableGameObject>>()
        {
            [ItemTypes.Boots] = HermesBoots,
            [ItemTypes.SharpeningStone] = SharpeningStone,
            [ItemTypes.Shield] = NkhShield,
        };
    }
}
