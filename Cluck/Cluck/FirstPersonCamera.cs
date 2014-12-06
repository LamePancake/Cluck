using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Audio;

namespace Cluck
{
    public class FirstPersonCamera : GameComponent
    {
        public enum Actions
        {
            Crouch,
            Jump,
            MoveForwards,
            MoveBackwards,
            StrafeRight,
            StrafeLeft,
            Run
        }

        public enum Posture
        {
            Standing,
            Crouching,
            Rising,
            Jumping,
			Sliding
        };

        /// <summary>
        /// Specifies the signature for a callback function to be invoked when the player moves.
        /// The function receives the previous and desired positions and the direction of movement
        /// and returns the new camera position. Allows for keeping the camera in bounds and arbitrary
        /// restriction of movement.
        /// </summary>
        /// <param name="prevPos">The camera position in the last update.</param>
        /// <param name="desiredPos">The desired position.</param>
        /// <param name="direction">The direction of movement.</param>
        /// <returns>The new position for the camera.</returns>
        public delegate void PositionUpdateCallback(ref Vector3 prevPos, ref Vector3 desiredPos, ref Vector3 direction, out Vector3 finalPos);

        public const float MAX_SPRINTING_TIME = 5;

        public const float DEFAULT_FOVX = 90.0f;
        public const float DEFAULT_ZNEAR = 0.1f;
        public const float DEFAULT_ZFAR = 1000.0f;

        private static Vector3 WORLD_X_AXIS = new Vector3(1.0f, 0.0f, 0.0f);
        private static Vector3 WORLD_Y_AXIS = new Vector3(0.0f, 1.0f, 0.0f);
        private static Vector3 WORLD_Z_AXIS = new Vector3(0.0f, 0.0f, 1.0f);
        private const float DEFAULT_ACCELERATION_X = 8.0f;
        private const float DEFAULT_ACCELERATION_Y = 8.0f;
        private const float DEFAULT_ACCELERATION_Z = 8.0f;
        private const float DEFAULT_VELOCITY_X = 1.0f;
        private const float DEFAULT_VELOCITY_Y = 1.0f;
        private const float DEFAULT_VELOCITY_Z = 1.0f;
        private const float DEFAULT_RUNNING_MULTIPLIER = 2.0f;
        private const float DEFAULT_SLIDING_MULTIPLIER = 3.0f;
        private const float DEFAULT_CROUCHING_MULTIPLIER = 0.95f;
        private const float DEFAULT_SPEED_ROTATION = 0.3f;
        private const float HEIGHT_MULTIPLIER_CROUCHING = 0.6f;
		private const float HEIGHT_MULTIPLIER_SLIDING = 0.67f;

        private const float BOBBINGSPEED_RUNNING = 0.30f;
        private const float BOBBINGSPEED_WALKING = 0.25f;
        private const float BOBBINGAMOUNT_RUNNING = 0.05f;
        private const float BOBBINGAMOUNT_WALKING = 0.02f;

        private float fovx;
        private float aspectRatio;
        private float znear;
        private float zfar;
        private float accumHeadingDegrees;
        private float accumPitchDegrees;
        private float eyeHeightStanding;
        private float eyeHeightCrouching;
		private float eyeHeightSliding;
        private PositionUpdateCallback posUpdate;
        private Vector3 eye;
        private Vector3 target;
        private Vector3 targetYAxis;
        private Vector3 xAxis;
        private Vector3 yAxis;
        private Vector3 zAxis;
        private Vector3 viewDir;
        private Vector3 acceleration;
        private Vector3 currentVelocity;
        private Vector3 velocity;
        private Vector3 velocityWalking;
        private Vector3 velocityRunning;
		private Vector3 velocitySliding;
        private Vector3 velocityCrouching;
        private Quaternion orientation;
        private Matrix viewMatrix;
        private Matrix projMatrix;

        private Posture posture;
        private Posture prevPosture;
        private float rotationSpeed;

        private InputManager inputManager;

        private const float ARM_SCALE = 0.03f;
        private const float RIGHT_ARM_X_OFFSET = 20;
        private const float ARM_Y_OFFSET = -15;
        private const float ARM_Z_OFFSET = 33;
        private const float LEFT_ARM_X_OFFSET = -20;
        private const float MIN_RIGHT_ARM_X_OFFSET = 15.0f;
        private const float MIN_LEFT_ARM_X_OFFSET = -15.0f;

        private const float CHICKEN_OFFSET_X = 0;
        private const float CHICKEN_OFFSET_Y = -30;
        private const float CHICKEN_OFFSET_Z = 57;

        private const float ARM_MOVEMENT = 25.0f;
        
        float leftXOffset;
        float rightXOffset;

        private bool isClapping;
        private bool isSprinting;
        private bool handsGoingBack = false;

        public bool chickenCaught;
        HeadBob head;

		private float slideElapsedSeconds;
        private bool isSliding;

        private float clap = 0;

        private float sprintingTime = MAX_SPRINTING_TIME;

        private QuickTimeEvent qte;

        private bool dead;
        private int tiltFix = 0;
        private int slideFix = 0;

        private SoundEffectInstance walk;
        private SoundEffectInstance slide;
        
        public void Reset()
        {
            head = new HeadBob();
            qte = new QuickTimeEvent();
            Position = new Vector3(0, 0, 0);
            chickenCaught = false;
            sprintingTime = MAX_SPRINTING_TIME;
            dead = false;

            Quaternion tilt = Quaternion.Identity;
            Quaternion.CreateFromAxisAngle(ref WORLD_Z_AXIS, MathHelper.ToRadians(1 * tiltFix), out tilt);
            Quaternion.Concatenate(ref orientation, ref tilt, out orientation);
            tiltFix = 0;
            handsGoingBack = false;

            if (slideFix > 0)
            {
                Quaternion slideTilt = Quaternion.Identity;
                Quaternion.CreateFromAxisAngle(ref WORLD_Z_AXIS, MathHelper.ToRadians(-slideFix), out slideTilt);
                Quaternion.Concatenate(ref orientation, ref slideTilt, out orientation);
                slideFix = 0;
            }
            isSliding = false;
            isSprinting = false;
            walk = null;
            slide = null;
        }

        public FirstPersonCamera(Game game) : base(game)
        {
            UpdateOrder = 1;

            // Initialize camera state.
            fovx = DEFAULT_FOVX;
            znear = DEFAULT_ZNEAR;
            zfar = DEFAULT_ZFAR;
            accumHeadingDegrees = 0.0f;
            accumPitchDegrees = 0.0f;
            eyeHeightStanding = 0.0f;
            eyeHeightCrouching = 0.0f;
			eyeHeightSliding = 0.0f;
            eye = Vector3.Zero;
            target = Vector3.Zero;
            targetYAxis = Vector3.UnitY;
            xAxis = Vector3.UnitX;
            yAxis = Vector3.UnitY;
            zAxis = Vector3.UnitZ;
            viewDir = Vector3.Forward;
            acceleration = new Vector3(DEFAULT_ACCELERATION_X, DEFAULT_ACCELERATION_Y, DEFAULT_ACCELERATION_Z);
            velocityWalking = new Vector3(DEFAULT_VELOCITY_X, DEFAULT_VELOCITY_Y, DEFAULT_VELOCITY_Z);
            velocityRunning = velocityWalking * DEFAULT_RUNNING_MULTIPLIER;
			velocitySliding = velocityWalking * DEFAULT_SLIDING_MULTIPLIER;
            velocityCrouching = velocityWalking * DEFAULT_CROUCHING_MULTIPLIER;

            velocity = velocityWalking;
            orientation = Quaternion.Identity;
            viewMatrix = Matrix.Identity;
            posture = Posture.Standing;
            
            // Initialize mouse and keyboard input.
            rotationSpeed = DEFAULT_SPEED_ROTATION;

            // Setup perspective projection matrix.
            Rectangle clientBounds = game.Window.ClientBounds;
            float aspect = (float)clientBounds.Width / (float)clientBounds.Height;
            Perspective(fovx, aspect, znear, zfar);

            inputManager = new InputManager();

            leftXOffset = LEFT_ARM_X_OFFSET;
            rightXOffset = RIGHT_ARM_X_OFFSET;
            chickenCaught = false;
            head = new HeadBob();

			slideElapsedSeconds = 0.0f;
            isSliding = false;

            qte = new QuickTimeEvent();

            dead = false;
            isSprinting = false;
        }

        public override void Initialize()
        {
            base.Initialize();

            Rectangle clientBounds = Game.Window.ClientBounds;
#if WINDOWS
            Mouse.SetPosition(clientBounds.Width / 2, clientBounds.Height / 2);
#endif
        }

        /// <summary>
        /// Moves the camera by dx world units to the left or right; dy
        /// world units upwards or downwards; and dz world units forwards
        /// or backwards.
        /// </summary>
        /// <param name="dx">Distance to move left or right.</param>
        /// <param name="dy">Distance to move up or down.</param>
        /// <param name="dz">Distance to move forwards or backwards.</param>
        public void Move(float dx, float dy, float dz)
        {
            // Calculate the forwards direction. Can't just use the
            // camera's view direction as doing so will cause the camera to
            // move more slowly as the camera's view approaches 90 degrees
            // straight up and down.

            Vector3 forwards = Vector3.Normalize(Vector3.Cross(WORLD_Y_AXIS, xAxis));
            Vector3 prev = Position;

            eye += xAxis * dx;
            eye += WORLD_Y_AXIS * dy;
            eye += forwards * dz;

            if (posUpdate != null)
                posUpdate(ref prev, ref eye, ref forwards, out eye);

            Position = eye;
        }

       
        public void Perspective(float fovx, float aspect, float znear, float zfar)
        {
            this.fovx = fovx;
            this.aspectRatio = aspect;
            this.znear = znear;
            this.zfar = zfar;

            float aspectInv = 1.0f / aspect;
            float e = 1.0f / (float)Math.Tan(MathHelper.ToRadians(fovx) / 2.0f);
            float fovy = 2.0f * (float)Math.Atan(aspectInv / e);
            float xScale = 1.0f / (float)Math.Tan(0.5f * fovy);
            float yScale = xScale / aspectInv;

            projMatrix.M11 = xScale;
            projMatrix.M12 = 0.0f;
            projMatrix.M13 = 0.0f;
            projMatrix.M14 = 0.0f;

            projMatrix.M21 = 0.0f;
            projMatrix.M22 = yScale;
            projMatrix.M23 = 0.0f;
            projMatrix.M24 = 0.0f;

            projMatrix.M31 = 0.0f;
            projMatrix.M32 = 0.0f;
            projMatrix.M33 = (zfar + znear) / (znear - zfar);
            projMatrix.M34 = -1.0f;

            projMatrix.M41 = 0.0f;
            projMatrix.M42 = 0.0f;
            projMatrix.M43 = (2.0f * zfar * znear) / (znear - zfar);
            projMatrix.M44 = 0.0f;
        }

        public void Rotate(float headingDegrees, float pitchDegrees)
        {
            headingDegrees *= rotationSpeed;
            pitchDegrees *= rotationSpeed;

            headingDegrees = -headingDegrees;
            pitchDegrees = -pitchDegrees;
            
            accumPitchDegrees += pitchDegrees;

            if (accumPitchDegrees > 90.0f)
            {
                pitchDegrees = 90.0f - (accumPitchDegrees - pitchDegrees);
                accumPitchDegrees = 90.0f;
            }

            if (accumPitchDegrees < -90.0f)
            {
                pitchDegrees = -90.0f - (accumPitchDegrees - pitchDegrees);
                accumPitchDegrees = -90.0f;
            }

            accumHeadingDegrees += headingDegrees;

            if (accumHeadingDegrees > 360.0f)
                accumHeadingDegrees -= 360.0f;

            if (accumHeadingDegrees < -360.0f)
                accumHeadingDegrees += 360.0f;

            float heading = MathHelper.ToRadians(headingDegrees);
            float pitch = MathHelper.ToRadians(pitchDegrees);
            Quaternion rotation = Quaternion.Identity;

            // Rotate the camera about the world Y axis.
            if (heading != 0.0f)
            {
                Quaternion.CreateFromAxisAngle(ref WORLD_Y_AXIS, heading, out rotation);
                Quaternion.Concatenate(ref rotation, ref orientation, out orientation);
            }

            // Rotate the camera about its local X axis.
            if (pitch != 0.0f)
            {
                Quaternion.CreateFromAxisAngle(ref WORLD_X_AXIS, pitch, out rotation);
                Quaternion.Concatenate(ref orientation, ref rotation, out orientation);
            }

            UpdateViewMatrix();
        }

        public override void Update(GameTime gameTime)
        {
            Input i = inputManager.Update(Game.Window.ClientBounds);

            if (i.IsClapping() && !chickenCaught && !dead)
            {
                isClapping = true;
            }
            else if (i.IsClapping() && chickenCaught)
            {

                isClapping = false;
            }

            if ((i.IsSprinting() || i.IsSliding() || isSliding) && sprintingTime > 0 && !dead && !isCrouching())
            {
                float elaspedSeconds = (float)gameTime.ElapsedGameTime.TotalSeconds;
                sprintingTime -= elaspedSeconds;
            }
            else if (!i.IsSprinting() && !i.IsSliding() && sprintingTime < MAX_SPRINTING_TIME || isCrouching())
            {
                sprintingTime += ((float)gameTime.ElapsedGameTime.TotalSeconds) / 2;
            }

            sprintingTime = MathHelper.Clamp(sprintingTime, 0, MAX_SPRINTING_TIME);

            if (sprintingTime > 0 && i.IsSprinting())
            {
                isSprinting = true;
            }
            else
            {
                isSprinting = false;
            }

            if (chickenCaught)
            {
                if (!qte.update((float)gameTime.ElapsedGameTime.TotalSeconds, i))
                {
                    chickenCaught = false;
                    qte.reset();
                }
            }
            else
            {
                qte.reset();
            }

            UpdateCamera(gameTime, i);

            if (!dead)
            {
                if (i.IsSprinting() && !i.IsCrouching() && sprintingTime > 0)
                {
                    eye = head.Update(i.GetLeft() + i.GetRight(), i.GetForward() + i.GetBackward(), eye, (float)gameTime.ElapsedGameTime.Milliseconds, BOBBINGSPEED_RUNNING, BOBBINGAMOUNT_RUNNING);
                }
                else
                {
                    eye = head.Update(i.GetLeft() + i.GetRight(), i.GetForward() + i.GetBackward(), eye, (float)gameTime.ElapsedGameTime.Milliseconds, BOBBINGSPEED_WALKING, BOBBINGAMOUNT_WALKING);
                }

                eye.Y = MathHelper.Clamp(eye.Y, eyeHeightCrouching, eyeHeightStanding + BOBBINGAMOUNT_RUNNING);
            }


			if (isSliding && !chickenCaught && GamePad.GetState(PlayerIndex.One).IsConnected)
            {
                GamePad.SetVibration(PlayerIndex.One, 0.1f, 0.2f);
            }

            base.Update(gameTime);
        }

        /// <summary>
        /// Calculates the world transformation matrix for the weapon attached
        /// to the FirstPersonCamera. The weapon moves along with the camera.
        /// The offsets are to ensure the weapon is slightly in front of the
        /// camera and to one side.
        /// </summary>
        /// <param name="xOffset">How far to position the weapon left or right.</param>
        /// <param name="yOffset">How far to position the weapon up or down.</param>
        /// <param name="zOffset">How far to position the weapon in front or behind.</param>
        /// <param name="scale">How much to scale the weapon.</param>
        /// <returns>The weapon world transformation matrix.</returns>
        public Matrix GetLeftArmWorldMatrix(float gameTime)
        {
            Vector3 leftArmPos = eye;

            if (isClapping)
            {
                if (leftXOffset <= MIN_LEFT_ARM_X_OFFSET && !handsGoingBack)
                {
                    leftXOffset += ARM_MOVEMENT * gameTime;
                }
                else if (!chickenCaught)
                {
                    isClapping = false;
                }
            }
            else
            {
                if (leftXOffset > LEFT_ARM_X_OFFSET)
                {
                    handsGoingBack = true;
                    leftXOffset -= ARM_MOVEMENT * gameTime;

                }
                else
                {
                    handsGoingBack = false;
                }
                chickenCaught = false;

            }
           
            leftArmPos += viewDir * ARM_Z_OFFSET;
            leftArmPos += yAxis * ARM_Y_OFFSET;
            leftArmPos += xAxis * leftXOffset;
  
            return /*Matrix.CreateScale(ARM_SCALE)
                * */Matrix.CreateRotationX(MathHelper.ToRadians(PitchDegrees))
                * Matrix.CreateRotationY(MathHelper.ToRadians(HeadingDegrees))
                * Matrix.CreateTranslation(leftArmPos);
        }

        public Vector3 GetChickenPosition()
        {
            Vector3 chickenPos = eye;
            chickenPos += viewDir * CHICKEN_OFFSET_Z;
            chickenPos += yAxis * CHICKEN_OFFSET_Y;
            chickenPos += xAxis * CHICKEN_OFFSET_X;

            Matrix chickenWorld = Matrix.CreateRotationX(MathHelper.ToRadians(PitchDegrees))
                * Matrix.CreateRotationY(MathHelper.ToRadians(HeadingDegrees))
                * Matrix.CreateTranslation(chickenPos);

            return new Vector3(chickenWorld.M41, chickenWorld.M42, chickenWorld.M43);
        }

        /// <summary>
        /// Calculates the world transformation matrix for the weapon attached
        /// to the FirstPersonCamera. The weapon moves along with the camera.
        /// The offsets are to ensure the weapon is slightly in front of the
        /// camera and to one side.
        /// </summary>
        /// <param name="xOffset">How far to position the weapon left or right.</param>
        /// <param name="yOffset">How far to position the weapon up or down.</param>
        /// <param name="zOffset">How far to position the weapon in front or behind.</param>
        /// <param name="scale">How much to scale the weapon.</param>
        /// <returns>The weapon world transformation matrix.</returns>
        public Matrix GetRightArmWorldMatrix(float gameTime)
        {
            Vector3 rightArmPos = eye;

            if (isClapping)
            {
                if (rightXOffset >= MIN_RIGHT_ARM_X_OFFSET && !handsGoingBack)
                {
                    rightXOffset -= ARM_MOVEMENT * gameTime;
                }
                else if (!chickenCaught)
                {
                    isClapping = false;
                }
            }
            else 
            {
                if (rightXOffset < RIGHT_ARM_X_OFFSET)
                {
                    handsGoingBack = true;
                    rightXOffset += ARM_MOVEMENT * gameTime;

                }
                else
                {
                    handsGoingBack = false;
                }
                chickenCaught = false;

            }

            rightArmPos += viewDir * ARM_Z_OFFSET;
            rightArmPos += yAxis * ARM_Y_OFFSET;
            rightArmPos += xAxis * rightXOffset;

            return /*Matrix.CreateScale(ARM_SCALE)
                * */Matrix.CreateRotationX(MathHelper.ToRadians(PitchDegrees))
                * Matrix.CreateRotationY(MathHelper.ToRadians(HeadingDegrees))
                * Matrix.CreateTranslation(rightArmPos);
        }


        /// <summary>
        /// Determines which way to move the camera based on player input.
        /// The returned values are in the range [-1,1].
        /// </summary>
        /// <param name="direction">The movement direction.</param>
        private Vector3 GetMovementDirection(Vector3 direction, Input i)
        {
            direction.X = 0.0f;
            direction.Y = 0.0f;
            direction.Z = 0.0f;

            direction.Z += i.GetForward() + i.GetBackward();
            direction.X += i.GetRight() + i.GetLeft();

            if ((direction.X != 0 || direction.Z != 0))
            {
                if (walk != null && !walk.IsDisposed && walk.State == SoundState.Paused)
                    walk.Resume();
                else if (walk != null && !walk.IsDisposed && walk.State != SoundState.Playing)
                    walk.Play();
                else
                    walk.Pause();

                if (isSprinting)
                    walk.Pitch = 0.55f;
                else if (i.IsCrouching())
                    walk.Pitch = -0.05f;
                else
                    walk.Pitch = 0.05f;
            }
            else
            {
                if (walk != null && !walk.IsDisposed && walk.State == SoundState.Playing )
                    walk.Pause();
            }

            if (isSliding)
            {
                if ((direction.X != 0 || direction.Z != 0))
                {
                    if (slide != null && !slide.IsDisposed && slide.State != SoundState.Playing)
                    {
                        walk.Pause();
                        slide.Play();
                    }

                    switch (posture)
                    {
                        case Posture.Standing:
                            posture = Posture.Sliding;
                            direction.Y -= 1.0f;
                            currentVelocity.Y = 0.0f;
                            break;

                        case Posture.Sliding:
                            direction.Y -= 1.0f;
                            break;

                        default:
                            break;
                    }
                }
                else
                {
                    isSliding = false;
                }
            }
            else
            {
                if (slide != null && !slide.IsDisposed && slide.State == SoundState.Playing)
                    slide.Stop();

                isSliding = false;
                switch (posture)
                {
                    case Posture.Sliding:
                        posture = Posture.Rising;
                        direction.Y += 1.0f;
                        currentVelocity.Y = 0.0f;
                        break;

                    case Posture.Rising:
                        direction.Y += 1.0f;
                        break;

                    default:
                        break;
                }
            }
			

            if (i.IsCrouching() || dead)
            {
                switch (posture)
                {
                case Posture.Standing:
                    posture = Posture.Crouching;
                    direction.Y -= 1.0f;
                    currentVelocity.Y = 0.0f;
                    break;

                case Posture.Crouching:
                    direction.Y -= 1.0f;
                    break;

                case Posture.Rising:
                    // Finish rising before allowing another crouch.
                    direction.Y += 1.0f;
                    currentVelocity.Y = 0.0f;
                    break;

                default:
                    break;
                }
            }
            else
            {
                switch (posture)
                {
                case Posture.Crouching:
                    posture = Posture.Rising;
                    direction.Y += 1.0f;
                    currentVelocity.Y = 0.0f;
                    break;

                case Posture.Rising:
                    direction.Y += 1.0f;
                    currentVelocity.Y = 0.0f;
                    break;

                default:
                    break;
                }
            }

            //if (i.IsJumping())
            //{
            //    switch (posture)
            //    {
            //    case Posture.Standing:
            //        posture = Posture.Jumping;
            //        currentVelocity.Y = velocity.Y;
            //        direction.Y += 1.0f;
            //        break;

            //    case Posture.Jumping:
            //        direction.Y += 1.0f;
            //        break;

            //    default:
            //        break;
            //    }
            //}
            //else
            //{
            //    if (posture == Posture.Jumping)
            //        direction.Y += 1.0f;
            //}
            return direction;
        }

        private void UpdateCamera(GameTime gameTime, Input i)
        {
            prevPosture = posture;
            float elapsedTimeSec = (float)gameTime.ElapsedGameTime.TotalSeconds;
            Vector3 direction = new Vector3();

            //if (slideFix > 0 && !isSliding)
            //{
            //    Quaternion tilt = Quaternion.Identity;
            //    Quaternion.CreateFromAxisAngle(ref WORLD_Z_AXIS, MathHelper.ToRadians(-slideFix), out tilt);
            //    Quaternion.Concatenate(ref orientation, ref tilt, out orientation);
            //    slideFix = 0;
            //}

            if (i.IsSliding() && !isSliding && posture != Posture.Rising && sprintingTime > 0)
            {
                //Quaternion tilt = Quaternion.Identity;
                //Quaternion.CreateFromAxisAngle(ref WORLD_Z_AXIS, MathHelper.ToRadians(3), out tilt);
                //Quaternion.Concatenate(ref orientation, ref tilt, out orientation);
                //slideFix = 3;
                isSliding = true;
            }

            if (isSliding)
                velocity = velocitySliding;
            else if (i.IsSprinting() && !i.IsCrouching() && sprintingTime > 0)
                velocity = velocityRunning;
            else if (i.IsCrouching() || dead)
                velocity = velocityCrouching;
            else
                velocity = velocityWalking;

            if(!dead)
            {
			    direction = GetMovementDirection(direction, i);

                Rotate(i.GetViewX(), i.GetViewY()); 
			    UpdateVelocity(ref direction, elapsedTimeSec);
                UpdatePosition(ref direction, elapsedTimeSec);
            }
        }

        /// <summary>
        /// Moves the camera based on player input.
        /// </summary>
        /// <param name="direction">Direction moved.</param>
        /// <param name="elapsedTimeSec">Elapsed game time.</param>
        private void UpdatePosition(ref Vector3 direction, float elapsedTimeSec)
        {
            if (currentVelocity.LengthSquared() != 0.0f)
            {
                // Only move the camera if the velocity vector is not of zero
                // length. Doing this guards against the camera slowly creeping
                // around due to floating point rounding errors.

                Vector3 displacement = (currentVelocity * elapsedTimeSec) +
                    (0.5f * acceleration * elapsedTimeSec * elapsedTimeSec);

                // Floating point rounding errors will slowly accumulate and
                // cause the camera to move along each axis. To prevent any
                // unintended movement the displacement vector is clamped to
                // zero for each direction that the camera isn't moving in.
                // Note that the UpdateVelocity() method will slowly decelerate
                // the camera's velocity back to a stationary state when the
                // camera is no longer moving along that direction. To account
                // for this the camera's current velocity is also checked.

                if (direction.X == 0.0f && (float)Math.Abs(currentVelocity.X) < 1e-6f)
                    displacement.X = 0.0f;

                if (direction.Y == 0.0f && (float)Math.Abs(currentVelocity.Y) < 1e-6f)
                    displacement.Y = 0.0f;

                if (direction.Z == 0.0f && (float)Math.Abs(currentVelocity.Z) < 1e-6f)
                    displacement.Z = 0.0f;

                Move(displacement.X, displacement.Y, displacement.Z);

                switch (posture)
                {
                case Posture.Standing:
                    break;

                case Posture.Crouching:
                    if (eye.Y < eyeHeightCrouching)
                        eye.Y = eyeHeightCrouching;
                    break;

                case Posture.Rising:
                    if (eye.Y >= eyeHeightStanding)
                    {
                        eye.Y = eyeHeightStanding;
                        posture = Posture.Standing;
                        currentVelocity.Y = 0.0f;
                    }
                    break;

                case Posture.Jumping:
                    if (eye.Y < eyeHeightStanding)
                    {
                        eye.Y = eyeHeightStanding;
                        posture = Posture.Standing;
                        currentVelocity.Y = 0.0f;
                    }
                    break;
				case Posture.Sliding:
                    if (eye.Y <= eyeHeightSliding)
                    {
                        eye.Y = eyeHeightSliding;

                        slideElapsedSeconds += elapsedTimeSec;
                        if (slideElapsedSeconds >= 0.8f)//0.55
                        {
                            posture = Posture.Rising;
                            //Quaternion tilt = Quaternion.Identity;
                            //Quaternion.CreateFromAxisAngle(ref WORLD_Z_AXIS, MathHelper.ToRadians(-slideFix), out tilt);
                            //Quaternion.Concatenate(ref orientation, ref tilt, out orientation);
                            slideElapsedSeconds = 0;
                            slideFix = 0;
                            isSliding = false;
                        }
                    }
                    break;
                }
            }

            // Continuously update the camera's velocity vector even if the
            // camera hasn't moved during this call. When the camera is no
            // longer being moved the camera is decelerating back to its
            // stationary state.


            //UpdateVelocity(ref direction, elapsedTimeSec);
        }

        /// <summary>
        /// Updates the camera's velocity based on the supplied movement
        /// direction and the elapsed time (since this method was last
        /// called). The movement direction is the in the range [-1,1].
        /// </summary>
        /// <param name="direction">Direction moved.</param>
        /// <param name="elapsedTimeSec">Elapsed game time.</param>
        private void UpdateVelocity(ref Vector3 direction, float elapsedTimeSec)
        {
            if (direction.LengthSquared() > 1)
            {
                velocity *= 0.7071f;
            }

            if (direction.X != 0.0f)
            {
                // Camera is moving along the x axis.
                // Linearly accelerate up to the camera's max speed.

                currentVelocity.X += direction.X * acceleration.X * elapsedTimeSec;

                if (currentVelocity.X > velocity.X)
                    currentVelocity.X = velocity.X;
                else if (currentVelocity.X < -velocity.X)
                    currentVelocity.X = -velocity.X;
            }
            else
            {
                // Camera is no longer moving along the x axis.
                // Linearly decelerate back to stationary state.

                if (currentVelocity.X > 0.0f)
                {
                    if ((currentVelocity.X -= acceleration.X * elapsedTimeSec) < 0.0f)
                        currentVelocity.X = 0.0f;
                }
                else
                {
                    if ((currentVelocity.X += acceleration.X * elapsedTimeSec) > 0.0f)
                        currentVelocity.X = 0.0f;
                }
            }

            if (direction.Y != 0.0f)
            {
                // Camera is moving along the y axis. There are two cases here:
                // jumping and crouching. When jumping we're always applying a
                // negative acceleration to simulate the force of gravity.
                // However when crouching we apply a positive acceleration and
                // rely more on the direction.

                if (posture == Posture.Jumping)
                    currentVelocity.Y += direction.Y * -acceleration.Y * elapsedTimeSec;
                else
                    currentVelocity.Y += direction.Y * acceleration.Y * elapsedTimeSec;

                if (currentVelocity.Y > velocity.Y)
                    currentVelocity.Y = velocity.Y;
                else if (currentVelocity.Y < -velocity.Y)
                    currentVelocity.Y = -velocity.Y;
            }
            else
            {
                // Camera is no longer moving along the y axis.
                // Linearly decelerate back to stationary state.

                if (currentVelocity.Y > 0.0f)
                {
                    if ((currentVelocity.Y -= acceleration.Y * elapsedTimeSec) < 0.0f)
                        currentVelocity.Y = 0.0f;
                }
                else
                {
                    if ((currentVelocity.Y += acceleration.Y * elapsedTimeSec) > 0.0f)
                        currentVelocity.Y = 0.0f;
                }
            }

            if (direction.Z != 0.0f)
            {
                // Camera is moving along the z axis.
                // Linearly accelerate up to the camera's max speed.

                currentVelocity.Z += direction.Z * acceleration.Z * elapsedTimeSec;

                if (currentVelocity.Z > velocity.Z)
                    currentVelocity.Z = velocity.Z;
                else if (currentVelocity.Z < -velocity.Z)
                    currentVelocity.Z = -velocity.Z;
            }
            else
            {
                // Camera is no longer moving along the z axis.
                // Linearly decelerate back to stationary state.

                if (currentVelocity.Z > 0.0f)
                {
                    if ((currentVelocity.Z -= acceleration.Z * elapsedTimeSec) < 0.0f)
                        currentVelocity.Z = 0.0f;
                }
                else
                {
                    if ((currentVelocity.Z += acceleration.Z * elapsedTimeSec) > 0.0f)
                        currentVelocity.Z = 0.0f;
                }
            }
        }

        private void UpdateViewMatrix()
        {
            Matrix.CreateFromQuaternion(ref orientation, out viewMatrix);

            xAxis.X = viewMatrix.M11;
            xAxis.Y = viewMatrix.M21;
            xAxis.Z = viewMatrix.M31;

            yAxis.X = viewMatrix.M12;
            yAxis.Y = viewMatrix.M22;
            yAxis.Z = viewMatrix.M32;

            zAxis.X = viewMatrix.M13;
            zAxis.Y = viewMatrix.M23;
            zAxis.Z = viewMatrix.M33;

            viewMatrix.M41 = -Vector3.Dot(xAxis, eye);
            viewMatrix.M42 = -Vector3.Dot(yAxis, eye);
            viewMatrix.M43 = -Vector3.Dot(zAxis, eye);

            viewDir.X = -zAxis.X;
            viewDir.Y = -zAxis.Y;
            viewDir.Z = -zAxis.Z;


        }

        public float GetStaminaRatio()
        {
            float ratio = (sprintingTime / MAX_SPRINTING_TIME);
            ratio = MathHelper.Clamp(ratio, 0, 1);
            return ratio;
        }

        public QuickTimeEvent GetQTE()
        {
            return qte;
        }

        public void KillPlayer()
        {
            Quaternion tilt = Quaternion.Identity;
            Quaternion.CreateFromAxisAngle(ref WORLD_Z_AXIS, MathHelper.ToRadians(-1), out tilt);
            Quaternion.Concatenate(ref orientation, ref tilt, out orientation);
            tiltFix++;
        }

    #region Properties

        public bool Dead
        {
            get { return dead; }
            set { dead = value; }
        }

        public Vector3 Acceleration
        {
            get { return acceleration; }
            set { acceleration = value; }
        }

        public Posture CurrentPosture
        {
            get { return posture; }
        }

        public Vector3 CurrentVelocity
        {
            get { return currentVelocity; }
        }


        public float EyeHeightStanding
        {
            get { return eyeHeightStanding; }

            set
            {
                eyeHeightStanding = value;
                eyeHeightCrouching = value * HEIGHT_MULTIPLIER_CROUCHING;
				eyeHeightSliding = value * HEIGHT_MULTIPLIER_SLIDING;
                eye.Y = eyeHeightStanding;
                UpdateViewMatrix();
            }
        }

        public float HeadingDegrees
        {
            get { return -accumHeadingDegrees; }
        }

        public Quaternion Orientation
        {
            get { return orientation; }
        }

        public float PitchDegrees
        {
            get { return -accumPitchDegrees; }
        }

        public Vector3 Position
        {
            get { return eye; }

            set
            {
                eye = value;
                UpdateViewMatrix();
            }
        }

        public PositionUpdateCallback PositionUpdate
        {
            get { return posUpdate; }
            set { posUpdate = value; }
        }

        public Matrix ProjectionMatrix
        {
            get { return projMatrix; }
        }

        public float RotationSpeed
        {
            get { return rotationSpeed; }
            set { rotationSpeed = value; }
        }

        public Vector3 VelocityWalking
        {
            get { return velocityWalking; }
            set { velocityWalking = value; }
        }

        public Vector3 VelocityRunning
        {
            get { return velocityRunning; }
            set { velocityRunning = value; }
        }

		public Vector3 VelocitySliding
        {
            get { return velocitySliding; }
            set { velocitySliding = value; }
        }

        public Vector3 VelocityCrouching
        {
            get { return velocityCrouching; }
            set { velocityCrouching = value; }
        }

        public Vector3 ViewDirection
        {
            get { return viewDir; }
        }

        public Matrix ViewMatrix
        {
            get { return viewMatrix; }
        }

        public Matrix ViewProjectionMatrix
        {
            get { return viewMatrix * projMatrix; }
        }

        public Vector3 XAxis
        {
            get { return xAxis; }
        }

        public Vector3 YAxis
        {
            get { return yAxis; }
        }

        public Vector3 ZAxis
        {
            get { return zAxis; }
        }

        public bool isJumping()
        {
            if (posture == Posture.Jumping || (posture == Posture.Rising && prevPosture != Posture.Crouching))
            {
                return true;
            }
            return false;
        }

        public bool isCrouching()
        {
            if (posture == Posture.Crouching)
            {
                return true;
            }

            return false;
        }

        public SoundEffectInstance Walk
        {
            get { return walk; }
            set { walk = value; }
        }

        public SoundEffectInstance Slide
        {
            get { return slide; }
            set { slide = value; }
        }

        public bool IsSliding() { return isSliding; }
        public bool IsSprinting() { return isSprinting; }
        public bool IsClapping() { return isClapping; }

    #endregion
    }
}