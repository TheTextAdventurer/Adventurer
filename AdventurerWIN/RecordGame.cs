using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Windows.Forms;
using GameEngine;

namespace AdventurerWIN
{
    [Serializable]
    public class RecordGame
    {
        public int CarriageReturnDelay { get; set; } = 2000;
        public int KeyPressDelay { get; set; } = 250;

        [Browsable(false)]
        public bool Playback { get; private set; } = false;
        [Browsable(false)]
        private DateTime lastInput = DateTime.Now;
        [Browsable(false)]
        public string File { get; private set; }
        [Browsable(false)]
        public List<Input> PlayerInput { get; set; } = new List<Input>();
        [Browsable(false)]
        public GameData Data { get; private set; }

        [NonSerialized()]
        private Timer StepPlayerInputList = null;   //step through the PlayerInput list
        [NonSerialized()]
        private Timer StepPlayerInput = null;      //Step through each charcter of the current item of the player input list
        [NonSerialized()]
        private Timer CarriageReturn = null;   //step through the PlayerInput list


        private int PlayerInputStep = -1;
        private int PlayerInputCounter = 0;

        public RecordGame(GameData pData, string pFile)
        {

            File = pFile;

            //we are making a clone of the GameData item
            IFormatter formatter = new BinaryFormatter();
            Stream stream = new MemoryStream();
            using (stream)
            {
                formatter.Serialize(stream, pData);
                stream.Seek(0, SeekOrigin.Begin);
                Data = (GameData)formatter.Deserialize(stream);
            }            
        }
        
        public void AddInput (string pText)
        {
            DateTime n = DateTime.Now;          
            PlayerInput.Add(new Input((int)(n - lastInput).TotalMilliseconds, pText));
            lastInput = n;
        }

        #region replay code

        public void StartReplay()
        {
            Playback = true;

            CarriageReturn = new Timer
            {
                Interval = CarriageReturnDelay
            };
            CarriageReturn.Tick += SendCarriageReturn_Tick;

            StepPlayerInputList = new Timer();
            StepPlayerInputList.Tick += StepPlayerInputList_Tick;
            PlayerInputStep = 0;
            StepPlayerInputList.Interval = PlayerInput[PlayerInputStep].Delay;

            StepPlayerInput = new Timer();
            StepPlayerInput.Tick += StepPlayerInput_Tick;
            StepPlayerInput.Interval = KeyPressDelay;


            StepPlayerInputList.Enabled = true;

        }

        private void SendCarriageReturn_Tick(object sender, EventArgs e)
        {
            CarriageReturn.Enabled = false;
            SendCarriageReturn();
            StepPlayerInputList.Enabled = true;
        }

        private void StepPlayerInput_Tick(object sender, EventArgs e)
        {
            if (PlayerInputCounter >= PlayerInput[PlayerInputStep].Text.Length) //finished outputting a word
            {
                StepPlayerInput.Enabled = false;                
                PlayerInputStep++;

                if (PlayerInputStep >= PlayerInput.Count())
                {
                    Playback = false;
                    SendReplayFinished();
                }
                else
                {
                    StepPlayerInputList.Interval = PlayerInput[PlayerInputStep].Delay;
                    CarriageReturn.Enabled = true;                                        
                }
            }
            else
            {
                SendKeyStroke(PlayerInput[PlayerInputStep].Text.Substring(PlayerInputCounter, 1));
                PlayerInputCounter++;
            }
        }

        private void StepPlayerInputList_Tick(object sender, EventArgs e)
        {
            StepPlayerInputList.Enabled = false;

            if (PlayerInputStep <= PlayerInput.Count())
            {
                StepPlayerInputList.Enabled = false;
                StepPlayerInput.Enabled = true;
                PlayerInputCounter = 0;
            }
        }
        
        #endregion


        #region class

        [Serializable]
        public class Input
        {
            public Input(int delay, string text)
            {
                Delay = delay;
                Text = text;
            }

            public int Delay { get; set; }
            public string Text { get; set; }

            public override string ToString()
            {
                return $"{Delay}\t,\t{Text}";
            }

        }

        #endregion

        #region IO

        public static void Save(RecordGame g)
        {
            var serializer = new BinaryFormatter();
            using (Stream s = new FileStream(g.File, FileMode.Create, FileAccess.Write, FileShare.None))
            {
                serializer.Serialize(s, g);
                s.Close();               
            }
        }

        public static RecordGame Load(string pPath)
        {
            var serializer = new BinaryFormatter();
            using (Stream s = new FileStream(pPath, FileMode.Open, FileAccess.Read, FileShare.None))
            {
                RecordGame r = (RecordGame)serializer.Deserialize(s);
                s.Close();
                return r;
            }
        }

        #endregion

        #region events

        [field:NonSerialized]
        public event EventHandler<Keystroke> eKeystroke;

        public class Keystroke : EventArgs
        {
            public string Key { get; private set; }
            public Keystroke(string pKey)
            {
                Key = pKey;
            }
        }

        private void SendKeyStroke(string pChar)
        {
            eKeystroke?.Invoke(this, new Keystroke(pChar));
        }

        [field: NonSerialized]
        public event EventHandler<EventArgs> ReplayFinished;
        private void SendReplayFinished()
        {
            ReplayFinished?.Invoke(this, null);
        }

        [field: NonSerialized]
        public event EventHandler<EventArgs> eCarriageReturn;
        private void SendCarriageReturn()
        {
            eCarriageReturn?.Invoke(this, null);
        }

        #endregion
    }
}
