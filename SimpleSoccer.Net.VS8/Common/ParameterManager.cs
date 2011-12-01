//------------------------------------------------------------------------
//
//Name:  ParameterManager.cs
//
//Desc:  singleton class to handle the loading of default parameter
//       values from an initialization file: 'params.ini'
//
//Author: Mat Buckland 2003 (fup@ai-junkie.com)
//Ported By: Scott McCain (scott_mccain@cox.net)
//------------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Text;

namespace SimpleSoccer.Net
{
    class ParameterManager : ParameterFileBase
    {
        private static ParameterManager _instance = null;
        public static ParameterManager Instance
        {
            get { return (_instance == null ? _instance = new ParameterManager() : _instance); }
        }

        public readonly double GoalWidth = 0.0;
        public readonly int NumSupportSpotsX = 0;
        public readonly int NumSupportSpotsY = 0;

        public readonly double SpotPassSafeScore = 0.0;
        public readonly double SpotCanScoreFromPositionScore = 0.0;
        public readonly double SpotDistFromControllingPlayerScore = 0.0;
        public readonly double SpotClosenessToSupportingPlayerScore = 0.0;
        public readonly double SpotAheadOfAttackerScore = 0.0;
        public readonly double SupportSpotUpdateFreq = 0.0;

        public readonly double ChancePlayerAttemptsPotShot;
        public readonly double ChanceOfUsingArriveTypeReceiveBehavior;

        public readonly double BallSize;
        public readonly double BallMass;
        public readonly double Friction;

        public readonly double KeeperInBallRange;
        public readonly double PlayerInTargetRange;
        public readonly double PlayerKickingDistance;
        public readonly double PlayerKickFrequency;


        public readonly double PlayerMass;
        public readonly double PlayerMaxForce;
        public readonly double PlayerMaxSpeedWithBall;
        public readonly double PlayerMaxSpeedWithoutBall;
        public readonly double PlayerMaxTurnRate;
        public readonly double PlayerScale;
        public readonly double PlayerComfortZone;
        public readonly double PlayerKickingAccuracy;

        public readonly int NumAttemptsToFindValidStrike;

        public readonly double MaxDribbleForce;
        public readonly double MaxShootingForce;
        public readonly double MaxPassingForce;

        public readonly double WithinRangeOfHome;
        public readonly double WithinRangeOfSupportSpot;

        public readonly double MinPassDist;
        public readonly double GoalkeeperMinPassDist;

        public readonly double GoalKeeperTendingDistance;
        public readonly double GoalKeeperInterceptRange;
        public readonly double BallWithinReceivingRange;

        public bool ShowStates;
        public bool ShowIDs;
        public bool ShowSupportSpots;
        public bool ShowRegions;
        public bool ShowControllingTeam;
        public bool ShowViewTargets;
        public bool ShowHighlightIfThreatened;

        public readonly int FrameRate;

        public readonly double SeparationCoefficient;
        public readonly double ViewDistance;
        public readonly bool NonPenetrationConstraint;
        public readonly double DefenseInterceptRange;
        public readonly double FieldOfView;
        public readonly double PassThreatRadius;


        public readonly double DefenseInterceptRangeSq;
        public readonly double BallWithinReceivingRangeSq;
        public readonly double KeeperInBallRangeSq;
        public readonly double PlayerInTargetRangeSq;
        public readonly double PlayerKickingDistanceSq;
        public readonly double PlayerComfortZoneSq;
        public readonly double GoalKeeperInterceptRangeSq;
        public readonly double WithinRangeOfSupportSpotSq;

        public ParameterManager()
            : base("Params.config")
        {
            GoalWidth = GetNextParameter<double>();

            NumSupportSpotsX = GetNextParameter<int>();
            NumSupportSpotsY = GetNextParameter<int>();

            SpotPassSafeScore = GetNextParameter<double>();
            SpotCanScoreFromPositionScore = GetNextParameter<double>();
            SpotDistFromControllingPlayerScore = GetNextParameter<double>();
            SpotClosenessToSupportingPlayerScore = GetNextParameter<double>();
            SpotAheadOfAttackerScore = GetNextParameter<double>();

            SupportSpotUpdateFreq = GetNextParameter<double>();

            ChancePlayerAttemptsPotShot = GetNextParameter<double>();
            ChanceOfUsingArriveTypeReceiveBehavior = GetNextParameter<double>();

            BallSize = GetNextParameter<double>();
            BallMass = GetNextParameter<double>();
            Friction = GetNextParameter<double>();

            KeeperInBallRange = GetNextParameter<double>();
            PlayerInTargetRange = GetNextParameter<double>();
            PlayerKickingDistance = GetNextParameter<double>();
            PlayerKickFrequency = GetNextParameter<double>();


            PlayerMass = GetNextParameter<double>();
            PlayerMaxForce = GetNextParameter<double>();
            PlayerMaxSpeedWithBall = GetNextParameter<double>();
            PlayerMaxSpeedWithoutBall = GetNextParameter<double>();
            PlayerMaxTurnRate = GetNextParameter<double>();
            PlayerScale = GetNextParameter<double>();
            PlayerComfortZone = GetNextParameter<double>();
            PlayerKickingAccuracy = GetNextParameter<double>();

            NumAttemptsToFindValidStrike = GetNextParameter<int>();



            MaxDribbleForce = GetNextParameter<double>();
            MaxShootingForce = GetNextParameter<double>();
            MaxPassingForce = GetNextParameter<double>();

            WithinRangeOfHome = GetNextParameter<double>();
            WithinRangeOfSupportSpot = GetNextParameter<double>();

            MinPassDist = GetNextParameter<double>();
            GoalkeeperMinPassDist = GetNextParameter<double>();

            GoalKeeperTendingDistance = GetNextParameter<double>();
            GoalKeeperInterceptRange = GetNextParameter<double>();
            BallWithinReceivingRange = GetNextParameter<double>();

            ShowStates = GetNextParameter<bool>();
            ShowIDs = GetNextParameter<bool>();
            ShowSupportSpots = GetNextParameter<bool>();
            ShowRegions = GetNextParameter<bool>();
            ShowControllingTeam = GetNextParameter<bool>();
            ShowViewTargets = GetNextParameter<bool>();
            ShowHighlightIfThreatened = GetNextParameter<bool>();

            FrameRate = GetNextParameter<int>();

            SeparationCoefficient = GetNextParameter<double>();
            ViewDistance = GetNextParameter<double>();
            NonPenetrationConstraint = GetNextParameter<bool>();
            DefenseInterceptRange = GetNextParameter<double>();
            FieldOfView = GetNextParameter<double>();
            PassThreatRadius = GetNextParameter<double>();


            DefenseInterceptRangeSq = DefenseInterceptRange * DefenseInterceptRange;
            BallWithinReceivingRangeSq = BallWithinReceivingRange * BallWithinReceivingRange;
            KeeperInBallRangeSq = KeeperInBallRange * KeeperInBallRange;
            PlayerInTargetRangeSq = PlayerInTargetRange * PlayerInTargetRange;
            PlayerKickingDistance += BallSize;
            PlayerKickingDistanceSq = PlayerKickingDistance * PlayerKickingDistance;
            PlayerComfortZoneSq = PlayerComfortZone * PlayerComfortZone;
            GoalKeeperInterceptRangeSq = GoalKeeperInterceptRange * GoalKeeperInterceptRange;
            WithinRangeOfSupportSpotSq = WithinRangeOfSupportSpot * WithinRangeOfSupportSpot;
        }
    }
}
