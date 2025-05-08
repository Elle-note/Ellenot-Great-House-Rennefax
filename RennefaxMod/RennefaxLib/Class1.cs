using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RimWorld;
using Verse;
using VanillaGenesExpanded;
using GreatHouseRennefax;
using Verse.Noise;
using Verse.AI;
using UnityEngine;
using Verse.Sound;

namespace GreatHouseRennefax
{

    //    public class ThingSetMaker_Effigy : ThingSetMaker - Fallback, do not use
    //       protected override void Generate(ThingSetMakerParams parms, List<Thing> outThings) 
    //      {
    //      Thing thing = ThingMaker.MakeThing(ThingDefOf.RenEffigy);
    //      }
    [DefOf]
    public static class GHRDefOf
    {
        public static ThingDef RenEffigy;
        public static DamageDef petrification;
        public static GeneDef GHR_WyrmHeartGene;
        public static HediffDef GHRTestHealHediff;
    }

    // G E N E S
    public class GeneExtensionPowerLevel : DefModExtension
    {
        public int powerLevel;
    }
    public class Gene_PowerLevel : Gene
    {
        public override void Tick()
        {
            if (pawn.IsHashIntervalTick(100))
            {
                CheckWear();
            }
        }
        public int powerLevel;
        public override void PostAdd()
        {
            base.PostAdd();
            powerLevel = Rand.Range(1, 3);
        }
        public void AddPowerLevel(int increaseBy = 1)
        {
            powerLevel += increaseBy;
        }
        public void SetPowerLevel(int newPowerLevel)
        {
            powerLevel = newPowerLevel;
        }
        public int GetPowerLevel()
        {
            return powerLevel;
        }
        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Values.Look(ref powerLevel, nameof(powerLevel), 1);
        }
        //    public static Dictionary
        //    public Gene_Purity purityCrippling;
        //    public Gene_Purity purityPitiful;
        //    public Gene_Purity purityNuisance;
        //    public Gene_Purity purityAdapted;
        //    public Gene_Purity purityNaturalized;
        //    public Gene_Purity purityMastered;
        //    public Gene_Purity purityPerfect;
        //    private void ConstructGeneDict() // Unsure how necessary this is. Intended to map data for use by specific genes; Ints correspond to different gene 'power levels'
        //    {

        //        Dictionary<Gene_Purity, int> geneDict = new Dictionary<Gene_Purity, int>()
        //    {
        //        { purityCrippling, 1 },
        //        { purityPitiful, 2 },
        //        { purityNuisance, 3 },
        //        { purityAdapted, 4 },
        //        { purityNaturalized, 5 },
        //        { purityMastered, 6 },
        //        { purityPerfect, 7 }
        //    }; 
        private void CheckWear()
        {
            Hediff wyrmWearHediff = pawn?.health?.hediffSet?.hediffs?.Where(x => x?.def?.defName == "wyrmWearHediff") // Checks pawn hediffs and if has one of def wyrmWearHediff
             .FirstOrDefault();
            if (wyrmWearHediff == null || wyrmWearHediff.Severity < 1f) // If hediff is nonexistent or severity less than 1
            {
                return;
            }
            Thing statue = ThingMaker.MakeThing(GHRDefOf.RenEffigy, null); // Our statue
            statue.TryGetComp<CompQuality>().SetQuality(QualityCategory.Excellent, ArtGenerationContext.Outsider);
            if (statue != null)
                GenSpawn.Spawn(statue, this.pawn.PositionHeld, this.pawn.MapHeld);
            DamageInfo petrDamage = new DamageInfo(GHRDefOf.petrification, 500f, 0f, -1f, this.pawn, null, null); // Custom Petrification dmginfo
            this.pawn.Kill(petrDamage);


        }
    }
}
public class Gene_ReadsPowerLevel : Gene_PowerLevel
{
    public int ReadGetPowerLevel()
    {
        Gene_PowerLevel powerLevelGene = pawn.genes.GetFirstGeneOfType<Gene_PowerLevel>();
        if (powerLevelGene != null)
        {
            Log.Message(powerLevelGene.powerLevel);
            return powerLevelGene.GetPowerLevel();
        }
        return 0;
    }
}

public class EffigyComp : ThingComp
{
    public override IEnumerable<FloatMenuOption> CompFloatMenuOptions(Pawn selPawn)
    {

        yield return new FloatMenuOption(
            $"{selPawn.Label} ",
            delegate
            {
                //Spot for prayer code. Pray at statue > Starts timer to spawn Lesser Wyrm boss
            },
            MenuOptionPriority.Default);
    }
}



// E F F I G Y
public class Building_RaematEffigy : Building
{
    private int countdownTicks = GenDate.TicksPerDay; // Time to Wyrm spawn after prayer
                                                      // private int spawnTile = -1; Don't do this here, do it in the wyrm spawn method
    
    
    public override void PostMake()
    {
        base.PostMake();
        // Do Wyrm spawn code

    }

}

// W Y R M
public class ThingSetMaker_LesserWyrm : ThingSetMaker
{
    protected override IEnumerable<ThingDef> AllGeneratableThingsDebugSub(ThingSetMakerParams parms)
    {
        throw new NotImplementedException();
    }
    
    protected override void Generate(ThingSetMakerParams parms, List<Thing> outThings)
    {
        ThingDef building = GHRDefOf.RenEffigy;
        Map map = Find.CurrentMap;
        List<Thing> spawnerStatues = map.listerThings.ThingsOfDef(building);
        Thing statueToSpawnFrom = spawnerStatues.First();
        IntVec3 statueLocation = statueToSpawnFrom.Position;
        PawnKindDef pawnKindDef = PawnKindDef.Named("GHR_LesserWyrm");
        Pawn wyrm = PawnGenerator.GeneratePawn(new PawnGenerationRequest(pawnKindDef, null,PawnGenerationContext.NonPlayer,map.Tile,true,false,false,false,true,0,false,true,false,false,false,false,false,false,true,0,0,null,0))
        ;
    }
}