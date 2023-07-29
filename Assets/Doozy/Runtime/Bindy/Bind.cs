// Copyright (c) 2015 - 2023 Doozy Entertainment. All Rights Reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

using System;
using System.Collections.Generic;
using System.Linq;
using Doozy.Runtime.Common;
using Doozy.Runtime.Common.Attributes;
using UnityEngine;
// ReSharper disable UnusedMember.Global
// ReSharper disable UnusedTypeParameter
// ReSharper disable UnusedMethodReturnValue.Global
// ReSharper disable MemberCanBePrivate.Global

namespace Doozy.Runtime.Bindy
{
    /// <summary>
    /// Represents a bind, which is a collection of one or more <see cref="Bindable"/> objects that can be bound to each other to synchronize their values.
    /// </summary>
    public partial class Bind
    {
        [ExecuteOnReload]
        // ReSharper disable once UnusedMember.Local
        private void OnReload()
        {
            binds ??= new HashSet<Bind>();
            binds.Clear();

            m_Bindables ??= new HashSet<Bindable>();
            m_Bindables.Clear();

            m_BindableGuids ??= new HashSet<Guid>();
            m_BindableGuids.Clear();
        }

        /// <summary>
        /// Database of all the Bind objects that have been created by the Bindy system in the current session
        /// </summary>
        private static HashSet<Bind> binds { get; set; } = new();

        /// <summary> The unique identifier for this bind, which consists of a category and a name. </summary>
        public BindId id { get; }

        /// <summary> The unique identifier for this bind, which is a globally unique identifier. </summary>
        public Guid guid { get; }

        private HashSet<Bindable> m_Bindables;
        /// <summary> The list of bindables that are currently part of this bind. </summary>
        private HashSet<Bindable> bindables => m_Bindables ?? (m_Bindables = new HashSet<Bindable>());

        private HashSet<Guid> m_BindableGuids;
        /// <summary> The list of bindable GUIDs that are currently part of this bind. </summary>
        private HashSet<Guid> bindableGuids => m_BindableGuids ?? (m_BindableGuids = new HashSet<Guid>());

        /// <summary>
        /// The last bindable that set a value in this bind.
        /// This is the one that set the lastValue.
        /// </summary>
        public Bindable lastBindable { get; private set; }

        /// <summary>
        /// The last value that was set by one of the bindables in this bind.
        /// </summary>
        public object lastValue { get; private set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="Bind"/> class with the default category and name.
        /// </summary>
        public Bind() : this(new BindId(CategoryNameId.defaultCategory, CategoryNameId.defaultName))
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Bind"/> class with the specified category and name.
        /// </summary>
        /// <param name="category"> The category of the bind </param>
        /// <param name="name"> The name of the bind </param>
        public Bind(string category, string name) : this(new BindId(category, name))
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Bind"/> class with the specified identifier.
        /// </summary>
        /// <param name="id">The identifier for the bind.</param>
        // ReSharper disable once SuggestBaseTypeForParameterInConstructor
        public Bind(BindId id)
        {
            this.id = new BindId(id.Category, id.Name);
            guid = Guid.NewGuid();
        }

        /// <summary>
        /// Notifies all bindables in this bind that the value of the specified bindable has changed.
        /// </summary>
        /// <param name="changedBindableGuid"> The GUID of the bindable that has been changed </param>
        public void NotifyChange(Guid changedBindableGuid)
        {
            if (changedBindableGuid == Guid.Empty)
                return; // The bindable that triggered the notification is not part of this bind

            if (!bindableGuids.Contains(changedBindableGuid))
                return; // The bindable that triggered the notification is not part of this bind

            if (bindables.FirstOrDefault(b => b.guid == changedBindableGuid) is not {} changedBindable)
                return; // The bindable that triggered the notification is not part of this bind

            if (changedBindable.valueType == null)
            {
                Debug.LogWarning($"[Bindy] Bind '{id}' has a bindable with value type 'null' - this is not allowed");
                return;
            }

            // ReSharper disable once ForeachCanBePartlyConvertedToQueryUsingAnotherGetEnumerator
            foreach (Bindable bindable in bindables)
            {
                if (bindable == changedBindable) continue; // Skip the bindable that triggered the notification
                ConnectionType connection = bindable.connectionType;
                if (connection == ConnectionType.Sender) continue; // Skip the bindable if it is a source bindable
                Bindable.ProcessValue(this, changedBindable, bindable);
            }

            lastBindable = changedBindable;
            lastValue = changedBindable.value;
        }

        /// <summary>
        /// Adds the specified <see cref="Bindable"/> object to this bind.
        /// </summary>
        /// <param name="bindable"> The bindable to add to the bind </param>
        /// <returns> The bind </returns>
        public Bind AddBindable(Bindable bindable)
        {
            if (bindable == null)
                return this;

            if(bindable.bind != null && bindable.bind != this)
                bindable.bind.RemoveBindable(bindable); 

            if (!bindableGuids.Contains(bindable.guid))
                bindableGuids.Add(bindable.guid);

            if (!bindables.Contains(bindable))
                bindables.Add(bindable);

            bindable.bind = this;
            bindable.RunOnBindBehavior();
            return this;
        }

        /// <summary>
        /// Adds the specified <see cref="Bindable"/> objects to this bind.
        /// </summary>
        /// <param name="bindableA"> The first bindable to add to the bind </param>
        /// <param name="bindableB"> The second bindable to add to the bind </param>
        /// <returns> The bind </returns>
        public Bind AddBindables(Bindable bindableA, Bindable bindableB)
        {
            AddBindable(bindableA);
            AddBindable(bindableB);
            return this;
        }

        /// <summary>
        /// Adds the specified <see cref="Bindable"/> objects to this bind.
        /// </summary>
        /// <param name="bindablesToAdd"> The bindables to add to the bind </param>
        /// <returns> The bind </returns>
        public Bind AddBindables(params Bindable[] bindablesToAdd)
        {
            foreach (Bindable bindable in bindablesToAdd)
                AddBindable(bindable);
            return this;
        }

        /// <summary>
        /// Adds the specified <see cref="Bindable"/> objects to this bind.
        /// </summary>
        /// <param name="bindablesToAdd"> The bindables to add to the bind </param>
        /// <typeparam name="T"> The type of the bindables </typeparam>
        /// <returns> The bind </returns>
        public Bind AddBindables<T>(params Bindable[] bindablesToAdd)
        {
            foreach (Bindable bindable in bindablesToAdd)
                AddBindable(bindable);
            return this;
        }

        /// <summary>
        /// Removes the specified <see cref="Bindable"/> object from this bind.
        /// </summary>
        /// <param name="bindable">The bindable to remove from the bind.</param>
        /// <returns> The bind </returns>
        public Bind RemoveBindable(Bindable bindable)
        {
            if (bindable == null)
                return this;

            if (bindableGuids.Contains(bindable.guid))
                bindableGuids.Remove(bindable.guid);

            if (bindables.Contains(bindable))
                bindables.Remove(bindable);

            if (bindable.bind == this)
                bindable.bind = null;

            return this;
        }

        /// <summary>
        /// Removes the <see cref="Bindable"/> object with the specified GUID from this bind.
        /// </summary>
        /// <param name="bindableGuid"> The GUID of the bindable to remove from the bind </param>
        /// <returns> The bind </returns>
        public Bind RemoveBindable(Guid bindableGuid)
        {
            Bindable b = bindables.FirstOrDefault(b => b.guid == bindableGuid);

            return
                b == null
                    ? this
                    : RemoveBindable(b);
        }

        /// <summary>
        /// Removes the <see cref="Bindable"/> object with the specified GUID from this bind.
        /// </summary>
        /// <param name="bindableGuid"> The GUID of the bindable to remove from the bind </param>
        /// <returns> The bind </returns>
        public Bind RemoveBindable(string bindableGuid) =>
            RemoveBindable(new Guid(bindableGuid));

        /// <summary>
        /// Removes the specified <see cref="Bindable"/> objects from this bind.
        /// </summary>
        /// <param name="bindablesToRemove"> The bindables to remove from the bind </param>
        /// <returns> The bind </returns>
        public Bind RemoveBindables(params Bindable[] bindablesToRemove)
        {
            foreach (Bindable bindable in bindablesToRemove)
                RemoveBindable(bindable);
            return this;
        }

        /// <summary>
        /// Removes the specified <see cref="Bindable"/> objects with the specified GUIDs from this bind.
        /// </summary>
        /// <param name="bindableGuidsToRemove"> The GUIDs of the bindables to remove from the bind </param>
        /// <returns> The bind </returns>
        public Bind RemoveBindables(params Guid[] bindableGuidsToRemove)
        {
            foreach (Guid bindableGuid in bindableGuidsToRemove)
                RemoveBindable(bindableGuid);
            return this;
        }

        /// <summary>
        /// Removes the specified <see cref="Bindable"/> objects with the specified GUIDs from this bind.
        /// </summary>
        /// <param name="bindableGuidsToRemove"> The GUIDs of the bindables to remove from the bind </param>
        /// <returns> The bind </returns>
        public Bind RemoveBindables(params string[] bindableGuidsToRemove)
        {
            foreach (string bindableGuid in bindableGuidsToRemove)
                RemoveBindable(bindableGuid);
            return this;
        }

           #region Contains Methods

        /// <summary>
        /// Checks if Binds database contains the given Bind.
        /// </summary>
        /// <param name="guid"> Guid to check </param>
        /// <returns> True if the Binds database contains the given Bind </returns>
        public static bool ContainsBind(Guid guid)
        {
            if (guid == Guid.Empty) return false;
            binds.Remove(null);
            return binds.Any(bind => bind.guid == guid);
        }

        /// <summary>
        /// Checks if the Binds database contains a Bind with the given Guid.
        /// </summary>
        /// <param name="guid"> Guid to check </param>
        /// <returns> True if the Binds database contains a Bind with the given Guid </returns>
        public static bool ContainsBind(string guid) =>
            ContainsBind(new Guid(guid));

        /// <summary>
        /// Checks if the Binds database contains a Bind with the given BindId.
        /// Note that a null BindId or if the BindId uses the default BindId will return false.
        /// </summary>
        /// <param name="id"> BindId to check </param>
        /// <returns> True if the Binds database contains a Bind with the given BindId </returns>
        public static bool ContainsBind(BindId id)
        {
            if (id == null) return false;
            if (id.isDefaultId) return false;
            binds.Remove(null);
            return binds.Any(bind => bind.id == id);
        }

        /// <summary>
        /// Checks if the Binds database contains a Bind with the given category and name.
        /// If the category and name match the default BindId, this method will return false.
        /// </summary>
        /// <param name="category"> Category to check </param>
        /// <param name="name"> Name to check </param>
        /// <returns> True if the Binds database contains a Bind with the given category and name </returns>
        public static bool ContainsBind(string category, string name) =>
            ContainsBind(new BindId(category, name));

        #endregion

        /// <summary>
        /// Returns a new Bind with the default BindId.
        /// </summary>
        /// <returns> New Bind of the given type, with the default BindId </returns>
        public static Bind GetBind() =>
            GetBind(null);

        /// <summary>
        /// Returns the Bind with the given BindId.
        /// If the BindId is null or uses the default BindId, a new Bind will be created and returned.
        /// </summary>
        /// <param name="id"> BindId to check </param>
        /// <returns> The Bind with the given BindId </returns>
        public static Bind GetBind(BindId id)
        {
            Bind newBind;
            id ??= new BindId();
            if (id.isDefaultId)
            {
                //create a new Bind with the default BindId, but with a new Guid and return it
                newBind = new Bind(id);
                binds.Add(newBind);
                return newBind;
            }

            //check if the Bindy database contains a Bind with the given BindId and return it
            foreach (Bind bind in binds.Where(bind => bind.id == id))
                return bind;

            //no Bind with the given BindId was found, so create a new one and return it
            newBind = new Bind(id);
            binds.Add(newBind);
            return newBind;
        }

        /// <summary>
        /// Gets the Bind with the given category and name from the Bindy database.
        /// If one is not found, it will create a new one and add it to the database.
        /// </summary>
        /// <param name="category"> Category of the Bind </param>
        /// <param name="name"> Name of the Bind </param>
        /// <returns> The Bind with the given category and name </returns>
        public static Bind GetBind(string category, string name) =>
            GetBind(new BindId(category, name));

        /// <summary>
        /// Adds a new Bind to the Bindy database 
        /// </summary>
        /// <param name="bind"> Bind to add </param>
        public static (bool, string) AddBind(Bind bind)
        {
            if (bind == null) return (false, "Can't add a null bind");
            if (binds.Contains(bind)) return (false, $"Bind '{bind.id}' already exists");
            binds.Add(bind);
            return (true, $"Bind '{bind.id}' added");
        }

        /// <summary>
        /// Removes a Bind from the Bindy database
        /// </summary>
        /// <param name="bind"> Bind to remove </param>
        public static (bool, string) RemoveBind(Bind bind)
        {
            if (bind == null) return (false, "Can't remove a null bind");
            if (!binds.Contains(bind)) return (false, $"Bind '{bind.id}' does not exist");
            binds.Remove(bind);
            return (true, $"Bind '{bind.id}' removed");
        }
    }
}
