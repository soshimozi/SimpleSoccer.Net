//------------------------------------------------------------------------
//
//  Name: PlayerBase.cs
//
//  Desc: Definition of a soccer player base class. The player inherits
//        from the autolist class so that any player created will be 
//        automatically added to a list that is easily accesible by any
//        other game objects. (mainly used by the steering behaviors and
//        player state classes)
//
//  Author: Mat Buckland 2003 (fup@ai-junkie.com)
//  Ported By: Scott McCain (scott_mccain@cox.net)
//------------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Text;

namespace SimpleSoccer.Net
{
    // TODO: Add Regions and Organize Code
    public class PlayerBase : MovingEntity, IDisposable
    {
        public enum PlayerRoles
        {
            GoalKeeper,
            Attacker,
            Defender
        }

        public PlayerBase(SoccerTeam homeTeam, int homeRegionIndex, Vector2D heading, Vector2D velocity,
                       double mass, double maxForce, double maxSpeed, double maxTurnRate, double scale, PlayerRoles role)
            : base(homeTeam.Pitch.GetRegion(homeRegionIndex).VectorCenter, scale * 10.0, velocity, maxSpeed, heading, mass, new Vector2D(scale, scale), maxTurnRate, maxForce)
        {
            _playerRole = role;
            _team = homeTeam;
            _distanceToBallSquared = double.MaxValue;
            _homeRegionIndex = homeRegionIndex;
            _kickoffRegionIndex = homeRegionIndex;

            Vector2D[] player = new Vector2D[4] { new Vector2D(-3, 8), new Vector2D(3, 10), new Vector2D(3, -10), new Vector2D(-3, -8) };

            for (int vertexIndex = 0; vertexIndex < 4; vertexIndex++)
            {
                _vecPlayerVB.Add(player[vertexIndex]);

                //set the bounding radius to the length of the 
                //greatest extent
                if (Math.Abs(player[vertexIndex].X) > BoundingRadius)
                {
                    BoundingRadius = Math.Abs(player[vertexIndex].X);
                }

                if (Math.Abs(player[vertexIndex].Y) > BoundingRadius)
                {
                    BoundingRadius = Math.Abs(player[vertexIndex].Y);
                }
            }

            //set up the steering behavior class
            _steeringBehaviors = new SteeringBehaviors(this, Ball);

            //a player's start target is its start position (because it's just waiting)
            _steeringBehaviors.Target = _team.Pitch.GetRegion(_homeRegionIndex).VectorCenter;

            AutoList<PlayerBase>.GetAllMembers().Add(this);

            _defaultHomeRegionIndex = _homeRegionIndex;
        }

        /// <summary>
        /// Role of this player
        /// </summary>
        private PlayerRoles _playerRole;

        /// <summary>
        /// SoccerTeam this player belongs to
        /// </summary>
        private SoccerTeam _team;

        /// <summary>
        /// Steering behavior manager
        /// </summary>
        protected SteeringBehaviors _steeringBehaviors;

        /// <summary>
        /// The default region this player is assigned to on field
        /// </summary>
        private int _defaultHomeRegionIndex;

        /// <summary>
        /// The region this player is assigned to on field
        /// </summary>
        private int _homeRegionIndex;

        /// <summary>
        /// The region this player goes to before kickoff
        /// </summary>
        private int _kickoffRegionIndex;

        /// <summary>
        /// The distance to the ball (in squared-space). This value is queried 
        /// a lot so it's calculated once each time-step and stored here.
        /// </summary>
        private double _distanceToBallSquared;

        /// <summary>
        /// The vertex buffer
        /// </summary>
        protected List<Vector2D> _vecPlayerVB = new List<Vector2D>();

        /// <summary>
        /// The buffer for the transformed vertices
        /// </summary>
        //private List<Vector2D> _vecPlayerVBTransform = new List<Vector2D>();

        public double DistanceToBallSquared
        {
            get { return _distanceToBallSquared; }
            set { _distanceToBallSquared = value; }
        }

        public SteeringBehaviors SteeringBehaviors
        {
            get { return _steeringBehaviors; }
            set { _steeringBehaviors = value; }
        }

        public SoccerBall Ball
        {
            get { return _team.Pitch.Ball; }
        }

        public int HomeRegionIndex
        {
            get { return _homeRegionIndex; }
            set { _homeRegionIndex = value; }
        }

        public Region HomeRegion
        {
            get { return _team.Pitch.GetRegion(_homeRegionIndex); }
        }

        public SoccerTeam Team
        {
            get { return _team; }
            set { _team = value; }
        }

        public SoccerPitch Pitch
        {
            get
            {
                return _team.Pitch;
            }
        }

        public PlayerRoles PlayerRole
        {
            get { return _playerRole; }
            set { _playerRole = value; }
        }

        public bool BallInKeeperRange
        {
            get { return (Vector2D.Vec2DDistanceSq(Position, Ball.Position) < ParameterManager.Instance.KeeperInBallRangeSq); }
        }

        public bool BallInKickingRange
        {
            get { return (Vector2D.Vec2DDistanceSq(Ball.Position, Position) < ParameterManager.Instance.PlayerKickingDistanceSq); }
        }

        //public bool BallInDefensiveRange
        //{
        //    get { return (Vector2D.Vec2DDistanceSq(Position, Ball.Position) < ParameterManager.Instance.DefenseInterceptRangeSq); }
        //}

        public bool BallInReceivingRange
        {
            get
            {
                return (Vector2D.Vec2DDistanceSq(Position, Ball.Position) < ParameterManager.Instance.BallWithinReceivingRangeSq);
            }
        }

        public bool InHomeRegion
        {
            get 
            { 
                return (_playerRole == PlayerRoles.GoalKeeper ?
                    Team.Pitch.GetRegion(_homeRegionIndex).IsPositionInside(Position, Region.RegionModifier.Normal) :
                    Team.Pitch.GetRegion(_homeRegionIndex).IsPositionInside(Position, Region.RegionModifier.Halfsize));
            } 
        }

        public bool IsAheadOfAttacker
        {
            get
            {
                return Math.Abs(Position.X - Team.OpponentsGoal.GoalLineCenter.X) <
                        Math.Abs(Team.ControllingPlayer.Position.X - Team.OpponentsGoal.GoalLineCenter.X);
            }
        }

        //public bool AtSupportSpot
        //{
        //    get { return (Vector2D.Vec2DDistanceSq(Position, Team.DetermineBestSupportingPosition()) < ParameterManager.Instance.PlayerInTargetRangeSq); }
        //}

        public void SetDefaultHomeRegion() { HomeRegionIndex = _defaultHomeRegionIndex; }

        public bool AtTarget
        {
            get { return (Vector2D.Vec2DDistanceSq(Position, SteeringBehaviors.Target) < ParameterManager.Instance.PlayerInTargetRangeSq); }
        }

        public bool IsClosestTeamMemberToBall
        {
            get { return Team.PlayerClosestToBall == this; }
        }

        public bool IsClosestPlayerOnPitchToGoal
        {
            get { return (DistanceToOpposingGoal < Team.OpponentClosestToGoal.DistanceToHomeGoal); }
        }

        public bool IsClosestPlayerOnPitchToBall
        {
            get
            {
                return IsClosestTeamMemberToBall && (DistanceToBallSquared < Team.OpposingTeam.DistanceToBallOfClosestPlayerSquared);
            }
        }

        public bool IsControllingPlayer
        {
            get { return (Team.ControllingPlayer == this); }
        }

        public bool InHotRegion
        {
            get 
            {
                return Math.Abs(Position.Y - Team.OpponentsGoal.GoalLineCenter.Y) < Team.Pitch.PlayingArea.Left / 3.0;
            }
        }

        public double DistanceToOpposingGoal
        {
            get { return Math.Abs(Position.X - Team.OpponentsGoal.GoalLineCenter.X); }
        }

        public double DistanceToHomeGoal
        {
            get { return Math.Abs(Position.X - Team.HomeGoal.GoalLineCenter.X); }
        }

        public void SetKickoffRegion()
        {
            _homeRegionIndex = _kickoffRegionIndex;
        }

        #region Helper Functions
        public void TrackBall()
        {
            RotateHeadingToFacePosition(Ball.Position);
        }

        public void TrackTarget()
        {
            Heading = Vector2D.Vec2DNormalize(SteeringBehaviors.Target - Position);
        }

        //------------------------------------------------------------------------
        //
        //binary predicates for std::sort (see CanPassForward/Backward)
        //------------------------------------------------------------------------
        public bool SortByDistanceToOpponentsGoal(PlayerBase p1,
                                            PlayerBase p2)
        {
            return (p1.DistanceToOpposingGoal < p2.DistanceToOpposingGoal);
        }

        public bool SortByReversedDistanceToOpponentsGoal(PlayerBase p1,
                                                    PlayerBase p2)
        {
            return (p1.DistanceToOpposingGoal > p2.DistanceToOpposingGoal);
        }

        //------------------------- WithinFieldOfView ---------------------------
        //
        //  returns true if subject is within field of view of this player
        //-----------------------------------------------------------------------
        bool IsPositionInFrontOfPlayer(Vector2D position)
        {
            Vector2D ToSubject = position - Position;

            if (ToSubject.GetDotProduct(Heading) > 0)

                return true;

            else

                return false;
        }

        //------------------------- IsThreatened ---------------------------------
        //
        //  returns true if there is an opponent within this player's 
        //  comfort zone
        //------------------------------------------------------------------------
        public bool IsThreatened()
        {
            for (int opponentIndex = 0; opponentIndex < Team.OpposingTeam.Players.Count; opponentIndex++)
            {
                //calculate distance to the player. if dist is less than our
                //comfort zone, and the opponent is infront of the player, return true
                if (IsPositionInFrontOfPlayer(Team.OpposingTeam.Players[opponentIndex].Position) &&
                   (Vector2D.Vec2DDistanceSq(Position, Team.OpposingTeam.Players[opponentIndex].Position) < ParameterManager.Instance.PlayerComfortZoneSq))
                {
                    return true;
                }

            }// next opp

            return false;
        }

        //----------------------------- FindSupport -----------------------------------
        //
        //  determines the player who is closest to the SupportSpot and messages him
        //  to tell him to change state to SupportAttacker
        //-----------------------------------------------------------------------------
        public void FindSupport()
        {
            PlayerBase BestSupportPly;

            //if there is no support we need to find a suitable player.
            if (Team.SupportingPlayer == null)
            {
                BestSupportPly = Team.DetermineBestSupportingAttacker();

                Team.SupportingPlayer = BestSupportPly;

                MessageDispatcher.Instance.DispatchMsg(new TimeSpan(0),
                                        ObjectId,
                                        Team.SupportingPlayer.ObjectId,
                                        (int)SoccerGameMessages.SupportAttacker,
                                        null);
            }

            BestSupportPly = Team.DetermineBestSupportingAttacker();

            //if the best player available to support the attacker changes, update
            //the pointers and send messages to the relevant players to update their
            //states
            if (BestSupportPly != null && (BestSupportPly != Team.SupportingPlayer))
            {

                if (Team.SupportingPlayer != null)
                {
                    MessageDispatcher.Instance.DispatchMsg(new TimeSpan(0),
                                            ObjectId,
                                            Team.SupportingPlayer.ObjectId,
                                            (int)SoccerGameMessages.GoHome,
                                            null);
                }



                Team.SupportingPlayer = BestSupportPly;

                MessageDispatcher.Instance.DispatchMsg(new TimeSpan(0),
                                        ObjectId,
                                        Team.SupportingPlayer.ObjectId,
                                        (int)SoccerGameMessages.SupportAttacker,
                                        null);
            }
        }
        #endregion

        #region IDisposable Members

        public void Dispose()
        {
            AutoList<PlayerBase>.GetAllMembers().Remove(this);
        }

        #endregion
    }
}
