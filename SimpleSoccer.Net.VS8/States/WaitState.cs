using System;
using System.Collections.Generic;
using System.Text;

namespace SimpleSoccer.Net
{
    class WaitState : State<FieldPlayer>
    {
        private static WaitState _instance = null;

        public static WaitState Instance
        {
            get { return (_instance == null ? _instance = new WaitState() : _instance); }
        }
        
        public override void Enter(FieldPlayer entity)
        {
            //if the game is not on make sure the target is the center of the player's
            //home region. This is ensure all the players are in the correct positions
            //ready for kick off
            if (!entity.Pitch.GameInPlay)
            {
                entity.SteeringBehaviors.Target = entity.HomeRegion.VectorCenter;
            }
        }

        public override void Execute(FieldPlayer entity)
        {
            //if the player has been jostled out of position, get back in position  
            if (!entity.AtTarget)
            {
                entity.SteeringBehaviors.Arrive = true;

                return;
            }

            else
            {
                entity.SteeringBehaviors.Arrive = false;

                entity.Velocity = new Vector2D(0, 0);

                //the player should keep his eyes on the ball!
                entity.TrackBall();
            }

            //if this player's team is controlling AND this player is not the attacker
            //AND is further up the field than the attacker he should request a pass.
            if (entity.Team.InControl &&
               !entity.IsControllingPlayer &&
                 entity.IsAheadOfAttacker)
            {
                entity.Team.RequestPass(entity);

                return;
            }

            if (entity.Pitch.GameInPlay)
            {
                //if the ball is nearer this player than any other team member  AND
                //there is not an assigned receiver AND neither goalkeeper has
                //the ball, go chase it
                if (entity.IsClosestTeamMemberToBall &&
                    entity.Team.ReceivingPlayer == null &&
                    !entity.Pitch.GoalKeeperHasBall)
                {
                    entity.StateMachine.ChangeState(ChaseBallState.Instance);

                    return;
                }
            }
        }

        public override void Exit(FieldPlayer entity)
        {
        }

        public override bool OnMessage(FieldPlayer entity, Telegram message)
        {
            return false;
        }

    }
}
