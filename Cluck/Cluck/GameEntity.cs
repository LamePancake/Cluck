using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace Cluck
{
    class GameEntity
    {
        private int componentFlags;
        private List<Component> components;
         
        public GameEntity()
        {
            componentFlags = (int)component_flags.none;
            components = new List<Component>();
        }

        public void AddComponent(Component component)
        {
            components.Add(component);
            componentFlags |= component.GetFlag();
        }

        public void RemoveComponent(Component component)
        {
            if (HasComponent(component.GetFlag()))
            {
                components.Remove(component);
                componentFlags ^= component.GetFlag();
            }
        }

        public bool HasComponent(int cFlag)
        {
            return (componentFlags & cFlag) == cFlag; 
        }

        public T GetComponent<T>() where T: Component
        {
            foreach(Component c in components)
            {
                if (c is T)
                    return (T)c;
            }
            return null;
        }
    }
}
