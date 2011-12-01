using System;
using System.Collections.Generic;
using System.Text;

namespace SimpleSoccer.Net
{
    class ReceiveBallState : State<FieldPlayer>
    {
        private static ReceiveBallState _instance = null;
        public static ReceiveBallState Instance
        {
            get
            {
                if (_instance == null) _instance = new ReceiveBallState();
                return _instance;
            }
        }

        public override void Enter(FieldPlayer player)
        {
            Random random = new Random();

            //let the team know this player is receiving the ball
            player.Team.ReceivingPlayer = player;

            //this player is also now the controlling player
            player.Team.ControllingPlayer = player;

            //there are two types of receive behavior. One uses arrive to direct
            //the receiver to the position sent by the passer in its telegram. The
            //other uses the pursuit behavior to pursue the ball. 
            //This statement selects between them dependent on the probability
            //ChanceOfUsingArriveTypeReceiveBehavior, whether or not an opposing
            //player is close to the receiving player, and whether or not the receiving
            //player is in the opponents 'hot region' (the third of the pitch closest
            //to the opponent's goal
            const double PassThreatRadius = 70.0;

            if ((player.InHotRegion ||
                  random.NextDouble() < ParameterManager.Instance.ChanceOfUsingArriveTypeReceiveBehavior) &&
               !player.Team.IsOpponentWithinRadius(player.Position, PassThreatRadius))
            {
                player.SteeringBehaviors.Arrive = true;

                System.Diagnostics.Debug.WriteLine(string.Format("Player {0} enters receive state (Using Arrive)", player.ObjectId));
            }
            else
            {
                player.SteeringBehaviors.Pursuit = true;

                System.Diagnostics.Debug.WriteLine(string.Format("Player {0} enters receive state (Using Pursuit)", player.ObjectId));
            }

        }

        public override void Execute(FieldPlayer player)
        {
            //if the ball comes close enough to the player or if his team lose control
            //he should change state to chase the ball
            if (player.BallInReceivingRange || !player.Team.InControl)
            {
                player.StateMachine.ChangeState(ChaseBallState.Instance);

                return;
            }

            if (player.SteeringBehaviors.Pursuit)
            {
                player.SteeringBehaviors.Target = player.Ball.Position;
            }

            //if the player has 'arrived' at the steering target he should wait and
            //turn to face the ball
            if (player.AtTarget)
            {
                player.SteeringBehaviors.Arrive = false;
                player.SteeringBehaviors.Pursuit = false;
                player.TrackBall();
                player.Velocity = new Vector2D(0, 0);
            }

        }

        public override void Exit(FieldPlayer player)
        {
            player.SteeringBehaviors.Arrive = false;
            player.SteeringBehaviors.Pursuit = false;

            player.Team.ReceivingPlayer = null;
        }

        public override bool OnMessage(FieldPlayer player, Telegram message)
        {
            return false;
        }
    }
}
