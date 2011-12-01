using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using System.Linq;
//using ObjectQuery;

namespace SimpleSoccer.Net
{
    /// <summary>
    /// Defines a Team of SoccerBasePlayers for the Soccer game simulation
    /// </summary>
    public class SoccerTeam
    {
        #region Public Structs and Enums
        public enum SoccerTeamColor
        {
            Blue,
            Red
        }
        #endregion

        #region Private Constants
        public const int TeamSize = 4;
        #endregion

        #region Private Instance Fields
        private SoccerPitch _pitch;
        private SoccerTeam _opposingTeam;
        private StateMachine<SoccerTeam> _stateMachine;
        private List<PlayerBase> _players;

        private SoccerGoal _opponentsGoal;
        private SoccerGoal _homeGoal;

        // references to "key" players
        private PlayerBase _controllingPlayer = null;
        private PlayerBase _supportingPlayer = null;
        private PlayerBase _receivingPlayer = null;
        private PlayerBase _playerClosestToBall = null;
        private PlayerBase _opponentClosestToGoal = null; // used for offsides determiniation
        private SoccerTeamColor _color;

        private SupportSpotCalculator _supportSpotCalculator;

        private double _distSqToBallOfClosestPlayer = double.MaxValue;

        private Random rng = new Random();
        #endregion

        #region Construction
        public SoccerTeam(SoccerGoal homeGoal, SoccerGoal opponentsGoal, SoccerPitch pitch, SoccerTeamColor color)
        {
            _homeGoal = homeGoal;
            _opponentsGoal = opponentsGoal;
            _pitch = pitch;
            _color = color;

            _opposingTeam = null;
            _distSqToBallOfClosestPlayer = 0.0;
            _supportingPlayer = null;
            _controllingPlayer = null;
            _playerClosestToBall = null;

            _stateMachine = new StateMachine<SoccerTeam>(this);

            _stateMachine.CurrentState = DefendingState.Instance;
            _stateMachine.PreviousState = DefendingState.Instance;
            _stateMachine.GlobalState = null;

            //create the players and goalkeeper
            CreatePlayers();

            //set default steering behaviors
            foreach (PlayerBase player in _players)
            {
                player.SteeringBehaviors.Seperation = true;
            }

            _supportSpotCalculator = new SupportSpotCalculator(ParameterManager.Instance.NumSupportSpotsX, ParameterManager.Instance.NumSupportSpotsY, this);
        }
        #endregion

        #region Public Instance Properties
        public StateMachine<SoccerTeam> FSM
        {
            get { return _stateMachine; }
        }

        public SoccerTeamColor Color
        {
            get { return _color; }
            set { _color = value; }
        }

        public double DistanceToBallOfClosestPlayerSquared
        {
            get { return _distSqToBallOfClosestPlayer; }
        }

        public PlayerBase ControllingPlayer
        {
            get { return _controllingPlayer; }
            set 
            {
                if (value != null)
                {
                    OpposingTeam.LostControl();
                }

                _controllingPlayer = value;
            }
        }

        private void LostControl()
        {
            _controllingPlayer = null;
        }

        public PlayerBase SupportingPlayer
        {
            get { return _supportingPlayer; }
            set { _supportingPlayer = value; }
        }

        public PlayerBase ReceivingPlayer
        {
            get { return _receivingPlayer; }
            set { _receivingPlayer = value; }
        }

        public PlayerBase PlayerClosestToBall
        {
            get { return _playerClosestToBall; }
            set { _playerClosestToBall = value; }
        }

        public PlayerBase OpponentClosestToGoal
        {
            get { return _opponentClosestToGoal; }
            set { _opponentClosestToGoal = value; }
        }

        public SoccerGoal HomeGoal
        {
            get { return _homeGoal; }
            set { _homeGoal = value; }
        }

        public SoccerGoal OpponentsGoal
        {
            get { return _opponentsGoal; }
            set { _opponentsGoal = value; }
        }

        public List<PlayerBase> Players
        {
            get { return _players; }
        }

        public SoccerTeam OpposingTeam
        {
            get { return _opposingTeam; }
            set { _opposingTeam = value; }
        }

        public SoccerPitch Pitch
        {
            get { return _pitch; }
            set { _pitch = value; }
        }

        public bool InControl
        {
            get { return _controllingPlayer != null; }
        }
        #endregion

        #region Public Instance Methods
        /// <summary>
        /// Called every tick to update the team state
        /// </summary>
        public void Update()
        {
            //this information is used frequently so it's more efficient to 
            //calculate it just once each frame
            CalculateClosestPlayerToBall();

            CalculateClosestOpponentToGoal();

            //the team state machine switches between attack/defense behavior. It
            //also handles the 'kick off' state where a team must return to their
            //kick off positions before the whistle is blown
            _stateMachine.Update();

            //now update each player
            foreach (PlayerBase player in _players)
            {
                player.Update();
            }
        }

        /// <summary>
        /// Calculates the closest opponent to this teams goal
        /// </summary>
        public void CalculateClosestOpponentToGoal()
        {
            double ClosestSoFar = double.MaxValue;


            for (int memberIndex = 0; memberIndex < _opposingTeam.Players.Count; memberIndex++)
            {
                //calculate the dist. to home goal, remember the opponents home goal
                // is our target
                if (_opposingTeam.Players[memberIndex].PlayerRole != PlayerBase.PlayerRoles.GoalKeeper)
                {
                    double dist = _opposingTeam.Players[memberIndex].DistanceToHomeGoal;
                    if (dist < ClosestSoFar)
                    {
                        ClosestSoFar = dist;
                        _opponentClosestToGoal = _opposingTeam.Players[memberIndex];
                    }
                }
            }
        }

        /// <summary>
        /// sets _playerClosestToBall to the player closest to the ball
        /// </summary>
        public void CalculateClosestPlayerToBall()
        {
            double ClosestSoFar = double.MaxValue;

            for (int playerIndex = 0; playerIndex < _players.Count; playerIndex++)
            {
                //calculate the dist. Use the squared value to avoid sqrt
                double dist = Vector2D.Vec2DDistanceSq(_players[playerIndex].Position, Pitch.Ball.Position);

                //keep a record of this value for each player
                _players[playerIndex].DistanceToBallSquared = dist;

                if (dist < ClosestSoFar)
                {
                    ClosestSoFar = dist;

                    _playerClosestToBall = _players[playerIndex];
                }
            }

            _distSqToBallOfClosestPlayer = ClosestSoFar;
        }


        /// <summary>
        /// calculate the closest player to the SupportSpot
        /// 
        /// </summary>
        /// <returns></returns>
        public PlayerBase DetermineBestSupportingAttacker()
        {
            double ClosestSoFar = double.MaxValue;

            PlayerBase BestPlayer = null;


            for (int playerIndex = 0; playerIndex < _players.Count; playerIndex++)
            {
                //only attackers utilize the BestSupportingSpot
                if ((_players[playerIndex].PlayerRole == PlayerBase.PlayerRoles.Attacker) && (_players[playerIndex] != _controllingPlayer))
                {
                    //calculate the dist. Use the squared value to avoid sqrt
                    double dist = Vector2D.Vec2DDistanceSq(_players[playerIndex].Position, _supportSpotCalculator.GetBestSupportingSpot());

                    //if the distance is the closest so far and the player is not a
                    //goalkeeper and the player is not the one currently controlling
                    //the ball, keep a record of this player
                    if ((dist < ClosestSoFar))
                    {
                        ClosestSoFar = dist;

                        BestPlayer = _players[playerIndex];
                    }
                }
            }

            return BestPlayer;
        }

        /// <summary>
        ///  The best pass is considered to be the pass that cannot be intercepted 
        ///  by an opponent and that is as far forward of the receiver as possible
        /// 
        /// </summary>
        /// <param name="passer"></param>
        /// <param name="receiver"></param>
        /// <param name="PassTarget"></param>
        /// <param name="power"></param>
        /// <param name="MinPassingDistance"></param>
        /// <returns></returns>
        public bool FindPass(PlayerBase passer,
                                 out PlayerBase receiver,
                                 out Vector2D PassTarget,
                                 double power,
                                 double MinPassingDistance)
        {

            receiver = null;
            PassTarget = null;

            double ClosestToGoalSoFar = double.MaxValue;
            Vector2D Target = new Vector2D();

            //iterate through all this player's team members and calculate which
            //one is in a position to be passed the ball 
            foreach (PlayerBase currentPlayer in _players)
            {
                //make sure the potential receiver being examined is not this player
                //and that it is further away than the minimum pass distance
                if ((currentPlayer != passer) &&
                    (Vector2D.Vec2DDistanceSq(passer.Position, currentPlayer.Position) >
                     MinPassingDistance * MinPassingDistance))
                {
                    if (GetBestPassToReceiver(passer, currentPlayer, ref Target, power))
                    {
                        //if the pass target is the closest to the opponent's goal line found
                        // so far, keep a record of it
                        double Dist2Goal = Math.Abs(Target.X - OpponentsGoal.GoalLineCenter.X);

                        if (Dist2Goal < ClosestToGoalSoFar)
                        {
                            ClosestToGoalSoFar = Dist2Goal;

                            //keep a record of this player
                            receiver = currentPlayer;

                            //and the target
                            PassTarget = Target;
                        }
                    }
                }
            }//next team member

            if (receiver != null) return true;

            else return false;
        }


        /// <summary>
        ///  Three potential passes are calculated. One directly toward the receiver's
        ///  current position and two that are the tangents from the ball position
        ///  to the circle of radius 'range' from the receiver.
        ///  These passes are then tested to see if they can be intercepted by an
        ///  opponent and to make sure they terminate within the playing area. If
        ///  all the passes are invalidated the function returns false. Otherwise
        ///  the function returns the pass that takes the ball closest to the 
        ///  opponent's goal area.
        /// 
        /// </summary>
        /// <param name="passer"></param>
        /// <param name="receiver"></param>
        /// <param name="PassTarget"></param>
        /// <param name="power"></param>
        /// <returns></returns>
        public bool GetBestPassToReceiver(PlayerBase passer,
                                               PlayerBase receiver,
                                               ref Vector2D PassTarget,
                                               double power)
        {
            //first, calculate how much time it will take for the ball to reach 
            //this receiver, if the receiver was to remain motionless 
            double time = Pitch.Ball.CalucateTimeToCoverDistance(Pitch.Ball.Position,
                                                              receiver.Position,
                                                              power);

            //return false if ball cannot reach the receiver after having been
            //kicked with the given power
            if (time < 0) return false;

            //the maximum distance the receiver can cover in this time
            double InterceptRange = time * receiver.MaxSpeed;

            //Scale the intercept range
            const double ScalingFactor = 0.3;
            InterceptRange *= ScalingFactor;

            //now calculate the pass targets which are positioned at the intercepts
            //of the tangents from the ball to the receiver's range circle.
            Vector2D ip1 = new Vector2D(), ip2 = new Vector2D();

            Geometry.GetTangentPoints(receiver.Position,
                             InterceptRange,
                             Pitch.Ball.Position,
                             ip1,
                             ip2);

            const int NumPassesToTry = 3;
            Vector2D[] Passes = new Vector2D[NumPassesToTry] { ip1, receiver.Position, ip2 };


            // this pass is the best found so far if it is:
            //
            //  1. Further upfield than the closest valid pass for this receiver
            //     found so far
            //  2. Within the playing area
            //  3. Cannot be intercepted by any opponents

            double ClosestSoFar = double.MaxValue;
            bool bResult = false;

            for (int pass = 0; pass < NumPassesToTry; ++pass)
            {
                double dist = Math.Abs(Passes[pass].X - OpponentsGoal.GoalLineCenter.X);

                if ((dist < ClosestSoFar) &&
                    Pitch.PlayingArea.IsPositionInside(Passes[pass]) &&
                    IsPassSafeFromAllOpponents(Pitch.Ball.Position,
                                               Passes[pass],
                                               receiver,
                                               power))
                {
                    ClosestSoFar = dist;
                    PassTarget = Passes[pass];
                    bResult = true;
                }
            }

            return bResult;
        }


        /// <summary>
        ///  test if a pass from 'from' to 'to' can be intercepted by an opposing player
        /// 
        /// </summary>
        /// <param name="from"></param>
        /// <param name="target"></param>
        /// <param name="receiver"></param>
        /// <param name="opp"></param>
        /// <param name="PassingForce"></param>
        /// <returns></returns>
        public bool IsPassSafeFromOpponent(Vector2D from,
                                                Vector2D target,
                                                PlayerBase receiver,
                                                PlayerBase opp,
                                                double PassingForce)
        {
            //move the opponent into local space.
            Vector2D ToTarget = target - from;
            Vector2D ToTargetNormalized = Vector2D.Vec2DNormalize(ToTarget);

            Vector2D LocalPosOpp = Transformations.PointToLocalSpace(opp.Position,
                                                   ToTargetNormalized,
                                                   ToTargetNormalized.Perp,
                                                   from);

            //if opponent is behind the kicker then pass is considered okay(this is 
            //based on the assumption that the ball is going to be kicked with a 
            //velocity greater than the opponent's max velocity)
            if (LocalPosOpp.X < 0)
            {
                return true;
            }

            //if the opponent is further away than the target we need to consider if
            //the opponent can reach the position before the receiver.
            if (Vector2D.Vec2DDistanceSq(from, target) < Vector2D.Vec2DDistanceSq(opp.Position, from))
            {
                if (receiver != null)
                {
                    if (Vector2D.Vec2DDistanceSq(target, opp.Position) >
                         Vector2D.Vec2DDistanceSq(target, receiver.Position))
                    {
                        return true;
                    }

                    else
                    {
                        return false;
                    }

                }

                else
                {
                    return true;
                }
            }

            //calculate how long it takes the ball to cover the distance to the 
            //position orthogonal to the opponents position
            double TimeForBall =
                Pitch.Ball.CalucateTimeToCoverDistance(new Vector2D(0, 0),
                                                 new Vector2D(LocalPosOpp.X, 0),
                                                 PassingForce);

            //now calculate how far the opponent can run in this time
            double reach = opp.MaxSpeed * TimeForBall +
                          Pitch.Ball.BoundingRadius +
                          opp.BoundingRadius;

            //if the distance to the opponent's y position is less than his running
            //range plus the radius of the ball and the opponents radius then the
            //ball can be intercepted
            if (Math.Abs(LocalPosOpp.Y) < reach)
            {
                return false;
            }

            return true;
        }

        /// <summary>
        ///  Tests a pass from position 'from' to position 'target' against each member
        ///  of the opposing team. Returns true if the pass can be made without
        ///  getting intercepted
        /// 
        /// </summary>
        /// <param name="from"></param>
        /// <param name="target"></param>
        /// <param name="receiver"></param>
        /// <param name="PassingForce"></param>
        /// <returns></returns>
        public bool IsPassSafeFromAllOpponents(Vector2D from,
                                                    Vector2D target,
                                                    PlayerBase receiver,
                                                    double PassingForce)
        {
            foreach (PlayerBase opponent in OpposingTeam.Players)
            {
                if (!IsPassSafeFromOpponent(from, target, receiver, opponent, PassingForce))
                {
                    return false;
                }
            }

            return true;
        }

        public bool CanShoot(Vector2D BallPos,
                                  double power)
        {
            Vector2D vect = new Vector2D(0, 0);
            return CanShoot(BallPos, power, ref vect);
        }
                                  
        /// <summary>
        ///  Given a ball position, a kicking power and a reference to a vector2D
        ///  this function will sample random positions along the opponent's goal-
        ///  mouth and check to see if a goal can be scored if the ball was to be
        ///  kicked in that direction with the given power. If a possible shot is 
        ///  found, the function will immediately return true, with the target 
        ///  position stored in the vector ShotTarget.
        /// 
        /// </summary>
        /// <param name="BallPos"></param>
        /// <param name="power"></param>
        /// <param name="ShotTarget"></param>
        /// <returns></returns>
        public bool CanShoot(Vector2D BallPos,
                                  double power,
                                  ref Vector2D ShotTarget)
        {
            //the number of randomly created shot targets this method will test 
            int NumAttempts = ParameterManager.Instance.NumAttemptsToFindValidStrike;

            while (NumAttempts-- > 0)
            {
                //choose a random position along the opponent's goal mouth. (making
                //sure the ball's radius is taken into account)
                ShotTarget = OpponentsGoal.GoalLineCenter;

                //the y value of the shot position should lay somewhere between two
                //goalposts (taking into consideration the ball diameter)
                int MinYVal = (int)(OpponentsGoal.LeftPost.Y + Pitch.Ball.BoundingRadius);
                int MaxYVal = (int)(OpponentsGoal.RightPost.Y - Pitch.Ball.BoundingRadius);

                ShotTarget.Y = (double)rng.Next(MinYVal, MaxYVal);

                //make sure striking the ball with the given power is enough to drive
                //the ball over the goal line.
                double time = Pitch.Ball.CalucateTimeToCoverDistance(BallPos,
                                                                  ShotTarget,
                                                                  power);

                //if it is, this shot is then tested to see if any of the opponents
                //can intercept it.
                if (time >= 0)
                {
                    if (IsPassSafeFromAllOpponents(BallPos, ShotTarget, null, power))
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        /// <summary>
        ///  sends a message to all players to return to their home areas forthwith
        /// 
        /// </summary>
        public void ReturnAllFieldPlayersToHome()
        {
            foreach (PlayerBase player in _players)
            {
                if (player.PlayerRole != PlayerBase.PlayerRoles.GoalKeeper)
                {
                    MessageDispatcher.Instance.DispatchMsg(new TimeSpan(0), 0, player.ObjectId, (int)SoccerGameMessages.GoHome, null);
                }
            }
        }

        /// <summary>
        /// Render the soccer team
        /// </summary>
        /// <param name="g"></param>
        public void Render(Graphics g)
        {
            foreach (PlayerBase player in _players)
            {
                player.Render(g);
            }

            Brush textBrush = new SolidBrush(System.Drawing.Color.White);

            //show the controlling team and player at the top of the display
            if (ParameterManager.Instance.ShowControllingTeam && this.InControl)
            {
                if (Color == SoccerTeamColor.Blue)
                {
                    g.DrawString("Blue in Control", GDI.TextFont, textBrush, new Point(20, 3));
                }
                else if (Color == SoccerTeamColor.Red)
                {
                    g.DrawString("Red in Control", GDI.TextFont, textBrush, new Point(20, 3));
                }

                string text = string.Format("Controlling Player: {0}", ControllingPlayer.ObjectId);
                g.DrawString(text, GDI.TextFont, textBrush, new Point(Pitch.ClientWidth - (int)g.MeasureString(text, GDI.TextFont).Width - 20, 3));
            }

            if (ParameterManager.Instance.ShowSupportSpots && this.InControl)
            {
                _supportSpotCalculator.Render(g);
            }
        }

        /// <summary>
        /// Finds a player by its id
        /// </summary>
        /// <param name="playerId"></param>
        /// <returns></returns>
        public PlayerBase FindPlayerById(int playerId)
        {
            return _players.Where(p => p.ObjectId == playerId).FirstOrDefault();
        }

        /// <summary>
        /// Sets a particular players home region index
        /// </summary>
        /// <param name="playerId"></param>
        /// <param name="region"></param>
        public void SetPlayerHomeRegion(int playerId, int region)
        {
            PlayerBase player = FindPlayerById(playerId);
            if (player != null)
            {
                player.HomeRegionIndex = region;
            }
        }

        /// <summary>
        /// Finds targets for waiting players
        /// </summary>
        public void UpdateTargetsOfWaitingPlayers()
        {
            foreach (PlayerBase player in _players)
            {
                if (player.PlayerRole != PlayerBase.PlayerRoles.GoalKeeper)
                {
                    FieldPlayer fieldPlayer = player as FieldPlayer;
                    if (fieldPlayer.StateMachine.IsInState(WaitState.Instance) ||
                        fieldPlayer.StateMachine.IsInState(ReturnToHomeRegionState.Instance))
                    {
                        fieldPlayer.SteeringBehaviors.Target = fieldPlayer.HomeRegion.VectorCenter;
                    }
                }
            }
        }


        /// <summary>
        ///  Determines if any of the team are not located within their home region
        /// 
        /// </summary>
        /// <returns></returns>
        public bool AllPlayersAtHome()
        {
            foreach (PlayerBase player in _players)
            {
                if (!player.InHomeRegion)
                {
                    return false;
                }
            }

            return true;
        }

        /// <summary>
        ///  This tests to see if a pass is possible between the requester and
        ///  the controlling player. If it is possible a message is sent to the
        ///  controlling player to pass the ball asap.
        /// 
        /// </summary>
        /// <param name="requester"></param>
        public void RequestPass(FieldPlayer requester)
        {
            //maybe put a restriction here
            if (rng.NextDouble() > 0.1) return;

            if (IsPassSafeFromAllOpponents(ControllingPlayer.Position,
                                           requester.Position,
                                           requester,
                                           ParameterManager.Instance.MaxPassingForce))
            {

                //tell the player to make the pass
                //let the receiver know a pass is coming 
                MessageDispatcher.Instance.DispatchMsg(new TimeSpan(0),
                                      requester.ObjectId,
                                      ControllingPlayer.ObjectId,
                                      (int)SoccerGameMessages.PassToMe,
                                      requester);

            }
        }


        /// <summary>
        ///  returns true if an opposing player is within the radius of the position
        ///  given as a parameter
        /// 
        /// </summary>
        /// <param name="pos"></param>
        /// <param name="rad"></param>
        /// <returns></returns>
        public bool IsOpponentWithinRadius(Vector2D pos, double rad)
        {

            foreach (PlayerBase opponent in OpposingTeam.Players)
            {
                if (Vector2D.Vec2DDistanceSq(pos, opponent.Position) < rad * rad)
                    return true;
            }
            return false;
        }


        /// <summary>
        /// Calculates the best supporting position
        /// </summary>
        public void DetermineBestSupportingPosition()
        {
            _supportSpotCalculator.DetermineBestSupportingPosition();
        }

        public   Vector2D  GetSupportSpot()
        {
            return _supportSpotCalculator.GetBestSupportingSpot();
        }

        #endregion

        #region Private Instance Methods
        /// <summary>
        /// Create the players for the team
        /// </summary>
        private void CreatePlayers()
        {

            _players = new List<PlayerBase>();
            if (Color == SoccerTeamColor.Blue)
            {
                //goalkeeper
                _players.Add(new GoalKeeper(this,
                                       1,
                                       TendGoalState.Instance,
                                       new Vector2D(0, 1),
                                       new Vector2D(0.0, 0.0),
                                       ParameterManager.Instance.PlayerMass,
                                       ParameterManager.Instance.PlayerMaxForce,
                                       ParameterManager.Instance.PlayerMaxSpeedWithoutBall,
                                       ParameterManager.Instance.PlayerMaxTurnRate,
                                       ParameterManager.Instance.PlayerScale));

                //create the players
                //_players.Add(new FieldPlayer(this,
                //                       3,
                //                       WaitState.Instance,
                //                       new Vector2D(0, 1),
                //                       new Vector2D(0.0, 0.0),
                //                       ParameterManager.Instance.PlayerMass,
                //                       ParameterManager.Instance.PlayerMaxForce,
                //                       ParameterManager.Instance.PlayerMaxSpeedWithoutBall,
                //                       ParameterManager.Instance.PlayerMaxTurnRate,
                //                       ParameterManager.Instance.PlayerScale,
                //                       SoccerPlayerBase.PlayerRoles.Defender));

                _players.Add(new FieldPlayer(this,
                                       4,
                                       WaitState.Instance,
                                       new Vector2D(0, 1),
                                       new Vector2D(0.0, 0.0),
                                       ParameterManager.Instance.PlayerMass,
                                       ParameterManager.Instance.PlayerMaxForce,
                                       ParameterManager.Instance.PlayerMaxSpeedWithoutBall,
                                       ParameterManager.Instance.PlayerMaxTurnRate,
                                       ParameterManager.Instance.PlayerScale,
                                       PlayerBase.PlayerRoles.Defender));

                //_players.Add(new FieldPlayer(this,
                //                       5,
                //                       WaitState.Instance,
                //                       new Vector2D(0, 1),
                //                       new Vector2D(0.0, 0.0),
                //                       ParameterManager.Instance.PlayerMass,
                //                       ParameterManager.Instance.PlayerMaxForce,
                //                       ParameterManager.Instance.PlayerMaxSpeedWithoutBall,
                //                       ParameterManager.Instance.PlayerMaxTurnRate,
                //                       ParameterManager.Instance.PlayerScale,
                //                      SoccerPlayerBase.PlayerRoles.Defender));

                //create the players
                _players.Add(new FieldPlayer(this,
                                       6,
                                       WaitState.Instance,
                                       new Vector2D(0, 1),
                                       new Vector2D(0.0, 0.0),
                                       ParameterManager.Instance.PlayerMass,
                                       ParameterManager.Instance.PlayerMaxForce,
                                       ParameterManager.Instance.PlayerMaxSpeedWithoutBall,
                                       ParameterManager.Instance.PlayerMaxTurnRate,
                                       ParameterManager.Instance.PlayerScale,
                                       PlayerBase.PlayerRoles.Attacker));


                //_players.Add(new FieldPlayer(this,
                //                       7,
                //                       WaitState.Instance,
                //                       new Vector2D(0, 1),
                //                       new Vector2D(0.0, 0.0),
                //                       ParameterManager.Instance.PlayerMass,
                //                       ParameterManager.Instance.PlayerMaxForce,
                //                       ParameterManager.Instance.PlayerMaxSpeedWithoutBall,
                //                       ParameterManager.Instance.PlayerMaxTurnRate,
                //                       ParameterManager.Instance.PlayerScale,
                //                      SoccerPlayerBase.PlayerRoles.Attacker));

                _players.Add(new FieldPlayer(this,
                                       8,
                                       WaitState.Instance,
                                       new Vector2D(0, 1),
                                       new Vector2D(0.0, 0.0),
                                       ParameterManager.Instance.PlayerMass,
                                       ParameterManager.Instance.PlayerMaxForce,
                                       ParameterManager.Instance.PlayerMaxSpeedWithoutBall,
                                       ParameterManager.Instance.PlayerMaxTurnRate,
                                       ParameterManager.Instance.PlayerScale,
                                       PlayerBase.PlayerRoles.Attacker));

            }

            else
            {


                //create the players
                _players.Add(new FieldPlayer(this,
                                           9,
                                           WaitState.Instance,
                                           new Vector2D(0, -1),
                                           new Vector2D(0.0, 0.0),
                                           ParameterManager.Instance.PlayerMass,
                                           ParameterManager.Instance.PlayerMaxForce,
                                           ParameterManager.Instance.PlayerMaxSpeedWithoutBall,
                                           ParameterManager.Instance.PlayerMaxTurnRate,
                                           ParameterManager.Instance.PlayerScale,
                                           PlayerBase.PlayerRoles.Attacker));

                //_players.Add(new FieldPlayer(this,
                //                           10,
                //                           WaitState.Instance,
                //                           new Vector2D(0, -1),
                //                           new Vector2D(0.0, 0.0),
                //                           ParameterManager.Instance.PlayerMass,
                //                           ParameterManager.Instance.PlayerMaxForce,
                //                           ParameterManager.Instance.PlayerMaxSpeedWithoutBall,
                //                           ParameterManager.Instance.PlayerMaxTurnRate,
                //                           ParameterManager.Instance.PlayerScale,
                //                           SoccerPlayerBase.PlayerRoles.Attacker));

                _players.Add(new FieldPlayer(this,
                                           11,
                                           WaitState.Instance,
                                           new Vector2D(0, -1),
                                           new Vector2D(0.0, 0.0),
                                           ParameterManager.Instance.PlayerMass,
                                           ParameterManager.Instance.PlayerMaxForce,
                                           ParameterManager.Instance.PlayerMaxSpeedWithoutBall,
                                           ParameterManager.Instance.PlayerMaxTurnRate,
                                           ParameterManager.Instance.PlayerScale,
                                           PlayerBase.PlayerRoles.Attacker));



                //_players.Add(new FieldPlayer(this,
                //                           12,
                //                           WaitState.Instance,
                //                           new Vector2D(0, -1),
                //                           new Vector2D(0.0, 0.0),
                //                           ParameterManager.Instance.PlayerMass,
                //                           ParameterManager.Instance.PlayerMaxForce,
                //                           ParameterManager.Instance.PlayerMaxSpeedWithoutBall,
                //                           ParameterManager.Instance.PlayerMaxTurnRate,
                //                           ParameterManager.Instance.PlayerScale,
                //                           SoccerPlayerBase.PlayerRoles.Defender));


                _players.Add(new FieldPlayer(this,
                                           13,
                                           WaitState.Instance,
                                           new Vector2D(0, -1),
                                           new Vector2D(0.0, 0.0),
                                           ParameterManager.Instance.PlayerMass,
                                           ParameterManager.Instance.PlayerMaxForce,
                                           ParameterManager.Instance.PlayerMaxSpeedWithoutBall,
                                           ParameterManager.Instance.PlayerMaxTurnRate,
                                           ParameterManager.Instance.PlayerScale,
                                           PlayerBase.PlayerRoles.Defender));

                //_players.Add(new FieldPlayer(this,
                //                           14,
                //                           WaitState.Instance,
                //                           new Vector2D(0, -1),
                //                           new Vector2D(0.0, 0.0),
                //                           ParameterManager.Instance.PlayerMass,
                //                           ParameterManager.Instance.PlayerMaxForce,
                //                           ParameterManager.Instance.PlayerMaxSpeedWithoutBall,
                //                           ParameterManager.Instance.PlayerMaxTurnRate,
                //                           ParameterManager.Instance.PlayerScale,
                //                           SoccerPlayerBase.PlayerRoles.Defender));

                //goalkeeper
                _players.Add(new GoalKeeper(this,
                                           16,
                                           TendGoalState.Instance,
                                           new Vector2D(0, -1),
                                           new Vector2D(0.0, 0.0),
                                           ParameterManager.Instance.PlayerMass,
                                           ParameterManager.Instance.PlayerMaxForce,
                                           ParameterManager.Instance.PlayerMaxSpeedWithoutBall,
                                           ParameterManager.Instance.PlayerMaxTurnRate,
                                           ParameterManager.Instance.PlayerScale));

            }

            //register the players with the entity manager
            foreach (PlayerBase player in _players)
            {
                EntityManager.Instance.RegisterEntity(player);
            }
        }

        #endregion
    }
}
