//------------------------------------------------------------------------
//
//  Name:   MovingEntity.cs
//
//  Desc:   A base class defining an entity that moves. The entity has 
//          a local coordinate system and members for defining its
//          mass and velocity.
//
//  Author: Mat Buckland (fup@ai-junkie.com)
//  Ported By: Scott McCain (scott_mccain@cox.net)
//------------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;

namespace SimpleSoccer.Net
{
    public class MovingEntity : BaseGameEntity
    {
        private Vector2D _velocity;
        private Vector2D _heading;
        private Vector2D _side;
        private double _mass;
        private double _maxSpeed;
        private double _maxForce;
        private double _maxTurnRate;

        public Vector2D Velocity
        {
            set { _velocity = value; }
            get { return _velocity; }
        }

        public Vector2D Heading
        {
            set { _heading = value; }
            get { return _heading; }
        }

        public Vector2D Side
        {
            set { _side = value; }
            get { return _side; }
        }

        public double Mass
        {
            set { _mass = value; }
            get { return _mass; }
        }

        public double MaxSpeed
        {
            set { _maxSpeed = value; }
            get { return _maxSpeed; }
        }

        public double MaxForce
        {
            set { _maxForce = value; }
            get { return _maxForce; }
        }

        public double MaxTurnRate
        {
            set { _maxTurnRate = value;  }
            get { return _maxTurnRate; }
        }

        public double Speed
        {
            get { return _velocity.Length; }
        }

        public bool RotateHeadingToFacePosition(Vector2D target)
        {
            Vector2D delta = target - Position;
            Vector2D toTarget = Vector2D.Vec2DNormalize(delta);

            double dot = _heading.GetDotProduct(toTarget);

            //some compilers lose acurracy so the value is clamped to ensure it
            //remains valid for the acos
            if (dot < -1)
                dot = -1;
            else if (dot > 1)
                dot = 1;

            //first determine the angle between the heading vector and the target
            double angle = Math.Acos(dot);

            //return true if the player is facing the target
            if (angle < .00001) return true;

            //clamp the amount to turn to the max turn rate
            if (angle > _maxTurnRate) angle = _maxTurnRate;

            //The next few lines use a rotation matrix to rotate the player's heading
            //vector accordingly
            Matrix2D rotationMatrix = new Matrix2D();

            //notice how the direction of rotation has to be determined when creating
            //the rotation matrix
            rotationMatrix.Rotate(angle * _heading.Sign(toTarget));
            rotationMatrix.TransformVector2Ds(_heading);
            rotationMatrix.TransformVector2Ds(_velocity);

            //finally recreate m_vSide
            _side = _heading.Perp;

            return false;
        }

        public MovingEntity(Vector2D position, double radius, Vector2D velocity, double maxSpeed, Vector2D heading, double mass, Vector2D scale, double turnRate, double maxForce) :
            base(BaseGameEntity.GetNextId())
        {
            Position = position;
            BoundingRadius = radius;
            Velocity = velocity;
            Scale = scale;

            _heading = heading;
            _maxSpeed = maxSpeed;
            _mass = mass;
            _maxTurnRate = turnRate;
            _side = _heading.Perp;
            _maxForce = maxForce;
        }

        public override void Render(Graphics g)
        {
        }

    }
}
