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
        public float time;

        public EntityMemory()
        {
            position = new Vector3();
            time = -1;
        }

        public EntityMemory(Vector3 pos, float miliseconds)
        {
            position = new Vector3();
            position = pos;
            time = miliseconds;
        }
    }

    class SensoryMemoryComponent : Component
    {
        private double FOV = Math.PI; // 180 degrees
        private double rangeOfSight = 10000;
        private PositionComponent myPosition;
        private KinematicComponent myKinematic;
        private Dictionary<GameEntity, EntityMemory> memories;
        private GameTime time;

        public SensoryMemoryComponent(PositionComponent ownersPosition, KinematicComponent ownersKinematic)
            : base((int)component_flags.sensory)
        {
            myPosition = ownersPosition;
            myKinematic = ownersKinematic;
            time = new GameTime();
            memories = new Dictionary<GameEntity, EntityMemory>();
        }

        public void UpdateSenses(List<GameEntity> entities)
        {
           foreach (GameEntity entity in entities)
           {
               if (entity.HasComponent((int)component_flags.position))
               {
                   PositionComponent entityPos = entity.GetComponent<PositionComponent>();

                   if (WithinView(myPosition.GetPosition(), myKinematic.heading, entityPos.GetPosition()))
                   {
                       if (memories.ContainsKey(entity))
                       {
                           EntityMemory entityMem = memories[entity];
                           entityMem.time = time.ElapsedGameTime.Milliseconds;
                           entityMem.position = entityPos.GetPosition();

                           //Console.WriteLine("Memory Updated.");
                       }
                       else
                       {
                           EntityMemory entityMem = new EntityMemory(entityPos.GetPosition(), time.ElapsedGameTime.Milliseconds);
                           memories.Add(entity, entityMem);
                           Console.WriteLine("New Memory.");
                       }
                   }
               }
           }
        }

        public float GetEntityTimeMemory(GameEntity entity)
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

        public bool WithinView(Vector3 entityPos, Vector3 entityHeading, Vector3 otherEntityPos)
        {
            Vector3 toTarget = otherEntityPos - entityPos;

           // Console.WriteLine("Dist " + toTarget.Length());

            if (toTarget.Length() < rangeOfSight)
            {
                toTarget.Normalize();

                return (Vector3.Dot(entityHeading, (toTarget)) >= Math.Cos(FOV / 2.0));
            }

            return false;
        }
    }
}
