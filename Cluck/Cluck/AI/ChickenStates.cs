using System;
using System.Collections.Generic;
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
            Console.WriteLine("State Entered: Meander");
            if (entity.HasComponent((int)component_flags.aiSteering)
                && entity.HasComponent((int)component_flags.kinematic))
            {
                SteeringComponent steer = entity.GetComponent<SteeringComponent>();
                KinematicComponent kinematic = entity.GetComponent<KinematicComponent>();
                kinematic.maxSpeed = kinematic.maxWalkSpeed;
                steer.SetWander(true);
            }
        }

        public void Execute(AIThinking component, GameEntity entity)
        {

            if (entity.HasComponent((int)component_flags.sensory))
            {
                SensoryMemoryComponent sensory = entity.GetComponent<SensoryMemoryComponent>();

                if (sensory.PlayerSpotted())
                {
                    component.ChangeStates(RunAway.Instance);
                }
            }

            //testEntity.GetSleepy();
            //testEntity.GetThirsty();

            //if (testEntity.IsThirsty())
            //{
            //    component.ChangeStates(Drink.Instance);
            //}
            //else if (testEntity.IsSleepy())
            //{
            //    component.ChangeStates(Sleep.Instance);
            //}
        }

        public void Exit(AIThinking component, GameEntity entity)
        {
            Console.WriteLine("State Exited: Meander");
            if (entity.HasComponent((int)component_flags.aiSteering))
            {
                SteeringComponent steer = entity.GetComponent<SteeringComponent>();

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
            Console.WriteLine("State Entered: RunAway");

            if (entity.HasComponent((int)component_flags.aiSteering)
                && entity.HasComponent((int)component_flags.kinematic))
            {
                SteeringComponent steer = entity.GetComponent<SteeringComponent>();
                KinematicComponent kinematic = entity.GetComponent<KinematicComponent>();
                kinematic.maxSpeed = kinematic.maxRunSpeed;
                
                steer.SetFlee(true);
            }
        }

        public void Execute(AIThinking component, GameEntity entity)
        {

            //testEntity.Rest();
            //testEntity.GetThirsty();

            //if (testEntity.IsRested())
            //{
            //    if (testEntity.IsThirsty())
            //    {
            //        component.ChangeStates(Drink.Instance);
            //    }
            //    else
            //    {
            //        component.ChangeStates(Work.Instance);
            //    }
            //}
        }

        public void Exit(AIThinking component, GameEntity entity)
        {
            Console.WriteLine("State Exited: RunAway");

            if (entity.HasComponent((int)component_flags.aiSteering))
            {
                SteeringComponent steer = entity.GetComponent<SteeringComponent>();

                steer.SetFlee(false);
            }
        }
    }
}
