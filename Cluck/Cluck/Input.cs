﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Cluck
{
    public class Input
    {
        private float moveForward;
        private float moveBackward;
        private float strafeRight;
        private float strafeLeft;

        private bool jump;
        private bool crouch;
        private bool sprint;

        private float mouseX;
        private float mouseY;

        public Input()
        {
            moveForward = 0.0f;
            moveBackward = 0.0f;
            strafeRight = 0.0f;
            strafeLeft = 0.0f;

            jump = false;
            crouch = false;
            sprint = false;

            mouseX = 0;
            mouseY = 0;
        }

        public float GetForward() { return moveForward; }
        public float GetBackward() { return moveBackward; }
        public float GetLeft() { return strafeLeft; }
        public float GetRight() { return strafeRight; }

        public bool IsJumping() { return jump; }
        public bool IsCrouching() { return crouch; }
        public bool IsSprinting() { return sprint; }

        public float GetViewX() { return mouseX; }
        public float GetViewY() { return mouseY; }

        public void SetForward(float f) { moveForward = f; }
        public void SetBackward(float b) { moveBackward = b; }
        public void SetLeft(float l) { strafeLeft = l; }
        public void SetRight(float r) { strafeRight = r; }

        public void SetJumping(bool j) { jump = j; }
        public void SetCrouching(bool c) { crouch = c; }
        public void SetSprinting(bool s) { sprint = s; }

        public void SetViewX(float x) { mouseX = x; }
        public void SetViewY(float Y) { mouseY = Y; }

        public void AddViewX(float x) { mouseX += x; }
        public void AddViewY(float Y) { mouseY += Y; }
    }
}