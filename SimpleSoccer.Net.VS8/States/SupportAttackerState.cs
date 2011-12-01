using System;
using System.Collections.Generic;
using System.Text;

namespace SimpleSoccer.Net
{
    class SupportAttackerState : State<FieldPlayer>
    {
        private static SupportAttackerState _instance = null;
        public static SupportAttackerState Instance
        {
            get
            {
                if (_instance == null) _instance = new SupportAttackerState();
                return _instance;
            }
        }


        public override void Enter(FieldPlayer player)
        {
            player.SteeringBehaviors.Arrive = true;

            player.SteeringBehaviors.Target = player.Team.GetSupportSpot();

        }

        public override void Execute(FieldPlayer player)
        {
            //if his team loses control go back home
            if (!player.Team.InControl)
            {
                player.StateMachine.ChangeState(ReturnToHomeRegionState.Instance);
                return;
            }


            //if the best supporting spot changes, change the steering target
            if (player.Team.GetSupportSpot() != player.SteeringBehaviors.Target)
            {
                player.SteeringBehaviors.Target = player.Team.GetSupportSpot();

                player.SteeringBehaviors.Arrive = true;
            }

            //if this player has a shot at the goal AND the attacker can pass
            //the ball to him the attacker should pass the ball to this player
            if (player.Team.CanShoot(player.Position,
                                         ParameterManager.Instance.MaxShootingForce))
            {
                player.Team.RequestPass(player);
            }


            //if this player is located at the support spot and his team still have
            //possession, he should remain still and turn to face the ball
            if (player.AtTarget)
            {
                player.SteeringBehaviors.Arrive = false;

                //the player should keep his eyes on the ball!
                player.TrackBall();

                player.Velocity = new Vector2D(0, 0);

                //if not threatened by another player request a pass
                if (!player.IsThreatened())
                {
                    player.Team.RequestPass(player);
                }
            }

        }

        public override void Exit(FieldPlayer player)
        {
            //set supporting player to null so that the team knows it has to 
            //determine a new one.
            player.Team.SupportingPlayer = null;

            player.SteeringBehaviors.Arrive = false;
        }

        public override bool OnMessage(FieldPlayer player, Telegram message)
        {
            return false;
        }
    }
}
