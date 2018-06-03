using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Linq;

namespace Adventurer
{
    /// <summary>
    /// Contains the unpacked and processed game data from a .DAT file
    /// </summary>
    class GameData
    {

        private static string _ObviousExits = "Obvious exits: "; //5
        private static string _None = "none.\r\n";

        public GameHeader Header;
        public GameFooter Footer;
        public Room[] Rooms = null;
        public Action[] Actions = null;
        public string[] Verbs = null;
        public string[] Nouns = null;
        public string[] Messages = null;
        public string GameName = null;
        public int TurnCounter { get; set;}

        public UndoBlock CurrentUndoBlock;
        public bool RecordUndo { get; set; }
        public List<UndoBlock> UndoHistory { get; set; }
        public int UndoPosition { get; set; }

        private string CurrentFolder
        {
            get
            {
                return System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
            }
        }

        #region public instance methods

        public GameData()
        {
            RecordUndo = false;
            TakeSuccessful = false;
            BitFlags = new bool[32];
            Counters = new int[32];
            SavedRooms = new int[32];
            PlayerNoun = null;
            EndGame = false;
            RecordUndo = true;
            CurrentUndoBlock = new UndoBlock();
            UndoHistory = new List<UndoBlock>();
            UndoPosition = -1;
        }

        /// <summary>
        /// Save the current game
        /// </summary>
        /// <param name="pSaveName">Filename to save to</param>
        /// <param name="pSave">GameData class to extract save data</param>
        public string SaveSnapshot()
        {
            string gameRoot = GameName.Substring(0, GameName.IndexOf("."));
            int saves = Directory.GetFiles(CurrentFolder, gameRoot + "_*.sav").Length + 1;
            string sg;
            using (StreamWriter sw = new StreamWriter((sg=string.Format("{0}_{1}.sav", gameRoot, saves))))
            {
                //write header
                sw.WriteLine(string.Format("\"{0}\"", this.GameName));
                sw.WriteLine(this.Items.Count(i => i.Moved()));
                sw.WriteLine(this.BitFlags.Count());
                sw.WriteLine(this.Counters.Count());
                sw.WriteLine(this.SavedRooms.Count());


                //get all the changed items
                this.Items.Select((item, indx) => new { item, indx })
                    .Where(i => i.item.Moved())
                    .All(i => { sw.WriteLine(i.indx); sw.WriteLine(i.item.Location); return true; });

                this.BitFlags.Select((bf, indx) => new { bf, indx })
                    .All(i => { sw.WriteLine(i.indx); sw.WriteLine(i.bf ? 1 : 0); return true; });

                this.Counters.Select((ct, indx) => new { ct, indx })
                    .All(i => { sw.WriteLine(i.indx); sw.WriteLine(i.ct); return true; });

                this.SavedRooms.Select((sr, indx) => new { sr, indx })
                    .All(i => { sw.WriteLine(i.indx); sw.WriteLine(i.sr); return true; });


                sw.WriteLine(this.CurrentRoom);
                sw.WriteLine(this.TakeSuccessful ? 1 : 0);
                sw.WriteLine(this.CurrentCounter);
                sw.WriteLine(this.LampLife);
                sw.WriteLine(this.PlayerNoun);
                sw.WriteLine(this.SavedRoom);
                sw.WriteLine(this.TurnCounter);

            }

            return sg;
        }

        #endregion

        #region public static methods

        /// <summary>
        /// Load the provided snap shot
        /// </summary>
        /// <param name="pSaveName">Snapshot to load</param>
        /// <returns>GameData class</returns>
        public static GameData LoadSnapShot(string pAdvGame, string pSnapShot)
        {

            GameData gd = Load(pAdvGame);

            Datafile.Load(pSnapShot);
            Datafile.getTokens(1);//skip the first line.

            int[] header = Datafile.GetTokensAsInt(4);

            //header[0] = changed items - multiply number by two as they are in pairs of index id and new location
            //header[1] = bitflags  - pairs index, value
            //header[2] = counters - pairs index, value
            //header[3] = saved rooms - pairs index, value

            //get header
            int[] intarray = Datafile.GetTokensAsInt(header[0] * 2);
            for (int i = 0; i < intarray.Length; i += 2)
                gd.Items[intarray[i]].Location = intarray[i + 1];

            //bit glags
            intarray = Datafile.GetTokensAsInt(header[1] * 2);
            for (int i = 0; i < intarray.Length; i += 2)
                gd.BitFlags[intarray[i]] = intarray[i + 1] == 1;

            intarray = Datafile.GetTokensAsInt(header[2] * 2);
            for (int i = 0; i < intarray.Length; i += 2)
                gd.Counters[intarray[i]] = intarray[i + 1];

            intarray = Datafile.GetTokensAsInt(header[3] * 2);
            for (int i = 0; i < intarray.Length; i += 2)
                gd.SavedRooms[intarray[i]] = intarray[i + 1];

            gd.CurrentRoom = Datafile.GetTokensAsInt(1).First();
            gd.TakeSuccessful = Datafile.GetTokensAsInt(1).First() == 1;
            gd.CurrentCounter = Datafile.GetTokensAsInt(1).First();
            gd.LampLife = Datafile.GetTokensAsInt(1).First();
            gd.PlayerNoun = Datafile.getTokens(1).First();
            gd.SavedRoom = Datafile.GetTokensAsInt(1).First();
            gd.TurnCounter = Datafile.GetTokensAsInt(1).First();



            return gd;
        }

        /// <summary>
        /// Load the adventure game from the provided dat file
        /// </summary>
        /// <param name="pFile"></param>
        /// <returns>Game data class</returns>
        public static GameData Load(string pFile)
        {

            string[] directionsLong = { "North", "South", "East", "West", "Up", "Down" };
            int VERB_TAKE = 10;
            int VERB_DROP = 18;

            GameData gd = new GameData();

            Datafile.Load(pFile);


            int[] header = Datafile.GetTokensAsInt(12);

            gd.Header = new GameHeader(header);
            gd.Verbs = new string[gd.Header.NumNounVerbs];
            gd.Nouns = new string[gd.Header.NumNounVerbs];
            gd.Rooms = new Room[gd.Header.NumRooms];
            gd.Messages = new string[gd.Header.NumMessages];
            gd.Items = new Item[gd.Header.NumItems];
            gd.GameName = pFile;
            gd.CurrentRoom = gd.Header.StartRoom;
            gd.LampLife = gd.Header.LightDuration;


            int ctr = 0;

            List<Action> Actions = new List<Action>();

            #region Actions

            for (ctr = 0; ctr < gd.Header.NumActions; ctr++)
                Actions.Add(new Action(Datafile.GetTokensAsInt(8)));


            #endregion

            #region Words

            /*
             * An interleaved list of verb/noun that begins
             * with the entries "AUT" and "ANY" that we skip
             * 
             * An entry beginning with a star is a synonym of the first
             * preceeding word that doesn't begin with a star
             */

            int v = 0;
            int n = 0;
            string[] word = Datafile.getTokens(gd.Header.NumNounVerbs * 2);

            for (ctr = 0/*SKIP*/; ctr < word.Count(); ctr++)
            {

                if (ctr % 2 == 0)
                {

                    gd.Verbs[v] = word[ctr];

                    if (gd.Verbs[v].StartsWith("*") & gd.Verbs[v].Length > (gd.Header.WordLength + 1))
                        gd.Verbs[v] = gd.Verbs[v].Substring(0, gd.Header.WordLength + 1);
                    else if (!gd.Verbs[v].StartsWith("*") & gd.Verbs[v].StartsWith("*") && word[ctr].Length > gd.Header.WordLength)
                        gd.Verbs[v] = gd.Verbs[v].Substring(0, gd.Header.WordLength);

                    v++;
                }
                else
                {
                    gd.Nouns[n] = word[ctr];

                    if (gd.Nouns[n].StartsWith("*") & gd.Nouns[n].Length > (gd.Header.WordLength + 1))
                        gd.Nouns[n] = gd.Nouns[n].Substring(0, gd.Header.WordLength + 1);
                    else if (!gd.Nouns[n].StartsWith("*") & word[ctr].Length > gd.Header.WordLength)
                        gd.Nouns[n] = gd.Nouns[n].Substring(0, gd.Header.WordLength);

                    n++;
                }

            }

            #endregion

            #region Rooms

            for (ctr = 0; ctr < gd.Rooms.Length; ctr++)
            {
                gd.Rooms[ctr] = new Room(Datafile.GetTokensAsInt(6), Datafile.getTokens(1).First());


                gd.Rooms[ctr].Description += "\n\n" + _ObviousExits;

                if (gd.Rooms[ctr].Exits.Count(e => e > 0) > 0)
                {
                    gd.Rooms[ctr].Description +=
                        gd.Rooms[ctr].Exits
                                .Select((val, ind) => new { val, ind })
                                .Where(i => i.val > 0)
                                .Select(i => directionsLong[i.ind])
                                .Aggregate((current, next) => current + ", " + next);

                }
                else
                {
                    gd.Rooms[ctr].Description += _None; //none
                }


            }


            #endregion

            #region Build Game Messages

            gd.Messages = Datafile.getTokens(gd.Messages.Length);

            #endregion

            #region Items

            for (ctr = 0; ctr < gd.Items.Length; ctr++)
                gd.Items[ctr] = new Item(Datafile.getTokens(1).First(), Datafile.GetTokensAsInt(1).First());

            #endregion

            #region Add any comments to actions

            for (ctr = 0; ctr < gd.Header.NumActions; ctr++)
                Actions[ctr].Comment = Datafile.getTokens(1).First();

            #endregion

            #region Generate get/drop actions for items that can be carried

            for (int itemCtr = 0; itemCtr < gd.Items.Count(); itemCtr++)
            {
                if (gd.Items[itemCtr].Word != null)
                {
                    Actions.Add
                        (
                            new Action()
                            {
                                Comment = "Autotake for " + gd.Items[itemCtr].Description
                                ,
                                Verb = VERB_TAKE
                                ,
                                Noun = gd.Nouns.TakeWhile(nn => nn != gd.Items[itemCtr].Word).Count()
                                ,
                                Conditions = new int[][] { new int[] { 2, itemCtr } }
                                ,
                                Actions = new int[][] { new int[] { 52, itemCtr, 0 } }
                            }
                        );

                    Actions.Add
                        (
                            new Action()
                            {
                                Comment = "Autodrop for " + gd.Items[itemCtr].Description
                                ,
                                Verb = VERB_DROP
                                ,
                                Noun = gd.Nouns.TakeWhile(nn => nn != gd.Items[itemCtr].Word).Count()
                                ,
                                Conditions = new int[][] { new int[] { 1, itemCtr } }
                                ,
                                Actions = new int[][] { new int[] { 53, itemCtr, 0 } }
                            }
                        );
                }

            }

            #endregion



            //      Child action processing

            //      All actions that follow an action with a component 73 are noun == 0 && verb == 0
            //      and are the children of that action. This method moves them into a array of
            //      their parent
            //      e.g 157 in Adv01.dat
            List<Action> childs = null;
            for (int i = Actions.Count() - 1; i >= 0; i--)
            {
                if (Actions[i].Actions.Count(act => act[0] == 73) > 0)
                {
                    int j = i + 1;
                    childs = new List<Action>();
                    while (Actions[j].Verb == 0 && Actions[j].Noun == 0)
                    {
                        childs.Add(Actions[j]);
                        j++;
                    }

                    Actions[i].Children = childs.ToArray();
                    Actions.RemoveRange(i + 1, childs.Count());
                }
            }

            /*
             Claymorgue castle fix

             Action 148 has no conditions and a noun and a verb of 0, 
             and follows a user tiggered action. This particular set of 
             conditions only occurs in Claymorgue and none of the other 13
             adventures. I think that when this set of conditions occurs,
             the latter action should be treated as a child of the former, 
                and would require some special treatment when the DAT file is loaded.
            */
            Action ac;
            for (var i = 1; i < Actions.Count(); i++)
            {
                ac = Actions[i];

                if (
                        Actions[i].Conditions.Length == 0
                        && Actions[i].Actions.Length > 0
                        && Actions[i].Verb == 0
                        && Actions[i].Noun == 0
                        && Actions[i - 1].Verb > 0
                    )
                {

                    Actions[i - 1].Children =
                        new Action[] { Actions[i] };

                    Actions[i] = null;
                }
            }

            //If words aren't present for SAV GAM add them
            //for adv06, 07, 09, 10, 11, 12, 13, 14a
            #region Add save game

            if (!gd.Nouns.Contains("GAM") && !gd.Verbs.Contains("SAV"))
            {

                Array.Resize(ref gd.Nouns, gd.Nouns.Length + 1);
                gd.Nouns[gd.Nouns.Length - 1] = "GAM";

                Array.Resize(ref gd.Verbs, gd.Verbs.Length + 1);
                gd.Verbs[gd.Verbs.Length - 1] = "SAV";

                Actions.Add(
                        new Action(
                                new int[]
                                {
                                    (gd.Verbs.Length - 1) * 150     //verb
                                    + (gd.Nouns.Length - 1)   //noun
                                    , 0 //condition 1
                                    , 0 //condition 2
                                    , 0 //condition 3
                                    , 0 //condition 4
                                    , 0 //condition 5
                                    , 71 * 150
                                    , 0
                                }
                            )
                    );

            }

            #endregion


            gd.Actions = Actions.Where(a => a != null).ToArray();

            gd.Footer = new GameFooter(Datafile.GetTokensAsInt(3));






            #region create first undo

            gd.RecordUndo = true;
            gd.BeginUndo();

            gd.CurrentRoom = gd.Header.StartRoom;
            gd.TakeSuccessful = false;
            gd.CurrentCounter = 0;
            gd.LampLife = gd.Header.LightDuration;
            gd.PlayerNoun = "";
            gd.SavedRoom = 0;
            gd.EndGame = false;
            gd.UndoPosition = -1;


            for (ctr = 0; ctr < gd.BitFlags.Length; ctr++)
                gd.ChangeBitFlag(ctr, gd.BitFlags[ctr]);

            for (ctr = 0; ctr < gd.Counters.Length; ctr++)
                gd.ChangeCounter(ctr, gd.Counters[ctr]);

            for (ctr = 0; ctr < gd.Items.Count(); ctr++)
                gd.ChangeItemLocation(ctr, gd.Items[ctr].Location);

            gd.EndUndo();
            #endregion




            return gd;

        }

        #endregion

        #region output dat as XML

        public void SaveAsUnFormattedXML(string pFile)
        {
            Datafile.Load(pFile);

            XElement gameData = new XElement("GameData");

            string[] headerDesc =
             { "Unknown"
             , "NumItems"
             , "NumActions"
             , "NumNounVerbs"
             , "NumRooms"
             , "MaxCarry"
             , "StartRoom"
             , "TotalTreasures"
             , "WordLength"
             ,"LightDuration"
             , "NumMessages"
             ,"TreasureRoom"};


        int[] header = Datafile.GetTokensAsInt(12);
            gameData.Add
                (
                    new XElement("Header", header.Select((val, ind) => new XElement(String.Format("{0}", headerDesc[ind]), val)))
                );

            string[] actionsDesc = { "NounVerb", "Condition1", "Condition2", "Condition3", "Condition4", "Condition5", "Action1", "Action2", "Action3" };
            gameData.Add
                (
                    new XElement("Actions",

                        Enumerable.Range(0, header[2] + 1).
                            Select(a => new XElement("Action", new XAttribute("Index",a),Datafile.GetTokensAsInt(8).Select((val, ind) => new XElement(String.Format("{0}", actionsDesc[ind]), val)))
                            ))
                );

            gameData.Add
                (
                    new XElement("Words",

                        Enumerable.Range(0, (header[3] + 1) * 2).
                            Select(a => Datafile.getTokens(1).Select(val => new XElement("Word", val)))
                            )
                );


            string[] roomDesc = { "North", "South", "East", "West", "Up", "Down", "Description" };
            gameData.Add
                (
                    new XElement("Rooms",

                        Enumerable.Range(0, header[4] + 1).
                            Select(a =>
                                        new XElement("Room"
                                            , new XAttribute("Index", a)
                                            , new object[] {
                                             Datafile.getTokens(7).Select((val, ind) => new XElement(String.Format("{0}", roomDesc[ind]), val.Trim())) }
                                             ))


                            )
                            
                );



            gameData.Add
                (
                    new XElement("Messages",
                        Enumerable.Range(0, header[10] + 1).
                            Select(a => new XElement("Message", new XAttribute("Index", a),Datafile.getTokens(1))))
                );

            string[] itemDescripion = { "Name", "Word" };
            gameData.Add
                (
                    new XElement("Items",
                        Enumerable.Range(0, header[1] + 1).
                            Select(a => new XElement("Item",
                                new XAttribute("Index", a),
                                Datafile.getTokens(1).First().Split(new char[] { '/' }, StringSplitOptions.RemoveEmptyEntries)
                                    .Select((v,i) => new XElement(itemDescripion[i], v))
                                    , new XElement("Location", Datafile.GetTokensAsInt(1))
                            )))
                );

            gameData.Add
                (
                    new XElement("Comments",
                        
                        Enumerable.Range(0, header[2] + 1).
                            Select(a => new XElement("Comment", new XAttribute("Index", a), Datafile.getTokens(1))
                            ))
                );

            string[] commentDesc = { "Version", "AdventureNumber", "Unkown" };
            ; gameData.Add
                 (
                     new XElement("Footer", Datafile.getTokens(3).Select((val, ind) => new XElement(String.Format("{0}", commentDesc[ind]), val)))
                 );

            gameData.Save(GameName + ".unformatted.xml");

        }

        /// <summary>
        /// convert the loaded game into easy to read XML
        /// </summary>
        public void SaveAsFormattedXML()
        {
            XElement gameData = new XElement("GameData");
            gameData.Add(
                       new XElement("Header"
                            , new XElement("FileName", GameName)
                            , new XElement("Unknown", Header.Unknown)
                            , new XElement("MaxCarry", Header.MaxCarry)
                            , new XElement("StartRoom", Header.StartRoom)
                            , new XElement("TotalTreasures", Header.TotalTreasures)
                            , new XElement("WordLength", Header.WordLength)
                            , new XElement("LightDuration", Header.LightDuration)
                            , new XElement("TreasureRoom", Header.TreasureRoom)
                       )
                );

            //
            gameData.Add(new XElement("Actions"
                    , new XAttribute("Count", Actions.Count())
                    , Actions.Select((val, ind) => MakeAction(val, ind, false)))
                );


            //add words
            gameData.Add(
                   new XElement("Words"

                        , new XElement("Verbs", new XAttribute("Count", Verbs.Count())
                        , Verbs.Select((val, ind) => new { index = ind, value = val })
                            .Where(v => !v.value.StartsWith("*"))
                            .Select(v =>
                                new XElement("Verb", new object[] { new XAttribute("value", v.value), new XAttribute("index", v.index)
                                , from al in Verbs.Skip(v.index + 1).TakeWhile(al => al.StartsWith("*")) select new XElement("Alias", al) }
                                ))
                                )
                        , new XElement("Nouns", new XAttribute("Count", Nouns.Count())
                        , Nouns.Select((val, ind) => new { index = ind, value = val })
                            .Where(v => !v.value.StartsWith("*") & v.value.Length > 0)
                            .Select(v =>
                                new XElement("Noun", new object[] { new XAttribute("value", v.value), new XAttribute("index", v.index)
                                , from al in Nouns.Skip(v.index + 1).TakeWhile(al => al.StartsWith("*")) select new XElement("Alias", al) }
                                ))
                            )
                            )
                );

            //add rooms
            string[] directionsLong = { "North", "South", "East", "West", "Up", "Down" };
            gameData.Add(
                   new XElement("Rooms"
                   , new XAttribute("Count", Rooms.Count())
                        , Rooms.Select((val, ind) => new XElement("Room",
                            new object[] {
                                new XAttribute("index", ind)
                                , new XElement("Description", val.RawDescription)
                                , val.Exits.Select((v,i)=> new XElement(directionsLong[i], v))
                            })))

                );


            //messaes
            gameData.Add(
                new XElement("Messages"
                , new XAttribute("Count", Messages.Count())
                , Messages.Select((val, ind) => new XElement("Message",
                new object[] {
                                new XAttribute("index", ind)
                                , val
                })))
                );

            gameData.Add(
                new XElement("Items"
                , new XAttribute("Count", Items.Count())
                , Items.Select((val, ind) => new XElement("Item",
                new object[] {
                                new XAttribute("index", ind)
                                , new XElement("Description", val.Description)
                                , new XElement("RoomID", val.Location)
                                , val.Word != null ? new XElement("Word", val.Word) :  null
                }))));

            gameData.Add(
                new XElement("Footer"
                        , new XElement("Version", Footer.Version)
                        , new XElement("AdventureNumber", Footer.AdventureNumber)
                        , new XElement("Unknown", Footer.Unknown)
                    )
                );

            gameData.Save(GameName + ".xml");
        }

        static string[] conditions = {"item arg carried"
                        ,"item arg in room with player"
                        ,"item arg carried or in room with player"
                        ,"player in room arg"//3
                        ,"item arg not in room with player"//4
                        ,"item arg not carried" //5
                        ,"player not it room arg"   //6
                        ,"bitflag arg is set"
                        ,"bitflag arg is false"
                        ,"something carried"
                        ,"nothing carried"
                        ,"item arg not carried or in room with player"//11
                        ,"item arg in game"//12
                        ,"item arg not in game"//13
                        ,"current counter less than arg"//14
                        ,"current counter greater than arg"//15
                        ,"object arg in initial location" //16
                        ,"object arg not in initial location"//17
                        ,"current counter equals arg"};

        int[] conditionsWithItems = { 0, 1, 2, 5, 11, 12, 13, 16, 17 };


        static string[] actions =
            { "get item ARG1, check if can carry"   //52
                ,"drops item ARG1 into current room"    //53
                ,"move room ARG1"       //54
                ,"Item ARG1 is removed from the game (put in room 0)"   //55
                ,"set darkness flag"
                ,"clear darkness flag"
                ,"set ARG1 flag"    //58
                ,"Item ARG1 is removed from the game (put in room 0)"//59
                ,"set ARG1 flag"   //60
                ,"Death, clear dark flag, move to last room"
                ,"item ARG1 is moved to room ARG2"  //62
                ,"game over"
                ,"look"
                ,"score"//65
                ," output inventory"
                ,"Set bit 0 true"
                ,"Set bit 0 false"
                ,"refill lamp"
                ,"clear screen"
                ,"save game"
                ,"swap item locations ARG1 ARG2"//72
                ,"continue with next action"
                ,"take item ARG1, no check done to see if can carry"//74
                ,"put item 1 ARG1 with item2 ARG2"//75
                ,"look"
                ,"decement current counter"//77
                ,"output current counter"//77
                ,"set current counter value arg1"
                ,"swap location with saved location"
                ,"Select counter arg1. Current counter is swapped with backup counter"//80
                ,"add to current counter"//81
                ,"subtract from current counter"//82
                ,"echo noun without cr"//83
                ,"echo noun"
                ,"Carriage Return"//85
                ,"Swap current location value with backup location-swap value"//86
                ,"wait 2 seconds"};

        static int[] twoArgActions = { 62, 72, 75 };
        static int[] oneActionArgs = { 52, 53, 54, 55, 58, 59, 60, 74, 78, 81, 82, 83, 79 };


        static int[] actionArgsWithOneItem = { 52, 53, 55, 59, 74, 62 }; //note 62, a two arg action which moves item arg1 to room arg2
        static int[] actionsWithTwoItems = { 72, 75 };

        /// <summary>
        /// Build an XML element for the provided action in a big, messy statement which I had a lot of fun writing ;)
        /// </summary>
        /// <param name="pAction"></param>
        /// <param name="pIndex"></param>
        /// <param name="pIsChild"></param>
        /// <returns></returns>
        private XElement MakeAction(Action pAction, int pIndex, bool pIsChild)
        {
            return new XElement(pIsChild ? "ChildAction" : "Action"
                    , new object[]
                    {
                        new XAttribute("index", pIndex)
                        , pIsChild  || pAction.Children == null ? null:  new XAttribute("HasChildren", pAction.Children != null)
                        , pAction.Verb == 0 ? new XAttribute("Auto", pAction.Noun) : new XAttribute("Input", string.Format("{0} {1}", Verbs[pAction.Verb], Nouns[pAction.Noun]))
                        , new XElement("Verb", pAction.Verb)
                        , new XElement("Noun", pAction.Noun)
                        ,
                            !string.IsNullOrWhiteSpace(pAction.Comment)  //comments may be stored in the DAT file, so used those preferentially
                            ? new XElement("Comment", pAction.Comment)
                            : null

                        , new XElement("Conditions"
                            , pAction.Conditions.Where(con => con[0] > 0)
                                .Select(con=>
                                    new XElement("Condition"
                                    , new XElement("ConditionID", con[0]-1)
                                    , new XElement("Description", conditions[con[0]-1])
                                    , new XElement("arg", con[1] )
                                    , conditionsWithItems.Contains(con[0]-1) ? new XElement("Arg1Item", Items[con[1]].Description) : null
                                    )
                        ))

                        , new XElement("ActionComponents"
                            , pAction.Actions.Where(act => act[0] > 0)
                                .Select(act=> new XElement("Action"
                                    , new XElement("Description",

                                            //this beast of a statement determines how the description of the
                                            //action is generated
                                            (act[0] > 0 & act[0] < 52)
                                                ? String.Format("A Output message: {0}", replaceChars(Messages[act[0]], new char[] {'\r','\n' }))//output message
                                                : act[0] > 101
                                                    ? String.Format("A Output message: {0}", replaceChars(Messages[act[0]-50], new char[] {'\r','\n' }))    //output message
                                                    //vvv Output a string based on the number of args the action has
                                                    : oneActionArgs.Contains(act[0])
                                                        ? string.Format("{0}: {1}", actions[act[0] - 52], act[1])
                                                        : twoArgActions.Contains(act[0])
                                                            ? string.Format("{0}: {1} {2}", actions[act[0] - 52], act[1], act[2])
                                                            : string.Format("{0}", actions[act[0] - 52])



                                    )
                                    , act[1] > 0 ? new XElement("arg1", act[1] ): null
                                    , act[2] > 0 ? new XElement("arg2", act[2] ): null

                                    //if a two item argument output the item descriptions
                                    , actionsWithTwoItems.Contains(act[0]) ? new object[] {new XElement("Arg1Item", Items[act[1]].Description), new XElement("Arg2Item", Items[act[2]].Description) } : null

                                    //if a one item argument that uses items output the item description
                                    , actionArgsWithOneItem.Contains(act[0]) ? new XElement("Arg1Item", Items[act[1]].Description) : null

                                    )

                                    )
                        )
                                                 , pAction.Children != null ?
                            pAction.Children.Select((val,ind) => MakeAction(val,ind, true))
                            : null
                    }
                );
        }

        private string replaceChars(string pInput, char[] pReplace)
        {
            foreach (char r in pReplace)
                pInput = pInput.Replace(r, ' ');
            return pInput;
        }

        #endregion

        #region snapshot data

        public Item[] Items = null;
        public bool[] BitFlags = new bool[32];
        public int[] Counters = new int[32];
        public int[] SavedRooms = new int[32];

        private int _CurrentRoom;
        public int CurrentRoom
        {
            get { return _CurrentRoom; }
            set { AddToCurrentUndoBlock(ChangeType.Update, ChangeItem.CurrentRoom, ChangeItemDataType.Int, (object)value, (object)_CurrentRoom, 0); _CurrentRoom = value; }
        }

        private bool _takeSuccessful;
        public bool TakeSuccessful { get { return _takeSuccessful; } set { AddToCurrentUndoBlock(ChangeType.Update, ChangeItem.TakeSuccessful, ChangeItemDataType.Int, (object)value, (object)_takeSuccessful, 0); _takeSuccessful = value; } }

        private int _currentCounter;
        public int CurrentCounter { get { return _currentCounter; } set { AddToCurrentUndoBlock(ChangeType.Update, ChangeItem.CurrentCount, ChangeItemDataType.Int, (object)value, (object)_currentCounter, 0); _currentCounter = value; } }

        private int _lampLife;
        public int LampLife { get { return _lampLife; } set { AddToCurrentUndoBlock(ChangeType.Update, ChangeItem.LampLife, ChangeItemDataType.Int, (object)value, (object)_lampLife, 0); _lampLife = value; } }

        private string _playerNoun;
        public string PlayerNoun
        {
            get { return _playerNoun; }
            set { AddToCurrentUndoBlock(ChangeType.Update, ChangeItem.PlayerNoun, ChangeItemDataType.String, (object)value, (object)_playerNoun, 0); _playerNoun = value; }
        }

        private int _savedRoom;
        public int SavedRoom { get { return _savedRoom; } set { AddToCurrentUndoBlock(ChangeType.Update, ChangeItem.SavedRoom, ChangeItemDataType.Int, (object)value, (object)_savedRoom, 0); _savedRoom = value; } }

        private bool _endGame;
        public bool EndGame { get { return _endGame; } set { AddToCurrentUndoBlock(ChangeType.Update, ChangeItem.EndGame, ChangeItemDataType.Bool, (object)value, (object)_endGame, 0); _endGame = value; } }

        #endregion        

        #region undo methods


        public void SetCurrentUndo(int pIndex)
        {
            CurrentUndoBlock = UndoHistory[pIndex];
        }

        private void AddToCurrentUndoBlock(ChangeType pChanged, ChangeItem pItem, ChangeItemDataType pDataType, object pNewData, object pOldData, int pIndex)
        {
            if (RecordUndo)
                CurrentUndoBlock.Add(new ChangeRepresentationObject(pChanged, pItem, pDataType, pNewData, pOldData, pIndex));
        }


        public void BeginUndo()
        {
            CurrentUndoBlock = new UndoBlock();
        }

        public void EndUndo()
        {
            UndoHistory.Add(CurrentUndoBlock);
            UndoPosition = UndoHistory.Count() - 1;
        }

        public void ChangeItemLocation(int pIndex, int pLocation)
        {
            AddToCurrentUndoBlock(ChangeType.Update, ChangeItem.Item, ChangeItemDataType.Int, (object)pLocation, (Object)Items[pIndex].Location, pIndex);

            Items[pIndex].Location = pLocation;
        }

        public void ChangeBitFlag(int pIndex, bool pVal)
        {
            AddToCurrentUndoBlock(ChangeType.Update, ChangeItem.BitFlag, ChangeItemDataType.Bool, (object)pVal, (object)BitFlags[pIndex], pIndex);
            BitFlags[pIndex] = pVal;
        }

        public void ChangeCounter(int pIndex, int pVal)
        {
            AddToCurrentUndoBlock(ChangeType.Update, ChangeItem.Counter, ChangeItemDataType.Bool, (object)pVal, (object)Counters[pIndex], pIndex);
            Counters[pIndex] = pVal;
        }

        public class UndoBlock
        {

            public UndoBlock()
            {
                Block = new List<ChangeRepresentationObject>();
            }

            public List<ChangeRepresentationObject> Block { get; set; }

            public void Add(ChangeRepresentationObject pCRO)
            {
                Block.Add(pCRO);
            }

        }

        public enum ChangeType
        {
            Update
            , Insert
        }

        public enum ChangeItem
        {
            Item
            , BitFlag
            , Counter
            , SavedRooms
            , CurrentRoom
            , TakeSuccessful
            , CurrentCount
            , LampLife
            , PlayerNoun
            , SavedRoom
            , EndGame
        }

        public enum ChangeItemDataType
        {
            Int
            , String
            , Bool
        }

        public class ChangeRepresentationObject
        {
            public ChangeType Changed;
            public ChangeItem Item;
            public ChangeItemDataType DataType;
            public int Index;
            public object NewData;
            public object OldData;

            public ChangeRepresentationObject(ChangeType pChanged, ChangeItem pItem, ChangeItemDataType pDataType, object pNewData, object pOldData, int pIndex)
            {
                Changed = pChanged;
                Item = pItem;
                DataType = pDataType;
                NewData = pNewData;
                OldData = pOldData;
                Index = pIndex;
            }
        }
        #endregion

        #region game structure classes

        internal class GameHeader
        {
            public GameHeader(int[] pVals)
            {
                Unknown = pVals[0];
                NumItems = pVals[1] + 1;
                NumActions = pVals[2] + 1;
                NumNounVerbs = pVals[3] + 1;
                NumRooms = pVals[4] + 1;
                MaxCarry = pVals[5];
                StartRoom = pVals[6];
                TotalTreasures = pVals[7];
                WordLength = pVals[8];
                LightDuration = pVals[9];
                NumMessages = pVals[10] + 1;
                TreasureRoom = pVals[11];
            }

            public int Unknown { get; private set; }
            public int NumItems { get; private set; }
            public int NumActions { get; private set; }
            public int NumNounVerbs { get; private set; }
            public int NumRooms { get; private set; }
            public int MaxCarry { get; private set; }
            public int StartRoom { get; private set; }
            public int TotalTreasures { get; private set; }
            public int WordLength { get; private set; }
            public int LightDuration { get; private set; }
            public int NumMessages { get; private set; }
            public int TreasureRoom { get; private set; }
        }

        internal class GameFooter
        {
            public GameFooter(int[] pVals)
            {
                Version = pVals[0];
                AdventureNumber = pVals[1];
                Unknown = pVals[2];
            }

            public int Version { get; set; }
            public int AdventureNumber { get; set; }
            public int Unknown { get; set; }
        }


        internal class Room
        {
            public Room(int[] pExits, string pDescription)
            {
                Description = pDescription;
                RawDescription = pDescription;  //used for debug
                Exits = pExits;
            }

            public string Description { get; set; }
            public string RawDescription { get; set; }
            public int[] Exits { get; private set; }

            /// <summary>
            /// Output the class as it's corresponding DAT file entry
            /// </summary>
            /// <returns></returns>
            public override string ToString()
            {
                return string.Format("\"{0}\"\r\n{1}", Description, Exits.Select(i => i.ToString() + "\r\n"));
            }

        }

        internal class Item
        {
            public Item(string pDescription, int pLocation)
            {
                //an item description contains an associated word if a slash if present
                //this means the item can be taken and dropped
                string[] val = pDescription.Split(new char[] { '/' });

                Description = val[0];
                Location = pLocation;
                OriginalLocation = pLocation;
                if (val.Length > 1)
                    Word = val[1];
                else
                    Word = null;
            }

            public void Reset()
            {
                Location = OriginalLocation;
            }

            public bool Moved()
            {
                return OriginalLocation != Location;
            }

            public string Description { get; private set; }
            public int Location { get; set; }
            private int OriginalLocation { get; set; }

            /// <summary>
            /// If an item has one of these it can be picked up without a special action 
            /// created for it
            /// </summary>
            public string Word { get; private set; }

            public override string ToString()
            {
                return String.Format("\"{0}{1}\" {2}", Description, String.IsNullOrEmpty(Word) ? "" : "/" + Word + "/", OriginalLocation);
            }
        }

        internal class Action
        {
            public Action()
            {

            }
            public Action(int[] pData)
            {
                Comments = new List<string>();

                /*
                    8 item integer array representing an action

                    [0] verb/noun
                    [1 - 5] conditons
                    [6 - 7] actions

                */

                Verb = pData[0] / 150;
                Noun = pData[0] % 150;

                //5 conditions
                Conditions = pData.Skip(1)
                                    .Take(5)
                                    //.Where(con => con % 20 > 0)
                                    .Select(con => con % 20 > 0
                                                    ? new int[] { con % 20, con / 20 }
                                                    : new int[] { 0, 0 })
                                    .ToArray();


                //action args are stored in conditions
                int[] actarg = pData.Skip(1)
                                    .Take(5)
                                    .Where(con => con % 20 == 0)
                                    .Select(con => con / 20)
                                    .ToArray();



                //get all four arguments
                Actions = pData
                                 .Skip(6)
                                 .Take(2)
                                   //.Where(val => val > 0)
                                   .Select(val =>
                                            val > 0 ?
                                                val % 150 > 0
                                                ? new int[][] { new int[] { val / 150, 0, 0 }, new int[] { val % 150, 0, 0 } }
                                                : new int[][] { new int[] { val / 150, 0, 0 }, new int[] { 0, 0, 0 } }
                                            : new int[][] { new int[] { 0, 0, 0 }, new int[] { 0, 0, 0 } })
                                   .SelectMany(val => val)
                                   .ToArray();


                int aaPos = 0;
                //asign the action args to the action
                foreach (int[] a in Actions)
                {
                    switch (a[0])
                    {
                        //require 1 argument
                        case 52:
                        case 53:
                        case 54:
                        case 55:
                        case 58:
                        case 59:
                        case 60:
                        case 74:
                        case 81:
                        case 82:
                        case 83:
                        case 87:
                        case 79:    //set current counter
                            a[1] = actarg[aaPos];
                            aaPos++;
                            break;

                        //actipons that require 2 args
                        case 62:
                        case 72:
                        case 75:
                            a[1] = actarg[aaPos];
                            a[2] = actarg[aaPos + 1];
                            aaPos += 2;
                            break;
                    }
                }

            }

            public int Verb { get; set; }
            public int Noun { get; set; }
            public int[][] Conditions { get; set; }
            public int[][] Actions { get; set; }
            public string Comment { get; set; }
            public Action[] Children { get; set; }
            List<string> Comments;

            /// <summary>
            /// Output the action as a scott free format
            /// </summary>
            /// <returns></returns>
            private int[] ToDat()
            {
                int VN = Verb * 150 + Noun;

                int[] con = Conditions
                                .Select((val, ind) => val[0] > 0
                                                        ? val[0] + val[1] * 20
                                                        : 0)
                                .ToArray();


                int[] args = Actions
                                .SelectMany(a => a.Skip(1))
                                .Where(a => a > 0)
                                .Select(a => a * 20)
                                .ToArray();

                //add the args back into the empty condition blocks
                int argctr = 0;
                if (args.Count(a => a > 0) > 0)
                {
                    for (int ctr = 0; ctr < con.Length; ctr++)
                    {
                        if (con[ctr] == 0)
                        {
                            con[ctr] = args[argctr];
                            argctr++;
                        }
                        if (argctr > args.Count()-1)
                            break;
                    }
                }

                int[] acts =
                    {
                        Actions[0][0] > 0
                            ? Actions [0][0] * 150 + Actions[1][0]
                            : 0
                        ,

                        Actions[2][0] > 0
                            ? Actions [2][0] * 150 + Actions[3][0]
                            : 0

                    };

                return new int[]
                    {
                        VN
                        , con[0]
                        , con[1]
                        , con[2]
                        , con[3]
                        , con[4]
                        , acts[0]
                        , acts[1]

                    };


            }

            public override string ToString()
            {
                return String.Join("\r\n", ToDat().Select(c => c.ToString()));

            }


        }
        #endregion

        #region load 

        /// <summary>
        /// Load the DAT game file and break it into chunks
        /// </summary>
        private static class Datafile
        {
            private static string file = null;
            private static int pos = 0;

            public static void Load(string pFile)
            {
                pos = 0;
                file = System.IO.File.ReadAllText(pFile).Trim();
            }

            static string[] le = new string[] { "\n", "\r" };


            public static bool EOF { get { return !(pos < file.Length); } }

            /// <summary>
            /// Get the required number of DAT chunks as string
            /// </summary>
            /// <param name="pCount"></param>
            /// <returns></returns>
            public static string[] getTokens(int pCount)
            {
                string[] retval = new string[pCount];
                int ctr = 0;

                while (ctr < pCount)
                {


                    switch (file[pos].ToString())
                    {
                        case "\"":

                            do
                            {
                                retval[ctr] += file[pos];
                                pos++;
                            } while (!EOF && file[pos].ToString() != "\"");

                            break;

                        default:
                            do
                            {
                                retval[ctr] += file[pos];
                                pos++;
                            } while (!EOF && file[pos] != '\n');
                            break;
                    }

                    do
                    {
                        pos++;
                    } while (!EOF && le.Contains(file[pos].ToString()));


                    retval[ctr] = retval[ctr].Trim(new char[] { ' ', '"', '\r','\n' });


                    ctr++;
                }
                return retval;
            }

            /// <summary>
            /// Get the required number of DAT chunks as int
            /// </summary>
            /// <param name="pCount"></param>
            /// <returns></returns>
            public static int[] GetTokensAsInt(int pCount)
            {
                return getTokens(pCount).Select(i => Convert.ToInt32(i)).ToArray();
            }

        }

        #endregion

    }
}
