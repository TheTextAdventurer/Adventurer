using System;

namespace GameEngine
{
    static partial class Advent
    {

        static Random _rnd = new Random();

        static string _RoomView = null;
        static string _RoomItems = null;

        static GameData _GameData = null;

        static public string GameName => _GameData.GameName;
        static public bool ISGameOver => _GameData.EndGame;
        static public int TurnCounter => _GameData.TurnCounter;

    }
}
