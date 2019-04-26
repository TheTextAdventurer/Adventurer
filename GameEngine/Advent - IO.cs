﻿namespace GameEngine
{
    /// <summary>
    /// Load new games and save games
    /// </summary>
    static partial class Advent
    {
        /// <summary>
        /// Load a game
        /// </summary>
        /// <param name="pFile">Game file</param>
        public static void LoadGame(string pFile)
        {
            _GameData = GameData.Load(pFile);
            SearchActions(0, 0);
        }

        /// <summary>
        /// Restore a saved game
        /// </summary>
        /// <param name="pAdvGame">Game to load</param>
        /// <param name="pSnapShot">save game data</param>
        public static void RestoreGame(string pAdvGame, string pSnapShot)
        {
            _GameData = GameData.LoadSnapShot(pAdvGame, pSnapShot);
            PerformActionEffect(64, 0, 0);//look
        }
    }
}