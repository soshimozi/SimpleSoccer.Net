using System;
using System.Collections.Generic;
using System.Text;

namespace SimpleSoccer.Net
{
    class PrepareForKickoffState : State<SoccerTeam>
    {
        public override void Enter(SoccerTeam entity)
        {
            //reset key player pointers
            entity.ControllingPlayer = null;
            entity.SupportingPlayer = null;
            entity.ReceivingPlayer = null;
            entity.PlayerClosestToBall = null;

            //send Msg_GoHome to each player.
            entity.ReturnAllFieldPlayersToHome();
        }

        public override void Execute(SoccerTeam entity)
        {
            //if both teams in position, start the game
            if (entity.AllPlayersAtHome() && entity.OpposingTeam.AllPlayersAtHome())
            {
                entity.FSM.ChangeState(DefendingState.Instance);
            }
        }

        public override void Exit(SoccerTeam entity)
        {
            entity.Pitch.GameInPlay = true;
        }

        public override bool OnMessage(SoccerTeam entity, Telegram message)
        {
            return false;
        }

        private static PrepareForKickoffState _instance = null;

        public static PrepareForKickoffState Instance
        {
            get { return (_instance == null ? _instance = new PrepareForKickoffState() : _instance); }
        }
    }
}
