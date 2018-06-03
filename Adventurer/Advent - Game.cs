using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace Adventurer
{
    static partial class Advent
    {
        #region IO
        /// <summary>
        /// Load a game
        /// </summary>
        /// <param name="pFile">Game file</param>
        public static void LoadGame(string pFile)
        {
            _GameData = GameData.Load(pFile);            
            _GameData.BeginUndo();
            SearchActions(0, 0);
            _GameData.EndUndo();
        }

        public static void SaveAsFormattedXML()
        {
            _GameData.SaveAsFormattedXML();
        }

        public static void SaveAsUnFormattedXML(string pFile)
        {
            _GameData.SaveAsUnFormattedXML(pFile);
        }

        /// <summary>
        /// Restore a saved game
        /// </summary>
        /// <param name="pFile">Game file</param>
        public static void RestoreGame(string pAdvGame, string pSnapShot)
        {
            _GameData = GameData.LoadSnapShot(pAdvGame, pSnapShot);
            PerformActionComponent(64, 0, 0);//look
        }

        #endregion

        #region undo functionality

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

        #endregion

        #region game code

        /// <summary>
        /// Process the user input from the game
        /// </summary>
        /// <param name="pWords">User input</param>
        /// <remarks>Bit of miss this, must tidy up.</remarks>
        public static void ProcessText(string pInput)
        {
            string[] words = pInput.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

            if (pInput.StartsWith("#"))
            {           
                if (words.Length == 1 && words.First().Equals("#undo", StringComparison.OrdinalIgnoreCase))
                {
                    if (!Undo())
                        SetGameOutput("A voice BOOOOMS out: \"NOTHING TO UNDO\"", true);
                    return;
                }
                else if (words.Length == 1 && words.First().Equals("#redo", StringComparison.OrdinalIgnoreCase))
                {
                    if (!Redo())
                        SetGameOutput("A voice BOOOOMS out: \"NOTHING TO REDO\"", true);
                    return;
                }
            }

            _GameData.BeginUndo();
            _GameData.TurnCounter++;
            SetGameOutput("", true);

            pInput = pInput.Trim();

            if (string.IsNullOrEmpty(pInput))
            {
                SetGameOutput(_Sysmessages[11], true);
                _GameData.EndUndo();
                return;
            }

            string verb = "";
            int verbID = -1;
            int nounID = -1;
            int temp = 0;

            if (words.Length == 1 && string.IsNullOrEmpty(words.First()))
            {
                _GameData.PlayerNoun = "";
                SetGameOutput(_Sysmessages[11], true);
                onUpdateView();
            }
            else
            {
                verb = ShrinkWord(words.First());

                if (verb.Equals("i", StringComparison.OrdinalIgnoreCase))
                    verb = "INV";

                verbID = SearchWordList(_GameData.Verbs, verb);


                //verb not recognised or isn't direction
                if (verbID == -1)
                {
                    if ((temp = IsDirection(verb)) > -1) //is direction?
                    {
                        verbID = (int)_Constants.VERB_GO;
                        nounID = temp;
                    }
                    if (verbID == -1)
                    {
                        SetGameOutput(string.Format("\"{0}\" {1}", words.First(), _Sysmessages[1]), true);
                        onUpdateView();
                        _GameData.EndUndo();
                        return;
                    }
                }
            }


            if (words.Length > 1 && nounID == -1)//two words entered
            {
                _GameData.PlayerNoun = words[1];
                nounID = SearchWordList(_GameData.Nouns, _GameData.PlayerNoun.ToUpper());
            }
            else if //take / drop <no word>
            (_GameData.PlayerNoun == "" && (verbID == (int)_Constants.VERB_TAKE || verbID == (int)_Constants.VERB_DROP))
            {
                SetGameOutput(_Sysmessages[11], true);
                onUpdateView();
                _GameData.EndUndo();
                return;
            }

            //we're now at point where the entered data appears to do something..

            //Check lamp life
            if (CheckCondition(13, (int)_Constants.LIGHTSOURCE) & _GameData.LampLife > 0)
            { //is the light source in the game
                _GameData.LampLife--;
            }

            //player moving in direction
            if (verbID == (int)_Constants.VERB_GO && nounID > -1 && nounID < 7)
            {

                if (_GameData.Rooms[_GameData.CurrentRoom].Exits[nounID - 1] > 0)
                {

                    //direction being moved in exists
                    //note the subtratcion - north is always 1, remove 1
                    PerformActionComponent(54, _GameData.Rooms[_GameData.CurrentRoom].Exits[nounID - 1], 0);

                    SetGameOutput(
                        IsDark()
                                   ? _Sysmessages[17]    //dangerous to move in dark
                                   : _Sysmessages[0]    //can move
                            , true
                        );
                }
                else
                {
                    //can't go in that direction
                    if (IsDark())
                    {
                        SetGameOutput(_Sysmessages[18], true);
                        PerformActionComponent(63, 0, 0);
                        _GameData.EndUndo();
                        return;
                    }
                    else
                        SetGameOutput(_Sysmessages[2], true);
                }

            }
            else
            {

                ////take / drop all
                //if ((verbID == (int)_Constants.VERB_TAKE || verbID == (int)_Constants.VERB_DROP) && _GameData.PlayerNoun.ToUpper() == "ALL")
                //{

                //    //we're only intesred in standard items that have an associated word,
                //    //can be naturally picked and dropped. Special cases that have no word
                //    //such as the magic mirror in ADV01.dat are picked and dropped by special actions
                //    bool happened = false;
                //    for (var i = 0; i < _GameData.Items.Length; i++)
                //    {

                //        if (_GameData.Items[i].Location == (verbID == (int)_Constants.VERB_TAKE ? _GameData.CurrentRoom : (int)_Constants.INVENTORY)
                //             && _GameData.Items[i].Word != null)
                //        {

                //            if (verbID == (int)_Constants.VERB_TAKE)
                //            {

                //                if (!IsDark())
                //                {

                //                    if (GetItemsAt((int)_Constants.INVENTORY).Length < _GameData.Header.MaxCarry)
                //                    {
                //                        PerformActionComponent(52, i, 0);
                //                        SetGameOutput(_GameData.Items[i].Description + ": " + _Sysmessages[0], false);
                //                    }
                //                    else
                //                    {
                //                        SetGameOutput(_Sysmessages[8], false);
                //                        break;
                //                    }
                //                }
                //                else
                //                    SetGameOutput(_Sysmessages[16], false); //too dark to take all
                //            }
                //            else
                //            {
                //                //drop all
                //                _GameData.ChangeItemLocation(i, _GameData.CurrentRoom);
                //                SetGameOutput(_GameData.Items[i].Description + ": " + _Sysmessages[0], false);
                //            }

                //            happened = true;
                //        }
                //    }

                //    if (!happened)
                //        SetGameOutput(verbID == (int)_Constants.VERB_TAKE ? _Sysmessages[21] : _Sysmessages[22], true);

                //    PerformActionComponent(64, 0, 0); //look
                //}
                //else
                //{
                //    //compare against custome actions
                //    SearchActions(verbID, nounID);
                //    _GameData.EndUndo();
                //    return;
                //}

                SearchActions(verbID, nounID);
                _GameData.EndUndo();
                return;

            }

            SearchActions(0, 0);

            _GameData.EndUndo();
        }

        /// <summary>
        /// Shrink the provided word down to the game's word length
        /// </summary>
        /// <param name="pWord">Word to shrink</param>
        /// <returns>Shrunk word</returns>
        static string ShrinkWord(string pWord)
        {
            return (pWord.Length > _GameData.Header.WordLength
                   ? pWord.Substring(0, _GameData.Header.WordLength)
                   : pWord)
                   .ToUpper();
        }


        /// <summary>
        /// Is it dark
        /// </summary>
        /// <returns></returns>
        static bool IsDark()
        {

            return (_GameData.BitFlags[(int)_Constants.DARKNESSFLAG] && CheckCondition(12, (int)_Constants.LIGHTSOURCE));

        }

        /// <summary>
        /// Look for direction incuding alisises
        /// </summary>
        /// <param name="pDir"></param>
        /// <returns></returns>
        private static int IsDirection(string pDir)
        {
            //directions are always 1 to 6 in the NOUN list
            int retVal = -1;
            for (int d = 1; d < 7; d++)
            {
                if (_GameData.Nouns[d].StartsWith(pDir))
                {
                    retVal = d;
                    break;
                }
            }
            return retVal;    

        }

        /// <summary>
        /// Attemp to execute the provided action
        /// </summary>
        /// <param name="pAction"></param>
        /// <returns></returns>
        private static bool ExcecuteAction(GameData.Action pAction)
        {
            if (ActionTest(pAction.Conditions))
            {
                //step through the components
                foreach (int[] act in pAction.Actions.Where(a=>a[0] > 0))
                    PerformActionComponent(act[0], act[1], act[2]);

                return true;
            }
            return false;
        }

        /// <summary>
        ///  Examine the enture actions list for entries with a matching verb/noun
        /// </summary>
        /// <param name="pVerb"></param>
        /// <param name="pNoun"></param>
        /// <param name="pStartAction"></param>
        /// <param name="pStartComponent"></param>
        private static void SearchActions(int pVerb, int pNoun)
        {

            //determines if we output a message upon completion
            //0 no message, 1 don't understand, 2 beyond my power
            //message used if input is via a user, pVerb > 0
            var msg = pVerb > 0 ? 1 : 0;


            var parentOutcome = false;

            List<GameData.Action> candidates =
                        _GameData.Actions.Where(a =>
                            ((pVerb == 0 && a.Verb == pVerb)
                                & (
                                    (pNoun == 0 && (a.Noun == pNoun || a.Noun == 100)   //0 or 100 actions occur automatically
                                    ||
                                    ((_rnd.Next(100) + 1) < a.Noun)     //probabability based action
                                ))
                            )
                            //user entered actions
                            || (pVerb == a.Verb && pNoun == a.Noun)
                            || (pVerb == a.Verb && a.Noun == 0)
                        ).ToList();


            foreach (GameData.Action act in candidates)
            {
                //now attempt to execute
                if (parentOutcome = ExcecuteAction(act))
                {
                    if (act.Children != null)
                        act.Children.All(ch => { ExcecuteAction(ch); return true; });
                }

                //do more stuff
                if (_GameData.EndGame)
                {
                    PerformActionComponent(64, 0, 0); //look
                    return;
                }

                if (pVerb > 0 && parentOutcome)
                {
                    //this is user input, and the same verb noun combination may be used
                    //under different conditions, so bail if we've successfully processed
                    //user input
                    break;
                }
            }

            //output a can't do that message if we recognise a player verb in the list, but not a noun
            if (pVerb > 0 && !parentOutcome & _GameData.Actions.Count(act => act.Verb == pVerb) > 0)
            {
                msg = 2;
            }


            if (pVerb > 0)
            { //only do after user input

                if (!parentOutcome)
                {
                    if (msg == 1) //don't understand
                        SetGameOutput(_Sysmessages[15], true);
                    else if (msg == 2) //Can't do that yet
                        SetGameOutput(_Sysmessages[14], true);
                }

                SearchActions(0, 0); //auto actions
                                     //DisableUserInput(false);
            }

            //lamp stuff, is it in the game
            if (CheckCondition(13, (int)_Constants.LIGHTSOURCE))
            {

                if (_GameData.LampLife == 0)
                {
                    _GameData.ChangeBitFlag((int)_Constants.LIGHOUTFLAG, true);
                    SetGameOutput(_Sysmessages[19], false);
                    _GameData.LampLife = 0;
                }
                else if (_GameData.LampLife > 0 && _GameData.LampLife < 25 &&
                  CheckCondition(3, (int)_Constants.LIGHTSOURCE) &&
                  _GameData.LampLife % 5 == 0)
                    SetGameOutput(_Sysmessages[20], false); //light growing dim
            }

            //DisableUserInput(false);
            PerformActionComponent(64, 0, 0); //look



        }

        /// <summary>
        /// Check the provided condition
        /// </summary>
        /// <param name="pCon">Condition</param>
        /// <param name="pArg">Argument</param>
        /// <returns>Condition met</returns>
        private static bool CheckCondition(int pCon, int pArg)
        {
            bool retVal = false;

            switch (pCon)
            {
                case 1: //item carried
                    retVal = _InventoryLocations.Contains(_GameData.Items[pArg].Location);
                    break;

                case 2: //item in room with player
                    retVal = _GameData.Items[pArg].Location == _GameData.CurrentRoom;
                    break;

                case 3: //item carried or in room with player
                    retVal = CheckCondition(1, pArg) || CheckCondition(2, pArg);
                    break;

                case 4: //player in room X
                    retVal = _GameData.CurrentRoom == pArg;
                    break;

                case 5: //item not in room with player
                    retVal = _GameData.Items[pArg].Location != _GameData.CurrentRoom;
                    break;

                case 6: //item not carried
                    retVal = !_InventoryLocations.Contains(_GameData.Items[pArg].Location);
                    break;

                case 7: //player not it room
                    retVal = _GameData.CurrentRoom != pArg;
                    break;

                case 8: //bitflag X is set
                    retVal = _GameData.BitFlags[pArg] == true;
                    break;

                case 9: //bitflag X is false
                    retVal = _GameData.BitFlags[pArg] != true;
                    break;

                case 10: //something carried
                    return _GameData.Items.Count(i => _InventoryLocations.Contains(i.Location) == true) > 0;

                case 11: //nothing carried
                    return _GameData.Items.Count(i => _InventoryLocations.Contains(i.Location) == true) == 0;

                case 12: //item not carried or in room with player
                    retVal = CheckCondition(6, pArg) & CheckCondition(5, pArg);
                    break;

                case 13: //item in game
                    retVal = (_GameData.Items[pArg].Location != (int)_Constants.STORE);
                    break;

                case 14: //item not in game
                    retVal = _GameData.Items[pArg].Location == (int)_Constants.STORE;
                    break;

                case 15: //current counter less than arg
                    retVal = _GameData.CurrentCounter <= pArg;
                    break;

                case 16: //current counter greater than arg
                    retVal = _GameData.CurrentCounter > pArg;
                    break;

                case 17: //object in initial location
                    retVal = _GameData.Items[pArg].Moved() == false;
                    break;

                case 18: //object not in initial location
                    retVal = _GameData.Items[pArg].Moved() == true;
                    break;

                case 19: //current counter equals
                    retVal = _GameData.CurrentCounter == pArg;
                    break;
            }

            return retVal;
        }



        /// <summary>
        /// Search the provided word list
        /// </summary>
        /// <param name="pWordList">Arry to search</param>
        /// <param name="pWord">word to search</param>
        /// <returns>-1 if not present</returns>
        private static int SearchWordList(string[] pWordList, string pWord)
        {
            pWord = ShrinkWord(pWord);

            //value of the length of the array indicates no match, 0 the first match etc
            int retVal = pWordList.TakeWhile(
                    w => !(w.StartsWith("*") ? w.Substring(1) : w)
                            .Equals(pWord, StringComparison.OrdinalIgnoreCase)//remove aliase marker                            
                                                                              //reduce to word length
                ).Count();

            if (retVal == pWordList.Count())
                retVal = -1;
            else if (pWordList[retVal].StartsWith("*"))//alias located
            {
                do
                {   //reverse up the list until the first none star
                    retVal--;
                } while (pWordList[retVal].StartsWith("*"));
            }

            return retVal;
        }

        /// <summary>
        /// Perform the provded action component
        /// </summary>
        /// <param name="pAct">Action</param>
        /// <param name="pArg1">Action argument 1</param>
        /// <param name="pArg2">Action argument 1</param>
        private static void PerformActionComponent(int pAct, int pArg1, int pArg2)
        {


            if (pAct < 52 || pAct > 101)
            {
                SetGameOutput(_GameData.Messages[pAct - (pAct > 101 ? 50 : 0)], false);
                PerformActionComponent(86, 0, 0);//carriage return
            }
            else
            {
                switch (pAct)
                {

                    case 52: //get item, check if can carry
                        _GameData.TakeSuccessful = false;
                        if (GetItemsAt(_InventoryLocations).Count() < _GameData.Header.MaxCarry)
                        {
                            _GameData.Items[pArg1].Location = (int)_Constants.INVENTORY;
                            _GameData.TakeSuccessful = true;
                        }
                        else
                            SetGameOutput(_Sysmessages[8], true);
                        break;

                    case 53: //drops item into current room
                        _GameData.ChangeItemLocation(pArg1, _GameData.CurrentRoom);
                        break;

                    case 54: //move room
                        _GameData.CurrentRoom = pArg1;
                        PerformActionComponent(64, 0, 0);
                        break;

                    case 55: //Item <arg> is removed from the game (put in room 0)
                    case 59:
                        _GameData.ChangeItemLocation(pArg1, (int)_Constants.STORE);
                        break;

                    case 56: //set darkness flag
                        _GameData.ChangeBitFlag((int)_Constants.DARKNESSFLAG, true);
                        break;

                    case 57: //clear darkness flag
                        _GameData.ChangeBitFlag((int)_Constants.DARKNESSFLAG, false);
                        break;

                    case 58: //set pArg1 flag
                        _GameData.ChangeBitFlag(pArg1, true);
                        break;

                    case 60: //set pArg1 flag
                        _GameData.ChangeBitFlag(pArg1, false);
                        break;

                    case 61: //Death, clear dark flag, move to last room NOT GAME OVER, move to limbo room
                        PerformActionComponent(57, 0, 0);
                        _GameData.CurrentRoom = _GameData.Rooms.Count() - 1;
                        SetGameOutput(_Sysmessages[24], false);
                        break;

                    case 62: //item is moved to room
                        _GameData.ChangeItemLocation(pArg1, pArg2);
                        break;

                    case 63: //game over
                        _GameData.EndGame = true;
                        SetGameOutput(_Sysmessages[25], false);
                        break;

                    case 64: //look
                    case 76:

                        if (_GameData.BitFlags[(int)_Constants.DARKNESSFLAG] && CheckCondition(12, (int)_Constants.LIGHTSOURCE))
                            _RoomView = _Sysmessages[16];
                        else
                        {

                            string roomitems = String.Join(", ", _GameData.Items.Where(i => i.Location == _GameData.CurrentRoom)
                                                .Select(i => i.Description)
                                                .ToArray()
                                                );

                            string desc = _GameData.Rooms[_GameData.CurrentRoom].Description;

                            _RoomView =
                                (
                                    (desc.StartsWith("*")
                                    ? desc = desc.Substring(1)
                                    : _Sysmessages[3] + desc)

                                    + (roomitems == "" ? "" : "\r\n\r\n" + _Sysmessages[4] + roomitems)
                                );


                        }

                        onUpdateView();

                        break;

                    case 65: //score
                        int storedItems = _GameData.Items.Count(i => i.Location == _GameData.Header.TreasureRoom
                                && i.Description.StartsWith("*"));

                        SetGameOutput(string.Format(
                                            _Sysmessages[13]
                                            , storedItems
                                            , Math.Floor((storedItems * 1.0 / _GameData.Header.TotalTreasures) * 100)), false);

                        if (storedItems == _GameData.Header.TotalTreasures)
                        {
                            SetGameOutput(_Sysmessages[26], true);
                            PerformActionComponent(63, 0, 0);
                        }

                        break;

                    case 66: // output inventory

                        string[] items = GetItemsAt(_InventoryLocations)
                                        .Select(i => i.Description)
                                        .ToArray();

                        SetGameOutput(_Sysmessages[9] +
                                        (items.GetLength(0) == 0
                                        ? _Sysmessages[12]
                                        : String.Join(", ", items)), false);

                        PerformActionComponent(86, 0, 0);
                        PerformActionComponent(86, 0, 0);

                        break;

                    case 67:
                        _GameData.ChangeBitFlag(0, true);
                        break;

                    case 68:
                        _GameData.ChangeBitFlag(0, false);
                        break;

                    case 69: //refill lamp
                        _GameData.LampLife = _GameData.Header.LightDuration;
                        _GameData.ChangeBitFlag((int)_Constants.LIGHOUTFLAG, false);
                        _GameData.ChangeItemLocation((int)_Constants.LIGHTSOURCE, (int)_Constants.INVENTORY);
                        break;

                    case 70: //clear screen
                        SetGameOutput("", true);
                        _RoomView = null;
                        break;

                    case 71: //save game

                        SetGameOutput(string.Format("Game {0} saved", _GameData.SaveSnapshot()), true);                      
                        PerformActionComponent(86, 0, 0);   //carriage return
                        break;

                    case 72: // swap item locations
                        int loc = _GameData.Items[pArg1].Location;
                        _GameData.ChangeItemLocation(pArg1, _GameData.Items[pArg2].Location);
                        _GameData.ChangeItemLocation(pArg2, loc);
                        break;

                    case 73: //continue with next action
                        break;

                    case 74: //take item, no check done to see if can carry
                        _GameData.ChangeItemLocation(pArg1, (int)_Constants.INVENTORY);
                        break;

                    case 75: //put item 1 with item2
                        _GameData.ChangeItemLocation(pArg1, _GameData.Items[pArg2].Location);
                        break;

                    case 77: //decement current counter
                        if (_GameData.CurrentCounter > 0)
                            _GameData.CurrentCounter--;
                        break;

                    case 78: //output current counter
                        SetGameOutput(_GameData.CurrentCounter + "\r\n", false);
                        break;

                    case 79: //set current counter value
                        _GameData.CurrentCounter = pArg1;
                        break;

                    case 80: //swap location with saved location
                        int j = _GameData.CurrentRoom;
                        _GameData.CurrentRoom = _GameData.SavedRoom;
                        _GameData.SavedRoom = j;
                        break;

                    case 81: //"Select a counter. Current counter is swapped with backup counter @".replace("@", pValue1);
                        int temp = _GameData.CurrentCounter;
                        _GameData.CurrentCounter = _GameData.Counters[pArg1];
                        _GameData.ChangeCounter(pArg1, temp);
                        break;

                    case 82: //add to current counter
                        _GameData.CurrentCounter += pArg1;
                        break;

                    case 83: //subtract from current counter
                        _GameData.CurrentCounter -= pArg1;
                        if (_GameData.CurrentCounter < -1)
                            _GameData.CurrentCounter = -1;
                        break;

                    case 84: //echo noun without cr
                        SetGameOutput(_GameData.PlayerNoun, false);
                        break;

                    case 85: //echo noun
                        SetGameOutput(_GameData.PlayerNoun + "\r\n", false);
                        break;

                    case 86: //Carriage Return"
                        SetGameOutput("\r\n", false);
                        break;

                    case 87: //Swap current location value with backup location-swap value
                        int temp1 = _GameData.CurrentRoom;
                        _GameData.CurrentRoom = _GameData.SavedRooms[pArg1];
                        _GameData.SavedRooms[pArg1] = temp1;
                        break;

                    case 88: //wait 2 seconds
                        Thread.Sleep(2000);
                        break;
                }
            }
        }

        /// <summary>
        /// Check the provided conditions
        /// </summary>
        /// <param name="pConds">Condition block</param>
        /// <returns>Have they been met</returns>
        private static bool ActionTest(int[][] pConds)
        {
            foreach (int[] con in pConds.Where(c=>c[0]> 0))
            {
                if (!CheckCondition(con[0], con[1]))
                    return false;
            }
            return true;
        }

        /// <summary>
        /// get the items at the specified location
        /// </summary>
        /// <param name="pLocation">RoomID</param>
        /// <returns>Array of items</returns>
        private static GameData.Item[] GetItemsAt(int pLocation)
        {
            return _GameData.Items.Where(i => i.Location == pLocation).ToArray();
        }

        /// <summary>
        /// get the items at the specified location
        /// </summary>
        /// <param name="pLocation">RoomID</param>
        /// <returns>Array of items</returns>
        private static GameData.Item[] GetItemsAt(int[] pLocation)
        {
            return _GameData.Items.Where(i => pLocation.Contains(i.Location)).ToArray();
        }

        #endregion
    }
}