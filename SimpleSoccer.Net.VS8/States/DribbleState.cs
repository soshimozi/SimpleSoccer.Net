using System;
using System.Collections.Generic;
using System.Text;

namespace SimpleSoccer.Net
{
    class DribbleState : State<FieldPlayer>
    {
        private static DribbleState _instance = null;
        public static DribbleState Instance
        {
            get
            {
                if (_instance == null) _instance = new DribbleState();
                return _instance;
            }
        }

        public override void Enter(FieldPlayer player)
        {
            //let the team know this player is controlling
            player.Team.ControllingPlayer = player;

            System.Diagnostics.Debug.WriteLine(string.Format("Player {0} enters dribble state", player.ObjectId));

        }

        public override void Execute(FieldPlayer player)
        {
            double dot = player.Team.HomeGoal.FacingDirection.GetDotProduct(player.Heading);

            //if the ball is between the player and the home goal, it needs to swivel
            // the ball around by doing multiple small kicks and turns until the player 
            //is facing in the correct direction
            if (dot < 0)
            {
                //the player's heading is going to be rotated by a small amount (Pi/4) 
                //and then the ball will be kicked in that direction
                Vector2D direction = player.Heading;

                //calculate the sign (+/-) of the angle between the player heading and the 
                //facing direction of the goal so that the player rotates around in the 
                //correct direction
                double angle = Utils.Math.Constants.QuarterPi * -1 *
                             player.Team.HomeGoal.FacingDirection.Sign(player.Heading);

                Transformations.Vec2DRotateAroundOrigin(direction, angle);

                //this value works well whjen the player is attempting to control the
                //ball and turn at the same time
                const double KickingForce = 0.8;

                player.Ball.Kick(direction, KickingForce);
            }

            //kick the ball down the field
            else
            {
                player.Ball.Kick(player.Team.HomeGoal.FacingDirection,
                                     ParameterManager.Instance.MaxDribbleForce);
            }

            //the player has kicked the ball so he must now change state to follow it
            player.StateMachine.ChangeState(ChaseBallState.Instance);

            return;

        }

        public override void Exit(FieldPlayer player)
        {
        }

        public override bool OnMessage(FieldPlayer player, Telegram message)
        {
            return false;
        }
    }
}
