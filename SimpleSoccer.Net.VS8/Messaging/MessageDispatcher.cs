//------------------------------------------------------------------------
//
//  Name:   MessageDispatcher.cs
//
//  Desc:   A message dispatcher. Manages messages of the type Telegram.
//          Instantiated as a singleton.
//
//  Author: Mat Buckland (fup@ai-junkie.com)
//  Ported By: Scott McCain (scott_mccain@cox.net)
//------------------------------------------------------------------------
using System;
using System.Collections;
using System.Text;
using SimpleSoccer.Net.Collections;

namespace SimpleSoccer.Net
{
    // TODO: Add Regions and Orgainize Code
    public class MessageDispatcher
    {
        private static MessageDispatcher _instance = null;
        public static MessageDispatcher Instance
        {
            get
            {
                if (_instance == null)
                    _instance = new MessageDispatcher();

                return _instance;
            }
        }

        private PriorityQueue<Telegram, double> _priorityQueue = new PriorityQueue<Telegram, double>();

        //this method is utilized by DispatchMsg or DispatchDelayedMessages.
        //This method calls the message handling member function of the receiving
        //entity, pReceiver, with the newly created telegram
        private void discharge(BaseGameEntity receiver, Telegram telegram)
        {
            receiver.HandleMessage(telegram);
        }


        //---------------------------- DispatchMsg ---------------------------
        //
        //  given a message, a receiver, a sender and any time delay, this function
        //  routes the message to the correct agent (if no delay) or stores
        //  in the message queue to be dispatched at the correct time
        //------------------------------------------------------------------------
        public void DispatchMsg(TimeSpan delay, int sender, int receiver, int messagecode, object additionalInfo)
        {
            //get a pointer to the receiver
            BaseGameEntity receiverEntity = EntityManager.Instance.FindEntity(receiver);

            //make sure the receiver is valid
            if (receiverEntity == null)
            {
                //#ifdef SHOW_MESSAGING_INFO
                //debug_con << "\nWarning! No Receiver with ID of " << receiver << " found" << "";
                //#endif
            }
            else
            {

                //create the telegram
                Telegram telegram = new Telegram(sender, receiver, messagecode, DateTime.Now, additionalInfo);

                //if there is no delay, route telegram immediately                       
                if (delay.TotalMilliseconds <= Telegram.SmallestDelay)
                {
                    //#ifdef SHOW_MESSAGING_INFO
                    //debug_con << "\nTelegram dispatched at time: " << TickCounter->GetCurrentFrame()
                    //     << " by " << sender << " for " << receiver 
                    //     << ". Msg is " << msg << "";
                    //#endif

                    //send the telegram to the recipient
                    discharge(receiverEntity, telegram);
                }

                  //else calculate the time when the telegram should be dispatched
                else
                {

                    telegram.DispatchTime = DateTime.Now + delay;

                    _priorityQueue.Enqueue(telegram, -delay.TotalMilliseconds);

                    //#ifdef SHOW_MESSAGING_INFO
                    //debug_con << "\nDelayed telegram from " << sender << " recorded at time " 
                    //        << TickCounter->GetCurrentFrame() << " for " << receiver
                    //        << ". Msg is " << msg << "";
                    //#endif
                }
            }

        }

        public void DispatchDelayedMessages()
        {
              //now peek at the queue to see if any telegrams need dispatching.
              //remove all telegrams from the front of the queue that have gone
              //past their sell by date
            while (_priorityQueue.Count > 0 &&
                    _priorityQueue.Peek().Value.DispatchTime >= DateTime.Now &&
                   _priorityQueue.Peek().Value.DispatchTime != DateTime.MinValue)
            {
                //read the telegram from the front of the queue
                Telegram telegram = _priorityQueue.Dequeue().Value;

                //find the recipient
                BaseGameEntity receiver = EntityManager.Instance.FindEntity(telegram.Receiver);

                //#ifdef SHOW_MESSAGING_INFO
                //debug_con << "\nQueued telegram ready for dispatch: Sent to " 
                //     << pReceiver->ID() << ". Msg is "<< telegram.Msg << "";
                //#endif

                //send the telegram to the recipient
                discharge(receiver, telegram);

            }
        }
    }

}
