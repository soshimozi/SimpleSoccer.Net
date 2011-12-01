//------------------------------------------------------------------------
//
//  Name:   StateMachine.cs
//
//  Desc:   State machine class. Inherit from this class and create some 
//          states to give your agents FSM functionality
//
//  Author: Mat Buckland (fup@ai-junkie.com)
//  Ported By: Scott McCain (scott_mccain@cox.net)
//------------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Text;

namespace SimpleSoccer.Net
{
    public class StateMachine<T>
    {
        private T _owner;
        private State<T> _currentState = null;
        private State<T> _previousState = null;
        private State<T> _globalState = null;

        public StateMachine(T owner)
        {
            _owner = owner;
        }

        public State<T> CurrentState
        {
            get { return _currentState; }
            set { _currentState = value; }
        }

        public State<T> PreviousState
        {
            get { return _previousState; }
            set { _previousState = value; }
        }

        public State<T> GlobalState
        {
            get { return _globalState; }
            set { _globalState = value; }
        }

        /// <summary>
        /// call this to updte the FSM
        /// </summary>
        public void Update()
        {
            //if a global state exists, call its execute method, else do nothing
            if (_globalState != null) _globalState.Execute(_owner);

            //same for the current state
            if (_currentState != null) _currentState.Execute(_owner);
        }

        public bool HandleMessage(Telegram msg)
        {
            //first see if the current state is valid and that it can handle
            //the message
            if (_currentState != null && _currentState.OnMessage(_owner, msg))
            {
                return true;
            }

            //if not, and if a global state has been implemented, send 
            //the message to the global state
            if (_globalState != null && _globalState.OnMessage(_owner, msg))
            {
                return true;
            }

            return false;
        }

        //change to a new state
        public void ChangeState(State<T> newState)
        {
            System.Diagnostics.Debug.Assert(newState != null, "StateMachine.ChangeState: trying to assing null State to current");

            //keep a record of the previous state
            _previousState = _currentState;

            //call the exit method of the existing state
            _currentState.Exit(_owner);


            //change state to the new state
            _currentState = newState;

            //call the entry method of the new state
            _currentState.Enter(_owner);
        }

        //change state back to the previous state
        public void RevertToPreviousState()
        {
            ChangeState(_previousState);
        }

        //returns true if the current state's type is equal to the type of the
        //class passed as a parameter. 
        public bool IsInState(State<T> state)
        {
            return (_currentState == state);
        }


    }
}
