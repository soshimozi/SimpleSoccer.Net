using System;
using System.Collections.Generic;
using System.Text;

namespace SimpleSoccer.Net
{
    class ChaseBallState : State<FieldPlayer>
    {
        private static ChaseBallState _instance = null;
        public static ChaseBallState Instance
        {
            get
            {
                if (_instance == null) _instance = new ChaseBallState();
                return _instance;
            }
        }

        public override void Enter(FieldPlayer entity)
        {
            entity.SteeringBehaviors.Seek = true;
        }

        public override void Execute(FieldPlayer player)
        {
            //if the ball is within kicking range the player changes state to KickBall.
            if (player.BallInKickingRange)
            {
                player.StateMachine.ChangeState(KickBallState.Instance);

                return;
            }

            //if the player is the closest player to the ball then he should keep
            //chasing it
            if (player.IsClosestTeamMemberToBall)
            {
                player.SteeringBehaviors.Target = player.Ball.Position;

                return;
            }

            //if the player is not closest to the ball anymore, he should return back
            //to his home region and wait for another opportunity
            player.StateMachine.ChangeState(ReturnToHomeRegionState.Instance);
        }

        public override void Exit(FieldPlayer entity)
        {
            entity.SteeringBehaviors.Seek = false;
        }

        public override bool OnMessage(FieldPlayer entity, Telegram message)
        {
            return false;
        }
    }
}
