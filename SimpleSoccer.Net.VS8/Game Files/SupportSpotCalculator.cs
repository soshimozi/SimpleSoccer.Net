//------------------------------------------------------------------------
//
//  Name:   SupportSpotCalculator.cs
//
//  Desc:   Class to determine the best spots for a suppoting soccer
//          player to move to.
//
//  Author: Mat Buckland 2003 (fup@ai-junkie.com)
//  Ported By: Scott McCain (scott_mccain@cox.net)
//------------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;

namespace SimpleSoccer.Net
{
  //a data structure to hold the values and positions of each spot
  class SupportSpot
  {
    
    public Vector2D  m_vPos;

    public double    m_dScore;

    public SupportSpot(Vector2D pos, double value)
    {
        m_vPos = pos;
        m_dScore = value;
    }
  }

    // TODO: Add Regions and Organize Code
    class SupportSpotCalculator
    {
        private SoccerTeam _team;
        private List<SupportSpot> _supportSpots = new List<SupportSpot>();
        private SupportSpot _bestSupportingSpot;
        private Regulator _regulator;

        public SupportSpotCalculator(int numX, int numY, SoccerTeam team)
        {
            Region PlayingField = team.Pitch.PlayingArea;

            //calculate the positions of each sweet spot, create them and 
            //store them in m_Spots
            double HeightOfSSRegion = PlayingField.Height * 0.8;
            double WidthOfSSRegion = PlayingField.Width * 0.9;
            double SliceX = WidthOfSSRegion / numX;
            double SliceY = HeightOfSSRegion / numY;

            double left = PlayingField.Left + (PlayingField.Width - WidthOfSSRegion) / 2.0 + SliceX / 2.0;
            double right = PlayingField.Right - (PlayingField.Width - WidthOfSSRegion) / 2.0 - SliceX / 2.0;
            double top = PlayingField.Top + (PlayingField.Height - HeightOfSSRegion) / 2.0 + SliceY / 2.0;

            _team = team;
            for (int x = 0; x < (numX / 2) - 1; ++x)
            {
                for (int y = 0; y < numY; ++y)
                {
                    if (_team.Color == SoccerTeam.SoccerTeamColor.Blue)
                    {
                        _supportSpots.Add(new SupportSpot(new Vector2D(left + x * SliceX, top + y * SliceY), 0.0));
                    }

                    else
                    {
                        _supportSpots.Add(new SupportSpot(new Vector2D(right - x * SliceX, top + y * SliceY), 0.0));
                    }
                }
            }

            //create the regulator
            _regulator = new Regulator(ParameterManager.Instance.SupportSpotUpdateFreq);

        }

        public Vector2D DetermineBestSupportingPosition()
        {
            //only update the spots every few frames                              
            if (!_regulator.IsReady /* && _bestSupportingSpot != null */)
            {
                return _bestSupportingSpot.m_vPos;
            }

            //reset the best supporting spot
            _bestSupportingSpot = null;

            double BestScoreSoFar = 0.0;


            for (int spotIndex = 0; spotIndex < _supportSpots.Count; spotIndex++)
            {
                //first remove any previous score. (the score is set to one so that
                //the viewer can see the positions of all the spots if he has the 
                //aids turned on)
                _supportSpots[spotIndex].m_dScore = 1.0;

                //Test 1. is it possible to make a safe pass from the ball's position 
                //to this position?
                if (_team.IsPassSafeFromAllOpponents(_team.ControllingPlayer.Position,
                                                       _supportSpots[spotIndex].m_vPos,
                                                       null,
                                                       ParameterManager.Instance.MaxPassingForce))
                {
                    _supportSpots[spotIndex].m_dScore += ParameterManager.Instance.SpotPassSafeScore;
                }


                Vector2D shotTarget = new Vector2D();

                //Test 2. Determine if a goal can be scored from this position.  
                if (_team.CanShoot(_supportSpots[spotIndex].m_vPos,
                                      ParameterManager.Instance.MaxShootingForce, ref shotTarget))
                {
                    _supportSpots[spotIndex].m_dScore += ParameterManager.Instance.SpotCanScoreFromPositionScore;
                }


                //Test 3. calculate how far this spot is away from the controlling
                //player. The further away, the higher the score. Any distances further
                //away than OptimalDistance pixels do not receive a score.
                if (_team.SupportingPlayer != null)
                {
                    const double OptimalDistance = 200.0;

                    double dist = Vector2D.Vec2DDistance(_team.ControllingPlayer.Position,
                                               _supportSpots[spotIndex].m_vPos);

                    double temp = Math.Abs(OptimalDistance - dist);

                    if (temp < OptimalDistance)
                    {

                        //normalize the distance and add it to the score
                        _supportSpots[spotIndex].m_dScore += ParameterManager.Instance.SpotDistFromControllingPlayerScore *
                                             (OptimalDistance - temp) / OptimalDistance;
                    }
                }

                //check to see if this spot has the highest score so far
                if (_supportSpots[spotIndex].m_dScore > BestScoreSoFar)
                {
                    BestScoreSoFar = _supportSpots[spotIndex].m_dScore;

                    _bestSupportingSpot = _supportSpots[spotIndex];
                }

            }

            return _bestSupportingSpot.m_vPos;
        }

        public Vector2D GetBestSupportingSpot()
        {
            if (_bestSupportingSpot != null)
            {
                return _bestSupportingSpot.m_vPos;
            }

            else
            {
                return DetermineBestSupportingPosition();
            }
        }

        public void Render(Graphics g)
        {

            for (int spt = 0; spt < _supportSpots.Count; ++spt)
            {
                g.DrawEllipse(Pens.Gray, new RectangleF(new Point((int)_supportSpots[spt].m_vPos.X, (int)_supportSpots[spt].m_vPos.Y), new SizeF((float)_supportSpots[spt].m_dScore, (float)_supportSpots[spt].m_dScore)));
            }

            if (_bestSupportingSpot != null)
            {
                g.DrawEllipse(Pens.Red, new RectangleF(new Point((int)_bestSupportingSpot.m_vPos.X, (int)_bestSupportingSpot.m_vPos.Y), new SizeF((float)_bestSupportingSpot.m_dScore, (float)_bestSupportingSpot.m_dScore)));
            }
        }

    }
}
