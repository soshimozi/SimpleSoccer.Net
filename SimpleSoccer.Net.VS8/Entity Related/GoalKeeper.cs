//------------------------------------------------------------------------
//
//  Name:   GoalKeeper.cs
//
//  Desc:   class to implement a goalkeeper agent
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
    public class GoalKeeper : PlayerBase
    {
        private Brush textBrush = new SolidBrush(Color.White);
        private Pen redPen = new Pen(Color.Red, 1.0f);
        private Pen bluePen = new Pen(Color.Blue, 1.0f);

        private Vector2D _lookAt = new Vector2D();
        private StateMachine<GoalKeeper> _stateMachine;

        public GoalKeeper(SoccerTeam homeTeam,
              int homeRegionIndex,
              State<GoalKeeper> startState,
              Vector2D heading,
              Vector2D velocity,
              double mass,
              double maxForce,
              double maxSpeed,
              double maxTurnRate,
              double scale)
            :
            base(homeTeam, homeRegionIndex, heading, velocity, mass, maxForce, maxSpeed, maxTurnRate, scale, PlayerRoles.GoalKeeper)
        {
            _stateMachine = new StateMachine<GoalKeeper>(this);
            _stateMachine.CurrentState = _stateMachine.PreviousState = startState;
            _stateMachine.GlobalState = GlobalKeeperState.Instance;
            _stateMachine.CurrentState.Enter(this);
        }

        public StateMachine<GoalKeeper> StateMachine
        {
            get
            {
                return _stateMachine;
            }
        }

        public Vector2D LookAt
        {
            get { return _lookAt; }
            set { _lookAt = value; }
        }

        public override void Update()
        {
            //run the logic for the current state
            _stateMachine.Update();

            //calculate the combined force from each steering behavior 
            Vector2D SteeringForce = _steeringBehaviors.CalculateSteeringForce();

            //Acceleration = Force/Mass
            Vector2D Acceleration = SteeringForce / Mass;

            //update velocity
            Velocity += Acceleration;

            //make sure player does not exceed maximum velocity
            Velocity.Truncate(MaxSpeed);

            //update the position
            Position += Velocity;

            //enforce a non-penetration constraint if desired
            if (ParameterManager.Instance.NonPenetrationConstraint)
            {
                EntityManager.EnforceNonPenetrationContraint<PlayerBase>(this, AutoList<PlayerBase>.GetAllMembers());
            }

            //update the heading if the player has a non zero velocity
            if (!Velocity.IsZero)
            {
                Heading = Vector2D.Vec2DNormalize(Velocity);

                Side = Heading.Perp;
            }

            //look-at vector always points toward the ball
            if (!Team.Pitch.GoalKeeperHasBall)
            {
                _lookAt = Vector2D.Vec2DNormalize(Ball.Position - Position);
            }
        }

        public bool BallWithinRangeForIntercept()
        {
            return (Vector2D.Vec2DDistanceSq(Team.HomeGoal.GoalLineCenter, Ball.Position) <=
                    ParameterManager.Instance.GoalKeeperInterceptRangeSq);
        }

        public bool TooFarFromGoalMouth()
        {
            return (Vector2D.Vec2DDistanceSq(Position, GetRearInterposeTarget()) >
                    ParameterManager.Instance.GoalKeeperInterceptRangeSq);
        }

        public Vector2D GetRearInterposeTarget()
        {
            double xPosTarget = Team.HomeGoal.GoalLineCenter.X;

            double yPosTarget = Team.Pitch.PlayingArea.VectorCenter.Y -
                                  ParameterManager.Instance.GoalWidth * 0.5 + (Ball.Position.Y * ParameterManager.Instance.GoalWidth) /
                                  Team.Pitch.PlayingArea.Height;

            return new Vector2D(xPosTarget, yPosTarget);
        }

        //-------------------- HandleMessage -------------------------------------
        //
        //  routes any messages appropriately
        //------------------------------------------------------------------------
        public override bool HandleMessage(Telegram msg)
        {
            return _stateMachine.HandleMessage(msg);
        }

        //--------------------------- Render -------------------------------------
        //
        //------------------------------------------------------------------------
        public override void Render(Graphics g)
        {
            List<Vector2D> transformed = Transformations.WorldTransform(_vecPlayerVB,
                                                                    Position,
                                                                    _lookAt,
                                                                    _lookAt.Perp,
                                                                    Scale);


            GDI.CurrentPen = Team.Color == SoccerTeam.SoccerTeamColor.Blue ? bluePen : redPen;
            GDI.DrawPolygon(g, transformed);

            //draw the head
            GDI.CurrentBrush = new SolidBrush(Color.FromArgb(133, 90, 0));
            GDI.DrawCircle(g, Position, 6.0f);

            //draw the ID
            if (ParameterManager.Instance.ShowIDs)
            {
                g.DrawString(ObjectId.ToString(), GDI.TextFont, textBrush, new PointF((float)Position.X - 20.0f, (float)Position.Y - GDI.TextFont.Height));
            }

            //draw the state
            if (ParameterManager.Instance.ShowStates)
            {
                Brush textBrush = new SolidBrush(Color.FromArgb(0, 170, 0));
                g.DrawString(_stateMachine.CurrentState.ToString(), GDI.TextFont, textBrush, new PointF((float)Position.X, (float)Position.Y - GDI.TextFont.Height));
            }
        }
    }
}
