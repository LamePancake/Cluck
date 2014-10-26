using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace Cluck
{
    class GameEntity
    {
        private static ulong currentID = 0;

        private readonly ulong myID;
        private int componentFlags;
         
        public GameEntity()
        {
            myID = currentID++;
            componentFlags = (int)component_flags.none;
        }

        /// <summary>
        /// Adds a component to this entity.
        /// </summary>
        /// <param name="component">The component to add.</param>
        public void AddComponent(Component component)
        {
            ComponentLists.AddComponent(this.myID, component);
            componentFlags |= component.GetFlag();
        }


        /// <summary>
        /// Removes a component from this entity.
        /// </summary>
        /// <typeparam name="T">The component to be removed.</typeparam>
        public void RemoveComponent<T>(component_flags cFlag) where T : Component
        {
            if (HasComponent((int)cFlag))
            {
                ComponentLists.RemoveComponent(this.myID, (int)cFlag);
                componentFlags ^= (int)cFlag;
            }
        }

        /// <summary>
        /// Determines whether this entity has a particular component.
        /// </summary>
        /// <param name="cFlag">The type of component for which to check.</param>
        /// <returns></returns>
        public bool HasComponent(int cFlag)
        {
            return (componentFlags & cFlag) == cFlag; 
        }

        /// <summary>
        /// Gets a component of the specified type if this entity has one.
        /// </summary>
        /// <typeparam name="T">The type of component to get.</typeparam>
        /// <returns>The component of type T, if this entity has one.</returns>
        public T GetComponent<T>(component_flags cFlag) where T: Component
        {
            T comp = (T)ComponentLists.GetComponent(this.myID, (int)cFlag);
            return comp;
        }
    }
}
