using System;
using System.Collections.Generic;
using System.Text;

namespace SimpleSoccer.Net
{
    public class GlobalPlayerState : State<FieldPlayer>
    {
        private static GlobalPlayerState _instance = null;
        public static GlobalPlayerState Instance
        {
            get { return (_instance == null ? _instance = new GlobalPlayerState() : _instance); }
        }

        public override void Enter(FieldPlayer entity)
        {
        }

        public override void Execute(FieldPlayer entity)
        {
            //if a player is in possession and close to the ball reduce his max speed
            if ((entity.BallInReceivingRange) && (entity.IsControllingPlayer))
            {
                entity.MaxSpeed = ParameterManager.Instance.PlayerMaxSpeedWithBall;
            }
            else
            {
                entity.MaxSpeed = ParameterManager.Instance.PlayerMaxSpeedWithoutBall;
            }
        }

        public override void Exit(FieldPlayer entity)
        {
        }

        public override bool OnMessage(FieldPlayer player, Telegram message)
        {
            switch (message.MessageCode)
            {
                case (int)SoccerGameMessages.ReceiveBall:
                    player.SteeringBehaviors.Target = message.ExtraInfo as Vector2D;

                    player.StateMachine.ChangeState(ReceiveBallState.Instance);
                    return true;

                case (int)SoccerGameMessages.SupportAttacker:
                    //if already supporting just return
                    if (player.StateMachine.IsInState(SupportAttackerState.Instance))
                    {
                        return true;
                    }

                    //set the target to be the best supporting position
                    player.SteeringBehaviors.Target = player.Team.GetSupportSpot();

                    //change the state
                    player.StateMachine.ChangeState(SupportAttackerState.Instance);

                    return true;

                case (int)SoccerGameMessages.Wait:
                    player.StateMachine.ChangeState(WaitState.Instance);
                    return true;

                case (int)SoccerGameMessages.GoHome:
                    player.SetDefaultHomeRegion();
                    player.StateMachine.ChangeState(ReturnToHomeRegionState.Instance);
                    break;

                case (int)SoccerGameMessages.PassToMe:
                //get the position of the player requesting the pass 
                FieldPlayer receiver = message.ExtraInfo as FieldPlayer;

                    System.Diagnostics.Debug.WriteLine(string.Format("Player {0} received request from {1} to make pass", player.ObjectId, receiver.ObjectId));

                //if the ball is not within kicking range or their is already a 
                //receiving player, this player cannot pass the ball to the player
                //making the request.
                if (player.Team.ReceivingPlayer != null ||
                !player.BallInKickingRange )
                {
                System.Diagnostics.Debug.WriteLine(string.Format("Player {0} cannot make requested pass <cannot kick ball>", player.ObjectId));

                return true;
                }

                //make the pass   
                player.Ball.Kick(receiver.Position - player.Ball.Position, 
                    ParameterManager.Instance.MaxPassingForce);


                System.Diagnostics.Debug.WriteLine(string.Format("Player {0} passed ball to requesting player", player.ObjectId));

                //let the receiver know a pass is coming 
                MessageDispatcher.Instance.DispatchMsg(new TimeSpan(0),
                    player.ObjectId,
                    receiver.ObjectId,
                    (int)SoccerGameMessages.ReceiveBall,
                    receiver.Position);



                //change state   
                player.StateMachine.ChangeState(WaitState.Instance);

                player.FindSupport();

                return true;
            }

            return false;
        }
    }
}
