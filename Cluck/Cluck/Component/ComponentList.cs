using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;

namespace Cluck
{
    /// <summary>
    /// Holds lists of all components and their associated entities.
    /// </summary>
    public class ComponentLists
    {
        /// <summary>
        /// An array containing a list for each type of component.
        /// </summary>
        private static Dictionary<ulong, Component>[] lists;

        /// <summary>
        /// This static contstructor initialises the component lists.
        /// </summary>
        static ComponentLists()
        {
            FieldInfo[] info = typeof(component_flags).GetFields(BindingFlags.Static | BindingFlags.Public);
            int size = info.Length;

            // size - 1 because we don't need a list to hold the "none" components of each entity
            size -= 1;

            lists = new Dictionary<ulong, Component>[size];

            for (int i = 0; i < size; i++)
                lists[i] = new Dictionary<ulong, Component>();
        }

        /// <summary>
        /// Gets a component, if any, for the specified entity.
        /// </summary>
        /// <param name="entityID">The ID of the entity to whom the component belongs.</param>
        /// <param name="componentFlag">The flag identifying the component to get.</param>
        /// <returns>The Component corresponding to the given entity ID and component type. Returns null if there is no associated component.</returns>
        public static Component GetComponent(ulong entityID, int componentFlag)
        {
            int whichList = GetListIndex(componentFlag);
            Component c;

            if (lists[whichList].TryGetValue(entityID, out c))
                return c;

            return null;
        }

        /// <summary>
        /// Sets a Component for the given entityID and component type.
        /// </summary>
        /// <param name="entityID">The ID of the entity to whom the component belongs.</param>
        /// <param name="componentFlag">The flag identifying the component to set.</param>
        public static void AddComponent(ulong entityID, Component component)
        {
            int whichList = GetListIndex(component.GetFlag());
            lists[whichList].Add(entityID, component);
        }

        /// <summary>
        /// Gets a component, if any, for the specified entity.
        /// </summary>
        /// <param name="entityID">The ID of the entity to whom the component belongs.</param>
        /// <param name="componentFlag">The flag identifying the component to remove.</param>
        public static void RemoveComponent(ulong entityID, int componentFlag)
        {
            int whichList = GetListIndex(componentFlag);
            lists[whichList].Remove(entityID);
        }

        /// <summary>
        /// Gets the Dictionary mapping entity ID's to Components for a given component type.
        /// 
        /// OR'ing flags together will return the component type with the highest number and is
        /// a bad idea. Passing component_flags.none will return null.
        /// </summary>
        /// <param name="flag">The flag corresponding to the component list to retrieve.</param>
        /// <returns>A dicitionary mapping entity ID's to components, or null if flag is
        /// component_flags.none.</returns>
        public static Dictionary<ulong, Component> GetList(component_flags flag)
        {
            int whichList;
            if (flag == component_flags.none)
                return null;

            whichList = GetListIndex((int)flag);
            return lists[whichList];
        }

        /// <summary>
        /// Clears all component lists.
        /// </summary>
        public static void ClearLists()
        {
            foreach (Dictionary<ulong, Component> d in lists)
            {
                d.Clear();
            }
        }

        /// <summary>
        /// Get the index to the entity-component map for a given component type.
        /// </summary>
        /// <param name="componentFlag">The flag indicating which component list to get.</param>
        /// <returns>The index to the entity-component map.</returns>
        private static int GetListIndex(int componentFlag)
        {
            int whichList = 0;
            while (componentFlag != 0)
            {
                componentFlag >>= 1;
                whichList++;
            }

            // Subtract 1 for a zero-based array, and another 1
            // because whichList will get incremented once extra when the component becomes 0
            return whichList - 2;
        }
    }
}
