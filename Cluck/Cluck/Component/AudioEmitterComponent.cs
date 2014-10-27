using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;

namespace Cluck
{
    class AudioEmitterComponent : Component
    {
        private AudioEmitter _emitter;
        private SoundEffectInstance prevSound;

        public AudioEmitterComponent(SoundEffectInstance s)
            : base((int)component_flags.audioEmitter)
        {
            _emitter = new AudioEmitter();
            prevSound = s;
        }

        public void SetPosition(Vector3 pos)
        {
            _emitter.Position = pos;
        }

        public AudioEmitter GetEmitter()
        {
            return _emitter;
        }

        public Vector3 GetPostion()
        {
            return _emitter.Position;
        }

        public SoundEffectInstance GetPrevSound()
        {
            return prevSound;
        }

        public void SetPrevSound(SoundEffectInstance s)
        {
            prevSound = s;
        }

        public void SetVelocity(Vector3 vel)
        {
            _emitter.Velocity = vel;
        }

        public Vector3 GetVelocity()
        {
            return _emitter.Velocity;
        }
    }
}

