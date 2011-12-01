using System;
using System.Collections.Generic;
using System.Text;

namespace SimpleSoccer.Net
{
    class ReturnHomeGoalState : State<GoalKeeper>
    {
        private static ReturnHomeGoalState _instance = null;
        public static ReturnHomeGoalState Instance
        {
            get
            {
                if (_instance == null) _instance = new ReturnHomeGoalState();
                return _instance;
            }
        }

        public override void Enter(GoalKeeper entity)
        {
            entity.SteeringBehaviors.Arrive = true;

            entity.SteeringBehaviors.Target = entity.HomeRegion.VectorCenter;
        }

        public override void Execute(GoalKeeper keeper)
        {
            keeper.SteeringBehaviors.Target = keeper.HomeRegion.VectorCenter;

            //if close enough to home or the opponents get control over the ball,
            //change state to tend goal
            if (keeper.InHomeRegion || !keeper.Team.InControl)
            {
                keeper.StateMachine.ChangeState(TendGoalState.Instance);
            }
        }

        public override void Exit(GoalKeeper entity)
        {
            entity.SteeringBehaviors.Arrive = false;
        }

        public override bool OnMessage(GoalKeeper entity, Telegram message)
        {
            return false;
        }
    }
}
