using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Cluck.AI
{
    class TestEntity : GameEntity
    {

        private int energized = 500;
        private int thirstQuenched = 50;
        private int fatigueLevel;
        private int thirstLevel;

        public TestEntity()
        {
            fatigueLevel = energized;
            thirstLevel = thirstQuenched;

            AIThinking aiThinkingComponent = new AIThinking(this, Work.Instance);
            AddComponent(aiThinkingComponent);
        }

        public void GetThirsty()
        {
            if (thirstLevel > 0)
            {
                thirstLevel--;

                if (thirstLevel < 0)
                {
                    thirstLevel = 0;
                }
            }
        }

        public void QuenchThirst()
        {
            thirstLevel++;
        }

        public void GetSleepy()
        {
            if (fatigueLevel > 0)
            {
                fatigueLevel--;

                if (fatigueLevel < 0)
                {
                    fatigueLevel = 0;
                }
            }
        }

        public void Rest()
        {
            fatigueLevel++;
        }

        public bool IsThirsty()
        {
            return (thirstLevel <= 0);
        }

        public bool IsSleepy()
        {
            return (fatigueLevel <= 0);
        }

        public bool IsRested()
        {
            return (fatigueLevel >= energized);
        }

        public bool ThirstQuenched()
        {
            return (thirstLevel >= thirstQuenched); 
        }
    }
}
