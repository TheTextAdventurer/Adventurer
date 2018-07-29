using System;

namespace GameEngine
{
    static partial class Advent
    {

        static Random _rnd = new Random();
        static string _RoomView = null;
        static GameData _GameData = null;

        static public string GameName { get { return _GameData.GameName; } }
        static public bool ISGameOver { get { return _GameData.EndGame; } }
        static public int TurnCounter { get { return _GameData.TurnCounter; } }

    }
}
