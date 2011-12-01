//------------------------------------------------------------------------
//
//Name:   Goal.cs
//
//Desc:   class to define a goal for a soccer pitch. The goal is defined
//        by two 2D vectors representing the left and right posts.
//
//        Each time-step the method Scored should be called to determine
//        if a goal has been scored.
//
//Author: Mat Buckland 2003 (fup@ai-junkie.com)
//Ported By: Scott McCain (scott_mccain@cox.net)
//------------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Text;

namespace SimpleSoccer.Net
{
    /// <summary>
    /// Defines a goal in the soccer game simulation
    /// </summary>
    public class SoccerGoal
    {
        #region Private Instance Fields
        private Vector2D _leftPost;
        private Vector2D _rightPost;
        private Vector2D _facingDirection;
        private Vector2D _goalLineCenter;
        private int _goalsScored;
        #endregion

        #region Construction
        public SoccerGoal(Vector2D leftPost, Vector2D rightPost, Vector2D facing)
        {
            _leftPost = leftPost;
            _rightPost = rightPost;
            _facingDirection = facing;

            _goalsScored = 0;
            _goalLineCenter = (_leftPost + _rightPost) / 2.0;
        }
        #endregion

        #region Public Instance Properties
        public Vector2D LeftPost
        {
            get { return _leftPost; }
            set { _leftPost = value; }
        }

        public Vector2D RightPost
        {
            get { return _rightPost; }
            set { _rightPost = value; }
        }

        public Vector2D FacingDirection
        {
            get { return _facingDirection; }
            set { _facingDirection = value; }
        }

        public Vector2D GoalLineCenter
        {
            get { return _goalLineCenter; }
            set { _goalLineCenter = value; }
        }

        public int GoalsScored
        {
            get { return _goalsScored; }
        }
        #endregion

        #region Public Instance Methods
        public bool CheckIfGoalScored(SoccerBall ball)
        {
            bool scored = Geometry.LineIntersection2D(ball.Position, ball.OldPosition, _leftPost, _rightPost);
            if( scored )
            {
                _goalsScored++;
            }

            return scored;
        }

        public void ResetGoals()
        {
            _goalsScored = 0;
        }
        #endregion
    }
}
