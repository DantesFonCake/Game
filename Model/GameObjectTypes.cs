using System;

namespace Game
{
    [Flags]
    public enum GameObjectTypes 
    {
        Stone= 0b00000001,
        Hana = 0b00000010,
        Kaba = 0b00000100,
        Hiro = 0b00001000,
        Naru = 0b00010000,
        Enemy = 0b100000,
        CollectableItem = 0b1000000,
        Exit = 0b10000000
    }
}