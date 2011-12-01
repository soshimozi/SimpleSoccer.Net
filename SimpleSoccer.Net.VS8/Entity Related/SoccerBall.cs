//------------------------------------------------------------------------
//
//  Name: SoccerBall.cs
//
//  Desc: Class to implement a soccer ball. This class inherits from
//        MovingEntity and provides further functionality for collision
//        testing and position prediction.
//
//  Author: Mat Buckland 2003 (fup@ai-junkie.com)
//  Ported By: Scott McCain (scott_mccain@cox.net)
//------------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;

namespace SimpleSoccer.Net
{
    // TODO: Add Regions and Organize Code
    public class SoccerBall : MovingEntity
    {
        private const double   Pi        = 3.14159;
        
        private List<Wall2D> _pitchBoundary;
        private Vector2D _oldPosition;

        private Random rng = new Random();

        private static Image soccerBallImage = null;
        public static Image BallImage
        {
            get
            {
                if (soccerBallImage == null) soccerBallImage = Properties.Resources.ball;
                return soccerBallImage;
            }
        }


        /// <summary>
        ///returns a random double in the range -1 < n < 1
        /// </summary>
        /// <returns></returns>
        private double RandomClamp()
        {
            return rng.NextDouble() - rng.NextDouble();
        }

        public Vector2D OldPosition
        {
            get { return _oldPosition; }
            set { _oldPosition = value; }
        }

        public List<Wall2D> PitchBoundary
        {
            get { return _pitchBoundary; }
        }

        public SoccerBall(Vector2D position, double ballSize, double mass, List<Wall2D> pitchBoundary)
            : base(position, ballSize, new Vector2D(0, 0), -1.0, new Vector2D(0, 1), mass, new Vector2D(1.0, 1.0), 0.0, 0.0)
        {
            _pitchBoundary = pitchBoundary;
        }

        public override void Render(Graphics g)
        {
            int imageX = (int)Position.X - SoccerBall.BallImage.Width / 2;
            int imageY = (int)Position.Y - SoccerBall.BallImage.Height / 2;

            g.DrawImage(SoccerBall.BallImage, imageX, imageY);

            //GDI.CurrentPen = Pens.White;
            //GDI.CurrentBrush = Brushes.Black;
            //GDI.DrawCircle(g, Position, BoundingRadius);
        }

        /// <summary>
        ///  this can be used to vary the accuracy of a player's kick. Just call it 
        ///  prior to kicking the ball using the ball's position and the ball target as
        ///  parameters.
        /// 
        /// </summary>
        /// <param name="BallPos"></param>
        /// <param name="BallTarget"></param>
        /// <returns></returns>
        public static Vector2D AddNoiseToKick(Vector2D BallPos, Vector2D BallTarget)
        {
            Random random = new Random();

            double displacement = (Pi - Pi * ParameterManager.Instance.PlayerKickingAccuracy) * Utils.Math.RandomClamped(random);

            Vector2D toTarget = BallTarget - BallPos;

            Transformations.Vec2DRotateAroundOrigin(toTarget, displacement);

            return toTarget + BallPos;
        }

        /// <summary>
        ///  applys a force to the ball in the direction of heading. Truncates
        ///  the new velocity to make sure it doesn't exceed the max allowable.
        /// 
        /// </summary>
        /// <param name="direction"></param>
        /// <param name="force"></param>
        public void Kick(Vector2D direction, double force)
        {
            //ensure direction is normalized
            direction.Normalize();

            //calculate the acceleration
            Vector2D acceleration = (direction * force) / Mass;

            //update the velocity
            Velocity = acceleration;
        }
        /// <summary>
        ///  updates the ball physics, tests for any collisions and adjusts
        ///  the ball's velocity accordingly
        /// 
        /// </summary>
        public override void Update()
        {
            //keep a record of the old position so the goal::scored method
            //can utilize it for goal testing
            _oldPosition = Position;

            //Test for collisions
            TestCollisionWithWalls(_pitchBoundary);

            //Simulate Prm.Friction. Make sure the speed is positive 
            //first though
            if (Velocity.LengthSquared > ParameterManager.Instance.Friction * ParameterManager.Instance.Friction)
            {
                Velocity += Vector2D.Vec2DNormalize(Velocity) * ParameterManager.Instance.Friction;

                Position += Velocity;



                //update heading
                Heading = Vector2D.Vec2DNormalize(Velocity);
            }
        }
        /// <summary>
        ///  Given a force and a distance to cover given by two vectors, this
        ///  method calculates how long it will take the ball to travel between
        ///  the two points
        /// 
        /// </summary>
        /// <param name="A"></param>
        /// <param name="B"></param>
        /// <param name="force"></param>
        /// <returns></returns>
        public double CalucateTimeToCoverDistance(Vector2D A,
                                              Vector2D B,
                                              double force)
        {
            //this will be the velocity of the ball in the next time step *if*
            //the player was to make the pass. 
            double speed = force / Mass;

            //calculate the velocity at B using the equation
            //
            //  v^2 = u^2 + 2as
            //

            //first calculate s (the distance between the two positions)
            double DistanceToCover = Vector2D.Vec2DDistance(A, B);

            double term = speed * speed + 2.0 * DistanceToCover * ParameterManager.Instance.Friction;

            //if  (u^2 + 2as) is negative it means the ball cannot reach point B.
            if (term <= 0.0) return -1.0;

            double v = Math.Sqrt(term);

            //it IS possible for the ball to reach B and we know its speed when it
            //gets there, so now it's easy to calculate the time using the equation
            //
            //    t = v-u
            //        ---
            //         a
            //
            return (v - speed) / ParameterManager.Instance.Friction;
        }

        /// <summary>
        ///  Given a time this method returns the ball position at that time in the
        ///  future
        /// </summary>
        /// <param name="time"></param>
        /// <returns></returns>
        public Vector2D CalculateFuturePosition(double time)
        {
            //using the equation s = ut + 1/2at^2, where s = distance, a = friction
            //u=start velocity

            //calculate the ut term, which is a vector
            Vector2D ut = Velocity * time;

            //calculate the 1/2at^2 term, which is scalar
            double half_a_t_squared = 0.5 * ParameterManager.Instance.Friction * time * time;

            //turn the scalar quantity into a vector by multiplying the value with
            //the normalized velocity vector (because that gives the direction)
            Vector2D scalarToVector = half_a_t_squared * Vector2D.Vec2DNormalize(Velocity);

            //the predicted position is the balls position plus these two terms
            return Position + ut + scalarToVector;
        }

        /// <summary>
        /// Tests to see if the ball has collided with a ball and reflects 
        /// the ball's velocity accordingly
        /// </summary>
        /// <param name="walls"></param>
        public void TestCollisionWithWalls(List<Wall2D> walls)
        {
            //test ball against each wall, find out which is closest
            int closestIndex = -1;

            Vector2D normalVector = Vector2D.Vec2DNormalize(Velocity);

            Vector2D intersectionPoint = new Vector2D(), collisionPoint = new Vector2D();

            double distToIntersection = double.MaxValue;

            ////iterate through each wall and calculate if the ball intersects.
            ////If it does then store the index into the closest intersecting wall
            for (int wallIndex = 0; wallIndex < walls.Count; wallIndex++)
            {
                //assuming a collision if the ball continued on its current heading 
                //calculate the point on the ball that would hit the wall. This is 
                //simply the wall's normal(inversed) multiplied by the ball's radius
                //and added to the balls center (its position)
                Vector2D thisCollisionPoint = Position - (walls[wallIndex].VectorNormal * BoundingRadius);

                //  //calculate exactly where the collision point will hit the plane    
                if (Geometry.WhereIsPoint(thisCollisionPoint,
                    walls[wallIndex].VectorFrom,
                    walls[wallIndex].VectorNormal) == Geometry.PlaneLocation.Behind)
                {
                    double distToWall = Geometry.DistanceToRayPlaneIntersection(thisCollisionPoint, walls[wallIndex].VectorNormal, walls[wallIndex].VectorFrom, walls[wallIndex].VectorNormal);
                    intersectionPoint = thisCollisionPoint + (distToWall * walls[wallIndex].VectorNormal);
                }
                else
                {
                    double distToWall = Geometry.DistanceToRayPlaneIntersection(thisCollisionPoint,
                        normalVector,
                        walls[wallIndex].VectorFrom,
                        walls[wallIndex].VectorNormal);

                    intersectionPoint = thisCollisionPoint + (distToWall * normalVector);

                }

                //check to make sure the intersection point is actually on the line
                //segment
                bool onLineSegment = false;

                if (Geometry.LineIntersection2D(walls[wallIndex].VectorFrom,
                                       walls[wallIndex].VectorTo,
                                       thisCollisionPoint - walls[wallIndex].VectorNormal * 20.0,
                                       thisCollisionPoint + walls[wallIndex].VectorNormal * 20.0))
                {

                    onLineSegment = true;
                }

                //Note, there is no test for collision with the end of a line segment
                //now check to see if the collision point is within range of the
                //velocity vector. [work in distance squared to avoid sqrt] and if it
                //is the closest hit found so far. 
                //If it is that means the ball will collide with the wall sometime
                //between this time step and the next one.
                double distSq = Vector2D.Vec2DDistanceSq(thisCollisionPoint, intersectionPoint);

                if ((distSq <= Velocity.LengthSquared) && (distSq < distToIntersection) && onLineSegment)
                {
                    distToIntersection = distSq;
                    closestIndex = wallIndex;
                    collisionPoint = intersectionPoint;
                }
            } // next wall


            //to prevent having to calculate the exact time of collision we
            //can just check if the velocity is opposite to the wall normal
            //before reflecting it. This prevents the case where there is overshoot
            //and the ball gets reflected back over the line before it has completely
            //reentered the playing area.
            if ((closestIndex >= 0) && normalVector.GetDotProduct(walls[closestIndex].VectorNormal) < 0)
            {
                Velocity.Reflect(walls[closestIndex].VectorNormal);
            }

        }

        //this is used by players and goalkeepers to 'trap' a ball -- to stop
        //it dead. That player is then assumed to be in possession of the ball
        //and m_pOwner is adjusted accordingly
        public void Trap() { Velocity.Zero(); }  

        /// <summary>
        ///  positions the ball at the desired location and sets the ball's velocity to
        ///  zero
        /// 
        /// </summary>
        /// <param name="NewPos"></param>
        public void PlaceAtPosition(Vector2D NewPos)
        {
            Position = NewPos;

            OldPosition = Position;

            Velocity.Zero();
        }
    }

}
