﻿// InteractionPatches.cs modified by Iron Wolf for Pawnmorph on 12/10/2019 6:31 PM
// last updated 12/10/2019  6:31 PM

using Harmony;
using JetBrains.Annotations;
using Pawnmorph.DefExtensions;
using RimWorld;
using Verse;

namespace Pawnmorph.HPatches
{

    static class InteractionPatches
    {
       
        [HarmonyPatch(typeof(Pawn_InteractionsTracker)), HarmonyPatch(nameof(Pawn_InteractionsTracker.TryInteractWith))]
        static class TryInteractWithPatch
        {
            [HarmonyPrefix]
            static bool SubstituteInteraction(Pawn recipient, ref InteractionDef intDef, Pawn ___pawn)
            {
                var ext = intDef.GetModExtension<InteractionGroupExtension>();
                InteractionDef alt = ext?.TryGetAlternativeFor(___pawn, recipient);
                if (alt != null)
                    intDef = alt;

                return true; 
            }
        }

        [HarmonyPatch(typeof(InteractionUtility)), HarmonyPatch(nameof(InteractionUtility.CanReceiveRandomInteraction))]
        static class SapientAnimalsRandomInteractionPatch
        {
            [HarmonyPrefix]
            static bool SapientAnimalPatch([NotNull] Pawn p, ref bool __result)
            {
                if (p.GetFormerHumanStatus() == FormerHumanStatus.Sapient)
                {
                    __result = InteractionUtility.CanReceiveInteraction(p) && (!p.Downed && !p.InAggroMentalState);
                    return false; 
                }

                return true; 
            }
        }
    }
}