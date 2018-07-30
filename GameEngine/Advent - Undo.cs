using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using GameEngine;

namespace GameEngine
{
    /// <summary>
    /// Undo / Redo functionality
    /// </summary>
    static partial class Advent
    {
        public static bool Undo()
        {
            if (_GameData.UndoPosition > 0)
            {
                RestoreUndoblock(true);
                PerformActionComponent(64, 0, 0);//look
                SearchActions(0, 0);
                return true;
            }
            return false;
        }

        public static bool Redo()
        {
            if (_GameData.UndoPosition < _GameData.UndoHistory.Count() - 1)
            {
                RestoreUndoblock(false);
                PerformActionComponent(64, 0, 0);//look
                SearchActions(0, 0);
                return true;
            }
            return false;
        }

        /// <summary>
        /// 
        /// 
        /// </summary>
        /// <param name="pUndo"></param>
        private static void RestoreUndoblock(bool pUndo)
        {

            if (!pUndo)
            {
                _GameData.UndoPosition++;
                _GameData.SetCurrentUndo(_GameData.UndoPosition);
            }


            _GameData.RecordUndo = false;


            foreach (GameData.ChangeRepresentationObject o in _GameData.CurrentUndoBlock.Block)
            {

                switch (o.Item)
                {
                    case GameData.ChangeItem.Item:
                        _GameData.Items[o.Index].Location = pUndo ? (int)o.OldData : (int)o.NewData;
                        break;

                    case GameData.ChangeItem.BitFlag:
                        _GameData.BitFlags[o.Index] = (bool)(pUndo ? o.OldData : o.NewData);
                        break;

                    case GameData.ChangeItem.Counter:
                        _GameData.Counters[o.Index] = (int)(pUndo ? o.OldData : o.NewData);
                        break;

                    case GameData.ChangeItem.SavedRooms:
                        _GameData.SavedRooms[o.Index] = (int)(pUndo ? o.OldData : o.NewData);
                        break;

                    case GameData.ChangeItem.CurrentRoom:
                        _GameData.CurrentRoom = (int)(pUndo ? o.OldData : o.NewData);
                        break;

                    case GameData.ChangeItem.TakeSuccessful:
                        _GameData.TakeSuccessful = (bool)(pUndo ? o.OldData : o.NewData);
                        break;

                    case GameData.ChangeItem.CurrentCount:
                        _GameData.CurrentCounter = (int)(pUndo ? o.OldData : o.NewData);
                        break;

                    case GameData.ChangeItem.LampLife:
                        _GameData.LampLife = (int)(pUndo ? o.OldData : o.NewData);
                        break;

                    case GameData.ChangeItem.PlayerNoun:
                        _GameData.PlayerNoun = (string)(pUndo ? o.OldData : o.NewData);
                        break;

                    case GameData.ChangeItem.SavedRoom:
                        _GameData.SavedRoom = (int)(pUndo ? o.OldData : o.NewData);
                        break;

                    case GameData.ChangeItem.EndGame:
                        _GameData.EndGame = (bool)(pUndo ? o.OldData : o.NewData);
                        break;
                }

            }

            _GameData.RecordUndo = true;

            if (pUndo)
            {
                _GameData.UndoPosition--;
                _GameData.SetCurrentUndo(_GameData.UndoPosition);
            }
        }
    }
}