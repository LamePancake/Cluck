using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using System.Linq;
using System.Text;

namespace Cluck.AI
{
    class Work : State
    {
        private static Work instance;

        private Work() { }

        public static Work Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new Work();
                }
                return instance;
            }
        }

        public void Enter(AIThinking component, GameEntity entity)
        {
            Console.WriteLine("State Entered: Work");
        }

        public void Execute(AIThinking component, GameEntity entity, GameTime deltaTime)
        {
            TestEntity testEntity = (TestEntity)entity;

            testEntity.GetSleepy();
            testEntity.GetThirsty();

            if (testEntity.IsThirsty())
            {
                component.ChangeStates(Drink.Instance);
            }
            else if (testEntity.IsSleepy())
            {
                component.ChangeStates(Sleep.Instance);
            }
        }

        public void Exit(AIThinking component, GameEntity entity)
        {
            Console.WriteLine("State Exited: Work");
        }
    }

    class Sleep : State
    {
        private static Sleep instance;

        private Sleep() { }

        public static Sleep Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new Sleep();
                }
                return instance;
            }
        }

        public void Enter(AIThinking component, GameEntity entity)
        {
            Console.WriteLine("State Entered: Sleep");
        }

        public void Execute(AIThinking component, GameEntity entity, GameTime deltaTime)
        {
            TestEntity testEntity = (TestEntity)entity;

            testEntity.Rest();
            testEntity.GetThirsty();

            if (testEntity.IsRested())
            {
                if (testEntity.IsThirsty())
                {
                    component.ChangeStates(Drink.Instance);
                }
                else
                {
                    component.ChangeStates(Work.Instance);
                }
            }
        }

        public void Exit(AIThinking component, GameEntity entity)
        {
            Console.WriteLine("State Exited: Sleep");
        }
    }

    class Drink : State
    {
        private static Drink instance;

        private Drink() { }

        public static Drink Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new Drink();
                }
                return instance;
            }
        }

        public void Enter(AIThinking component, GameEntity entity)
        {
            Console.WriteLine("State Entered: Drink");
        }

        public void Execute(AIThinking component, GameEntity entity, GameTime deltaTime)
        {
            TestEntity testEntity = (TestEntity)entity;

            testEntity.QuenchThirst();
            testEntity.GetSleepy();

            if (testEntity.ThirstQuenched())
            {
                if (testEntity.IsSleepy())
                {
                    component.ChangeStates(Sleep.Instance);
                }
                else
                {
                    component.ChangeStates(Work.Instance);
                }
            }
        }

        public void Exit(AIThinking component, GameEntity entity)
        {
            Console.WriteLine("State Exited: Drink");
        }
    }
}
