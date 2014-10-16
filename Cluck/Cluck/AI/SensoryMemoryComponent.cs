using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using System.Linq;
using System.Text;

namespace Cluck.AI
{
    class EntityMemory
    {
        public Vector3 position;
        public double time;

        public EntityMemory()
        {
            position = new Vector3();
            time = -1;
        }

        public EntityMemory(Vector3 pos, double miliseconds)
        {
            position = new Vector3();
            position = pos;
            time = miliseconds;
        }
    }

    class SensoryMemoryComponent : Component
    {
        private double FOV = Math.PI; // 180 degrees
        private double memorySpan = 5000;
        private double rangeOfSight = 500;
        private PositionComponent myPosition;
        private KinematicComponent myKinematic;
        private Dictionary<GameEntity, EntityMemory> memories;
        double totalTime;
        //private bool playerSpotted;

        public SensoryMemoryComponent(PositionComponent ownersPosition, KinematicComponent ownersKinematic)
            : base((int)component_flags.sensory)
        {
            myPosition = ownersPosition;
            myKinematic = ownersKinematic;
            memories = new Dictionary<GameEntity, EntityMemory>();
            //playerSpotted = false;
            totalTime = 0;
        }

        public void UpdateSenses(List<GameEntity> entities, GameTime time)
        {
           foreach (GameEntity entity in entities)
           {
               if (entity.HasComponent((int)component_flags.position))
               {
                   PositionComponent entityPos = entity.GetComponent<PositionComponent>();

                   if (memories.ContainsKey(entity))
                   {

                       EntityMemory entityMem = memories[entity];

                       if (WithinView(myPosition.GetPosition(), myKinematic.heading, entityPos.GetPosition()))
                       {
                           entityMem.position = entityPos.GetPosition();
                           entityMem.time = memorySpan;
                       }
                       else
                       {
                           if (entityMem.time > 0)
                           {
                               double remaining = entityMem.time - time.ElapsedGameTime.Milliseconds;

                               entityMem.time = remaining;

                               if (remaining <= 0)
                               {
                                   memories.Remove(entity);
                               }
                           }
                       }
                   }
                   else
                   {
                       EntityMemory entityMemEmpty = new EntityMemory();
                       memories.Add(entity, entityMemEmpty);
                   }
               }
           }
        }

        public bool HasMemories()
        {
            return (memories.Count > 0);
        }

        public EntityMemory GetMemory(GameEntity entity)
        {
            if (memories.ContainsKey(entity))
            {
                return memories[entity];
            }

            return null;
        }

        public double GetEntityTimeMemory(GameEntity entity)
        {
            if (memories.ContainsKey(entity))
            {
                return memories[entity].time;
            }

            return -1;
        }

        public Vector3 GetEntityPosMemory(GameEntity entity)
        {
            if (memories.ContainsKey(entity))
            {
                return memories[entity].position;
            }

            return Vector3.Zero;
        }

        public bool NewMemory(EntityMemory mem)
        {
            if (memorySpan - mem.time < 10)
            {
                return true;
            }

            return false;
        }

        public bool WithinView(Vector3 entityPos, Vector3 entityHeading, Vector3 otherEntityPos)
        {
            Vector3 toTarget = otherEntityPos - entityPos;
            
            if (toTarget.Length() < rangeOfSight)
            {
                toTarget.Normalize();

                return (Vector3.Dot(entityHeading, (toTarget)) >= Math.Cos(FOV / 2.0));
            }

            return false;
        }

        //public void PlayerSpotted(bool state)
        //{
        //    playerSpotted = state;
        //}

        //public bool PlayerSpotted()
        //{
        //    return playerSpotted;
        //}
    }
}
