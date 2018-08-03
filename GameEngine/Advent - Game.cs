using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace GameEngine
{
    /// <summary>
    /// Game processing goes on in here.
    /// </summary>
    static partial class Advent
    {
        /// <summary>
        /// Process the user input from the game
        /// </summary>
        /// <param name="pWords">User input</param>
        /// <remarks>
        /// Raises the events:
        ///     GameOutput
        /// </remarks>
        public static void ProcessText(string pInput)
        {
            pInput = pInput.Trim();

            if (string.IsNullOrEmpty(pInput))
            {
                SendGameMessages(_Sysmessages[11], true); //what
                _GameData.PlayerNoun = "";
                return;
            }


            string[] words = pInput.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

            if (pInput.StartsWith("#"))
            {
                if (words.Length == 1 && CompareString(words.First(), "#undo"))
                {
                    if (!Undo())
                        SendGameMessages("A voice BOOOOMS out: \"NOTHING TO UNDO\"", true);
                    return;
                }
                else if (words.Length == 1 && CompareString(words.First(), "#redo"))
                {
                    if (!Redo())
                        SendGameMessages("A voice BOOOOMS out: \"NOTHING TO REDO\"", true);
                    return;
                }
            }

            _GameData.BeginUndo();
            _GameData.TurnCounter++;
            SendGameMessages("", true);



            string verb = words.Count() == 0 
                            ? "" 
                            : ShrinkWord(words.First());

            if (CompareString(verb, "i"))
                verb = _GameData.Verbs.First(v => v.StartsWith("INV"));

            int verbID = SearchWordList(_GameData.Verbs, verb);
            int nounID = -1;            

            if (CompareString(verb, "i"))
                verb = _GameData.Verbs.First(v => v.StartsWith("INV"));
             
            //verb not recognised or isn't direction
            if (verbID == -1)
            {
                int temp = 0;
                if ((temp = IsDirection(verb)) > -1) //is direction?
                {
                    verbID = (int)_Constants.VERB_GO;
                    nounID = temp;
                }
                if (verbID == -1)
                {
                    SendGameMessages(string.Format("\"{0}\" {1}", words.First(), _Sysmessages[1]), true); //{0} is a word I don't know...sorry!
                    _GameData.EndUndo();
                    return;
                }
            }



            if (words.Length > 1 && nounID == -1)//two words entered
            {
                _GameData.PlayerNoun = words[1];
                nounID = SearchWordList(_GameData.Nouns, _GameData.PlayerNoun.ToUpper());
            }
            else if //take / drop followed by <no word>
            (_GameData.PlayerNoun == "" && (verbID == (int)_Constants.VERB_TAKE || verbID == (int)_Constants.VERB_DROP))
            {
                SendGameMessages(_Sysmessages[11], true); //What?
                _GameData.EndUndo();
                return;
            }

            //we're now at point where the entered data appears to do something...

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

                    SendGameMessages(
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
                        SendGameMessages(_Sysmessages[18], true);
                        PerformActionComponent(63, 0, 0);
                        _GameData.EndUndo();
                        return;
                    }
                    else
                        SendGameMessages(_Sysmessages[2], true);
                }

            }
            else
            {
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
        /// Is it dark?
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
                foreach (int[] act in pAction.Actions.Where(a => a[0] > 0))
                    PerformActionComponent(act[0], act[1], act[2]);

                return true;
            }
            return false;
        }

        /// <summary>
        ///  Examine the enture actions list for entries with a matching verb/noun
        /// </summary>
        /// <param name="pVerb">Int</param>
        /// <param name="pNoun">Int</param>
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
                        ChildActions(act.Children);
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
                msg = 2;

            if (pVerb > 0)
            { //only do after user input

                if (!parentOutcome)
                {
                    if (msg == 1) //don't understand
                        SendGameMessages(_Sysmessages[15], true);
                    else if (msg == 2) //Can't do that yet
                        SendGameMessages(_Sysmessages[14], true);
                }
                SearchActions(0, 0); //auto actions
            }

            //lamp stuff, is it in the game
            if (CheckCondition(13, (int)_Constants.LIGHTSOURCE))
            {

                if (_GameData.LampLife == 0)
                {
                    _GameData.ChangeBitFlag((int)_Constants.LIGHOUTFLAG, true);
                    SendGameMessages(_Sysmessages[19], false);
                    _GameData.LampLife = 0;
                }
                else if (_GameData.LampLife > 0 && _GameData.LampLife < 25 &&
                  CheckCondition(3, (int)_Constants.LIGHTSOURCE) &&
                  _GameData.LampLife % 5 == 0)
                    SendGameMessages(_Sysmessages[20], false); //light growing dim
            }

            PerformActionComponent(64, 0, 0); //look
        }

        /// <summary>
        /// recurse through any children
        /// </summary>
        /// <param name="pActions"></param>
        private static void ChildActions(GameData.Action[] pActions)
        {
            foreach (GameData.Action c in pActions)
            {
                ExcecuteAction(c);
                if (c.Children != null)
                    ChildActions(c.Children);
            }
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
        /// <returns>-1 if not present, else index of match</returns>
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
        /// Perform string comparison
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        private static bool CompareString(string a, string b)
        {
            return a.Equals(b, StringComparison.OrdinalIgnoreCase);
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
                SendGameMessages(_GameData.Messages[pAct - (pAct > 101 ? 50 : 0)], false);
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
                            SendGameMessages(_Sysmessages[8], true);
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
                        SendGameMessages(_Sysmessages[24], false);
                        break;

                    case 62: //item is moved to room
                        _GameData.ChangeItemLocation(pArg1, pArg2);
                        break;

                    case 63: //game over
                        _GameData.EndGame = true;
                        SendGameMessages(_Sysmessages[25], false);
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

                        SetRoomView();

                        break;

                    case 65: //score
                        int storedItems = _GameData.Items.Count(i => i.Location == _GameData.Header.TreasureRoom
                                && i.Description.StartsWith("*"));

                        SendGameMessages(string.Format(
                                            _Sysmessages[13]
                                            , storedItems
                                            , Math.Floor((storedItems * 1.0 / _GameData.Header.TotalTreasures) * 100)), false);

                        if (storedItems == _GameData.Header.TotalTreasures)
                        {
                            SendGameMessages(_Sysmessages[26], true);
                            PerformActionComponent(63, 0, 0);
                        }

                        break;

                    case 66: // output inventory

                        string[] items = GetItemsAt(_InventoryLocations)
                                        .Select(i => i.Description)
                                        .ToArray();

                        SendGameMessages(_Sysmessages[9] +
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
                        SendGameMessages("", true);
                        _RoomView = null;
                        break;

                    case 71: //save game

                        SendGameMessages(string.Format("Game {0} saved", _GameData.SaveSnapshot()), true);
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
                        SendGameMessages(_GameData.CurrentCounter + "\r\n", false);
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
                        SendGameMessages(_GameData.PlayerNoun, false);
                        break;

                    case 85: //echo noun
                        SendGameMessages(_GameData.PlayerNoun + "\r\n", false);
                        break;

                    case 86: //Carriage Return"
                        SendGameMessages("\r\n", false);
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
            foreach (int[] con in pConds.Where(c => c[0] > 0))
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
    }
}