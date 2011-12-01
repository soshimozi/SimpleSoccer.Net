using System;
using System.Collections.Generic;
using System.Text;

namespace SimpleSoccer.Net
{
    class TendGoalState : State<GoalKeeper>
    {
        public override void Enter(GoalKeeper entity)
        {
            //turn interpose on
            entity.SteeringBehaviors.InterposeTarget = true;
            entity.SteeringBehaviors.InterposeDist = ParameterManager.Instance.GoalKeeperTendingDistance;

            //interpose will position the agent between the ball position and a target
            //position situated along the goal mouth. This call sets the target
            entity.SteeringBehaviors.Target = entity.GetRearInterposeTarget();
        }

        public override void Execute(GoalKeeper keeper)
        {
            //the rear interpose target will change as the ball's position changes
            //so it must be updated each update-step 
            keeper.SteeringBehaviors.Target = keeper.GetRearInterposeTarget();

            //if the ball comes in range the keeper traps it and then changes state
            //to put the ball back in play
            if (keeper.BallInKeeperRange)
            {
                keeper.Ball.Trap();

                keeper.Pitch.GoalKeeperHasBall = true;

                keeper.StateMachine.ChangeState(PutBallBackInPlayState.Instance);

                return;
            }

            //if ball is within a predefined distance, the keeper moves out from
            //position to try and intercept it.
            if (keeper.BallWithinRangeForIntercept() && !keeper.Team.InControl)
            {
                keeper.StateMachine.ChangeState(InterceptBallState.Instance);
            }

            //if the keeper has ventured too far away from the goal-line and there
            //is no threat from the opponents he should move back towards it
            if (keeper.TooFarFromGoalMouth() && keeper.Team.InControl)
            {
                keeper.StateMachine.ChangeState(ReturnHomeGoalState.Instance);

                return;
            }

        }

        public override void Exit(GoalKeeper entity)
        {
            entity.SteeringBehaviors.InterposeTarget = false;
        }

        public override bool OnMessage(GoalKeeper entity, Telegram message)
        {
            return false;
        }

        private static TendGoalState _instance = null;

        public static TendGoalState Instance
        {
            get { return (_instance == null ? _instance = new TendGoalState() : _instance); }
        }

    }
}
