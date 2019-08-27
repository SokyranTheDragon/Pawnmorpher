﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Multiplayer.API;
using Pawnmorph.TfSys;
using Pawnmorph.Utilities;
using UnityEngine;
using RimWorld;
using Verse;

namespace Pawnmorph
{
    public class IngestionOutcomeDoer_CompleteTF : IngestionOutcomeDoer
    {
        protected override void DoIngestionOutcomeSpecial(Pawn pawn, Thing ingested)
        {
            if (MP.IsInMultiplayer)
            {
                Rand.PushState(RandUtilities.MPSafeSeed); 
            }

            foreach (Hediff hediff in pawn.health.hediffSet.hediffs) // Loop through all the hediffs on the pawn.
            {
                if (hediff is Hediff_Morph morph && morph.CurStage == morph.def.stages[0]) // When you find one that is a pawnmorph in the final stage...
                {
                    foreach (HediffStage stage in morph.def.stages) // ...loop through its stages...
                    {
                        foreach (HediffGiver giver in stage.hediffGivers) // ...and their hediffGivers...
                        {
                            if (giver is HediffGiver_TF giverTF) // ...until you find one that is of type HediffGiver_TF.
                            {
                                var mutagen = morph.GetMutagenDef();

                                pawn.health.RemoveHediff(morph);


                                var request = new TransformationRequest(giverTF.pawnkinds.RandElement(), pawn)
                                {
                                    tale = giverTF.tale
                                };

                                var inst = mutagen.MutagenCached.Transform(request);

                                if (inst != null)
                                {
                                    var comp = Find.World.GetComponent<PawnmorphGameComp>();
                                    comp.AddTransformedPawn(inst); 
                                }

                                if (MP.IsInMultiplayer)
                                {
                                    Rand.PopState();
                                }

                                //TransformerUtility.Transform(pawn, morph, giverTF.hediff, giverTF.pawnkinds, giverTF.tale); // When you do, use it's infor to transform the pawn.
                                return;
                            }
                        }
                    }
                }
            }

            if (MP.IsInMultiplayer)
            {
                Rand.PopState();
            }

        }
    }
}
