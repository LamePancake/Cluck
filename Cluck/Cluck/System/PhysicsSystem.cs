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
        private bool catchable;
        
        public Boolean Catchable
        {
            get { return catchable; }
            set { catchable =  value; }
        }
               
        private int chickenInRange;
        private int armIndex;
        FirstPersonCamera camera;

        public static int chickenCaughtIndex;

        public PhysicsSystem(FirstPersonCamera cam) 
            : base((int)component_flags.kinematic | (int)component_flags.collidable)
        {
            camera = cam;
            catchable = false;
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
                    PositionComponent cameraPos = g.GetComponent<PositionComponent>(component_flags.position);
                    cameraPos.SetPosition(camera.Position);
                    cameraPos.SetOrientation(camera.Orientation.W);
                }

                if (g.HasComponent((int)component_flags.kinematic) || g.HasComponent((int)component_flags.collidable))
                {
                    if (g.HasComponent((int)component_flags.caught) && !camera.chickenCaught)
                    {
                        g.RemoveComponent<CaughtComponent>(component_flags.caught);
                        PositionComponent p = g.GetComponent<PositionComponent>(component_flags.position);
                        p.SetPosition(new Vector3(p.GetPosition().X, 0, p.GetPosition().Z));

                        SteeringComponent steering = new SteeringComponent(p);

                        foreach (GameEntity entity in world)
                        {
                            if (entity.HasComponent((int)component_flags.camera) && entity.HasComponent((int)component_flags.position))
                            {
                                steering.SetScaryEntity(entity);
                            }
                        }

                        g.AddComponent(steering);
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

                    if (Catchable)
                    {
                        physicalObjects.ElementAt<GameEntity>(chickenInRange).GetComponent<Renderable>(component_flags.renderable).SetLineColor(new Vector4(1, 1, 0, 1));
                        physicalObjects.ElementAt<GameEntity>(chickenInRange).GetComponent<Renderable>(component_flags.renderable).SetBorderSize(0.4f);
                        physicalObjects.ElementAt<GameEntity>(chickenInRange).GetComponent<Renderable>(component_flags.renderable).SetAmbientColor(new Vector4(1, 1, 0, 1));
                        physicalObjects.ElementAt<GameEntity>(chickenInRange).GetComponent<Renderable>(component_flags.renderable).SetAmbientIntensity(0.4f);
                    }
                    else
                    {
                        physicalObjects.ElementAt<GameEntity>(j).GetComponent<Renderable>(component_flags.renderable).SetLineColor(new Vector4(0, 0, 0, 1));
                        physicalObjects.ElementAt<GameEntity>(j).GetComponent<Renderable>(component_flags.renderable).SetBorderSize(0.2f);
                        physicalObjects.ElementAt<GameEntity>(j).GetComponent<Renderable>(component_flags.renderable).SetAmbientColor(new Vector4(1, 1, 1, 1));
                        physicalObjects.ElementAt<GameEntity>(j).GetComponent<Renderable>(component_flags.renderable).SetAmbientIntensity(0.1f);
                        physicalObjects.ElementAt<GameEntity>(i).GetComponent<Renderable>(component_flags.renderable).SetLineColor(new Vector4(0, 0, 0, 1));
                        physicalObjects.ElementAt<GameEntity>(i).GetComponent<Renderable>(component_flags.renderable).SetBorderSize(0.2f);
                        physicalObjects.ElementAt<GameEntity>(i).GetComponent<Renderable>(component_flags.renderable).SetAmbientColor(new Vector4(1, 1, 1, 1));
                        physicalObjects.ElementAt<GameEntity>(i).GetComponent<Renderable>(component_flags.renderable).SetAmbientIntensity(0.1f);
                    }
                    
                    if(!physicalObjects.ElementAt<GameEntity>(j)
                        .HasComponent((int)component_flags.collidable))
                        continue;

                    //physicalObjects.ElementAt<GameEntity>(i).GetComponent<Renderable>(component_flags.renderable).SetInRange(false);
                    //physicalObjects.ElementAt<GameEntity>(j).GetComponent<Renderable>(component_flags.renderable).SetInRange(false);

                    if (Colliding(physicalObjects.ElementAt<GameEntity>(i),
                                  physicalObjects.ElementAt<GameEntity>(j)))
                    {
                        if (physicalObjects.ElementAt<GameEntity>(i).HasComponent((int)component_flags.arm) && physicalObjects.ElementAt<GameEntity>(j).HasComponent((int)component_flags.free))
                        {
                            Catchable = true;
                            chickenInRange = j;
                            armIndex = i;
                            
                            if (camera.IsClapping() && !camera.chickenCaught)
                            {
                                CatchChicken();
                            }
                        }
                        else if (physicalObjects.ElementAt<GameEntity>(i).HasComponent((int)component_flags.free) && physicalObjects.ElementAt<GameEntity>(j).HasComponent((int)component_flags.arm))
                        {
                            Catchable = true;
                            chickenInRange = i;
                            armIndex = j;
                            
                            if (camera.IsClapping() && !camera.chickenCaught)
                            {
                                CatchChicken();
                            }
                        }
                        else if ((physicalObjects.ElementAt<GameEntity>(i).HasComponent((int)component_flags.free) && (physicalObjects.ElementAt<GameEntity>(i).HasComponent((int)component_flags.aiSteering)) && physicalObjects.ElementAt<GameEntity>(j).HasComponent((int)component_flags.capture))
                            || (physicalObjects.ElementAt<GameEntity>(i).HasComponent((int)component_flags.capture) && physicalObjects.ElementAt<GameEntity>(j).HasComponent((int)component_flags.free) && (physicalObjects.ElementAt<GameEntity>(j).HasComponent((int)component_flags.aiSteering))))
                        {
                            if (physicalObjects.ElementAt<GameEntity>(i).HasComponent((int)component_flags.free))
                            {
                                physicalObjects.ElementAt<GameEntity>(i).RemoveComponent<FreeComponent>(component_flags.free);
                                chickenCaughtIndex = i;
                            }
                            else
                            {
                                physicalObjects.ElementAt<GameEntity>(j).RemoveComponent<FreeComponent>(component_flags.free);
                                chickenCaughtIndex = j;
                            }

                            TutorialScreen.remainingChickens--;
                            GameplayScreen.addTime = true;
                            GameplayScreen.remainingChickens--;
                            ArcadeScreen.chickenCaught = true;
                        }
                        else
                        {
                            Catchable = false;
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
            Renderable c1 = ent1.GetComponent<Renderable>(component_flags.renderable);
            Renderable c2 = ent2.GetComponent<Renderable>(component_flags.renderable);

            // Get the bounding sphere
            // Translate it to the entity's position
            // Loop through the other entity, performing the same steps with its spheres, checking whether they collide

            for (int i = 0; i < c1.GetModel().Meshes.Count; i++)
            {
                BoundingSphere c1BoundingSphere = c1.GetBoundingSphere();// c1.GetModel().Meshes[0].BoundingSphere;
                
                // Translate the bounding sphere to the appropriate place (hack to deal with arms' lack of position component)
                if (ent1.HasComponent((int)component_flags.position))
                {
                    c1BoundingSphere.Center = ent1.GetComponent<PositionComponent>(component_flags.position).GetPosition();
                    //c1BoundingSphere.Radius = ent1.GetComponent<Renderable>().GetModel().Meshes[0].BoundingSphere.Radius;
                }
                else
                {
                    c1BoundingSphere.Center = new Vector3(c2.GetMatrix().M41, c2.GetMatrix().M42, c2.GetMatrix().M43);
                    //c1BoundingSphere.Radius = ent1.GetComponent<Renderable>().GetModel().Meshes[0].BoundingSphere.Radius;
                }

                for (int j = 0; j < c2.GetModel().Meshes.Count; j++)
                {
                    BoundingSphere c2BoundingSphere = c2.GetBoundingSphere();//c2.GetModel().Meshes[0].BoundingSphere;

                    // Translate the bounding sphere to the appropriate place
                    if (ent2.HasComponent((int)component_flags.position))
                    {
                        c2BoundingSphere.Center = ent2.GetComponent<PositionComponent>(component_flags.position).GetPosition();
                        //c2BoundingSphere.Radius = ent2.GetComponent<Renderable>().GetModel().Meshes[0].BoundingSphere.Radius;
                    }
                    else
                    {
                        c2BoundingSphere.Center = new Vector3(c2.GetMatrix().M41, c2.GetMatrix().M42, c2.GetMatrix().M43);
                        //c2BoundingSphere.Radius = ent2.GetComponent<Renderable>().GetModel().Meshes[0].BoundingSphere.Radius;
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
                physicalObjects.ElementAt<GameEntity>(chickenInRange).RemoveComponent<SteeringComponent>(component_flags.aiSteering);
                physicalObjects.ElementAt<GameEntity>(chickenInRange).AddComponent(new CaughtComponent());
                camera.chickenCaught = true;
            }
        }
    }
}
