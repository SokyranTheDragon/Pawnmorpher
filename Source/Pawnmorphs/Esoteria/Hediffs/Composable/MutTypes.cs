﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using JetBrains.Annotations;
using Verse;

namespace Pawnmorph.Hediffs.Composable
{
    /// <summary>
    /// A class that determines which mutations to add
    /// </summary>
    public abstract class MutTypes
    {
        /// <summary>
        /// The epsilon for chance comparison.
        /// </summary>
        protected const float EPSILON = 0.000001f;

        /// <summary>
        /// Gets the list of available mutations.
        /// </summary>
        /// <returns>The mutations.</returns>
        /// <param name="hediff">Hediff.</param>
        public abstract IEnumerable<MutationEntry> GetMutations(Hediff_MutagenicBase hediff);

        /// <summary>
        /// Chechs whether this MutTypes is equivalent to another
        /// (meaning they produce the same list of mutations)
        /// </summary>
        /// <returns><c>true</c>, if to was equivalented, <c>false</c> otherwise.</returns>
        /// <param name="other">The other MutTypes.</param>
        public abstract bool EquivalentTo(MutTypes other);

        /// <summary>
        /// A debug string printed out when inspecting the hediffs
        /// </summary>
        /// <param name="hediff">The parent hediff.</param>
        /// <returns>The string.</returns>
        public virtual string DebugString(Hediff_MutagenicBase hediff) => "";
    }

    /// <summary>
    /// A simple MutTypes that returns ALL THE MUTATIONS _O/
    /// Good for chaotic mutations.
    /// </summary>
    public class MutTypes_All : MutTypes
    {
        /// <summary>
        /// The chance any particular mutation will be added (as a multiplier of the default chance).
        /// </summary>
        [UsedImplicitly] public float chance = 0.1f; // Low chance by default since this is for chaotic mutations

        /// <summary>
        /// Whether or not restricted mutations can be selected
        /// </summary>
        [UsedImplicitly] public bool allowRestricted = false;

        /// <summary>
        /// Gets the list of available mutations.
        /// </summary>
        /// <returns>The mutations.</returns>
        /// <param name="hediff">Hediff.</param>
        public override IEnumerable<MutationEntry> GetMutations(Hediff_MutagenicBase hediff)
        {
            return DefDatabase<MutationDef>.AllDefs
                    .Where(m => allowRestricted || !m.IsRestricted)
                    .Select(m => MutationEntry.FromMutation(m, chance));
        }

        /// <summary>
        /// Chechs whether this MutTypes is equivalent to another
        /// (meaning they produce the same list of mutations)
        /// </summary>
        /// <returns><c>true</c>, if to was equivalented, <c>false</c> otherwise.</returns>
        /// <param name="other">The other MutTypes.</param>
        public override bool EquivalentTo(MutTypes other)
        {
            return other is MutTypes_All otherAll
                    && Math.Abs(chance - otherAll.chance) < EPSILON;
        }

        /// <summary>
        /// A debug string printed out when inspecting the hediffs
        /// </summary>
        /// <param name="hediff">The parent hediff.</param>
        /// <returns>The string.</returns>
        public override string DebugString(Hediff_MutagenicBase hediff) => $"Chance: {chance.ToStringPercent()}";
    }

    /// <summary>
    /// A simple MutTypes that accepts a list of mutations directly from the XML
    /// </summary>
    public class MutTypes_List : MutTypes
    {
        /// <summary>
        /// The list of mutations to add.
        /// </summary>
        [UsedImplicitly] public List<MutationDef> mutations;

        /// <summary>
        /// The chance any particular mutation will be added (as a multiplier of the default chance).
        /// </summary>
        [UsedImplicitly] public float chance = 1f;

        /// <summary>
        /// Gets the list of available mutations.
        /// </summary>
        /// <returns>The mutations.</returns>
        /// <param name="hediff">Hediff.</param>
        public override IEnumerable<MutationEntry> GetMutations(Hediff_MutagenicBase hediff)
        {
            return mutations.Select(m => MutationEntry.FromMutation(m, chance));
        }

        /// <summary>
        /// Chechs whether this MutTypes is equivalent to another
        /// (meaning they produce the same list of mutations)
        /// </summary>
        /// <returns><c>true</c>, if to was equivalented, <c>false</c> otherwise.</returns>
        /// <param name="other">The other MutTypes.</param>
        public override bool EquivalentTo(MutTypes other)
        {
            return other is MutTypes_List otherList
                    && mutations.Equals(otherList.mutations)
                    && Math.Abs(chance - otherList.chance) < EPSILON;
        }

        /// <summary>
        /// A debug string printed out when inspecting the hediffs
        /// </summary>
        /// <param name="hediff">The parent hediff.</param>
        /// <returns>The string.</returns>
        public override string DebugString(Hediff_MutagenicBase hediff) => $"Chance: {chance.ToStringPercent()}";
    }

    /// <summary>
    /// A simple MutTypes that selects all mutations from a morph def
    /// </summary>
    public class MutTypes_Morph : MutTypes
    {
        /// <summary>
        /// The morph def to select mutations from.
        /// </summary>
        [UsedImplicitly] public MorphDef morphDef;

        /// <summary>
        /// The chance any particular mutation will be added (as a multiplier of the default chance).
        /// </summary>
        [UsedImplicitly] public float chance = 1f;

        /// <summary>
        /// Gets the list of available mutations.
        /// </summary>
        /// <returns>The mutations.</returns>
        /// <param name="hediff">Hediff.</param>
        public override IEnumerable<MutationEntry> GetMutations(Hediff_MutagenicBase hediff)
        {
            return morphDef.AllAssociatedMutations
                    .Select(m => MutationEntry.FromMutation(m, chance));
        }

        /// <summary>
        /// Chechs whether this MutTypes is equivalent to another
        /// (meaning they produce the same list of mutations)
        /// </summary>
        /// <returns><c>true</c>, if to was equivalented, <c>false</c> otherwise.</returns>
        /// <param name="other">The other MutTypes.</param>
        public override bool EquivalentTo(MutTypes other)
        {
            return other is MutTypes_Morph otherMorph
                    && morphDef.Equals(otherMorph.morphDef)
                    && Math.Abs(chance - otherMorph.chance) < EPSILON;
        }

        /// <summary>
        /// A debug string printed out when inspecting the hediffs
        /// </summary>
        /// <param name="hediff">The parent hediff.</param>
        /// <returns>The string.</returns>
        public override string DebugString(Hediff_MutagenicBase hediff)
        {
            StringBuilder builder = new StringBuilder();
            builder.AppendLine($"MorphDef: {morphDef.defName}");
            builder.AppendLine($"Chance: {chance.ToStringPercent()}");
            return builder.ToString();
        }
    }

    /// <summary>
    /// A simple MutTypes that selects all mutations from a class (including child classes)
    /// </summary>
    public class MutTypes_Class : MutTypes
    {
        /// <summary>
        /// The class def to select mutations from
        /// </summary>
        [UsedImplicitly] public AnimalClassDef classDef;

        /// <summary>
        /// The chance any particular mutation will be added (as a multiplier of the default chance).
        /// </summary>
        [UsedImplicitly] public float chance = 1f;

        /// <summary>
        /// Gets the list of available mutations.
        /// </summary>
        /// <returns>The mutations.</returns>
        /// <param name="hediff">Hediff.</param>
        public override IEnumerable<MutationEntry> GetMutations(Hediff_MutagenicBase hediff)
        {
            return classDef.GetAllMutationIn()
                    .Select(m => MutationEntry.FromMutation(m, chance));
        }

        /// <summary>
        /// Chechs whether this MutTypes is equivalent to another
        /// (meaning they produce the same list of mutations)
        /// </summary>
        /// <returns><c>true</c>, if to was equivalented, <c>false</c> otherwise.</returns>
        /// <param name="other">The other MutTypes.</param>
        public override bool EquivalentTo(MutTypes other)
        {
            return other is MutTypes_Class otherClass
                    && classDef.Equals(otherClass.classDef)
                    && Math.Abs(chance - otherClass.chance) < EPSILON;
        }

        /// <summary>
        /// A debug string printed out when inspecting the hediffs
        /// </summary>
        /// <param name="hediff">The parent hediff.</param>
        /// <returns>The string.</returns>
        public override string DebugString(Hediff_MutagenicBase hediff)
        {
            StringBuilder builder = new StringBuilder();
            builder.AppendLine($"ClassDef: {classDef.defName}");
            builder.AppendLine($"Chance: {chance.ToStringPercent()}");
            return builder.ToString();
        }
    }

    /// <summary>
    /// A MutTypes that selects mutations defined in HediffComp_MutagenicTypes
    /// 
    /// Most "dynamic" hediffs that want to share mutation data across stages will
    /// want to use this MutTypes, as MutTypes are stateless.
    /// </summary>
    public class MutTypes_FromComp : MutTypes
    {
        /// <summary>
        /// The chance any particular mutation will be added (as a multiplier of the default chance).
        /// </summary>
        [UsedImplicitly] public float chance = 1f;

        /// <summary>
        /// Gets the list of available mutations.
        /// </summary>
        /// <returns>The mutations.</returns>
        /// <param name="hediff">Hediff.</param>
        public override IEnumerable<MutationEntry> GetMutations(Hediff_MutagenicBase hediff)
        {
            return hediff.TryGetComp<HediffComp_MutTypeBase>()
                    .GetMutations()
                    .Select(m => MutationEntry.FromMutation(m, chance));
        }

        /// <summary>
        /// Chechs whether this MutTypes is equivalent to another
        /// (meaning they produce the same list of mutations)
        /// </summary>
        /// <returns><c>true</c>, if to was equivalented, <c>false</c> otherwise.</returns>
        /// <param name="other">The other MutTypes.</param>
        public override bool EquivalentTo(MutTypes other)
        {
            return other is MutTypes_FromComp otherComp
                    && Math.Abs(chance - otherComp.chance) < EPSILON;
        }

        /// <summary>
        /// A debug string printed out when inspecting the hediffs
        /// </summary>
        /// <param name="hediff">The parent hediff.</param>
        /// <returns>The string.</returns>
        public override string DebugString(Hediff_MutagenicBase hediff) => $"Chance: {chance.ToStringPercent()}";
    }

    /// <summary>
    ///     mut type that picks from mutation categories
    /// </summary>
    /// <seealso cref="Pawnmorph.Hediffs.Composable.MutTypes" />
    public class MutTypes_Category : MutTypes
    {
        /// <summary>
        ///     The category to chose from
        /// </summary>
        public MutationCategoryDef category;


        private List<MutationEntry> _cachedEntries;


        [NotNull]
        private IReadOnlyList<MutationEntry> CachedEntries
        {
            get
            {
                if (_cachedEntries == null)
                    _cachedEntries = category.AllMutations.Where(m => m.RestrictionLevel < category.restrictionLevel)
                                             .Select(m => MutationEntry.FromMutation(m))
                                             .ToList();

                return _cachedEntries;
            }
        }


        /// <summary>
        ///     Gets the list of available mutations.
        /// </summary>
        /// <returns>The mutations.</returns>
        /// <param name="hediff">Hediff.</param>
        public override IEnumerable<MutationEntry> GetMutations(Hediff_MutagenicBase hediff)
        {
            return CachedEntries;
        }

        /// <summary>
        /// A debug string printed out when inspecting the hediffs
        /// </summary>
        /// <param name="hediff">The parent hediff.</param>
        /// <returns>The string.</returns>
        public override string DebugString(Hediff_MutagenicBase hediff)
        {
            return $"choosing from {category.defName}"; 
        }

        /// <summary>
        ///     Chechs whether this MutTypes is equivalent to another
        ///     (meaning they produce the same list of mutations)
        /// </summary>
        /// <returns><c>true</c>, if to was equivalented, <c>false</c> otherwise.</returns>
        /// <param name="other">The other MutTypes.</param>
        public override bool EquivalentTo(MutTypes other)
        {
            if (other == this) return true;
            if (other == null) return false;
            if (!(other is MutTypes_Category oCat)) return false;
            return oCat.category == category; 
        }
    }
}
