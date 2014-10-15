using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Cluck.AI;

namespace Cluck
{
    class PhysicsSystem : GameSystem
    {
        /// <summary>
        /// The previous game time (used for calculating movement).
        /// </summary>
        private float prevTime = 0.0f;
        private List<GameEntity> physicalObjects;
        private Boolean catchable = false;
        private Boolean chickenCaught = false;
        private int chickenInRange;
        private int armIndex;
        FirstPersonCamera camera;

        public PhysicsSystem(FirstPersonCamera cam) 
            : base((int)component_flags.kinematic | (int)component_flags.collidable)
        {
            camera = cam;
        }

        public void Update(List<GameEntity> world, float gameTime)
        {
            // Get the list of objects that will be processed by the physics system
            physicalObjects = BuildPhysicalList(world);

            ApplyForces(gameTime - prevTime);
            ApplyCollisions();

            prevTime = gameTime;
        }

        /// <summary>
        /// Takes all physical objects from the world for processing.
        /// </summary>
        /// <param name="world">A list of entities in the world.</param>
        /// <returns>A list of objects with physical attributes.</returns>
        private List<GameEntity> BuildPhysicalList(List<GameEntity> world)
        {
            List<GameEntity> physicalWorld = new List<GameEntity>((Int32)64);
            foreach(GameEntity g in world)
            {
                if (g.HasComponent((int)component_flags.camera) && g.HasComponent((int)component_flags.position))
                {
                    g.GetComponent<PositionComponent>().SetPosition(camera.Position);
                    g.GetComponent<PositionComponent>().SetOrientation(camera.Orientation.W);
                }

                if (g.HasComponent((int)component_flags.kinematic) || g.HasComponent((int)component_flags.collidable))
                {
                    if (g.HasComponent((int)component_flags.caught) && !camera.chickenCaught)
                    {
                        g.RemoveComponent<CaughtComponent>();
                        PositionComponent p = g.GetComponent<PositionComponent>();
                        p.SetPosition(new Vector3(p.GetPosition().X, 0, p.GetPosition().Z));
                        g.AddComponent(new SteeringComponent(p));
                    }
                    physicalWorld.Add(g);
                }
            }
            return physicalWorld;
        }

        /// <summary>
        /// Checks for collisions among collidable objects.
        /// </summary>
        /// TODO: Optimise loop that checks for collidable components (remember the next collidable obj)
        private void ApplyCollisions()
        {
            int i;

            // Find the first collidable object
            for (i = 0; i < physicalObjects.Count(); i++)
                if(physicalObjects.ElementAt<GameEntity>(i)
                    .HasComponent((int)component_flags.collidable))
                    break;

            // No collidable objects; don't bother
            if(i == physicalObjects.Count())
                return;

            // Loop through the remaining entities to check for collisions
            for (; i < physicalObjects.Count(); i++)
            {
                if (!physicalObjects.ElementAt<GameEntity>(i)
                    .HasComponent((int)component_flags.collidable))
                    continue;

                // Check the collidable entity against every other collidable entity
                for (int j = i + 1; j < physicalObjects.Count(); j++)
                {
                    if(!physicalObjects.ElementAt<GameEntity>(j)
                        .HasComponent((int)component_flags.collidable))
                        continue;

                    if (Colliding(physicalObjects.ElementAt<GameEntity>(i),
                                  physicalObjects.ElementAt<GameEntity>(j)))
                    {
                        if (physicalObjects.ElementAt<GameEntity>(i).HasComponent(0x00200) && physicalObjects.ElementAt<GameEntity>(j).HasComponent(0x00010))
                        {
                            Console.WriteLine("arm " + i + ", chicken " + j);
                            catchable = true;
                            chickenInRange = j;
                            if (camera.IsClapping() && !camera.chickenCaught)
                            {
                                CatchChicken();
                            }
                        }
                        else if (physicalObjects.ElementAt<GameEntity>(i).HasComponent(0x00010) && physicalObjects.ElementAt<GameEntity>(j).HasComponent(0x00200))
                        {
                            Console.WriteLine("chicken " + i + ", arm " + j);
                            catchable = true;
                            chickenInRange = i;
                            armIndex = j;
                            if (camera.IsClapping() && !camera.chickenCaught)
                            {
                                CatchChicken();
                            }
                        }
                        else
                        {
                            catchable = false;
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Applies forces (gravity, wind, AI behaviours, etc) to all objects that can move.
        /// </summary>
        /// <param name="kinematicObjects">A list of all GameEntities with a KinematicComponent.</param>
        private void ApplyForces(float deltaTime)
        {
            // Look for force components in this object
            // Apply them to the kinematics components
            // Using the deltaTime, move objects appropriately
        }

        /// <summary>
        /// Determines whether two objects are colliding.
        /// </summary>
        /// <param name="ent1">The first entity.</param>
        /// <param name="ent2">The second entity.</param>
        /// <returns>True if ent1 and ent2 are colliding; false otherwise.</returns>
        /// TODO: Remove dependency on Renderable component (Collidable should contain all info).
        private bool Colliding(GameEntity ent1, GameEntity ent2)
        {
            Renderable c1 = ent1.GetComponent<Renderable>();
            Renderable c2 = ent2.GetComponent<Renderable>();

            // Get the bounding sphere
            // Translate it to the entity's position
            // Loop through the other entity, performing the same steps with its spheres, checking whether they collide

            for (int i = 0; i < c1.GetModel().Meshes.Count; i++)
            {
                BoundingSphere c1BoundingSphere = c1.GetModel().Meshes[i].BoundingSphere;
                
                // Translate the bounding sphere to the appropriate place (hack to deal with arms' lack of position component)
                if (ent1.HasComponent((int)component_flags.position))
                {
                    c1BoundingSphere.Center = ent1.GetComponent<PositionComponent>().GetPosition();
                }
                else
                {
                    c1BoundingSphere.Center = new Vector3(c2.GetMatrix().M41, c2.GetMatrix().M42, c2.GetMatrix().M43);
                }

                for (int j = 0; j < c2.GetModel().Meshes.Count; j++)
                {
                    BoundingSphere c2BoundingSphere = c2.GetModel().Meshes[j].BoundingSphere;

                    // Translate the bounding sphere to the appropriate place
                    if (ent2.HasComponent((int)component_flags.position))
                    {
                        c2BoundingSphere.Center = ent2.GetComponent<PositionComponent>().GetPosition();
                    }
                    else
                    {
                        c2BoundingSphere.Center = new Vector3(c2.GetMatrix().M41, c2.GetMatrix().M42, c2.GetMatrix().M43);
                    }

                    if (c1BoundingSphere.Intersects(c2BoundingSphere))
                        return true;
                }
            }
            return false;
        }

        private bool GetCatchState()
        {
            return catchable;
        }

        public void CatchChicken()
        {
            if (catchable && physicalObjects.ElementAt<GameEntity>(chickenInRange).HasComponent(0x00010))
            {
                physicalObjects.ElementAt<GameEntity>(chickenInRange).RemoveComponent<SteeringComponent>();
                physicalObjects.ElementAt<GameEntity>(chickenInRange).AddComponent(new CaughtComponent());
                chickenCaught = true;
                camera.chickenCaught = true;
            }
        }
    }
}
