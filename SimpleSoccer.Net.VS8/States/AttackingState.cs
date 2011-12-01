using System;
using System.Collections.Generic;
using System.Text;

namespace SimpleSoccer.Net
{
    class AttackingState : State<SoccerTeam>
    {
        void ChangePlayerHomeRegions(SoccerTeam team, int[] NewRegions)
        {
            for (int plyr = 0; plyr < SoccerTeam.TeamSize; ++plyr)
            {
                team.SetPlayerHomeRegion(team.Players[plyr].ObjectId, NewRegions[plyr]);
            }
        }

        public override void Enter(SoccerTeam entity)
        {
            //these define the home regions for this state of each of the players
            //int[] BlueRegions = new int[SoccerTeam.TeamSize] { 1, 3, 7, 5, 9, 10, 11 };
            //int[] RedRegions = new int[SoccerTeam.TeamSize] { 6, 7, 8, 12, 10, 14, 16 };

            int[] BlueRegions = new int[SoccerTeam.TeamSize] { 1, 4, 6, 8 };
            int[] RedRegions = new int[SoccerTeam.TeamSize] { 9, 11, 13, 16 };

            //set up the player's home regions
            if (entity.Color == SoccerTeam.SoccerTeamColor.Blue)
            {
                ChangePlayerHomeRegions(entity, BlueRegions);
            }
            else
            {
                ChangePlayerHomeRegions(entity, RedRegions);
            }

            //if a player is in either the Wait or ReturnToHomeRegion states, its
            //steering target must be updated to that of its new home region to enable
            //it to move into the correct position.
            entity.UpdateTargetsOfWaitingPlayers();
        }

        public override void Execute(SoccerTeam entity)
        {
            //if this team is no longer in control change states
            if (entity.ControllingPlayer == null)
            {
                entity.FSM.ChangeState(DefendingState.Instance);
                return;
            }

            //calculate the best position for any supporting attacker to move to
            entity.DetermineBestSupportingPosition();
        }

        public override void Exit(SoccerTeam entity)
        {
            entity.SupportingPlayer = null;
        }

        public override bool OnMessage(SoccerTeam entity, Telegram message)
        {
            return false;
        }

        private static AttackingState _instance = null;
        public static AttackingState Instance
        {
            get { return (_instance == null ? _instance = new AttackingState() : _instance); }
        }

    }
}
