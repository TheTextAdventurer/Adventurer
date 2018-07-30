using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Linq;

namespace GameEngine
{
    /// <summary>
    /// Contains methods for outputting a DAT file (in the ScottFree format)
    /// </summary>
    public partial class GameData
    {

        /// <summary>
        /// Convert the game DAT file into XML
        /// </summary>     
        public void SaveAsUncommentedXML(string pFile)
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
                            Select(a => new XElement("Action", new XAttribute("Index", a), Datafile.GetTokensAsInt(8).Select((val, ind) => new XElement(String.Format("{0}", actionsDesc[ind]), val)))
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
                            Select(a => new XElement("Message", new XAttribute("Index", a), Datafile.getTokens(1))))
                );

            string[] itemDescripion = { "Name", "Word" };
            gameData.Add
                (
                    new XElement("Items",
                        Enumerable.Range(0, header[1] + 1).
                            Select(a => new XElement("Item",
                                new XAttribute("Index", a),
                                Datafile.getTokens(1).First().Split(new char[] { '/' }, StringSplitOptions.RemoveEmptyEntries)
                                    .Select((v, i) => new XElement(itemDescripion[i], v))
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

        #region SaveAsCommentedXML
        /// <summary>
        /// Convert the game DAT file into XML with accompanying comments
        /// </summary>
        public void SaveAsCommentedXML()
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

        /// <summary>
        /// Load DAT file and output with multiline comments
        /// </summary>
        /// <param name="pFile"></param>
        public static void SaveAsCommentedDat(string pFile)
        {

            string outputFileName = Path.GetFileNameWithoutExtension(pFile) + "_commented" + Path.GetExtension(pFile);


            using (StreamWriter sw = new StreamWriter(outputFileName))
            {

                Datafile.Load(pFile);

                var header = new GameHeader(Datafile.GetTokensAsInt(12));

                sw.WriteLine("{0} /*Unknown*/", header.Unknown);
                sw.WriteLine("{0} /*Number of items*/", header.NumItems);
                sw.WriteLine("{0} /*Number of actions*/", header.NumActions);
                sw.WriteLine("{0} /*Number of Noun Verbs*/", header.NumNounVerbs);
                sw.WriteLine("{0} /*Number of Rooms*/", header.NumRooms);
                sw.WriteLine("{0} /*Maximum carry*/", header.MaxCarry);
                sw.WriteLine("{0} /*Start toom*/", header.StartRoom);
                sw.WriteLine("{0} /*Total treasures*/", header.TotalTreasures);
                sw.WriteLine("{0} /*Word length*/", header.WordLength);
                sw.WriteLine("{0} /*Light duration*/", header.LightDuration);
                sw.WriteLine("{0} /*Number of messages*/", header.NumMessages);
                sw.WriteLine("{0} /*Treasure room*/", header.TreasureRoom);

                //produces an array of arrays
                var labels = new string[] { "/*NounVerb*/", "/*Condition1*/", "/*Condition2*/", "/*Condition3*/", "/*Condition4*/", "/*Condition5*/", "/*Actions 1 and 2*/", "/*Actions 3 and 4*/" };
                int ctr = 1;
                foreach (var action in
                    Enumerable.Range(0, header.NumActions).Select(n => Datafile.getTokens(8).ToArray()).ToArray())
                {
                    sw.WriteLine("{0}\t\t\t/*Action index {1} - NounVerb*/", action.First(), ctr++);
                    for (int a = 1; a < 8; a++)
                        sw.WriteLine("{0}\t\t\t{1}", action[a], labels[a]);
                }

                int vb = 0, nn = 0,  j = 0;
                foreach (var w in Datafile.getTokens(header.NumNounVerbs * 2))
                {

                    if (j == 0) //verb
                    {
                        if (!w.StartsWith("*"))
                            vb++;

                        sw.WriteLine("\"{0}\"\t\t{1}"
                            , w
                            , !w.StartsWith("*") ? string.Format("/*Verb index {0}", vb)
                                                 : string.Format("/*synonym of verb index {0}", vb)
                                                 );

                    }
                    else if (j == 1)   //noun
                    {
                        if (!w.StartsWith("*"))
                            nn++;

                        sw.WriteLine("\"{0}\"\t\t{1}"
                            , w
                            , !w.StartsWith("*") ? string.Format("/*Noun index {0}", nn)
                                                 : string.Format("/*synonym of noun index {0}", nn)
                                                 );

                    }

                    j++;
                    if (j > 1)
                    {
                        j = 0;
                    }

                }

                ctr = 0;
                string[] dire = { "/*North ", "/*South ", "/*East ", "/*West ", "/*Up ", "/*Down " };
                foreach (var room in Enumerable.Range(0, header.NumRooms).Select(n => Datafile.getTokens(7).ToArray()))
                {

                    for (int q = 0; q < 6; q++)
                        sw.WriteLine("{0}\t{1}{2}*/", room[q], dire[q], room[q] == "0" ? " - not used" : " - links to room " + room[q]);

                    sw.WriteLine("\"{0}\" /*Room {1} Description*/", room.Last(), ctr++);
                }

                ctr = 0;

                foreach (var message in Datafile.getTokens(header.NumMessages))
                    sw.WriteLine("\"{0}\" /*Message {1}*/", message, ctr++);


                ctr = 0;
                foreach (var item in Enumerable.Range(0, header.NumItems).Select(n => Datafile.getTokens(2).ToArray()))
                    sw.WriteLine("\"{0}\" /*Item {1} Description*/ {2} /*Location*/", item.First(), ctr++, item.Last());

                ctr = 0;
                foreach (var actionMessage in Datafile.getTokens(header.NumActions))
                {
                    sw.WriteLine("\"{0}\" /*Action {1} description*/", actionMessage, ctr++);
                }

                var footer = new GameFooter(Datafile.GetTokensAsInt(3));

                sw.WriteLine("\"{0}\" /*Version number*/", footer.Version);
                sw.WriteLine("\"{0}\" /*Adventure number*/", footer.AdventureNumber);
                sw.WriteLine("\"{0}\" /*Unknown*/", footer.Unknown);
            }


        }        
    }
}
