//------------------------------------------------------------------------
//
//  Name:   EntityManager.cs
//
//  Desc:   Singleton class to handle the  management of Entities.          
//
//  Author: Mat Buckland (fup@ai-junkie.com)
//  Ported By: Scott McCain (scott_mccain@cox.net)
//------------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Text;

namespace SimpleSoccer.Net
{
    /// <summary>
    /// Provides a convientent way to manage game entities
    /// </summary>
    public class EntityManager
    {
        #region Singleton
        private static EntityManager _instance = null;
        public static EntityManager Instance
        {
            get
            {
                if (_instance == null)
                    _instance = new EntityManager();

                return _instance;
            }
        }
        #endregion

        #region Public Static Methods
        /// <summary>
        ///  tests to see if an entity is overlapping any of a number of entities
        ///  stored in a std container
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="entity"></param>
        /// <param name="conOb"></param>
        /// <returns></returns>
        public static bool Overlapped<T>(BaseGameEntity entity, List<T> conOb) where T : BaseGameEntity
        {
            return Overlapped(entity, conOb, 40.0);
        }

        /// <summary>
        ///  tests to see if an entity is overlapping any of a number of entities
        ///  stored in a std container
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="entity"></param>
        /// <param name="others"></param>
        /// <param name="MinDistBetweenObstacles"></param>
        /// <returns></returns>
        public static bool Overlapped<T>(BaseGameEntity entity, List<T> others, double MinDistBetweenObstacles) where T : BaseGameEntity
        {
            foreach (BaseGameEntity otherEntity in others)
            {
                if (Geometry.TwoCirclesOverlapped(entity.Position,
                                                   entity.BoundingRadius + MinDistBetweenObstacles,
                                                   otherEntity.Position,
                                                   otherEntity.BoundingRadius))
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        ///  tags any entities contained in a std container that are within the
        ///  radius of the single entity parameter
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="entity"></param>
        /// <param name="others"></param>
        /// <param name="radius"></param>
        public static void TagNeighbors<T>(BaseGameEntity entity, List<T> others, double radius) where T : BaseGameEntity
        {
            foreach (BaseGameEntity baseObject in others)
            {
                //first clear any current tag
                baseObject.BooleanTag = false;

                //work in distance squared to avoid sqrts
                Vector2D to = baseObject.Position - entity.Position;

                //the bounding radius of the other is taken into account by adding it 
                //to the range
                double range = radius + baseObject.BoundingRadius;

                //if entity within range, tag for further consideration
                if ((baseObject != entity) && (to.LengthSquared < range * range))
                {
                    baseObject.BooleanTag = true;
                }
            }
        }

        /// <summary>
        ///  Given an entity and a container of nearby
        ///  entities, this function checks to see if there is an overlap between
        ///  entities. If there is, then the entities are moved away from each
        ///  other  
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="entity"></param>
        /// <param name="others"></param>
        public static void EnforceNonPenetrationContraint<T>(BaseGameEntity entity, List<T> others) where T : BaseGameEntity
        {
            //iterate through all entities checking for any overlap of bounding
            //radii
            for (int entityIndex = 0; entityIndex < others.Count; entityIndex++)
            {
                //make sure we don't check against this entity
                if (others[entityIndex] == entity) continue;

                //calculate the distance between the positions of the entities
                Vector2D ToEntity = entity.Position - others[entityIndex].Position;

                double DistFromEachOther = ToEntity.Length;

                //if this distance is smaller than the sum of their radii then this
                //entity must be moved away in the direction parallel to the
                //ToEntity vector   
                double AmountOfOverLap = others[entityIndex].BoundingRadius + entity.BoundingRadius -
                                         DistFromEachOther;

                if (AmountOfOverLap >= 0)
                {
                    //move the entity a distance away equivalent to the amount of overlap.
                    entity.Position += ((ToEntity / DistFromEachOther) *
                                   AmountOfOverLap);
                }
            }//next entity
        }
        #endregion

        #region Private Instance Fields
        private Dictionary<int, BaseGameEntity> _entityMap = new Dictionary<int, BaseGameEntity>();
        #endregion

        #region Public Instance Methods
        public BaseGameEntity FindEntity(int id)
        {
            BaseGameEntity entity = null;
            if (_entityMap.ContainsKey(id))
            {
                entity = _entityMap[id];
            }

            return entity;
        }

        public void RegisterEntity(BaseGameEntity entity)
        {
            _entityMap.Add(entity.ObjectId, entity);
        }

        public void RemoveEntity(int id)
        {
            if (_entityMap.ContainsKey(id))
            {
                _entityMap.Remove(id);
            }
        }

        public void Reset()
        {
            _entityMap.Clear();
        }
        #endregion
    }
}
