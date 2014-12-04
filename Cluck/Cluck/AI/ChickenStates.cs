using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using System.Linq;
using System.Text;

namespace Cluck.AI
{
    class Meander : State
    {
        private static Meander instance;

        private Meander() { }

        public static Meander Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new Meander();
                }
                return instance;
            }
        }

        public void Enter(AIThinking component, GameEntity entity)
        {
            //Console.WriteLine("State Entered: Meander");
            if (entity.HasComponent((int)component_flags.aiSteering)
                && entity.HasComponent((int)component_flags.kinematic))
            {
                SteeringComponent steer = entity.GetComponent<SteeringComponent>(component_flags.aiSteering);
                KinematicComponent kinematic = entity.GetComponent<KinematicComponent>(component_flags.kinematic);
                kinematic.MaxSpeed = kinematic.MaxWalkSpeed;
                steer.SetWander(true);
            }
        }

        public void Execute(AIThinking component, GameEntity entity, GameTime deltaTime)
        {

            if (entity.HasComponent((int)component_flags.sensory) 
                && entity.HasComponent((int)component_flags.aiSteering)
                && entity.HasComponent((int)component_flags.position))
            {
                SensoryMemoryComponent sensory = entity.GetComponent<SensoryMemoryComponent>(component_flags.sensory);

                SteeringComponent steering = entity.GetComponent<SteeringComponent>(component_flags.aiSteering);

                PositionComponent position = entity.GetComponent<PositionComponent>(component_flags.position);

                GameEntity scary = steering.GetScaryEntity();

                if (scary != null)
                {
                    EntityMemory mem = sensory.GetMemory(scary);

                    if (mem != null)
                    {
                        if (sensory.NewMemory(mem))
                        {
                            steering.SetScaryPos(mem.position);
                            component.ChangeStates(RunAway.Instance);
                            //if ((mem.position - position.GetPosition()).Length() < 300)
                            //{
                            //    component.ChangeStates(FlyAway.Instance);
                            //}
                            //else
                            //{
                            //    component.ChangeStates(RunAway.Instance);
                            //}
                        }
                    }
                }
            }
        }

        public void Exit(AIThinking component, GameEntity entity)
        {
            //Console.WriteLine("State Exited: Meander");
            if (entity.HasComponent((int)component_flags.aiSteering))
            {
                SteeringComponent steer = entity.GetComponent<SteeringComponent>(component_flags.aiSteering);

                //steer.SetWander(false);
            }
        }
    }

    class RunAway : State
    {
        private static RunAway instance;

        private RunAway() { }

        public static RunAway Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new RunAway();
                }
                return instance;
            }
        }

        public void Enter(AIThinking component, GameEntity entity)
        {
            //Console.WriteLine("State Entered: RunAway");

            if (entity.HasComponent((int)component_flags.aiSteering)
                && entity.HasComponent((int)component_flags.kinematic))
            {
                SteeringComponent steer = entity.GetComponent<SteeringComponent>(component_flags.aiSteering);
                KinematicComponent kinematic = entity.GetComponent<KinematicComponent>(component_flags.kinematic);
                kinematic.MaxSpeed = kinematic.MaxRunSpeed;
                
                steer.SetFlee(true);
            }
        }

        public void Execute(AIThinking component, GameEntity entity, GameTime deltaTime)
        {

            if (entity.HasComponent((int)component_flags.sensory) 
                && entity.HasComponent((int)component_flags.aiSteering)
                && entity.HasComponent((int)component_flags.position))
            {
                SensoryMemoryComponent sensory = entity.GetComponent<SensoryMemoryComponent>(component_flags.sensory);

                SteeringComponent steering = entity.GetComponent<SteeringComponent>(component_flags.aiSteering);

                PositionComponent position = entity.GetComponent<PositionComponent>(component_flags.position);

                GameEntity scary = steering.GetScaryEntity();

                if (scary != null)
                {
                    EntityMemory mem = sensory.GetMemory(scary);

                    if (mem != null)
                    {
                        if (sensory.NewMemory(mem))
                        {
                            steering.SetScaryPos(mem.position);

                            //if ((mem.position - position.GetPosition()).Length() < 300)
                            //{
                            //    component.ChangeStates(FlyAway.Instance);
                            //}
                        }
                        else if (mem.time < 0)
                        {
                            component.ChangeStates(Meander.Instance);
                        }
                    }
                }
            }
        }

        public void Exit(AIThinking component, GameEntity entity)
        {
            //Console.WriteLine("State Exited: RunAway");

            if (entity.HasComponent((int)component_flags.aiSteering))
            {
                SteeringComponent steer = entity.GetComponent<SteeringComponent>(component_flags.aiSteering);

                steer.SetFlee(false);
            }
        }
    }

    class FlyAway : State
    {
        private static FlyAway instance;

        private FlyAway() { }

        public static FlyAway Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new FlyAway();
                }
                return instance;
            }
        }

        public void Enter(AIThinking component, GameEntity entity)
        {
            if (entity.HasComponent((int)component_flags.aiSteering)
                && entity.HasComponent((int)component_flags.kinematic))
            {
                SteeringComponent steer = entity.GetComponent<SteeringComponent>(component_flags.aiSteering);
                KinematicComponent kinematic = entity.GetComponent<KinematicComponent>(component_flags.kinematic);
                kinematic.MaxSpeed = kinematic.MaxFlySpeed;
                steer.SetWander(false);
                steer.SetFly(true);
            }
        }

        public void Execute(AIThinking component, GameEntity entity, GameTime deltaTime)
        {
            if (entity.HasComponent((int)component_flags.sensory) && entity.HasComponent((int)component_flags.aiSteering))
            {
                SensoryMemoryComponent sensory = entity.GetComponent<SensoryMemoryComponent>(component_flags.sensory);

                SteeringComponent steering = entity.GetComponent<SteeringComponent>(component_flags.aiSteering);

                GameEntity scary = steering.GetScaryEntity();

                if (scary != null)
                {
                    EntityMemory mem = sensory.GetMemory(scary);

                    if (mem != null)
                    {
                        //if (sensory.NewMemory(mem))
                        //{
                        //    steering.SetScaryPos(mem.position);
                        //    component.ChangeStates(RunAway.Instance);
                        //}
                    }
                    else
                    {
                        component.ChangeStates(Meander.Instance);
                    }
                }
            }
        }

        public void Exit(AIThinking component, GameEntity entity)
        {
            if (entity.HasComponent((int)component_flags.aiSteering))
            {
                SteeringComponent steer = entity.GetComponent<SteeringComponent>(component_flags.aiSteering);

                steer.SetWander(true);
                steer.SetFly(false);
            }
        }
    }

    class Attack : State
    {
        private static Attack instance;

        private Attack() { }

        public static Attack Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new Attack();
                }
                return instance;
            }
        }

        public void Enter(AIThinking component, GameEntity entity)
        {
            //Console.WriteLine("State Entered: Meander");
            if (entity.HasComponent((int)component_flags.aiSteering)
                && entity.HasComponent((int)component_flags.kinematic))
            {
                SteeringComponent steer = entity.GetComponent<SteeringComponent>(component_flags.aiSteering);
                KinematicComponent kinematic = entity.GetComponent<KinematicComponent>(component_flags.kinematic);
                kinematic.MaxSpeed = kinematic.MaxFlySpeed;
                steer.SetWander(true);
                steer.SetFlee(false);
                steer.SetFly(false);
                steer.SetSeek(true);
            }
        }

        public void Execute(AIThinking component, GameEntity entity, GameTime deltaTime)
        {

            
        }

        public void Exit(AIThinking component, GameEntity entity)
        {
            //Console.WriteLine("State Exited: Meander");
            if (entity.HasComponent((int)component_flags.aiSteering))
            {
                SteeringComponent steer = entity.GetComponent<SteeringComponent>(component_flags.aiSteering);

                //steer.SetWander(false);
            }
        }
    }
}
