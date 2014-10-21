using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Audio;

namespace Cluck
{
    public class AudioComponent : Component
    {
        private Dictionary<string, SoundEffectInstance> _soundEffects;

        private AudioEmitter _emitter;
        private AudioListener _listener;

        public AudioComponent()
            : base((int)component_flags.audio)
        {
            
            _soundEffects = new Dictionary<string, SoundEffectInstance>();
        }

        public AudioComponent(string[] names, SoundEffect[] sounds)
            : this(names, sounds, null, null)
        { }

        public AudioComponent(string[] names, SoundEffect[] sounds, AudioEmitter emitter, AudioListener listener)
            : base((int)component_flags.audio)
        {
            if (names.Length != sounds.Length)
                throw new ArgumentException("The names array must be the same length as the sounds array.");

            _soundEffects = new Dictionary<string, SoundEffectInstance>();
            for(int i = 0; i < sounds.Length; i++)
                _soundEffects.Add(names[i], sounds[i].CreateInstance());

            _emitter = emitter;
            _listener = listener;
        }

        public void Apply3DAll(AudioEmitter emitter, AudioListener listener)
        {
            if (emitter == null || listener == null)
                throw new ArgumentNullException("neither emitter nor listener can be null.");

            _emitter = emitter;
            _listener = listener;
            foreach (KeyValuePair<string, SoundEffectInstance> pair in _soundEffects)
                pair.Value.Apply3D(_listener, _emitter);
        }

        public Dictionary<string, SoundEffectInstance> GetSounds()
        {
            return _soundEffects;
        }

        public void AddSoundEffect(string name, SoundEffect sound)
        {
            if (sound == null)
                throw new ArgumentNullException("sound");

            _soundEffects.Add(name, sound.CreateInstance());
        }

        /// <summary>
        /// Add a sound effect to the list of effects contained in this audio component.
        /// </summary>
        /// <param name="name">The name for the sound effect.</param>
        /// <param name="sound">The SoundEffectInstance to add.</param>
        public void AddSoundEffect(string name, SoundEffectInstance sound)
        {
            if (sound == null)
                throw new ArgumentNullException("sound");

            _soundEffects.Add(name, sound);
        }
    }
}
