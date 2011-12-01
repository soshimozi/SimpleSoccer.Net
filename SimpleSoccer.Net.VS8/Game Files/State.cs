//------------------------------------------------------------------------
//
//  Name:   State.cs
//
//  Desc:   abstract base class to define an interface for a state
//
//  Author: Mat Buckland (fup@ai-junkie.com)
//  Ported By: Scott McCain (scott_mccain@cox.net)
//------------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Text;

namespace SimpleSoccer.Net
{
    public abstract class State<T>
    {
        /// <summary>
        /// this will execute when the state is entered
        /// </summary>
        /// <param name="entity"></param>
        public abstract void Enter(T entity);

        /// <summary>
        /// this is the states normal update function
        /// </summary>
        /// <param name="entity"></param>
        public abstract void Execute(T entity);

        /// <summary>
        /// this will execute when the state is exited. 
        /// </summary>
        /// <param name="entity"></param>
        public abstract void Exit(T entity);

        /// <summary>
        /// this executes if the agent receives a message from the 
        /// message dispatcher
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="message"></param>
        public abstract bool OnMessage(T entity, Telegram message);
    }
}
