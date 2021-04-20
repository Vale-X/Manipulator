using R2API;
using ManipulatorMod.SkillStates;
using ManipulatorMod.SkillStates.BaseStates;
using ManipulatorMod.SkillStates.Emotes;
using System.Collections.Generic;
using System;
using MonoMod.RuntimeDetour;
using EntityStates;
using RoR2;
using System.Reflection;

namespace ManipulatorMod.Modules
{
    public static class States
    {
        internal static List<Type> entityStates = new List<Type>();

        private static Hook set_stateTypeHook;
        private static Hook set_typeNameHook;
        private static readonly BindingFlags allFlags = BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static | BindingFlags.NonPublic;
        private delegate void set_stateTypeDelegate(ref SerializableEntityStateType self, Type value);
        private delegate void set_typeNameDelegate(ref SerializableEntityStateType self, String value);

        internal static void RegisterStates()
        {
            Type type = typeof(SerializableEntityStateType);
            HookConfig cfg = default;
            cfg.Priority = Int32.MinValue;
            set_stateTypeHook = new Hook(type.GetMethod("set_stateType", allFlags), new set_stateTypeDelegate(SetStateTypeHook), cfg);
            set_typeNameHook = new Hook(type.GetMethod("set_typeName", allFlags), new set_typeNameDelegate(SetTypeName), cfg);

            entityStates.Add(typeof(BaseManipulatorSkillState));

            entityStates.Add(typeof(ManipulatorMain));

            entityStates.Add(typeof(BaseEmote));
            entityStates.Add(typeof(Rest));
            entityStates.Add(typeof(Dance));

            entityStates.Add(typeof(BaseMeleeAttack));
            entityStates.Add(typeof(ElementalSlashState));

            entityStates.Add(typeof(ElementalSpellState));

            entityStates.Add(typeof(ElementalBlinkState));

            entityStates.Add(typeof(ElementalSwitchState));
        }

        private static void SetStateTypeHook(ref this SerializableEntityStateType self, Type value)
        {
            self._typeName = value.AssemblyQualifiedName;
        }

        private static void SetTypeName(ref this SerializableEntityStateType self, String value)
        {
            Type t = GetTypeFromName(value);
            if (t != null)
            {
                self.SetStateTypeHook(t);
            }
        }

        private static Type GetTypeFromName(String name)
        {
            Type[] types = EntityStateCatalog.stateIndexToType;
            return Type.GetType(name);
        }
    }
}