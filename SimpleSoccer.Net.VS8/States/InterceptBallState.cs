using System;
using System.Collections.Generic;
using System.Text;
using SimpleSoccer.Net;

namespace SimpleSoccer.Net
{
    class InterceptBallState : State<GoalKeeper>
    {
        public override void Enter(GoalKeeper entity)
        {
            entity.SteeringBehaviors.Pursuit = true;  

        }

        public override void Execute(GoalKeeper entity)
        {
            //if the goalkeeper moves to far away from the goal he should return to his
            //home region UNLESS he is the closest player to the ball, in which case,
            //he should keep trying to intercept it.
            if (entity.TooFarFromGoalMouth() && !entity.IsClosestPlayerOnPitchToBall)
            {
                entity.StateMachine.ChangeState(ReturnHomeGoalState.Instance);

                return;
            }

            //if the ball becomes in range of the goalkeeper's hands he traps the 
            //ball and puts it back in play
            if (entity.BallInKeeperRange)
            {
                entity.Ball.Trap();

                entity.Pitch.GoalKeeperHasBall = true;

                entity.StateMachine.ChangeState(PutBallBackInPlayState.Instance);

                return;
            }

        }

        public override void Exit(GoalKeeper entity)
        {
            entity.SteeringBehaviors.Pursuit = false;
        }

        public override bool OnMessage(GoalKeeper entity, Telegram message)
        {
            return false;
        }

        private static InterceptBallState _instance = null;

        public static InterceptBallState Instance
        {
            get { return (_instance == null ? _instance = new InterceptBallState() : _instance); }
        }
    }
}
