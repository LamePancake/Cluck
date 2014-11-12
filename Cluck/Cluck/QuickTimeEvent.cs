using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Cluck
{
    public class QuickTimeEvent
    {
        private float timer;
        private float MAX_TIME = 6;
        private int MAX_NUM_PRESSES = 4;
        private bool start;

        private int currentButton;
        private int counter;

        public QuickTimeEvent()
        {
            timer = 0;
            counter = 0;
            currentButton = -1;
            start = false;
        }

        public bool update(float gameTime, Input i)
        {
            start = true;
            timer += gameTime;

            if (Cluck.currentLevel >= 1 && Cluck.currentLevel < 5)
            {
                MAX_NUM_PRESSES = 4;
                MAX_TIME = 6;
            }
            else if (Cluck.currentLevel >= 5 && Cluck.currentLevel < 10)
            {
                MAX_NUM_PRESSES = 6;
                MAX_TIME = 4;
            }
            else
            {
                MAX_NUM_PRESSES = 8;
                MAX_TIME = 2;
            }

            if (currentButton == -1)
            {
                if (timer >= MAX_TIME)
                {
                    Random r = new Random();
                    currentButton = r.Next(4);
                    timer = 0;
                }
            }

            if (timer >= MAX_TIME)
            {
                if (counter < MAX_NUM_PRESSES)
                {
                    return false;
                }
                reset();
            }

            switch (currentButton)
            {
                case (int)Cluck.buttons.xq:
                    if (i.IsXPressed())
                    {
                        counter++;
                    }
                    break;
                case (int)Cluck.buttons.ye:
                    if (i.IsYPressed())
                    {
                        counter++;
                    }
                    break;
                case (int)Cluck.buttons.br:
                    if (i.IsBPressed())
                    {
                        counter++;
                    }
                    break;
                case (int)Cluck.buttons.af:
                    if (i.IsAPressed())
                    {
                        counter++;
                    }
                    break;
            }

            return true;
        }

        public void reset()
        {
            timer = 0;
            counter = 0;
            currentButton = -1;
            start = false;
        }

        public bool isOn()
        {
            return start;
        }

        public int getCurrentButton()
        {
            return currentButton;
        }
    }
}
