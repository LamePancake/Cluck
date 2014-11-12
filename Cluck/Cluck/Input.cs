using System;
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
        private bool clap;
        private bool slide;

        private bool x;
        private bool y;
        private bool b;
        private bool a;

        private float mouseX;
        private float mouseY;

        private float clapping;

        public Input()
        {
            moveForward = 0.0f;
            moveBackward = 0.0f;
            strafeRight = 0.0f;
            strafeLeft = 0.0f;

            jump = false;
            crouch = false;
            sprint = false;
            clap = false;
            slide = false;
            x = false;
            y = false;
            b = false;
            a = false;

            mouseX = 0;
            mouseY = 0;

            clapping = 0;
        }

        public float GetForward() { return moveForward; }
        public float GetBackward() { return moveBackward; }
        public float GetLeft() { return strafeLeft; }
        public float GetRight() { return strafeRight; }

        public bool IsJumping() { return jump; }
        public bool IsCrouching() { return crouch; }
        public bool IsSprinting() { return sprint; }
        public bool IsClapping() { return clap; }
        public bool IsSliding() { return slide; }

        public bool IsXPressed() { return x; }
        public bool IsYPressed() { return y; }
        public bool IsBPressed() { return b; }
        public bool IsAPressed() { return a; }

        public float GetViewX() { return mouseX; }
        public float GetViewY() { return mouseY; }

        public float GetClapping() { return clapping; }

        public void SetForward(float f) { moveForward = f; }
        public void SetBackward(float b) { moveBackward = b; }
        public void SetLeft(float l) { strafeLeft = l; }
        public void SetRight(float r) { strafeRight = r; }

        public void SetJumping(bool j) { jump = j; }
        public void SetCrouching(bool c) { crouch = c; }
        public void SetSprinting(bool s) { sprint = s; }
        public void SetClapping(bool c) { clap = c; }
        public void SetSliding(bool s) { slide = s; }

        public void SetXButton(bool d) { x = d; }
        public void SetYButton(bool d) { y = d; }
        public void SetBButton(bool d) { b = d; }
        public void SetAButton(bool d) { a = d; }

        public void SetViewX(float x) { mouseX = x; }
        public void SetViewY(float Y) { mouseY = Y; }

        public void AddViewX(float x) { mouseX += x; }
        public void AddViewY(float Y) { mouseY += Y; }

        public void SetClap(float c) { clapping = c; }
    }
}
