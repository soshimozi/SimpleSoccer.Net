//------------------------------------------------------------------------
//
//  Name:   Telegram.cs
//
//  Desc:   This defines a telegram. A telegram is a data structure that
//          records information required to dispatch messages. Messages 
//          are used by game agents to communicate with each other.
//
//  Author: Mat Buckland (fup@ai-junkie.com)
//  Ported By: Scott McCain (scott_mccain@cox.net)
//------------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Text;

namespace SimpleSoccer.Net
{
    // TODO: Add Regions and Organize Code
    // TODO: Unit test telegram code
    public class Telegram
    {
        private Guid _id = Guid.NewGuid();
        private int _sender;
        private int _receiver;
        private int _messageCode;
        private DateTime _dispatchTime;
        private object _extraInfo;

        public T GetExtraInfoTyped<T>() where T : class
        {
            return _extraInfo as T;
        }

        public object ExtraInfo
        {
            get { return _extraInfo; }
            set { _extraInfo = value; }
        }

        public DateTime DispatchTime
        {
            get { return _dispatchTime; }
            set { _dispatchTime = value; }
        }

        public int MessageCode
        {
            get { return _messageCode; }
            set { _messageCode = value; }
        }
        public int Receiver
        {
            get { return _receiver; }
            set { _receiver = value; }
        }

        public int Sender
        {
            get { return _sender; }
            set { _sender = value; }
        }

        public Telegram(int sender, int receiver, int messageCode, DateTime dispatchTime, object extraInfo)
        {
            _sender = sender;
            _receiver = receiver;
            _messageCode = messageCode;
            _dispatchTime = dispatchTime;
            _extraInfo = extraInfo;
        }

        public Telegram()
        {
        }

        //these telegrams will be stored in a priority queue. Therefore the >
        //operator needs to be overloaded so that the PQ can sort the telegrams
        //by time priority. Note how the times must be smaller than
        //SmallestDelay apart before two Telegrams are considered unique.
        public static double SmallestDelay = 0.25;

        public override int GetHashCode()
        {
            return _id.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            Telegram rhs = obj as Telegram;
            Telegram lhs = this;
            return (lhs.Sender == rhs.Sender && lhs.Receiver == rhs.Receiver && lhs.MessageCode == rhs.MessageCode && Math.Abs(((TimeSpan)(lhs.DispatchTime - rhs.DispatchTime)).TotalMilliseconds) <= SmallestDelay);
        }

        public static bool operator <(Telegram lhs, Telegram rhs)
        {
            if (lhs == rhs)
                return false;
            else
                return (lhs.DispatchTime < rhs.DispatchTime);
        }

        public static bool operator >(Telegram lhs, Telegram rhs)
        {
            if (lhs == rhs)
                return false;
            else
                return (lhs.DispatchTime > rhs.DispatchTime);
        }
    }
}
