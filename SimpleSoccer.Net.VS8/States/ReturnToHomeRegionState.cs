using System;
using System.Collections.Generic;
using System.Text;

namespace SimpleSoccer.Net
{
    class ReturnToHomeRegionState : State<FieldPlayer>
    {
        public override void Enter(FieldPlayer player)
        {
            player.SteeringBehaviors.Arrive = true;

            if (!player.HomeRegion.IsPositionInside(player.SteeringBehaviors.Target, Region.RegionModifier.Halfsize))
            {
                player.SteeringBehaviors.Target = player.HomeRegion.VectorCenter;
            }
        }

        public override void Execute(FieldPlayer player)
        {
            if (player.Pitch.GameInPlay)
            {
                //if the ball is nearer this player than any other team member  &&
                //there is not an assigned receiver && the goalkeeper does not gave
                //the ball, go chase it
                if (player.IsClosestTeamMemberToBall &&
                     (player.Team.ReceivingPlayer == null) &&
                     !player.Pitch.GoalKeeperHasBall)
                {
                    player.StateMachine.ChangeState(ChaseBallState.Instance);

                    return;
                }
            }

            //if game is on and close enough to home, change state to wait and set the 
            //player target to his current position.(so that if he gets jostled out of 
            //position he can move back to it)
            if (player.Pitch.GameInPlay && player.HomeRegion.IsPositionInside(player.Position, Region.RegionModifier.Halfsize))
            {
                player.SteeringBehaviors.Target = player.Position;
                player.StateMachine.ChangeState(WaitState.Instance);
            }
            //if game is not on the player must return much closer to the center of his
            //home region
            else if (!player.Pitch.GameInPlay && player.AtTarget)
            {
                player.StateMachine.ChangeState(WaitState.Instance);
            }
        }

        public override void Exit(FieldPlayer entity)
        {
            entity.SteeringBehaviors.Arrive = false;
        }

        public override bool OnMessage(FieldPlayer entity, Telegram message)
        {
            return false;
        }

        private static ReturnToHomeRegionState _instance = null;
        public static ReturnToHomeRegionState Instance
        {
            get { return (_instance == null ? _instance = new ReturnToHomeRegionState() : _instance); }
        }
    }
}
