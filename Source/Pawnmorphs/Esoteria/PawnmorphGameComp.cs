﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using JetBrains.Annotations;
using UnityEngine;
using Verse;
using Verse.Sound;
using static RimWorld.MoteMaker;
using RimWorld.Planet;
using RimWorld;
using Multiplayer.API;

namespace Pawnmorph
{
    public class PawnmorphGameComp : WorldComponent
    {
        public HashSet<PawnMorphInstance> pawnmorphs = new HashSet<PawnMorphInstance>(){};
        public HashSet<PawnMorphInstanceMerged> mergedpawnmorphs = new HashSet<PawnMorphInstanceMerged>() { }; //why are we using hashsets? 
        public HashSet<PawnKindDef> taggedAnimals = new HashSet<PawnKindDef>() { };

        public IEnumerable<PawnMorphInstance> MorphInstances => pawnmorphs;
        public IEnumerable<PawnMorphInstanceMerged> MergeInstances => mergedpawnmorphs;  


        [CanBeNull]
        public PawnMorphInstanceMerged GetMergeInstanceWithOriginal(Pawn original)
        {
            return mergedpawnmorphs.FirstOrDefault(i => i.origin == original || i.origin2 == original); 
        }

        [CanBeNull]
        public PawnMorphInstance GetInstanceWithOriginal(Pawn original)
        {
            return pawnmorphs.FirstOrDefault(i => i.origin == original); 
        }

        public PawnmorphGameComp(World world) : base(world)
        {
        }

        public void tagPawn(PawnKindDef pawnkind)
        {
            if (!taggedAnimals.Contains(pawnkind))
            {
                taggedAnimals.Add(pawnkind);
            }
        }

        public void addPawn(PawnMorphInstance pm)
        {
            this.pawnmorphs.Add(pm);
        }
        public void addPawnMerged(PawnMorphInstanceMerged pmm)
        {
            this.mergedpawnmorphs.Add(pmm);
        }

        public void removePawn(PawnMorphInstance pm)
        {
            this.pawnmorphs.Remove(pm);
        }
        public void removePawnMerged(PawnMorphInstanceMerged pmm)
        {
            this.mergedpawnmorphs.Remove(pmm);
        }

        public PawnMorphInstance retrieve(Pawn animal)
        {
            PawnMorphInstance pm = this.pawnmorphs.FirstOrDefault(instance => instance.replacement == animal);
            return pm;
        }

        public PawnMorphInstanceMerged retrieveMerged(Pawn animal)
        {
            PawnMorphInstanceMerged pm = this.mergedpawnmorphs.FirstOrDefault(instance => instance.replacement == animal);
            return pm;
        }

        public override void ExposeData()
        {
            Scribe_Collections.Look(ref this.pawnmorphs, "pawnmorphs", LookMode.Deep);
            Scribe_Collections.Look(ref this.mergedpawnmorphs, "pawnmorphs", LookMode.Deep);
            Scribe_Collections.Look(ref this.taggedAnimals, "taggedAnimals");
        }
    }
}
