//------------------------------------------------------------------------
//
//  Name:   SoccerPitch.cs
//
//  Desc:   A SoccerPitch is the main game object. It owns instances of
//          two soccer teams, two goals, the playing area, the ball
//          etc. This is the root class for all the game updates and
//          renders etc
//
//  Author: Mat Buckland 2003 (fup@ai-junkie.com)
//  Ported By: Scott McCain 2008 (scott_mccain@cox.net)
//
//------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;

namespace SimpleSoccer.Net
{
    /// <summary>
    /// A SoccerPitch is the main game object. It owns instances of
    ///  two soccer teams, two goals, the playing area, the ball
    ///  etc. This is the root class for all the game updates and
    ///  renders etc
    /// </summary>
    public class SoccerPitch
    {
        #region Private Constant Values
        const int NumRegionsHorizontal = 6;
        const int NumRegionsVertical = 3;
        #endregion

        #region Private Instance Fields
        private SoccerBall _ball;
        private SoccerTeam _redTeam;
        private SoccerTeam _blueTeam;
        private SoccerGoal _redGoal;
        private SoccerGoal _blueGoal;

        //container for the boundary walls
        private List<Wall2D> _walls;

        //defines the dimensions of the playing area
        private Region _playingArea;

        //the playing field is broken up into regions that the team
        //can make use of to implement strategies.
        private List<Region> _regions = new List<Region>();

        //true if a goal keeper has possession
        private bool _goalKeeperHasBall = false;

        //true if the game is in play. Set to false whenever the players
        //are getting ready for kickoff
        private bool _gameInPlay;

        //set true to pause the motion
        private bool _paused;

        //local copy of client window dimensions
        private int _clientWidth, _clientHeight;
        #endregion

        #region Construction
        public SoccerPitch(double width, double height)
        {
            //int GOAL_WIDTH = 2;
            //double ballSize = .5;
            //double ballMass = 1.0;
            _walls = new List<Wall2D>();

            _clientWidth = (int)width;
            _clientHeight = (int)height;

            _paused = false;
            _goalKeeperHasBall = false;
            _gameInPlay = true;

            //define the playing area
            _playingArea = new Region(20, _clientHeight - 20, 20, _clientWidth - 20);

            //create the regions  
            createRegions(_playingArea.Width / (double)NumRegionsHorizontal,
                          _playingArea.Height / (double)NumRegionsVertical);

            //create the goals
            _redGoal = new SoccerGoal(new Vector2D(_playingArea.Left, (_clientHeight - ParameterManager.Instance.GoalWidth) / 2),
                                   new Vector2D(_playingArea.Left, _clientHeight - (_clientHeight - ParameterManager.Instance.GoalWidth) / 2),
                                   new Vector2D(1.0, 0.0));



            _blueGoal = new SoccerGoal(new Vector2D(_playingArea.Right, (_clientHeight - ParameterManager.Instance.GoalWidth) / 2),
                                    new Vector2D(_playingArea.Right, _clientHeight - (_clientHeight - ParameterManager.Instance.GoalWidth) / 2),
                                    new Vector2D(-1, 0));


            //create the soccer ball
            _ball = new SoccerBall(new Vector2D(_clientWidth / 2.0, _clientHeight / 2.0),
                                     ParameterManager.Instance.BallSize,
                                     ParameterManager.Instance.BallMass,
                                     _walls);


            //create the teams 
            _redTeam = new SoccerTeam(_redGoal, _blueGoal, this, SoccerTeam.SoccerTeamColor.Red);
            _blueTeam = new SoccerTeam(_blueGoal, _redGoal, this, SoccerTeam.SoccerTeamColor.Blue);

            //make sure each team knows who their opponents are
            _redTeam.OpposingTeam = _blueTeam;
            _blueTeam.OpposingTeam = _redTeam;

            //create the walls
            Vector2D TopLeft = new Vector2D(_playingArea.Left, _playingArea.Top);
            Vector2D TopRight = new Vector2D(_playingArea.Right, _playingArea.Top);
            Vector2D BottomRight = new Vector2D(_playingArea.Right, _playingArea.Bottom);
            Vector2D BottomLeft = new Vector2D(_playingArea.Left, _playingArea.Bottom);

            _walls.Add(new Wall2D(BottomLeft, _redGoal.RightPost));
            _walls.Add(new Wall2D(_redGoal.LeftPost, TopLeft));
            _walls.Add(new Wall2D(TopLeft, TopRight));
            _walls.Add(new Wall2D(TopRight, _blueGoal.LeftPost));
            _walls.Add(new Wall2D(_blueGoal.RightPost, BottomRight));
            _walls.Add(new Wall2D(BottomRight, BottomLeft));

        }
        #endregion

        #region Public Instance Properties
        public int ClientWidth
        {
            get
            {
                return _clientWidth;
            }
        }

        public int ClientHeight
        {
            get
            {
                return _clientHeight;
            }
        }

        public bool GoalKeeperHasBall
        {
            get { return _goalKeeperHasBall; }
            set { _goalKeeperHasBall = value; }
        }

        public SoccerBall Ball
        {
            get { return _ball; }
            set { _ball = value; }
        }

        public SoccerTeam RedTeam
        {
            get { return _redTeam; }
            set { _redTeam = value; }
        }

        public SoccerTeam BlueTeam
        {
            get { return _blueTeam; }
            set { _blueTeam = value; }
        }

        public SoccerGoal RedGoal
        {
            get { return _redGoal; }
            set { _redGoal = value; }
        }

        public SoccerGoal BlueGoal
        {
            get { return _blueGoal; }
            set { _blueGoal = value; }
        }
        public  List<Wall2D> Walls
        {
            get { return _walls; }
            set { _walls = value; }
        }

        public Region PlayingArea
        {
            get { return _playingArea; }
            set { _playingArea = value; }
        }

        public bool GameInPlay
        {
            get { return _gameInPlay; }
            set { _gameInPlay = value; }
        }
        #endregion

        #region Public Instance Methods
        public void Update()
        {
            if (_paused) return;

            //update the balls
            _ball.Update();

            //update the teams
            _redTeam.Update();
            _blueTeam.Update();

            //if a goal has been detected reset the pitch ready for kickoff
            if (_blueGoal.CheckIfGoalScored(_ball) || _redGoal.CheckIfGoalScored(_ball))
            {
                _gameInPlay = false;

                //reset the ball                                                      
                _ball.PlaceAtPosition(new Vector2D((double)_clientWidth / 2.0, (double)_clientHeight / 2.0));

                //get the teams ready for kickoff
                _redTeam.FSM.ChangeState(PrepareForKickoffState.Instance);
                _blueTeam.FSM.ChangeState(PrepareForKickoffState.Instance);
            }
        }


        public void Render(Graphics g)
        {
            //draw the grass
            g.DrawRectangle(Pens.DarkGreen, new Rectangle(0, 0, _clientWidth, _clientHeight));
            g.FillRectangle(Brushes.DarkGreen, new Rectangle(0, 0, _clientWidth, _clientHeight));

            // render regions
            if (ParameterManager.Instance.ShowRegions)
            {
                for (int regionIndex = 0; regionIndex < _regions.Count; regionIndex++)
                {
                    _regions[regionIndex].Render(true, g);
                }
            }

            //render the goals
            g.DrawRectangle(Pens.Red, (float)_playingArea.Left, (float)(_clientHeight - ParameterManager.Instance.GoalWidth) / 2.0f, 40.0f, (float)ParameterManager.Instance.GoalWidth);
            g.DrawRectangle(Pens.Blue, (float)_playingArea.Right - 40.0f, (float)(_clientHeight - ParameterManager.Instance.GoalWidth) / 2.0f, 40.0f, (float)ParameterManager.Instance.GoalWidth);

            //render the pitch markings
            GDI.CurrentBrush = Brushes.Transparent;
            GDI.CurrentPen = Pens.White;
            GDI.DrawCircle(g, _playingArea.VectorCenter, _playingArea.Width * 0.125f);

            g.DrawLine(Pens.White, (float)_playingArea.VectorCenter.X, (float)_playingArea.Top, (float)_playingArea.VectorCenter.X, (float)_playingArea.Bottom);

            GDI.CurrentBrush = Brushes.White;
            GDI.DrawCircle(g, _playingArea.VectorCenter, 2.0f);

            _ball.Render(g);

            //Render the teams
            _redTeam.Render(g);
            _blueTeam.Render(g);

            //render the walls
            for (int wallIndex = 0; wallIndex < _walls.Count; ++wallIndex)
            {
                _walls[wallIndex].Render(false, g);
            }

            //show the score
            string redGoals = string.Format("Red: {0}", _blueGoal.GoalsScored);
            g.DrawString(redGoals, GDI.TextFont, Brushes.Red, new PointF(_clientWidth /2 + 10, _clientHeight - GDI.TextFont.Height));

            string blueGoals = string.Format("Blue: {0}", _redGoal.GoalsScored);
            g.DrawString(blueGoals, GDI.TextFont, Brushes.Blue, new PointF(_clientWidth / 2 - g.MeasureString(blueGoals, GDI.TextFont).Width - 10, _clientHeight - GDI.TextFont.Height));
        }

        public Region GetRegion(int index)
        {
            return _regions[index];
        }
        #endregion

        #region Private Instance Methods
        /// <summary>
        ///this instantiates the regions the players utilize to  position
        ///themselves
        /// </summary>
        /// <param name="width"></param>
        /// <param name="height"></param>
        private void createRegions(double width, double height)
        {
            for (int x = 0; x < NumRegionsHorizontal * NumRegionsVertical; x++)
            {
                _regions.Add(new Region());
            }

            int index = _regions.Count - 1;

            for (int col = 0; col < NumRegionsHorizontal; ++col)
            {
                for (int row = 0; row < NumRegionsVertical; ++row)
                {
                    Region region = new Region(_playingArea.Top + row * height,
                                                 _playingArea.Top + (row + 1) * height,
                                                 _playingArea.Left + col * width,
                                                 _playingArea.Left + (col + 1) * width);

                    region.RegionId = index;
                    _regions[index--] = region;
                }
            }
        }
        #endregion
    }
}
