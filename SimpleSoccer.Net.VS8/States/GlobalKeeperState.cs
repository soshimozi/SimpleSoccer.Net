using System;
using System.Collections.Generic;
using System.Text;

namespace SimpleSoccer.Net
{
    public class GlobalKeeperState : State<GoalKeeper>
    {
        public override void Enter(GoalKeeper entity)
        {
        }

        public override void Execute(GoalKeeper entity)
        {
        }

        public override void Exit(GoalKeeper entity)
        {
        }

        public override bool OnMessage(GoalKeeper entity, Telegram message)
        {
            switch (message.MessageCode)
            {
                case (int)SoccerGameMessages.GoHome:
                    entity.SetDefaultHomeRegion();
                    entity.StateMachine.ChangeState(ReturnHomeGoalState.Instance);
                    break;

                case (int)SoccerGameMessages.ReceiveBall:
                    entity.StateMachine.ChangeState(InterceptBallState.Instance);
                    break;

            }//end switch

            return false;
        }

        private static GlobalKeeperState _instance = null;
        public static GlobalKeeperState Instance
        {
            get { return (_instance == null ? _instance = new GlobalKeeperState() : _instance); }
        }
    }
}
