using System;
using System.Collections.Generic;
using System.Text;

namespace SimpleSoccer.Net
{
    class PutBallBackInPlayState : State<GoalKeeper>
    {
        private static PutBallBackInPlayState _instance = null;
        public static PutBallBackInPlayState Instance
        {
            get
            {
                if (_instance == null) _instance = new PutBallBackInPlayState();

                return _instance;
            }
        }

        public override void Enter(GoalKeeper keeper)
        {
            //let the team know that the keeper is in control
            keeper.Team.ControllingPlayer = keeper;

            //send all the players home
            keeper.Team.OpposingTeam.ReturnAllFieldPlayersToHome();
            keeper.Team.ReturnAllFieldPlayersToHome();

        }

        public override void Execute(GoalKeeper keeper)
        {
            PlayerBase receiver = null;
            Vector2D BallTarget;

            //test if there are players further forward on the field we might
            //be able to pass to. If so, make a pass.
            if (keeper.Team.FindPass(keeper,
                                        out receiver,
                                        out BallTarget,
                                        ParameterManager.Instance.MaxPassingForce,
                                        ParameterManager.Instance.GoalkeeperMinPassDist))
            {
                //make the pass   
                keeper.Ball.Kick(Vector2D.Vec2DNormalize(BallTarget - keeper.Ball.Position),
                                     ParameterManager.Instance.MaxPassingForce);

                //goalkeeper no longer has ball 
                keeper.Pitch.GoalKeeperHasBall = false;

                //let the receiving player know the ball's comin' at him
                MessageDispatcher.Instance.DispatchMsg(new TimeSpan(0), keeper.ObjectId,
                                    receiver.ObjectId,
                                    (int)SoccerGameMessages.ReceiveBall,
                                    BallTarget);

                //go back to tending the goal   
                keeper.StateMachine.ChangeState(TendGoalState.Instance);

                return;
            }

            keeper.Velocity = new Vector2D(0, 0);
        }

        public override void Exit(GoalKeeper entity)
        {
        }

        public override bool OnMessage(GoalKeeper entity, Telegram message)
        {
            return false;
        }


    }
}
