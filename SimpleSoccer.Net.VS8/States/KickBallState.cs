using System;
using System.Collections.Generic;
using System.Text;

namespace SimpleSoccer.Net
{
    class KickBallState : State<FieldPlayer>
    {
        private static KickBallState _instance = null;
        public static KickBallState Instance
        {
            get
            {
                if (_instance == null) _instance = new KickBallState();
                return _instance;
            }
        }

        public override void Enter(FieldPlayer player)
        {
            //let the team know this player is controlling
            player.Team.ControllingPlayer = player;

            //the player can only make so many kick attempts per second.
            if (!player.IsReadyForNextKick)
            {
                player.StateMachine.ChangeState(ChaseBallState.Instance);
            }

        }

        public override void Execute(FieldPlayer player)
        {
            Random random = new Random();

            //calculate the dot product of the vector pointing to the ball
            //and the player's heading
            Vector2D ToBall = player.Ball.Position - player.Position;
            double dot = player.Heading.GetDotProduct(Vector2D.Vec2DNormalize(ToBall));

            //cannot kick the ball if the goalkeeper is in possession or if it is 
            //behind the player or if there is already an assigned receiver. So just
            //continue chasing the ball
            if (player.Team.ReceivingPlayer != null ||
                player.Pitch.GoalKeeperHasBall ||
                (dot < 0))
            {
                System.Diagnostics.Debug.WriteLine("Goaly has ball / ball behind player");
                player.StateMachine.ChangeState(ChaseBallState.Instance);
                return;
            }

            /* Attempt a shot at the goal */

            //if a shot is possible, this vector will hold the position along the 
            //opponent's goal line the player should aim for.
            Vector2D BallTarget = new Vector2D();

            //the dot product is used to adjust the shooting force. The more
            //directly the ball is ahead, the more forceful the kick
            double power = ParameterManager.Instance.MaxShootingForce * dot;

            double distance = player.Position.Distance(player.Team.OpponentsGoal.GoalLineCenter);

            //if it is determined that the player could score a goal from this position
            //OR if he should just kick the ball anyway, the player will attempt
            //to make the shot
            if (player.Team.CanShoot(player.Ball.Position,
                                         power,
                                         ref BallTarget) ||
               (random.NextDouble() < ParameterManager.Instance.ChancePlayerAttemptsPotShot) ||
                distance < player.Pitch.PlayingArea.Width / 8)
            {
                System.Diagnostics.Debug.WriteLine(string.Format("Player {0} attempts a shot at ({1}, {2})", player.ObjectId, BallTarget.X, BallTarget.Y));

                //add some noise to the kick. We don't want players who are 
                //too accurate! The amount of noise can be adjusted by altering
                //Prm.PlayerKickingAccuracy
                BallTarget = SoccerBall.AddNoiseToKick(player.Ball.Position, BallTarget);

                //this is the direction the ball will be kicked in
                Vector2D KickDirection = BallTarget - player.Ball.Position;

                player.Ball.Kick(KickDirection, power);

                //change state   
                player.StateMachine.ChangeState(WaitState.Instance);

                player.FindSupport();

                return;
            }


            /* Attempt a pass to a player */

            //if a receiver is found this will point to it
            PlayerBase receiver = null;

            power = ParameterManager.Instance.MaxPassingForce * dot;

            //test if there are any potential candidates available to receive a pass
            if (player.IsThreatened() &&
                player.Team.FindPass(player,
                                        out receiver,
                                        out BallTarget,
                                        power,
                                        ParameterManager.Instance.MinPassDist))
            {
                //add some noise to the kick
                BallTarget = SoccerBall.AddNoiseToKick(player.Ball.Position, BallTarget);

                Vector2D KickDirection = BallTarget - player.Ball.Position;

                player.Ball.Kick(KickDirection, power);

                System.Diagnostics.Debug.WriteLine(string.Format("Player {0} passes the ball with force {1} to player {2} Target is ({3},{4})", player.ObjectId, power, receiver.ObjectId, BallTarget.X, BallTarget.Y));


                //let the receiver know a pass is coming 
                MessageDispatcher.Instance.DispatchMsg(
                    new TimeSpan(0),
                    player.ObjectId,
                    receiver.ObjectId,
                    (int)SoccerGameMessages.ReceiveBall,
                    BallTarget);


                //the player should wait at his current position unless instruced
                //otherwise  
                player.StateMachine.ChangeState(WaitState.Instance);

                player.FindSupport();

                return;
            }

            //cannot shoot or pass, so dribble the ball upfield
            else
            {
                player.FindSupport();

                player.StateMachine.ChangeState(DribbleState.Instance);
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
