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

        public void Update(List<GameEntity> world, GameTime gameTime)
        {
            // Get the list of objects that will be processed by the physics system
            physicalObjects = BuildPhysicalList(world);

            ApplyForces(gameTime);
            ApplyCollisions();
        }

        /// <summary>
        /// Takes all physical objects from the world for processing.
        /// </summary>
        /// <param name="world">A list of entities in the world.</param>
        /// <returns>A list of objects with physical attributes.</returns>
        private List<GameEntity> BuildPhysicalList(List<GameEntity> world)
        {
            List<GameEntity> physicalWorld = new List<GameEntity>((Int32)32);
            foreach(GameEntity g in world)
            {
                if (g.HasComponent((int)component_flags.camera) && g.HasComponent((int)component_flags.position))
                {
                    PositionComponent cameraPos = g.GetComponent<PositionComponent>(component_flags.position);
                    cameraPos.SetPosition(camera.Position);
                    cameraPos.SetOrientation(camera.Orientation.W);
                }

                // Set the currently caught chicken free and add the required components back to it
                if (g.HasComponent((int)component_flags.kinematic) || g.HasComponent((int)component_flags.collidable))
                {
                    if (g.HasComponent((int)component_flags.caught) && !camera.chickenCaught)
                    {
                        PositionComponent p = g.GetComponent<PositionComponent>(component_flags.position);
                        KinematicComponent k = g.GetComponent<KinematicComponent>(component_flags.kinematic);
                        g.RemoveComponent<CaughtComponent>(component_flags.caught);
                        //p.SetPosition(new Vector3(p.GetPosition().X, 0, p.GetPosition().Z));

                        SteeringComponent steering = new SteeringComponent(p);

                        foreach (GameEntity entity in world)
                        {
                            if (entity.HasComponent((int)component_flags.camera) && entity.HasComponent((int)component_flags.position))
                            {
                                steering.SetScaryEntity(entity);
                            }
                        }

                        g.AddComponent(steering);
                        if (camera.DroppedChicken)
                        {
                            p.SetPosition(new Vector3(camera.Position.X, 0, camera.Position.Z));
                            k.IsGrounded = true;
                            k.Forces.Clear();
                            k.Velocity = Vector3.Zero;
                            camera.DroppedChicken = false;
                        }
                        else
                        {
                            // Add a force to the chicken in the direction we're looking with the camera
                            k.Forces.Add(new Force(camera.ViewDirection * 20000, 100));
                            k.Forces.Add(KinematicComponent.Gravity);
                            k.IsGrounded = false;
                        }
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

                    // Offset arms forward
                    GameEntity arm = physicalObjects.ElementAt<GameEntity>(i);
                    Vector3 prevPos = Vector3.Zero;

                    if (arm.HasComponent((int)component_flags.position))
                    {
                        Vector3 amount = new Vector3(0, 0, 20);
                        amount = Vector3.Transform(amount, Matrix.CreateRotationY(camera.HeadingDegrees));
                        Vector3 offsetPos = arm.GetComponent<PositionComponent>(component_flags.position).GetPosition() + amount;
                        prevPos = arm.GetComponent<PositionComponent>(component_flags.position).GetPosition();
                        arm.GetComponent<PositionComponent>(component_flags.position).SetPosition(offsetPos);
                    }

                    if (SimpleColliding(physicalObjects.ElementAt<GameEntity>(i),
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
                        else if ((physicalObjects.ElementAt<GameEntity>(i).HasComponent((int)component_flags.free) && physicalObjects.ElementAt<GameEntity>(i).HasComponent((int)component_flags.aiSteering) && physicalObjects.ElementAt<GameEntity>(j).HasComponent((int)component_flags.capture))
                              || (physicalObjects.ElementAt<GameEntity>(i).HasComponent((int)component_flags.capture) && physicalObjects.ElementAt<GameEntity>(j).HasComponent((int)component_flags.free) && (physicalObjects.ElementAt<GameEntity>(j).HasComponent((int)component_flags.aiSteering))))
                        {
                            PositionComponent pos;
                            if (physicalObjects.ElementAt<GameEntity>(i).HasComponent((int)component_flags.free))
                            {
                                pos = physicalObjects.ElementAt<GameEntity>(i).GetComponent<PositionComponent>(component_flags.position);
                                // Check if the chicken is actually in the pen
                                if (pos.GetPosition().Y > GameplayScreen.PEN_HEIGHT)
                                    continue;
                                physicalObjects.ElementAt<GameEntity>(i).RemoveComponent<FreeComponent>(component_flags.free);
                                chickenCaughtIndex = i;
                            }
                            else
                            {
                                pos = physicalObjects.ElementAt<GameEntity>(j).GetComponent<PositionComponent>(component_flags.position);
                                // Check if the chicken is actually in the pen
                                if (pos.GetPosition().Y > GameplayScreen.PEN_HEIGHT)
                                    continue;
                                physicalObjects.ElementAt<GameEntity>(j).RemoveComponent<FreeComponent>(component_flags.free);
                                chickenCaughtIndex = j;
                            }

                            TutorialScreen.remainingChickens--;
                            TutorialScreen.scored = true;
                            GameplayScreen.addTime = true;
                            GameplayScreen.remainingChickens--;
                            ArcadeScreen.chickenCaught = true;
                        }
                        else
                        {
                            Catchable = false;
                        }
                    }

                    if (arm.HasComponent((int)component_flags.position))
                    {
                        arm.GetComponent<PositionComponent>(component_flags.position).SetPosition(prevPos);
                    }

                    Renderable rend;
                    if (Catchable)
                    {
                        rend = physicalObjects.ElementAt<GameEntity>(chickenInRange).GetComponent<Renderable>(component_flags.renderable);
                        rend.SetLineColor(new Vector4(1, 1, 0, 1));
                        rend.SetBorderSize(0.4f);
                        rend.SetAmbientColor(new Vector4(1, 1, 0, 1));
                        rend.SetAmbientIntensity(0.4f);
                    }
                    else
                    {
                        // Determine the index of the chicken, and if they're not catchable, set the colour back to normal
                        int idx = physicalObjects.ElementAt<GameEntity>(i).HasComponent((int)component_flags.arm) ? j : i;
                        rend = physicalObjects.ElementAt<GameEntity>(idx).GetComponent<Renderable>(component_flags.renderable);
                        rend.SetLineColor(new Vector4(0, 0, 0, 1));
                        rend.SetBorderSize(0.2f);
                        rend.SetAmbientColor(new Vector4(1, 1, 1, 1));
                        rend.SetAmbientIntensity(0.1f);
                        rend.SetLineColor(new Vector4(0, 0, 0, 1));
                        rend.SetBorderSize(0.2f);
                        rend.SetAmbientColor(new Vector4(1, 1, 1, 1));
                        rend.SetAmbientIntensity(0.1f);
                    }
                }
            }
        }

        /// <summary>
        /// Applies forces (gravity, wind, AI behaviours, etc) to all objects that can move.
        /// </summary>
        private void ApplyForces(GameTime gameTime)
        {
            GameEntity cur;
            KinematicComponent kinematicComponent;
            PositionComponent positionComponent;

            // Loop through all objects, applying any forces to those with kinematic components (so long
            // as they're not caught)
            for (int i = 0; i < physicalObjects.Count; i++)
            {
                cur = physicalObjects.ElementAt<GameEntity>(i);
                if (cur.HasComponent((int)component_flags.kinematic) && !cur.HasComponent((int)component_flags.caught))
                {
                    kinematicComponent = cur.GetComponent<KinematicComponent>(component_flags.kinematic);
                    positionComponent = cur.GetComponent<PositionComponent>(component_flags.position);
                    
                    // If the entity is on the ground, go to the next one. I don't have time to implement
                    // physics properly, so this is my hacky workaround.
                    if (kinematicComponent.IsGrounded)
                        continue;

                    int numForces = kinematicComponent.Forces.Count;
                    Vector3 finalForce = Vector3.Zero;
                    Vector3 acceleration;
                    Vector3 velocity = kinematicComponent.Velocity;
                    //velocity.X *= KinematicComponent.LinearDamping;
                    //velocity.Z *= KinematicComponent.LinearDamping;
                    Vector3 newPos = positionComponent.GetPosition();

                    for (int j = 0; j < kinematicComponent.Forces.Count; j++)
                    {
                        finalForce += kinematicComponent.Forces[j].DirectionMagnitude;

                        // If the force has a duration, then update it; if its duration is int.MaxValue, then the force is supposed to be applied
                        // every frame so skip this step
                        if (kinematicComponent.Forces[j].Duration != int.MaxValue)
                        {
                            kinematicComponent.Forces[j].Duration -= (int)gameTime.ElapsedGameTime.TotalMilliseconds;

                            // If the force's duration has elapsed, remove it from the list
                            if (kinematicComponent.Forces[j].Duration <= 0)
                                kinematicComponent.Forces.RemoveAt(j);
                        }
                    }

                    // Calculate the acceleration and velocity
                    acceleration = finalForce / kinematicComponent.Mass;
                    velocity += acceleration * (float)gameTime.ElapsedGameTime.TotalSeconds;
                    kinematicComponent.Velocity = velocity;

                    // Calculate the new position and keep it in bounds
                    Vector3 oldPos = newPos;
                    Vector3 forward = camera.ViewDirection;
                    float radius = 0.0f;
                    if(cur.HasComponent((int)component_flags.renderable))
                    {
                        Renderable r = cur.GetComponent<Renderable>(component_flags.renderable);
                        if (r.HasSphere)
                            radius = r.GetBoundingSphere().Radius;
                    }
                    newPos += velocity * (float)gameTime.ElapsedGameTime.TotalSeconds;

                    // Restrict the object's position so that it doesn't go through bounds
                    ClampPos(ref oldPos, ref newPos, ref forward, radius, out newPos);

                    // Once they hit the ground again, clear all the forces and allow Dan's
                    // AI physics stuff to take over
                    if (newPos.Y == 0)
                    {
                        kinematicComponent.Forces.Clear();
                        kinematicComponent.IsGrounded = true;
                        kinematicComponent.Velocity = Vector3.Zero;
                    }

                    positionComponent.SetPosition(newPos);
                }
            }
        }

        private bool SimpleColliding(GameEntity ent1, GameEntity ent2)
        {
            Renderable c1 = ent1.GetComponent<Renderable>(component_flags.renderable);
            Renderable c2 = ent2.GetComponent<Renderable>(component_flags.renderable);

            Vector3 pos1;
            Vector3 pos2;

            if (ent1.HasComponent((int)component_flags.position))
            {
                pos1 = ent1.GetComponent<PositionComponent>(component_flags.position).GetPosition();
            }
            else
            {
                pos1 = new Vector3(c1.GetMatrix().M41, c1.GetMatrix().M42, c1.GetMatrix().M43);
            }

            if (ent2.HasComponent((int)component_flags.position))
            {
                pos2 = ent2.GetComponent<PositionComponent>(component_flags.position).GetPosition();
            }
            else
            {
                pos2 = new Vector3(c2.GetMatrix().M41, c2.GetMatrix().M42, c2.GetMatrix().M43);
            }
            
            float dist = (pos1 - pos2).Length();
            float intersectDist = 65;

            if (Math.Abs(dist) < intersectDist)
            {
                return true;
            }
            

            return false;
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
                physicalObjects.ElementAt<GameEntity>(chickenInRange).GetComponent<KinematicComponent>(component_flags.kinematic).Forces.Clear();
                camera.chickenCaught = true;
            }
        }

        /// <summary>
        /// Keeps the camera out of the pen and within the bounds of the level.
        /// </summary>
        /// <param name="prevPos">The camera's previous position.</param>
        /// <param name="desiredPos">The camera's desired position.</param>
        /// <param name="forwards">The forward direction of the camera.</param>
        /// <returns></returns>
        private void ClampPos(ref Vector3 prev, ref Vector3 desiredPos, ref Vector3 forwards, float radius, out Vector3 finalPos)
        {
            Vector3 newPos = desiredPos;
            float penRight = GameplayScreen.PEN_RIGHT_BOUND;
            float penLeft = GameplayScreen.PEN_LEFT_BOUND;
            float penTop = GameplayScreen.PEN_TOP_BOUND;
            float penBottom = GameplayScreen.PEN_BOTTOM_BOUND;
            const float penHeight = GameplayScreen.PEN_HEIGHT;

            // Whether it's possible that we're colliding with the pen
            bool possible = (desiredPos.X + radius >= penLeft && desiredPos.X - radius <= penRight) && // The chicken is between the right and left pen walls
                            (desiredPos.Z + radius >= penTop && desiredPos.Z - radius <= penBottom) && // The chicken is between the top and bottom pen walls
                            (desiredPos.Y - radius < penHeight);                                       // The desired position is somewhere between the ground and the top of the wall


            #region Stay in Bounds
            newPos.X = MathHelper.Clamp(newPos.X, -1070, 1070);
            newPos.Z = MathHelper.Clamp(newPos.Z, -1070, 1070);
            newPos.Y = MathHelper.Clamp(newPos.Y, 0, float.PositiveInfinity);
            #endregion
            #region Collide with Pen Walls
            // If they're about to go into the pen and their path would have them collide with a wall
            if (possible)
            {
                #region Collide with Right and Left walls
                // Touching the left wall?
                if ((desiredPos.X + radius >= penLeft) && (desiredPos.X - radius < penLeft))
                {
                    if (forwards.X < 0)
                        newPos.X = penLeft + radius;
                    else
                        newPos.X = penLeft - radius;
                }
                // Touching the right wall?
                else if ((desiredPos.X + radius >= penRight) && (desiredPos.X - radius < penRight))
                {
                    if (forwards.X > 0)
                        newPos.X = penRight - radius;
                    else
                        newPos.X = penRight + radius;
                }
                #endregion
                #region Collide with Top and Bottom Walls
                // Touching the top wall?
                if ((desiredPos.Z + radius >= penTop) && (desiredPos.Z - radius < penTop))
                {
                    if (forwards.Z > 0)
                        newPos.Z = penTop - radius;
                    else
                        newPos.Z = penTop + radius;
                }
                // Touching the bottom wall?
                else if ((desiredPos.Z + radius >= penBottom) && (desiredPos.Z - radius < penBottom))
                {
                    if (forwards.Z < 0)
                        newPos.Z = penBottom + radius;
                    else
                        newPos.Z = penBottom - radius;
                }
                #endregion
            }
            #endregion
            finalPos = newPos;
        }
    }
}
