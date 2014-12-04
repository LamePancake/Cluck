using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using System.Linq;
using System.Text;

namespace Cluck
{
    /// <summary>
    /// Represents a force applied to an object.
    /// </summary>
    public class Force
    {
        /// <summary>
        /// The direction and magnitude of the force to apply.
        /// </summary>
        public Vector3 DirectionMagnitude;

        /// <summary>
        /// The duration over which the force is to be applied in milliseconds.
        /// Set this to int.MaxValue to apply the force indefinitely.
        /// </summary>
        public int Duration;
        
        /// <summary>
        /// Constructs a new force with the specified direction, magnitude and duration.
        /// </summary>
        /// <param name="dirMag">Vector representing the direction and magnitude of the force in the 3 axes.</param>
        /// <param name="duration">The duration (in milliseconds) over which to apply the force. int.MaxValue indicates
        /// that the force should be applied indefinitely.</param>
        public Force(Vector3 dirMag, int duration)
        {
            this.DirectionMagnitude = dirMag;
            this.Duration = duration;
        }
    }
    
    /// <summary>
    /// Stores the physical attributes of a game entity. Objects with this component will be processed by the
    /// physics system.
    /// </summary>
    public class KinematicComponent : Component
    {
        /// <summary>
        /// The gravitational force affecting all bodies that are not grounded.
        /// This force cannot be removed.
        /// </summary>
        public static Force Gravity = new Force(new Vector3(0, -1000f, 0), int.MaxValue);

        /// <summary>
        /// The amount by which velocity will be dampened by "friction". Velocity will be multiplied by
        /// this value every frame before forces are applied.
        /// </summary>
        public static float LinearDamping = 0.99f;

        /// <summary>
        /// The maximum acceleration that can be applied in a frame to this object.
        /// </summary>
        public float MaxAcceleration;
        /// <summary>
        /// The maximum running speed of the given object (if it runs).
        /// </summary>
        public float MaxRunSpeed;
        /// <summary>
        /// The maximum walking speed of the given object (if it walks).
        /// </summary>
        public float MaxWalkSpeed;
        /// <summary>
        /// The maximum flying speed of the given object (if it flies).
        /// </summary>
        public float MaxFlySpeed;
        /// <summary>
        /// The maximum speed that this object can achieve in this frame.
        /// </summary>
        public float MaxSpeed;
        /// <summary>
        /// The maximum possible angular acceleration for this object under any circumstances.
        /// </summary>
        public float MaxAngularAcceleration;
        /// <summary>
        /// The maximum angle (in radians) that the object may rotate in one frame.
        /// </summary>
        public float MaxRotation;

        /// <summary>
        /// The object's current rotation about its centre.
        /// </summary>
        public float Rotation;
        /// <summary>
        /// The object's current velocity.
        /// </summary>
        public Vector3 Velocity;

        /// <summary>
        /// The object's mass.
        /// </summary>
        public float Mass;

        /// <summary>
        /// The current direction which the object is facing.
        /// </summary>
        public Vector3 Heading;
        /// <summary>
        /// The vector parallel to the XZ plane and perpendicular to the heading.
        /// </summary>
        public Vector3 Side;

        /// <summary>
        /// Indicates whether the object is on the ground.
        /// </summary>
        public bool IsGrounded = true;

        /// <summary>
        /// All forces currently applied to this object.
        /// </summary>
        public List<Force> Forces;

        /// <summary>
        /// Constructs a kinematic component with the specified maximum values.
        /// </summary>
        /// <param name="maxAccel">The maximum acceleration that may be applied to this object in a frame.</param>
        /// <param name="maximumSpeed">The initial maximum speed for this object.</param>
        /// <param name="maximumRotation">The maximum rotation in radians as described above.</param>
        /// <param name="maxAngularAccel">The maximum angular acceleration as described above.</param>
        public KinematicComponent(float maxAccel, float maximumSpeed, float maximumRotation, float maxAngularAccel, float mass) 
            : base((int)component_flags.kinematic)
        {
            MaxAcceleration = maxAccel;
            MaxSpeed = maximumSpeed;
            MaxRotation = maximumRotation;
            MaxAngularAcceleration = maxAngularAccel;

            MaxWalkSpeed = maximumSpeed;
            MaxRunSpeed = maximumSpeed * 2.5f;
            MaxFlySpeed = MaxRunSpeed;

            Velocity = Vector3.Zero;
            Heading = new Vector3(1,0,0);
            Side = Util.PerpInZPlane(Heading);
            Rotation = 0;

            Mass = mass;
            Forces = new List<Force>();
        }
    }
}
