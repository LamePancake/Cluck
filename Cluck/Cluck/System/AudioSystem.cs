using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework;

namespace Cluck
{
    class AudioSystem : GameSystem
    {
        private SoundEffectInstance[] _sounds;
        private int cameraIndex;
        private Random r;

        public AudioSystem(SoundEffect[] sounds) 
            : base((int)component_flags.audioListener| (int)component_flags.audioEmitter)
        {
            _sounds = new SoundEffectInstance[sounds.Length];
            for (int i = 0; i < sounds.Length; i++)
            {
                _sounds[i] = sounds[i].CreateInstance();
            }

            cameraIndex = -1;
            r = new Random();
        }

        public void Update(List<GameEntity> world, float gameTime)
        {
            if (cameraIndex == -1)
            {
                for (int i = 0; i < world.Count; i++ )
                {
                    if (world[i].HasComponent((int)component_flags.camera))
                    {
                        cameraIndex = i;
                        break;
                    }
                }
            }

            if (cameraIndex != -1)
            {
                foreach (GameEntity g in world)
                {
                    if (g.HasComponent((int)component_flags.audioEmitter))
                    {
                        AudioEmitterComponent aec = g.GetComponent<AudioEmitterComponent>(component_flags.audioEmitter);
                        if (aec.GetPrevSound().State != SoundState.Playing)
                        {
                            int songIndex = r.Next(_sounds.Length);
                            AudioListenerComponent alc = world[cameraIndex].GetComponent<AudioListenerComponent>(component_flags.audioListener);
                            
                            Vector3 alcPos = world[cameraIndex].GetComponent<CameraComponent>(component_flags.camera).GetCamera().Position;
                            Vector3 aecPos = g.GetComponent<PositionComponent>(component_flags.position).GetPosition();
                        
                            //alc.SetVelocity(alcPos - alc.GetPostion());
                            //aec.SetVelocity(aecPos - aec.GetPostion());
                        
                            aec.SetPosition(aecPos);
                            alc.SetPosition(alcPos);
                            _sounds[songIndex].Apply3D(alc.GetListener(), aec.GetEmitter());

                            _sounds[songIndex].Play();
                            aec.SetPrevSound(_sounds[songIndex]);
                        }
                    }
                }
            }
        }
    }
}
