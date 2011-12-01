//------------------------------------------------------------------------
//
//  Name:   SteeringBehaviors.cs
//
//  Desc:   class to encapsulate steering behaviors for a soccer player
//
//  Author: Mat Buckland 2002 (fup@ai-junkie.com)
//  Ported By: Scott McCain (scott_mccain@cox.net)
//------------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;

namespace SimpleSoccer.Net
{
    // TODO: Add Regions and Organize Code
    public class SteeringBehaviors
    {
        /// <summary>
        /// Arrive makes use of these to determine how quickly a vehicle
        /// should decelerate to its target
        /// </summary>
        public enum DecelerationState
        { 
            Slow = 3, 
            Normal = 2, 
            Fast = 1 
        };

        enum BehaviorFlags
        {
            None = 0x0000,
            Seek = 0x0001,
            Arrive = 0x0002,
            Separation = 0x0004,
            Pursuit = 0x0008,
            Interpose = 0x0010
        };

        private SoccerBall _ball;
        private PlayerBase _player;
        private Vector2D _steeringForce = new Vector2D();
        private Vector2D _target = new Vector2D();
        private double _viewDistance = 0.0;
        private double _interposeDist = 0.0;
        private bool _tagged = false;

        /// <summary>
        /// Binary flags to indicate whether or not a behavior should be active 
        /// </summary>
        protected int _behaviorFlags;

        /// <summary>
        /// Multipliers. 
        /// </summary>
        protected double _separationMultiple;

        /// <summary>
        /// A vertex buffer to contain the feelers rqd for dribbling 
        /// </summary>
        protected List<Vector2D> _dribbleFeelers;


        /// <summary>
        ///  This function calculates how much of its max steering force the 
        ///  vehicle has left to apply and then applies that amount of the
        ///  force to add.
        /// </summary>
        /// <param name="steeringForce"></param>
        /// <param name="additionalForce"></param>
        /// <returns></returns>
        protected bool accumulateForce(ref Vector2D steeringForce, Vector2D additionalForce)
        {
            bool accumulated = false;

            //first calculate how much steering force we have left to use
            double magnitudeSoFar = steeringForce.Length;

            double magnitudeRemaining = _player.MaxForce - magnitudeSoFar;

            //return false if there is no more force left to use
            if (magnitudeRemaining <= 0.0) return false;

            //calculate the magnitude of the force we want to add
            double magnitudeToAdd = additionalForce.Length;

            //now calculate how much of the force we can really add  
            if (magnitudeToAdd > magnitudeRemaining)
            {
                magnitudeToAdd = magnitudeRemaining;
            }


            //add it to the steering force
            Vector2D add = (Vector2D.Vec2DNormalize(additionalForce) * magnitudeToAdd);
            steeringForce += add;

            //steeringForce.X += add.X;
            //steeringForce.Y += add.Y;

            accumulated = true;


            return accumulated;
        }
        
        /// <summary>
        /// This calculates a force repelling from the other neighbors
        /// </summary>
        /// <returns></returns>
        protected Vector2D calculateSeparationVector()
        {
            //iterate through all the neighbors and calculate the vector from the
            Vector2D steeringForce = new Vector2D();

            List<PlayerBase> allPlayers = AutoList<PlayerBase>.GetAllMembers();
            for (int playerIndex = 0; playerIndex < allPlayers.Count; playerIndex++)
            {
                //make sure this agent isn't included in the calculations and that
                //the agent is close enough
                if (allPlayers[playerIndex] != _player && allPlayers[playerIndex].SteeringBehaviors.Tagged)
                {
                    Vector2D toAgent = _player.Position - allPlayers[playerIndex].Position;

                    //scale the force inversely proportional to the agents distance  
                    //from its neighbor.
                    if (Math.Abs(toAgent.Length) > double.Epsilon)
                    {
                        steeringForce += Vector2D.Vec2DNormalize(toAgent) / toAgent.Length;
                    }
                }
            }

            return steeringForce;
        }

        /// <summary>
        ///  Given a target, this behavior returns a steering force which will
        ///  allign the agent with the target and move the agent in the desired
        ///  direction
        /// </summary>
        /// <returns></returns>
        protected Vector2D calculateSeekVector(Vector2D target)
        {

            Vector2D desiredVelocity = Vector2D.Vec2DNormalize(target - _player.Position)
                                      * _player.MaxSpeed;

            return (desiredVelocity - _player.Velocity);
        }

        /// <summary>
        ///  This behavior is similar to seek but it attempts to arrive at the
        ///  target with a zero velocity
        /// </summary>
        /// <param name="target"></param>
        /// <param name="decel"></param>
        /// <returns></returns>
        protected Vector2D calculateArriveVector(Vector2D target, DecelerationState decel)
        {
            Vector2D toTarget = target - _player.Position;

            //calculate the distance to the target
            double dist = toTarget.Length;

            if (dist > 0.0)
            {
                //because Deceleration is enumerated as an int, this value is required
                //to provide fine tweaking of the deceleration..
                double decelerationTweaker = 0.3;

                //calculate the speed required to reach the target given the desired
                //deceleration
                double speed = dist / ((double)decel * decelerationTweaker);

                //make sure the velocity does not exceed the max
                speed = Math.Min(speed, _player.MaxSpeed);

                //from here proceed just like Seek except we don't need to normalize 
                //the ToTarget vector because we have already gone to the trouble
                //of calculating its length: dist. 
                Vector2D desiredVelocity = toTarget * speed / dist;

                return (desiredVelocity - _player.Velocity);
            }

            return new Vector2D(0, 0);
        }

        /// <summary>
        /// This behavior creates a force that steers the agent towards the 
        //  ball
        /// </summary>
        /// <param name="ball"></param>
        /// <returns></returns>
        protected Vector2D calculatePursuitVector(SoccerBall ball)
        {
            Vector2D toBall = ball.Position - _player.Position;

            //the lookahead time is proportional to the distance between the ball
            //and the pursuer; 
            double lookAheadTime = 0.0;

            if (Math.Abs(ball.Speed) > Geometry.MinPrecision)
            {
                lookAheadTime = toBall.Length / ball.Speed;
            }

            //calculate where the ball will be at this time in the future
            _target = ball.CalculateFuturePosition(lookAheadTime);

            //now seek to the predicted future position of the ball
            return  calculateArriveVector(_target, DecelerationState.Fast);
        }

        /// <summary>
        /// Tags any vehicles within a predefined radius
        /// </summary>
        protected void findNeighbours()
        {
            List<PlayerBase> allPlayers = AutoList<PlayerBase>.GetAllMembers();
            for (int playerIndex = 0; playerIndex < allPlayers.Count; playerIndex++)
            {
                //first clear any current tag
                allPlayers[playerIndex].SteeringBehaviors.Tagged = false;

                //work in distance squared to avoid sqrts
                Vector2D to = allPlayers[playerIndex].Position - _player.Position;

                if (to.LengthSquared < (_viewDistance * _viewDistance))
                {
                    allPlayers[playerIndex].SteeringBehaviors.Tagged = true;
                }
            }//next
        }

        /// <summary>
        /// Given an opponent and an object position this method returns a 
        ///  force that attempts to position the agent between them
        /// </summary>
        /// <param name="ball"></param>
        /// <param name="target"></param>
        /// <param name="distFromTarget"></param>
        /// <returns></returns>
        protected Vector2D calculateInterposeVector(SoccerBall ball,
                                              Vector2D target,
                                              double distFromTarget)
        {
            return calculateArriveVector(target + Vector2D.Vec2DNormalize(ball.Position - target) * distFromTarget, DecelerationState.Normal);
        }

        
        /// <summary>
        ///  This method calls each active steering behavior and acumulates their
        ///  forces until the max steering force magnitude is reached at which
        ///  time the function returns the steering force accumulated to that 
        ///  point
        /// </summary>
        /// <returns></returns>
        protected Vector2D sumForces()
        {
            Vector2D force = new Vector2D();

            //the soccer players must always tag their neighbors
            findNeighbours();

            if (Seperation)
            {
                force += calculateSeparationVector() * _separationMultiple;

                if (!accumulateForce(ref _steeringForce, force)) return _steeringForce;
            }
            
            if (Seek)
            {
                force += calculateSeekVector(_target);
                if (!accumulateForce(ref _steeringForce, force)) return _steeringForce;
            }

            if (Arrive)
            {
                force += calculateArriveVector(_target, DecelerationState.Fast);
                if (!accumulateForce(ref _steeringForce, force)) return _steeringForce;
            }

            if (Pursuit)
            {
                force += calculatePursuitVector(_ball);

                if (!accumulateForce(ref _steeringForce, force)) return _steeringForce;
            }

            if (InterposeTarget)
            {
                force += calculateInterposeVector(_ball, _target, _interposeDist);

                if (!accumulateForce(ref _steeringForce, force)) return _steeringForce;
            }

            return _steeringForce;
        }

        public SteeringBehaviors(PlayerBase player, SoccerBall ball)
        {
            _player = player;
            _ball = ball;

            _separationMultiple = ParameterManager.Instance.SeparationCoefficient;
            _viewDistance = ParameterManager.Instance.ViewDistance;
            _tagged = false;
            _interposeDist = 0.0;
            _behaviorFlags = (int)BehaviorFlags.None;

            _dribbleFeelers = new List<Vector2D>(5);

            for (int i = 0; i < _dribbleFeelers.Count; i++)
            {
                _dribbleFeelers[i] = new Vector2D();
            }
        }

        /// <summary>
        ///  Calculates the overall steering force based on the currently active
        ///  steering behaviors. 
        /// </summary>
        /// <returns></returns>
        public Vector2D CalculateSteeringForce()
        {
            //reset the force
            _steeringForce.Zero();

            //this will hold the value of each individual steering force
            _steeringForce = sumForces();

            //make sure the force doesn't exceed the vehicles maximum allowable
            _steeringForce.Truncate(_player.MaxForce);

            return _steeringForce;
        }

        /// <summary>
        /// Render force lines
        /// </summary>
        /// <param name="g"></param>
        public void RenderAids(Graphics g)
        {
            //render the steering force
            Vector2D endPoint = _player.Position + _steeringForce * 20;

            g.DrawLine(Pens.Red, new PointF((float)_player.Position.X, (float)_player.Position.Y),
                                new PointF((float)endPoint.X, (float)endPoint.Y));
        }

        /// <summary>
        /// Calculates the forward component of the steering force
        /// </summary>
        public double ForwardComponent
        {
            get { return _player.Heading.GetDotProduct(_steeringForce); }
        }

        /// <summary>
        /// Calculates the side component of the steering force
        /// </summary>
        /// <returns></returns>
        public double SideComponent
        {
            get { return _player.Side.GetDotProduct(_steeringForce) * _player.MaxTurnRate; }
        }

        public bool Seek
        {
            get { return (_behaviorFlags & (int)BehaviorFlags.Seek) != 0; }
            set 
            {
                if (value)
                {
                    _behaviorFlags |= (int)BehaviorFlags.Seek;
                }
                else
                {
                    _behaviorFlags ^= (int)BehaviorFlags.Seek;
                }
            }
        }

        public bool Arrive
        {
            get { return (_behaviorFlags & (int)BehaviorFlags.Arrive) != 0; }
            set
            {
                if (value)
                {
                    _behaviorFlags |= (int)BehaviorFlags.Arrive;
                }
                else
                {
                    _behaviorFlags ^= (int)BehaviorFlags.Arrive;
                }
            }
        }

        public bool Seperation
        {
            get { return (_behaviorFlags & (int)BehaviorFlags.Separation) != 0; }
            set
            {
                if (value)
                {
                    _behaviorFlags |= (int)BehaviorFlags.Separation;
                }
                else
                {
                    _behaviorFlags ^= (int)BehaviorFlags.Separation;
                }
            }
        }

        public bool Pursuit
        {
            get { return (_behaviorFlags & (int)BehaviorFlags.Pursuit) != 0; }
            set
            {
                if (value)
                {
                    _behaviorFlags |= (int)BehaviorFlags.Pursuit;
                }
                else
                {
                    _behaviorFlags ^= (int)BehaviorFlags.Pursuit;
                }
            }

        }

        public bool InterposeTarget
        {
            get { return (_behaviorFlags & (int)BehaviorFlags.Interpose) != 0; }
            set
            {
                if (value)
                {
                    _behaviorFlags |= (int)BehaviorFlags.Interpose;
                }
                else
                {
                    _behaviorFlags ^= (int)BehaviorFlags.Interpose;
                }
            }

        }

        /// <summary>
        /// Used by group behaviors to tag neighbours 
        /// </summary>
        public bool Tagged
        {
            get { return _tagged; }
            set { _tagged = value; }
        }

        /// <summary>
        /// The distance the player tries to interpose from the target
        /// </summary>
        public double InterposeDist
        {
            get { return _interposeDist; }
            set { _interposeDist = value; }
        }

        /// <summary>
        /// The current target (usually the ball or predicted ball position)
        /// </summary>
        public Vector2D Target
        {
            get { return _target; }
            set { _target = value; }
        }

        /// <summary>
        /// The steering force created by the combined effect of all
        /// the selected behaviors
        /// </summary>
        public Vector2D SteeringForce
        {
            get { return _steeringForce; }
        }

        public PlayerBase Player
        {
            get { return _player; }
            set { _player = value; }
        }

        public SoccerBall Ball
        {
            get { return _ball; }
            set { _ball = value; }
        }
    }
}
