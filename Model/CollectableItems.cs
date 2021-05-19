using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;

namespace Game.Model
{
    public static class CollectableItems
    {
        public static Dictionary<ItemTypes, Func<GameModel, Point, CollectableGameObject>> Items = new Dictionary<ItemTypes, Func<GameModel, Point, CollectableGameObject>>()
        {
            [ItemTypes.Boots] = (game, point)
            => new CollectableGameObject(game, point,
                "Hermes Boots",
                "'No Plagiarism' Increases number of available steps",
                Properties.Resources.boots,
                () => game.PlayerScheduler.ChangeMaxActionCount(1)),
            [ItemTypes.SharpeningStone] = (game, point)
              => new CollectableGameObject(game, point,
                  "Sharpening Stone",
                  "Increases Physical damage",
                  Properties.Resources.boots,
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
                  }),
            [ItemTypes.Shield]=(game,point)
            =>new CollectableGameObject(game,point,
                "Nkh Shield",
                "'Someone stole a letter' Increases resistance to all damage types",
                Properties.Resources.shield,
                ()=>
                {
                    foreach (var hero in game.Snake.Heroes)
                    {
                        foreach (var attackType in Enum.GetValues(typeof(AttackType)).Cast<AttackType>())
                        {
                            hero.ChangeResistance(attackType, 10);
                        }
                    }
                }),
        };
    }
}
