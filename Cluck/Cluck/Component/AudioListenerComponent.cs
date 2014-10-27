using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;

namespace Cluck
{
    class AudioListenerComponent : Component
    {
        private AudioListener _listener;

        public AudioListenerComponent()
            : base((int)component_flags.audioListener)
        {
            _listener = new AudioListener();
        }

        public void SetPosition(Vector3 pos)
        {
            _listener.Position = pos;
        }

        public AudioListener GetListener()
        {
            return _listener;
        }

        public Vector3 GetPostion()
        {
            return _listener.Position;
        }

        public void SetVelocity(Vector3 vel)
        {
            _listener.Velocity = vel;
        }

        public Vector3 GetVelocity()
        {
            return _listener.Velocity;
        }
    }
}

