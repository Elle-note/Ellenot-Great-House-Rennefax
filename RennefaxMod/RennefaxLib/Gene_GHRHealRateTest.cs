﻿using System.Collections.Generic;
using Verse;
using GreatHouseRennefax;

public class Gene_GHRHealRateTest : Gene_ReadsPowerLevel
{

    Dictionary<int, float> powerDictHeal = new Dictionary<int, float>() // Matches power tiers to Severity floats
        {
                { 1, .1f },
                { 2, .2f },
                { 3, .3f },
                { 4, .4f },
                { 5, .6f },
                { 6, .9f },
                { 7, 1f }
        };
    public override void PostAdd()
    {
        base.PostAdd();
        int power = ReadGetPowerLevel(); // Gets our power level
        float healHediffSeverity = powerDictHeal.TryGetValue(power); // Matches our power level int to a corresponding float
        Log.Message(power);
        Log.Message(healHediffSeverity);
        if (power > 0)
        {
            Hediff healHediff = pawn.health.hediffSet.GetFirstHediffOfDef(GHRDefOf.GHRTestHealHediff);

            if (healHediff == null)
            {
                healHediff = HediffMaker.MakeHediff(GHRDefOf.GHRTestHealHediff, pawn);
                pawn.health.AddHediff(healHediff);
            }

            healHediff.Severity = healHediffSeverity; // Sets severity based on power level - higher = better healing
            Log.Message($"actual severity: {healHediffSeverity} - severity set to hediff: {healHediff.Severity}");
        }
    }

}
