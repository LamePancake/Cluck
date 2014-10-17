using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using System.Linq;
using System.Text;

namespace Cluck.AI
{
    class SteeringComponent : Component
    {

        SteeringBehaviours steeringBehaviours;
        private PositionComponent target;
        private GameEntity scaryEntity;
        private Vector3 scaryPos;
        public Vector3 wanderTarget;
        public float wanderOffset = 900;
        public float wanderRadius = 50;
        public float wanderJitter = 3.5f;
        public float feelerLength = 5f;
        public List<Vector3> feelers;
        private bool wanderOn;
        private bool fleeOn;
        public List<Vector3> feelers;
        public float feelerLength = 100;

        public SteeringComponent(PositionComponent targetPos) : base((int)component_flags.aiSteering)
        {
            wanderOn = true;
            fleeOn = false;

            target = targetPos;

            Random rando = new Random();

            double theta = rando.NextDouble() * (2*Math.PI);

            wanderTarget = new Vector3((float)(wanderRadius * Math.Cos(theta)), 0, (float)(wanderRadius * Math.Sin(theta)));

            steeringBehaviours = new SteeringBehaviours();
            feelers = new List<Vector3>();
            scaryPos = Vector3.Zero;

            feelers = new List<Vector3>();
        }

        public SteeringOutput Calculate(List<GameEntity> entities, PositionComponent position, KinematicComponent kinematics, float deltaTime)
        {
            float weightWander = 0.7f;
            float weightFlee = 0.4f;
            float weightWallAvoid = 1f;
            SteeringOutput steeringTot = new SteeringOutput();
            SteeringOutput steering = new SteeringOutput();

            if (wanderOn)
            {
                steering = steeringBehaviours.Wander(position, kinematics, this, deltaTime);

                steeringTot.linear += (steering.linear * weightWander);

                //if (!AccumulateForce(steeringTot.linear, steering.linear, kinematics, weightWander))
                //    return steeringTot;
            }

            if (fleeOn)
            {
                steering = steeringBehaviours.Flee(position, kinematics, scaryPos);

                steeringTot.linear += (steering.linear * weightFlee);

                //if (!AccumulateForce(steeringTot.linear, steering.linear, kinematics, weightFlee))
                //    return steeringTot;
            }

            steeringBehaviours.WallAvoidance(entities, position, kinematics, this);

            return steeringTot;
        }

        bool AccumulateForce(Vector3 RunningTot, Vector3 ForceToAdd, KinematicComponent kinematic, float weight)
        {
            ForceToAdd *= weight;

            double MagnitudeSoFar = RunningTot.Length();

            double MagnitudeRemaining = kinematic.maxSpeed - MagnitudeSoFar;

            if (MagnitudeRemaining <= 0.0) return false;

            double MagnitudeToAdd = ForceToAdd.Length();
  
            if (MagnitudeToAdd < MagnitudeRemaining)
            {
                RunningTot += ForceToAdd;
            }
            else
            {
                //add it to the steering force
                ForceToAdd.Normalize();
                RunningTot += (ForceToAdd * (float)MagnitudeRemaining); 
            }
            return true;
        }

        public Vector3 GetTarget()
        {
            return target.GetPosition();
        }

        public void SetWander(bool state)
        {
            wanderOn = state;
        }

        public void SetFlee(bool state)
        {
            fleeOn = state;
        }

        public void SetScaryPos(Vector3 scary)
        {
            scaryPos = scary;
        }

        public void SetScaryEntity(GameEntity entity)
        {
            scaryEntity = entity;
        }

        public GameEntity GetScaryEntity()
        {
            return scaryEntity;
        }
    }
}
