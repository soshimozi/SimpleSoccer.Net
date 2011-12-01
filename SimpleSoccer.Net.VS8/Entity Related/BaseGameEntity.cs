//------------------------------------------------------------------------
//
//  Name: BaseGameEntity.cs
//
//  Desc: Base class to define a common interface for all game
//        entities
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
    /// <summary>
    /// Provides an abstract base for all entities managed in the game
    /// </summary>
    public abstract class BaseGameEntity
    {
        private static int _nextId = 1;

        #region Private Instance Fields
        private int _objectId;
        private int _objectType = 0;
        private bool _booleanTag = false;
        private Vector2D _position = new Vector2D();
        private Vector2D _scale = new Vector2D();
        private double _boundingRadius = 0.0;
        #endregion

        #region Public Instance Properties

        public int ObjectId
        {
            get
            {
                return _objectId;
            }
            set
            {
                _objectId = value;
            }
        }

        public int ObjectType
        {
            get
            {
                return _objectType;
            }
            set
            {
                _objectType = value;
            }
        }

        public bool BooleanTag
        {
            get
            {
                return _booleanTag;
            }
            set
            {
                _booleanTag = value;
            }
        }

        public Vector2D Position
        {
            get
            {
                return _position;
            }
            set
            {
                _position = value;
            }
        }

        public Vector2D Scale
        {
            get
            {
                return _scale;
            }
            set
            {
                _scale = value;
            }
        }

        public double BoundingRadius
        {
            get
            {
                return _boundingRadius;
            }
            set
            {
                _boundingRadius = value;
            }
        }

        #endregion

        #region Construction
        public BaseGameEntity(int objectId) : base()
        {
            _objectId = objectId;

        }
        #endregion

        #region Public Abstract Instance Methods
        abstract public void Render(Graphics g);
        #endregion

        #region Overrideable Public Instance Methods
        virtual public bool HandleMessage(Telegram message)
        {
            return false;
        }
        virtual public void Update()
        { }
        #endregion

        protected static int GetNextId()
        {
            return _nextId++;
        }
    }
}
