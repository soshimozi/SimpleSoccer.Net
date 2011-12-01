//------------------------------------------------------------------------
//
//  Name:   FieldPlayer.cs
//
//  Desc:   Derived from a PlayerBase, this class encapsulates a player
//          capable of moving around a soccer pitch, kicking, dribbling,
//          shooting etc
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
    public class FieldPlayer : PlayerBase
    {
        private Brush textBrush = new SolidBrush(Color.White);
        private StateMachine<FieldPlayer> _stateMachine;
        private Regulator _kickRegulator;

        #region Public Instance Properties
        public StateMachine<FieldPlayer> StateMachine
        {
            get { return _stateMachine; }
            set { _stateMachine = value; }
        }
        #endregion

        public FieldPlayer(SoccerTeam homeTeam,
                      int homeRegionIndex,
                      State<FieldPlayer> startState,
                      Vector2D heading,
                      Vector2D velocity,
                      double mass,
                      double maxForce,
                      double maxSpeed,
                      double maxTurnRate,
                      double scale,
                      PlayerBase.PlayerRoles role)
            : base(homeTeam, homeRegionIndex, heading, velocity, mass, maxForce, maxSpeed, maxTurnRate, scale, role)
        {
            _stateMachine = new StateMachine<FieldPlayer>(this);

            if (startState != null)
            {
                _stateMachine.CurrentState = _stateMachine.PreviousState = startState;
                _stateMachine.GlobalState = GlobalPlayerState.Instance;
                _stateMachine.CurrentState.Enter(this);
            }

            _steeringBehaviors.Seperation = true;

            //set up the kick regulator
            _kickRegulator = new Regulator(ParameterManager.Instance.PlayerKickFrequency);
        }

        public override void Update()
        {
            //run the logic for the current state
            _stateMachine.Update();

            //calculate the combined steering force
            _steeringBehaviors.CalculateSteeringForce();

            //if no steering force is produced decelerate the player by applying a
            //braking force
            if (_steeringBehaviors.SteeringForce.IsZero)
            {
                double brakingRate = 0.8;

                Velocity = Velocity * brakingRate;
            }

            //the steering force's side component is a force that rotates the 
            //player about its axis. We must limit the rotation so that a player
            //can only turn by PlayerMaxTurnRate rads per update.
            double TurningForce = _steeringBehaviors.SideComponent;

            if (TurningForce < -ParameterManager.Instance.PlayerMaxTurnRate)
            {
                TurningForce = -ParameterManager.Instance.PlayerMaxTurnRate;
            }

            if (TurningForce > ParameterManager.Instance.PlayerMaxTurnRate)
            {
                TurningForce = ParameterManager.Instance.PlayerMaxTurnRate;
            }

            //rotate the heading vector
            Transformations.Vec2DRotateAroundOrigin(Heading, TurningForce);

            //make sure the velocity vector points in the same direction as
            //the heading vector
            Velocity = Heading * Velocity.Length;

            //and recreate m_vSide
            Side = Heading.Perp;


            //now to calculate the acceleration due to the force exerted by
            //the forward component of the steering force in the direction
            //of the player's heading
            Vector2D accel = Heading * _steeringBehaviors.ForwardComponent / Mass;

            Velocity += accel;

            //make sure player does not exceed maximum velocity
            Velocity.Truncate(MaxSpeed);

            //update the position
            Position += Velocity;


            //enforce a non-penetration constraint if desired
            if (ParameterManager.Instance.NonPenetrationConstraint)
            {
                EntityManager.EnforceNonPenetrationContraint(this, AutoList<PlayerBase>.GetAllMembers());
            }
        }

        //-------------------- HandleMessage -------------------------------------
        //
        //  routes any messages appropriately
        //------------------------------------------------------------------------
        public override bool HandleMessage(Telegram message)
        {
            return _stateMachine.HandleMessage(message);
        }

        //--------------------------- Render -------------------------------------
        //
        //------------------------------------------------------------------------
        public override void Render(Graphics g)
        {

            //set appropriate team color
            GDI.CurrentPen = (Team.Color == SoccerTeam.SoccerTeamColor.Blue) ? Pens.Blue : Pens.Red;

            // draw the body, translated to it's local coordinate space
            List<Vector2D> vectors = Transformations.WorldTransform(_vecPlayerVB, Position, Heading, Side, Scale);
            GDI.DrawPolygon(g, vectors);

            // draw his head
            GDI.CurrentBrush = new SolidBrush(Color.FromArgb(133, 90, 0));
            if (ParameterManager.Instance.ShowHighlightIfThreatened && (Team.ControllingPlayer == this) && IsThreatened()) GDI.CurrentBrush = Brushes.Yellow;
            GDI.DrawCircle(g, Position, 6.0f);

            //render the state
            if (ParameterManager.Instance.ShowStates)
            {
                Brush stateBrush = new SolidBrush( Color.FromArgb(0, 170, 0) );
                g.DrawString(_stateMachine.CurrentState.ToString(), GDI.TextFont, stateBrush, new PointF((float)Position.X, (float)Position.Y - GDI.TextFont.Height));
            }

            //show IDs
            if (ParameterManager.Instance.ShowIDs)
            {
                g.DrawString(ObjectId.ToString(), GDI.TextFont, textBrush, new PointF((float)Position.X - 20.0f, (float)Position.Y - GDI.TextFont.Height));
            }

            if (ParameterManager.Instance.ShowViewTargets)
            {
                g.FillEllipse(textBrush, new RectangleF((float)SteeringBehaviors.Target.X, (float)SteeringBehaviors.Target.Y, 3.0f, 3.0f));
                g.DrawString(ObjectId.ToString(), GDI.TextFont, Brushes.Red, new PointF((float)SteeringBehaviors.Target.X, (float)SteeringBehaviors.Target.Y));
            }
        }

        public bool IsReadyForNextKick
        {
            get
            {
                return _kickRegulator.IsReady;
            }
        }

    }
}
