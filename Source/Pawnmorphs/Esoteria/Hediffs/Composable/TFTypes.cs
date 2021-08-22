﻿using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using Verse;

namespace Pawnmorph.Hediffs.Composable
{
    /// <summary>
    /// A class that determines what kind(s) of animals a pawn can be transformed into
    /// </summary>
    public abstract class TFTypes
    {
        /// <summary>
        /// Gets the list of available pawnkinds to TF into.
        /// </summary>
        /// <returns>The mutations.</returns>
        /// <param name="hediff">Hediff.</param>
        public abstract IEnumerable<PawnKindDef> GetTFs(Hediff_MutagenicBase hediff);

        /// <summary>
        /// Chechs whether this TFTypes is equivalent to another
        /// (meaning they produce the same list of TFs)
        /// </summary>
        /// <returns><c>true</c>, if to was equivalented, <c>false</c> otherwise.</returns>
        /// <param name="other">The other TFTypes.</param>
        public abstract bool EquivalentTo(TFTypes other);
    }

    /// <summary>
    /// A simple TFTypes that returns ALL THE ANIMALS _O/
    /// Good for chaotic mutations.
    /// </summary>
    public class TFTypes_All : TFTypes
    {
        /// <summary>
        /// Gets the list of available pawnkinds to TF into.
        /// </summary>
        /// <returns>The mutations.</returns>
        /// <param name="hediff">Hediff.</param>
        public override IEnumerable<PawnKindDef> GetTFs(Hediff_MutagenicBase hediff)
        {
            return DefDatabase<PawnKindDef>.AllDefs.Where(p => p.RaceProps.Animal);
        }

        /// <summary>
        /// Chechs whether this TFTypes is equivalent to another
        /// (meaning they produce the same list of TFs)
        /// </summary>
        /// <returns><c>true</c>, if to was equivalented, <c>false</c> otherwise.</returns>
        /// <param name="other">The other TFTypes.</param>
        public override bool EquivalentTo(TFTypes other)
        {
            return other is TFTypes_All;
        }
    }

    /// <summary>
    /// A simple TFTypes that accepts a list of animals directly from the XML
    /// </summary>
    public class TFTypes_List : TFTypes
    {
        /// <summary>
        /// The list of PawnKindDefs that this TF can potentially transform into.
        /// </summary>
        [UsedImplicitly] public List<PawnKindDef> animals;

        /// <summary>
        /// Gets the list of available pawnkinds to TF into.
        /// </summary>
        /// <returns>The mutations.</returns>
        /// <param name="hediff">Hediff.</param>
        public override IEnumerable<PawnKindDef> GetTFs(Hediff_MutagenicBase hediff)
        {
            return animals;
        }

        /// <summary>
        /// Chechs whether this TFTypes is equivalent to another
        /// (meaning they produce the same list of TFs)
        /// </summary>
        /// <returns><c>true</c>, if to was equivalented, <c>false</c> otherwise.</returns>
        /// <param name="other">The other TFTypes.</param>
        public override bool EquivalentTo(TFTypes other)
        {
            return other is TFTypes_List otherList
                    && animals.Equals(otherList.animals);
        }
    }

    /// <summary>
    /// A simple TFTypes that selects all animals from a morph def
    /// </summary>
    public class TFTypes_Morph : TFTypes
    {
        /// <summary>
        /// The morph def to get potential animal forms from.
        /// </summary>
        [UsedImplicitly] public MorphDef morphDef;

        /// <summary>
        /// Gets the list of available pawnkinds to TF into.
        /// </summary>
        /// <returns>The mutations.</returns>
        /// <param name="hediff">Hediff.</param>
        public override IEnumerable<PawnKindDef> GetTFs(Hediff_MutagenicBase hediff)
        {
            return morphDef.PrimaryFeralPawnKinds;
        }

        /// <summary>
        /// Chechs whether this TFTypes is equivalent to another
        /// (meaning they produce the same list of TFs)
        /// </summary>
        /// <returns><c>true</c>, if to was equivalented, <c>false</c> otherwise.</returns>
        /// <param name="other">The other TFTypes.</param>
        public override bool EquivalentTo(TFTypes other)
        {
            return other is TFTypes_Morph otherMorph
                    && morphDef.Equals(otherMorph.morphDef);
        }
    }

    /// <summary>
    /// A simple TFTypes that selects all animals from a class (including child classes)
    /// </summary>
    public class TFTypes_Class : TFTypes
    {
        /// <summary>
        /// The class def to get potential animals from.
        /// </summary>
        [UsedImplicitly] public AnimalClassDef classDef;

        /// <summary>
        /// Gets the list of available pawnkinds to TF into.
        /// </summary>
        /// <returns>The mutations.</returns>
        /// <param name="hediff">Hediff.</param>
        public override IEnumerable<PawnKindDef> GetTFs(Hediff_MutagenicBase hediff)
        {
            return classDef.GetAllMorphsInClass()
                    .SelectMany(m => m.PrimaryFeralPawnKinds)
                    .Distinct();
        }

        /// <summary>
        /// Chechs whether this TFTypes is equivalent to another
        /// (meaning they produce the same list of TFs)
        /// </summary>
        /// <returns><c>true</c>, if to was equivalented, <c>false</c> otherwise.</returns>
        /// <param name="other">The other TFTypes.</param>
        public override bool EquivalentTo(TFTypes other)
        {
            return other is TFTypes_Class otherClass
                    && classDef.Equals(otherClass.classDef);
        }
    }

    /// <summary>
    /// A TFTypes that selects mutations defined in HediffComp_MutagenicTypes
    /// 
    /// Most "dynamic" hediffs that want to share mutation data across stages will
    /// want to use this TFTypes, as TFTypes are stateless.
    /// </summary>
    public class TFTypes_FromComp : TFTypes
    {
        /// <summary>
        /// Gets the list of available pawnkinds to TF into.
        /// </summary>
        /// <returns>The mutations.</returns>
        /// <param name="hediff">Hediff.</param>
        public override IEnumerable<PawnKindDef> GetTFs(Hediff_MutagenicBase hediff)
        {
            return hediff.TryGetComp<HediffComp_MutTypeBase>()
                    .GetTFs();
        }

        /// <summary>
        /// Chechs whether this TFTypes is equivalent to another
        /// (meaning they produce the same GetTFs)
        /// </summary>
        /// <returns><c>true</c>, if to was equivalented, <c>false</c> otherwise.</returns>
        /// <param name="other">The other TFTypes.</param>
        public override bool EquivalentTo(TFTypes other)
        {
            return other is TFTypes_FromComp;
        }
    }
}
