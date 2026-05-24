using Oxide.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Oxide.Plugins
{
    [Info("CalmLoot", "Yaowy", "1.6.8")]
    [Description("A custom loot pool for Calm server. Largely inspired by BetterLoot, originally by dcode, maintained by Fujicura, Misticos, Tryhard, and TGWA.")]
    class CalmLoot : RustPlugin
    {
        // ─── Prefab groups ───────────────────────────────────────────────────────

        private static readonly HashSet<string> BarrelPrefabs = new HashSet<string>
        {
            "assets/bundled/prefabs/radtown/loot_barrel_1.prefab",
            "assets/bundled/prefabs/radtown/loot_barrel_2.prefab",
            "assets/bundled/prefabs/autospawn/resource/loot/loot-barrel-1.prefab",
            "assets/bundled/prefabs/autospawn/resource/loot/loot-barrel-2.prefab"
        };

        private static readonly HashSet<string> TrashPilePrefabs = new HashSet<string>
        {
            "assets/bundled/prefabs/autospawn/resource/loot/trash-pile-1.prefab"
        };

        private static readonly HashSet<string> CrateBasicPrefabs = new HashSet<string>
        {
            "assets/bundled/prefabs/radtown/crate_basic.prefab"
        };

        private static readonly HashSet<string> CrateBasicJunglePrefabs = new HashSet<string>
        {
            "assets/bundled/prefabs/radtown/crate_basic_jungle.prefab"
        };

        private static readonly HashSet<string> CrateMinePrefabs = new HashSet<string>
        {
            "assets/bundled/prefabs/radtown/crate_mine.prefab"
        };

        private static readonly HashSet<string> CrateNormalPrefabs = new HashSet<string>
        {
            "assets/bundled/prefabs/radtown/crate_normal.prefab"
        };

        private static readonly HashSet<string> CrateNormal2Prefabs = new HashSet<string>
        {
            "assets/bundled/prefabs/radtown/crate_normal_2.prefab"
        };

        private static readonly HashSet<string> CrateNormal2FoodPrefabs = new HashSet<string>
        {
            "assets/bundled/prefabs/radtown/crate_normal_2_food.prefab"
        };

        private static readonly HashSet<string> CrateNormal2MedicalPrefabs = new HashSet<string>
        {
            "assets/bundled/prefabs/radtown/crate_normal_2_medical.prefab"
        };

        private static readonly HashSet<string> CrateShorePrefabs = new HashSet<string>
        {
            "assets/bundled/prefabs/radtown/crate_shore.prefab"
        };

        private static readonly HashSet<string> CrateToolsPrefabs = new HashSet<string>
        {
            "assets/bundled/prefabs/radtown/crate_tools.prefab"
        };

        private static readonly HashSet<string> CrateElitePrefabs = new HashSet<string>
        {
            "assets/bundled/prefabs/radtown/crate_elite.prefab"
        };

        private static readonly HashSet<string> CrateCannonsPrefabs = new HashSet<string>
        {
            "assets/bundled/prefabs/radtown/crate_cannons.prefab"
        };

        private static readonly HashSet<string> FoodboxPrefabs = new HashSet<string>
        {
            "assets/bundled/prefabs/radtown/foodbox.prefab"
        };

        private static readonly HashSet<string> MinecartPrefabs = new HashSet<string>
        {
            "assets/bundled/prefabs/radtown/minecart.prefab"
        };

        private static readonly HashSet<string> OilBarrelPrefabs = new HashSet<string>
        {
            "assets/bundled/prefabs/radtown/oil_barrel.prefab"
        };

        private static readonly HashSet<string> LootTrashPrefabs = new HashSet<string>
        {
            "assets/bundled/prefabs/radtown/loot_trash.prefab"
        };

        private static readonly HashSet<string> VehiclePartsPrefabs = new HashSet<string>
        {
            "assets/bundled/prefabs/radtown/vehicle_parts.prefab"
        };

        private static readonly HashSet<string> RoadsignPrefabs = new HashSet<string>
        {
            "assets/content/props/roadsigns/roadsign1.prefab",
            "assets/content/props/roadsigns/roadsign2.prefab",
            "assets/content/props/roadsigns/roadsign3.prefab",
            "assets/content/props/roadsigns/roadsign4.prefab",
            "assets/content/props/roadsigns/roadsign5.prefab",
            "assets/content/props/roadsigns/roadsign6.prefab",
            "assets/content/props/roadsigns/roadsign7.prefab",
            "assets/content/props/roadsigns/roadsign8.prefab",
            "assets/content/props/roadsigns/roadsign9.prefab"
        };

        private static readonly HashSet<string> SupplyDropPrefabs = new HashSet<string>
        {
            "assets/prefabs/misc/supply drop/supply_drop.prefab"
        };

        private static readonly HashSet<string> BradleyCratePrefabs = new HashSet<string>
        {
            "assets/prefabs/npc/m2bradley/bradley_crate.prefab"
        };

        private static readonly HashSet<string> HeliCratePrefabs = new HashSet<string>
        {
            "assets/prefabs/npc/patrol helicopter/heli_crate.prefab"
        };

        private static readonly HashSet<string> ChinookCratePrefabs = new HashSet<string>
        {
            "assets/prefabs/deployable/chinooklockedcrate/codelockedhackablecrate.prefab",
            "assets/prefabs/deployable/chinooklockedcrate/codelockedhackablecrate_ghostship.prefab",
            "assets/prefabs/deployable/chinooklockedcrate/codelockedhackablecrate_oilrig.prefab"
        };

        private static readonly HashSet<string> FoodCachePrefabs = new HashSet<string>
        {
            "assets/prefabs/misc/food cache/food_cache_001.prefab",
            "assets/prefabs/misc/food cache/food_cache_002.prefab",
            "assets/prefabs/misc/food cache/food_cache_003.prefab",
            "assets/prefabs/misc/food cache/food_cache_004.prefab",
            "assets/prefabs/misc/food cache/food_cache_005.prefab"
        };

        // ─── Loot tables ─────────────────────────────────────────────────────────

        private class LootItem
        {
            public string Shortname;
            public int Min;
            public int Max;

            public LootItem(string shortname, int min, int max)
            {
                Shortname = shortname;
                Min = min;
                Max = max;
            }
        }

        private static readonly List<LootItem> TrashPileLoot = new List<LootItem>
        {
            new LootItem("apple",           1, 2),
            new LootItem("bread.loaf",      1, 2),
            new LootItem("can.beans",       1, 2),
            new LootItem("can.tuna",        1, 2),
            new LootItem("chocolate",       1, 2),
            new LootItem("granolabar",      1, 2),
            new LootItem("smallwaterbottle",1, 1),
            new LootItem("potato",          1, 2),
            new LootItem("corn",            1, 2),
            new LootItem("honeycomb",       1, 2),
            new LootItem("antiradpills",    1, 2),
        };

        private static readonly List<LootItem> CrateBasicLoot = new List<LootItem>
        {
            // Clothing
            new LootItem("burlap.gloves",       1, 1),
            new LootItem("hat.beenie",           1, 1),
            new LootItem("hat.boonie",           1, 1),
            new LootItem("hat.cap",              1, 1),
            new LootItem("mask.balaclava",       1, 1),
            new LootItem("mask.bandana",         1, 1),
            // Deployables
            new LootItem("barricade.wood.cover", 1, 1),
            new LootItem("fireplace.stone",      1, 1),
            new LootItem("spikes.floor",         1, 1),
            new LootItem("water.barrel",         1, 1),
            new LootItem("mailbox",              1, 1),
            new LootItem("planter.large",        1, 1),
            new LootItem("planter.triangle",     1, 1),
            new LootItem("electric.igniter",     1, 1),
            new LootItem("rug.bear",             1, 1),
            new LootItem("rug",                  1, 1),
            new LootItem("shelves",              1, 1),
            new LootItem("sign.wooden.huge",     1, 1),
            new LootItem("sign.wooden.large",    1, 1),
            new LootItem("tool.binoculars",      1, 1),
            new LootItem("paddle",               1, 1),
            new LootItem("bucket.water",         1, 1),
            // Components
            new LootItem("rope",                 1, 1),
            new LootItem("metalblade",           1, 1),
            new LootItem("tarp",                 1, 1),
            new LootItem("sewingkit",            1, 2),
            new LootItem("metalpipe",            1, 1),
            new LootItem("metalspring",          1, 1),
            new LootItem("roadsigns",            1, 1),
            new LootItem("sheetmetal",           1, 1),
            new LootItem("semibody",             1, 1),
            new LootItem("fuse",                 1, 1),
        };

        private static readonly List<LootItem> CrateBasicJungleAmmo = new List<LootItem>
        {
            new LootItem("dart.scatter",         5, 12),
            new LootItem("dart.radiation",       5, 12),
            new LootItem("dart.incapacitate",    3, 5),
            new LootItem("dart.wood",            10, 12),
        };

        private static readonly List<LootItem> CrateBasicJungleOther = new List<LootItem>
        {
            new LootItem("rope",                 1, 4),
            new LootItem("ladder.wooden.wall",   1, 2),
            new LootItem("antiradpills",         1, 2),
            new LootItem("syringe.medical",      1, 2),
            new LootItem("largemedkit",          1, 1),
        };

        private static readonly List<LootItem> CrateMineTools = new List<LootItem>
        {
            new LootItem("hatchet",          1, 1),
            new LootItem("pickaxe",          1, 1),
            new LootItem("flashlight.held",  1, 1),
            new LootItem("tool.binoculars",  1, 1),
            new LootItem("hat.miner",        1, 1),
        };

        private static readonly List<LootItem> CrateMineResources = new List<LootItem>
        {
            new LootItem("lowgradefuel",     5, 10),
            new LootItem("metal.refined",    1, 3),
            new LootItem("gunpowder",        15, 30),
            new LootItem("metal.fragments",  30, 60),
            new LootItem("flare",            1, 2),
        };

        private static readonly List<LootItem> CrateNormalWeapons = new List<LootItem>
        {
            new LootItem("homingmissile.launcher",           1, 1),
            new LootItem("pistol.python",                    1, 1),
            new LootItem("pistol.semiauto",                  1, 1),
            new LootItem("revolver.hc",                      1, 1),
            new LootItem("rifle.semiauto",                   1, 1),
            new LootItem("rocket.launcher",                  1, 1),
            new LootItem("shotgun.pump",                     1, 1),
            new LootItem("smg.2",                            1, 1),
            new LootItem("smg.thompson",                     1, 1),
            new LootItem("flamethrower",                     1, 1),
        };

        private static readonly List<LootItem> CrateNormalArmor = new List<LootItem>
        {
            new LootItem("roadsign.gloves",                  1, 1),
            new LootItem("coffeecan.helmet",                 1, 1),
            new LootItem("heavy.plate.helmet",               1, 1),
            new LootItem("heavy.plate.jacket",               1, 1),
            new LootItem("heavy.plate.pants",                1, 1),
            new LootItem("hoodie",                           1, 1),
            new LootItem("nightvisiongoggles",               1, 1),
            new LootItem("roadsign.jacket",                  1, 1),
            new LootItem("roadsign.kilt",                    1, 1),
            new LootItem("shoes.boots",                      1, 1),
            new LootItem("hazmatsuit",                       1, 1),
        };

        private static readonly List<LootItem> CrateNormalComponents = new List<LootItem>
        {
            new LootItem("smgbody",                          1, 2),
            new LootItem("riflebody",                        1, 2),
            new LootItem("techparts",                        3, 5),
            new LootItem("metalpipe",                        5, 7),
            new LootItem("targeting.computer",               1, 2),
            new LootItem("cctv.camera",                      1, 2),
        };

        private static readonly List<LootItem> CrateNormalElectronics = new List<LootItem>
        {
            new LootItem("computerstation",                  1, 1),
            new LootItem("drone",                            1, 1),
            new LootItem("elevator",                         1, 1),
            new LootItem("locker",                           1, 1),
            new LootItem("modularcarlift",                   1, 1),
            new LootItem("electric.battery.rechargable.large", 1, 1),
            new LootItem("electric.battery.rechargable.medium", 1, 1),
            new LootItem("electric.furnace",                 1, 1),
            new LootItem("electric.fuelgenerator.small",     1, 1),
            new LootItem("industrial.crafter",               1, 1),
            new LootItem("powered.water.purifier",           1, 1),
            new LootItem("waterpump",                        1, 1),
            new LootItem("ptz.cctv.camera",                  1, 1),
            new LootItem("searchlight",                      1, 1),
            new LootItem("generator.wind.scrap",             1, 1),
        };

        private static readonly List<LootItem> CrateNormalDeployables = new List<LootItem>
        {
            new LootItem("furnace.large",                    1, 1),
            new LootItem("trap.landmine",                    1, 1),
            new LootItem("floor.ladder.hatch",               1, 1),
            new LootItem("floor.triangle.ladder.hatch",      1, 1),
            new LootItem("gates.external.high.stone",        1, 1),
            new LootItem("wall.external.high.stone",         1, 1),
            new LootItem("wall.frame.garagedoor",            1, 1),
            new LootItem("shutter.metal.embrasure.a",        1, 1),
            new LootItem("shutter.metal.embrasure.b",        1, 1),
            new LootItem("autoturret",                       1, 1),
            new LootItem("small.oil.refinery",               1, 1),
        };

        private static readonly List<LootItem> CrateNormalTools = new List<LootItem>
        {
            new LootItem("chainsaw",                         1, 1),
            new LootItem("axe.salvaged",                     1, 1),
            new LootItem("icepick.salvaged",                 1, 1),
        };

        private static readonly List<LootItem> CrateNormalAmmo = new List<LootItem>
        {
            new LootItem("grenade.f1",                       1, 3),
            new LootItem("grenade.flashbang",                1, 3),
            new LootItem("grenade.molotov",                  1, 3),
            new LootItem("grenade.smoke",                    1, 3),
        };

        private static readonly List<LootItem> CrateNormalWeaponMods = new List<LootItem>
        {
            new LootItem("weapon.mod.extendedmags",          1, 1),
            new LootItem("weapon.mod.gascompressionovedrive",1, 1),
            new LootItem("weapon.mod.holosight",             1, 1),
            new LootItem("weapon.mod.muzzleboost",           1, 1),
            new LootItem("weapon.mod.muzzlebrake",           1, 1),
        };

        private static readonly List<LootItem> CrateNormalOther = new List<LootItem>
        {
            new LootItem("supply.signal",                    1, 1),
            new LootItem("metal.shield",                     1, 1),
            new LootItem("clothing.mod.armorinsert_asbestos",1, 1),
            new LootItem("clothing.mod.armorinsert_lead",    1, 1),
            new LootItem("horse.armor.roadsign",             1, 1),
            new LootItem("horse.shoes.advanced",             1, 1),
            new LootItem("parachute",                        1, 1),
            new LootItem("handcuffs",                        1, 1),
            new LootItem("largemedkit",                      1, 1),
            new LootItem("syringe.medical",                  1, 1),
            new LootItem("rf_pager",                         1, 1),
        };

        private static readonly List<LootItem> CrateNormalUpgrades = new List<LootItem>
        {
            new LootItem("workbench.upgrade.accelerated",    1, 1),
            new LootItem("workbench.upgrade.comfort",        1, 1),
            new LootItem("workbench.upgrade.defensive",      1, 1),
            new LootItem("workbench.upgrade.efficiency",     1, 1),
            new LootItem("workbench.upgrade.prototype",      1, 1),
            new LootItem("workbench.upgrade.range",          1, 1),
            new LootItem("workbench.upgrade.recyclebin",     1, 1),
            new LootItem("workbench.upgrade.reinforced",     1, 1),
            new LootItem("workbench.upgrade.salvage",        1, 1),
            new LootItem("workbench.upgrade.surplus",        1, 1),
        };


        private static readonly List<LootItem> CrateNormal2Components = new List<LootItem>
        {
            new LootItem("roadsigns",                    2, 5),
            new LootItem("gears",                        2, 4),
            new LootItem("metalpipe",                    2, 5),
            new LootItem("metalspring",                  1, 3),
            new LootItem("sheetmetal",                   1, 3),
            new LootItem("semibody",                     1, 2),
            new LootItem("fuse",                         1, 2),
            new LootItem("targeting.computer",           1, 2),
            new LootItem("cctv.camera",                  1, 2),
        };

        private static readonly List<LootItem> CrateNormal2WeaponsArmorTools = new List<LootItem>
        {
            // Weapons
            new LootItem("shotgun.double",               1, 1),
            new LootItem("shotgun.waterpipe",            1, 1),
            new LootItem("pistol.revolver",              1, 1),
            new LootItem("t1_smg",                       1, 1),
            new LootItem("salvaged.cleaver",             1, 1),
            new LootItem("bow.compound",                 1, 1),
            new LootItem("knife.combat",                 1, 1),
            new LootItem("mace",                         1, 1),
            new LootItem("minicrossbow",                 1, 1),
            new LootItem("hammer.salvaged",              1, 1),
            new LootItem("explosive.satchel",            1, 1),
            new LootItem("speargun",                     1, 1),
            new LootItem("salvaged.sword",               1, 1),
            new LootItem("boomerang",                    1, 1),
            new LootItem("weapon.mod.flashlight",        1, 1),
            // Grenades
            new LootItem("grenade.beancan",              1, 1),
            new LootItem("grenade.bee",                  1, 1),
            // Armor
            new LootItem("hazmatsuit",                   1, 1),
            new LootItem("clothing.mod.armorinsert_wood",1, 1),
            new LootItem("bucket.helmet",                1, 1),
            new LootItem("hat.miner",                    1, 1),
            new LootItem("prisonerhood",                 1, 1),
            new LootItem("riot.helmet",                  1, 1),
            new LootItem("jacket.snow",                  1, 1),
            new LootItem("jacket",                       1, 1),
            new LootItem("pants",                        1, 1),
            new LootItem("shirt.collared",               1, 1),
            new LootItem("tshirt",                       1, 1),
            new LootItem("tshirt.long",                  1, 1),
            // Tools
            new LootItem("pickaxe",                      1, 1),
            new LootItem("hatchet",                      1, 1),
        };

        private static readonly List<LootItem> CrateNormal2ElectronicsOther = new List<LootItem>
        {
            // Electronics
            new LootItem("autoturret",                   1, 1),
            new LootItem("electric.audioalarm",          1, 1),
            new LootItem("electric.battery.rechargable.small", 1, 1),
            new LootItem("electric.doorcontroller",      1, 1),
            new LootItem("electric.heater",              1, 1),
            new LootItem("electric.blocker",             1, 1),
            new LootItem("electrical.branch",            1, 1),
            new LootItem("electrical.combiner",          1, 1),
            new LootItem("electric.orswitch",            1, 1),
            new LootItem("electric.xorswitch",           1, 1),
            new LootItem("electric.solarpanel.large",    1, 1),
            new LootItem("storageadaptor",               1, 1),
            new LootItem("industrial.combiner",          1, 1),
            new LootItem("industrial.conveyor",          1, 1),
            new LootItem("industrial.splitter",          1, 1),
            new LootItem("electric.flasherlight",        1, 1),
            new LootItem("electric.sirenlight",          1, 1),
            new LootItem("electric.switch",              1, 1),
            new LootItem("electric.splitter",            1, 1),
            new LootItem("electric.sprinkler",           1, 1),
            new LootItem("electric.timer",               1, 1),
            new LootItem("water.catcher.small",          1, 1),
            // Other
            new LootItem("kayak",                        1, 1),
            new LootItem("siegetower",                   1, 1),
            new LootItem("floor.grill",                  1, 1),
            new LootItem("floor.triangle.grill",         1, 1),
            new LootItem("gates.external.high.wood",     1, 1),
            new LootItem("ladder.wooden.wall",           1, 1),
            new LootItem("wall.external.high",           1, 1),
            new LootItem("wall.frame.fence.gate",        1, 1),
            new LootItem("wall.frame.fence",             1, 1),
            new LootItem("wall.window.bars.metal",       1, 1),
            new LootItem("watchtower.wood",              1, 1),
            new LootItem("barricade.sandbags",           1, 1),
            new LootItem("barricade.woodwire",           1, 1),
            new LootItem("trap.bear",                    1, 1),
            new LootItem("bed",                          1, 1),
            new LootItem("beehive",                      1, 1),
            new LootItem("ceilinglight",                 1, 1),
            new LootItem("chair",                        1, 1),
            new LootItem("chickencoop",                  1, 1),
            new LootItem("cookingworkbench",             1, 1),
            new LootItem("dropbox",                      1, 1),
            new LootItem("fridge",                       1, 1),
            new LootItem("hopper",                       1, 1),
            new LootItem("hab.armor",                    1, 1),
            new LootItem("iotable",                      1, 1),
            new LootItem("mixingtable",                  1, 1),
            new LootItem("tincan.alarm",                 1, 1),
            new LootItem("fluid.combiner",               1, 1),
            new LootItem("fluid.splitter",               1, 1),
            new LootItem("fluid.switch",                 1, 1),
            new LootItem("target.reactive",              1, 1),
            new LootItem("guntrap",                      1, 1),
            new LootItem("flameturret",                  1, 1),
            new LootItem("flare",                        4, 6),
            new LootItem("metal.detector",               1, 1),
            new LootItem("telephone",                    1, 1),
            new LootItem("reinforced.wooden.shield",     1, 1),
        };

        private static readonly List<LootItem> CrateNormal2FoodLoot = new List<LootItem>
        {
            new LootItem("apple",             1, 4),
            new LootItem("black.raspberries", 1, 4),
            new LootItem("blueberries",       1, 4),
            new LootItem("can.beans",         1, 4),
            new LootItem("can.tuna",          1, 4),
            new LootItem("chocolate",         1, 4),
            new LootItem("granolabar",        1, 4),
            new LootItem("pumpkin",           1, 4),
            new LootItem("smallwaterbottle",  1, 1),
            new LootItem("waterjug",          1, 1),
        };

        private static readonly List<LootItem> CrateNormal2MedicalLoot = new List<LootItem>
        {
            new LootItem("bandage",         2, 5),
            new LootItem("largemedkit",     1, 2),
            new LootItem("syringe.medical", 2, 4),
        };

        private static readonly List<LootItem> CrateShoreLoot = new List<LootItem>
        {
            new LootItem("antiradpills",    1, 1),
            new LootItem("apple",           1, 1),
            new LootItem("bread.loaf",      1, 2),
            new LootItem("can.beans",       1, 2),
            new LootItem("can.tuna",        1, 2),
            new LootItem("chocolate",       1, 2),
            new LootItem("granolabar",      1, 2),
            new LootItem("smallwaterbottle",1, 1),
            new LootItem("potato",          1, 2),
            new LootItem("corn",            1, 2),
            new LootItem("honeycomb",       1, 2),
            new LootItem("waterjug",        1, 2),
            new LootItem("jar.pickle",      1, 3),
        };

        private static readonly List<LootItem> CrateToolsWeapons = new List<LootItem>
        {
            new LootItem("hatchet",                 1, 1),
            new LootItem("pickaxe",                 1, 1),
            new LootItem("tool.binoculars",         1, 1),
            new LootItem("icepick.salvaged",        1, 1),
            new LootItem("axe.salvaged",            1, 1),
            new LootItem("chainsaw",                1, 1),
            new LootItem("jackhammer",              1, 1),
            new LootItem("salvaged.sword",          1, 1),
            new LootItem("mace",                    1, 1),
            new LootItem("salvaged.cleaver",        1, 1),
            new LootItem("bow.hunting",             1, 1),
            new LootItem("crossbow",                1, 1),
            new LootItem("knife.combat",            1, 1),
            new LootItem("autoturret",              1, 1),
            new LootItem("pistol.revolver",         1, 1),
        };

        private static readonly List<LootItem> CrateToolsOther = new List<LootItem>
        {
            // Clothing
            new LootItem("hoodie",                  1, 1),
            new LootItem("pants",                   1, 1),
            new LootItem("tshirt.long",             1, 1),
            new LootItem("wood.armor.jacket",       1, 1),
            new LootItem("wood.armor.pants",        1, 1),
            new LootItem("diving.mask",             1, 1),
            new LootItem("diving.tank",             1, 1),
            new LootItem("diving.wetsuit",          1, 1),
            new LootItem("diving.fins",             1, 1),
            // Ammo
            new LootItem("arrow.wooden",            5, 7),
            new LootItem("arrow.bone",              5, 10),
            new LootItem("arrow.fire",              3, 6),
            new LootItem("ammo.pistol",             15, 20),
            new LootItem("ammo.pistol.fire",        10, 12),
            new LootItem("ammo.pistol.hv",          10, 12),
            new LootItem("ammo.rifle",              12, 14),
            new LootItem("ammo.rifle.incendiary",   8, 10),
            new LootItem("ammo.rifle.hv",           10, 12),
            new LootItem("ammo.rifle.explosive",    8, 10),
            new LootItem("ammo.shotgun",            6, 8),
            new LootItem("ammo.shotgun.slug",       4, 6),
            new LootItem("ammo.shotgun.fire",       4, 8),
            // Resources
            new LootItem("lowgradefuel",            20, 50),
            // Components
            new LootItem("targeting.computer",      1, 2),
            new LootItem("cctv.camera",             1, 2),
            new LootItem("gears",                   1, 2),
            new LootItem("techparts",               1, 2),
        };

        private static readonly List<LootItem> CrateEliteLoot = new List<LootItem>
        {
            // Weapons
            new LootItem("smgbody",                          1, 2),
            new LootItem("riflebody",                        1, 2),
            new LootItem("supply.signal",                    1, 1),
            new LootItem("chainsaw",                         1, 1),
            new LootItem("flamethrower",                     1, 1),
            new LootItem("grenade.f1",                       1, 3),
            new LootItem("grenade.flashbang",                1, 3),
            new LootItem("grenade.molotov",                  1, 3),
            new LootItem("grenade.smoke",                    1, 3),
            new LootItem("homingmissile.launcher",           1, 1),
            new LootItem("metal.shield",                     1, 1),
            new LootItem("pistol.python",                    1, 1),
            new LootItem("pistol.semiauto",                  1, 1),
            new LootItem("revolver.hc",                      1, 1),
            new LootItem("rifle.semiauto",                   1, 1),
            new LootItem("rocket.launcher",                  1, 1),
            new LootItem("shotgun.pump",                     1, 1),
            new LootItem("smg.2",                            1, 1),
            new LootItem("smg.thompson",                     1, 1),
            new LootItem("longsword",                        1, 1),
            new LootItem("axe.salvaged",                     1, 1),
            new LootItem("icepick.salvaged",                 1, 1),
            new LootItem("rifle.ak",                         1, 1),
            new LootItem("rifle.bolt",                       1, 1),
            new LootItem("hmlmg",                            1, 1),
            new LootItem("smg.mp5",                          1, 1),
            new LootItem("rifle.sks",                        1, 1),
            new LootItem("explosives",                       1, 1),
            new LootItem("explosive.timed",                  1, 1),
            // Armor
            new LootItem("roadsign.gloves",                  1, 1),
            new LootItem("coffeecan.helmet",                 1, 1),
            new LootItem("heavy.plate.helmet",               1, 1),
            new LootItem("heavy.plate.jacket",               1, 1),
            new LootItem("heavy.plate.pants",                1, 1),
            new LootItem("hoodie",                           1, 1),
            new LootItem("nightvisiongoggles",               1, 1),
            new LootItem("roadsign.jacket",                  1, 1),
            new LootItem("roadsign.kilt",                    1, 1),
            new LootItem("shoes.boots",                      1, 1),
            new LootItem("hazmatsuit",                       1, 1),
            new LootItem("clothing.mod.armorinsert_asbestos",1, 1),
            new LootItem("clothing.mod.armorinsert_lead",    1, 1),
            new LootItem("horse.armor.roadsign",             1, 1),
            new LootItem("horse.shoes.advanced",             1, 1),
            new LootItem("metal.facemask",                   1, 1),
            new LootItem("metal.plate.torso",                1, 1),
            // Components
            new LootItem("techparts",                        2, 5),
            new LootItem("metalpipe",                        5, 7),
            new LootItem("targeting.computer",               1, 2),
            new LootItem("cctv.camera",                      1, 2),
            new LootItem("metal.refined",                    15, 30),
            // Electronics
            new LootItem("autoturret",                       1, 1),
            new LootItem("computerstation",                  1, 1),
            new LootItem("drone",                            1, 1),
            new LootItem("elevator",                         1, 1),
            new LootItem("locker",                           1, 1),
            new LootItem("modularcarlift",                   1, 1),
            new LootItem("small.oil.refinery",               1, 1),
            new LootItem("smart.alarm",                      1, 1),
            new LootItem("smart.switch",                     1, 1),
            new LootItem("storage.monitor",                  1, 1),
            new LootItem("electric.battery.rechargable.large",1, 1),
            new LootItem("electric.battery.rechargable.medium",1, 1),
            new LootItem("electric.counter",                 1, 1),
            new LootItem("electric.hbhfsensor",              1, 1),
            new LootItem("electric.laserdetector",           1, 1),
            new LootItem("electric.digitalclock",            1, 1),
            new LootItem("electric.furnace",                 1, 1),
            new LootItem("electric.andswitch",               1, 1),
            new LootItem("electrical.memorycell",            1, 1),
            new LootItem("electric.random.switch",           1, 1),
            new LootItem("electric.rf.broadcaster",          1, 1),
            new LootItem("electric.rf.receiver",             1, 1),
            new LootItem("electric.fuelgenerator.small",     1, 1),
            new LootItem("electric.seismicsensor",           1, 1),
            new LootItem("electric.teslacoil",               1, 1),
            new LootItem("industrial.crafter",               1, 1),
            new LootItem("powered.water.purifier",           1, 1),
            new LootItem("waterpump",                        1, 1),
            new LootItem("ptz.cctv.camera",                  1, 1),
            new LootItem("searchlight",                      1, 1),
            new LootItem("water.catcher.large",              1, 1),
            new LootItem("generator.wind.scrap",             1, 1),
            // Deployables
            new LootItem("barricade.concrete",               1, 1),
            new LootItem("barricade.metal",                  1, 1),
            new LootItem("furnace.large",                    1, 1),
            new LootItem("trap.landmine",                    1, 1),
            new LootItem("floor.ladder.hatch",               1, 1),
            new LootItem("floor.triangle.ladder.hatch",      1, 1),
            new LootItem("gates.external.high.stone",        1, 1),
            new LootItem("wall.external.high.stone",         1, 1),
            new LootItem("wall.frame.cell.gate",             1, 1),
            new LootItem("wall.frame.cell",                  1, 1),
            new LootItem("wall.frame.garagedoor",            1, 1),
            new LootItem("shutter.metal.embrasure.a",        1, 1),
            new LootItem("shutter.metal.embrasure.b",        1, 1),
            new LootItem("wall.window.glass.reinforced",     1, 1),
            new LootItem("door.double.hinged.toptier",       1, 1),
            new LootItem("door.hinged.toptier",              1, 1),
            new LootItem("wall.window.bars.toptier",         1, 1),
            // Weapon Mods
            new LootItem("weapon.mod.extendedmags",          1, 1),
            new LootItem("weapon.mod.gascompressionovedrive",1, 1),
            new LootItem("weapon.mod.holosight",             1, 1),
            new LootItem("weapon.mod.muzzleboost",           1, 1),
            new LootItem("weapon.mod.muzzlebrake",           1, 1),
            new LootItem("weapon.mod.lasersight",            1, 1),
            new LootItem("weapon.mod.small.scope",           1, 1),
            new LootItem("weapon.mod.silencer",              1, 1),
            // Other
            new LootItem("ballista.mounted",                 1, 1),
            new LootItem("ballista.static",                  1, 1),
            new LootItem("batteringram",                     1, 1),
            new LootItem("catapult",                         1, 1),
            new LootItem("parachute",                        1, 1),
            new LootItem("rf.detonator",                     1, 1),
            new LootItem("handcuffs",                        1, 1),
            new LootItem("largemedkit",                      1, 2),
            new LootItem("syringe.medical",                  1, 3),
            new LootItem("rf_pager",                         1, 1),
        };

        private static readonly List<LootItem> CrateCannonsloot = new List<LootItem>
        {
            // Medical
            new LootItem("bandage",             1, 3),
            new LootItem("largemedkit",         1, 1),
            new LootItem("syringe.medical",     1, 2),
            // Ammo
            new LootItem("ammo.pistol",         15, 20),
            new LootItem("ammo.rifle",          12, 14),
            new LootItem("ammo.shotgun",        6, 8),
            new LootItem("ammo.shotgun.slug",   4, 6),
            new LootItem("ammo.shotgun.fire",   4, 8),
            new LootItem("cannonball",          8, 12),
            // Tools
            new LootItem("hatchet",             1, 1),
            new LootItem("pickaxe",             1, 1),
            new LootItem("metal.detector",      1, 1),
        };

        private static readonly List<LootItem> FoodboxLoot = new List<LootItem>
        {
            new LootItem("antiradpills",    1, 2),
            new LootItem("apple",           1, 2),
            new LootItem("bread.loaf",      1, 2),
            new LootItem("can.beans",       1, 2),
            new LootItem("can.tuna",        1, 2),
            new LootItem("chocolate",       1, 2),
            new LootItem("granolabar",      1, 2),
            new LootItem("smallwaterbottle",1, 1),
            new LootItem("potato",          1, 2),
            new LootItem("corn",            1, 2),
            new LootItem("honeycomb",       1, 2),
            new LootItem("waterjug",        1, 2),
            new LootItem("jar.pickle",      1, 3),
        };

        private static readonly List<LootItem> MinecartLoot = new List<LootItem>
        {
            new LootItem("lowgradefuel",    5, 10),
            new LootItem("hat.miner",       1, 1),
            new LootItem("metal.refined",   1, 3),
            new LootItem("hatchet",         1, 1),
            new LootItem("pickaxe",         1, 1),
            new LootItem("tool.binoculars", 1, 1),
            new LootItem("gunpowder",       15, 30),
            new LootItem("metal.fragments", 30, 60),
            new LootItem("flare",           4, 6),
            new LootItem("flashlight.held", 1, 1),
        };

        private static readonly List<LootItem> OilBarrelLoot = new List<LootItem>
        {
            new LootItem("crude.oil",       15, 25),
            new LootItem("lowgradefuel",    8, 15),
        };

        private static readonly List<LootItem> LootTrashLoot = new List<LootItem>
        {
            new LootItem("metal.fragments", 10, 20),
            new LootItem("wood",            100, 200),
        };

        private static readonly List<LootItem> VehiclePartsLoot = new List<LootItem>
        {
            new LootItem("carburetor1",     1, 1),
            new LootItem("crankshaft1",     1, 1),
            new LootItem("piston1",         1, 1),
            new LootItem("sparkplug1",      1, 1),
            new LootItem("valve1",          1, 1),
            new LootItem("carburetor2",     1, 1),
            new LootItem("crankshaft2",     1, 1),
            new LootItem("piston2",         1, 1),
            new LootItem("sparkplug2",      1, 1),
            new LootItem("valve2",          1, 1),
        };

        private static readonly List<LootItem> RoadsignLoot = new List<LootItem>
        {
            new LootItem("metalpipe",       1, 2),
            new LootItem("roadsigns",       1, 2),
        };

        private static readonly List<LootItem> SupplyDropWeapons = new List<LootItem>
        {
            new LootItem("shotgun.pump",                1, 1),
            new LootItem("rifle.semiauto",              1, 1),
            new LootItem("smg.2",                       1, 1),
            new LootItem("smg.thompson",                1, 1),
            new LootItem("smg.mp5",                     1, 1),
            new LootItem("shotgun.spas12",              1, 1),
            new LootItem("pistol.m92",                  1, 1),
            new LootItem("rifle.m39",                   1, 1),
            new LootItem("rifle.lr300",                 1, 1),
            new LootItem("pistol.prototype17",          1, 1),
            new LootItem("metal.shield",                1, 2),
        };

        private static readonly List<LootItem> SupplyDropClothing = new List<LootItem>
        {
            new LootItem("coffeecan.helmet",            1, 1),
            new LootItem("jacket.snow",                 1, 1),
            new LootItem("riot.helmet",                 1, 1),
            new LootItem("hoodie",                      1, 1),
            new LootItem("pants",                       1, 1),
            new LootItem("roadsign.jacket",             1, 1),
            new LootItem("roadsign.kilt",               1, 1),
            new LootItem("roadsign.gloves",             1, 1),
            new LootItem("metal.facemask",              1, 1),
            new LootItem("metal.plate.torso",           1, 1),
        };

        private static readonly List<LootItem> SupplyDropComponents = new List<LootItem>
        {
            new LootItem("gears",                       5, 6),
            new LootItem("metalpipe",                   5, 8),
            new LootItem("sheetmetal",                  5, 8),
            new LootItem("techparts",                   4, 7),
        };

        private static readonly List<LootItem> SupplyDropAmmo = new List<LootItem>
        {
            new LootItem("ammo.shotgun",                8, 12),
            new LootItem("ammo.rifle",                  35, 60),
            new LootItem("ammo.pistol",                 20, 30),
            new LootItem("ammo.pistol.fire",            10, 20),
            new LootItem("ammo.pistol.hv",              10, 20),
            new LootItem("ammo.rifle.incendiary",       8, 16),
            new LootItem("ammo.rifle.hv",               10, 20),
            new LootItem("ammo.rifle.explosive",        8, 16),
            new LootItem("ammo.shotgun.slug",           4, 6),
            new LootItem("ammo.shotgun.fire",           4, 8),
            new LootItem("grenade.f1",                  2, 5),
        };

        private static readonly List<LootItem> SupplyDropExplosives = new List<LootItem>
        {
            new LootItem("explosive.satchel",           2, 4),
            new LootItem("explosive.timed",             1, 2),
        };

        private static readonly List<LootItem> SupplyDropTools = new List<LootItem>
        {
            new LootItem("chainsaw",                    1, 1),
            new LootItem("jackhammer",                  1, 1),
        };

        private static readonly List<LootItem> SupplyDropWeaponMods = new List<LootItem>
        {
            new LootItem("weapon.mod.lasersight",       1, 1),
            new LootItem("weapon.mod.small.scope",      1, 1),
            new LootItem("weapon.mod.holosight",        1, 1),
        };

        private static readonly List<LootItem> SupplyDropOther = new List<LootItem>
        {
            new LootItem("horse.shoes.advanced",        2, 3),
            new LootItem("horse.armor.roadsign",        1, 2),
        };

        private static readonly List<LootItem> RocketC4Pool = new List<LootItem>
        {
            new LootItem("ammo.rocket.basic",           1, 1),
            new LootItem("ammo.rocket.fire",            1, 1),
            new LootItem("ammo.rocket.hv",              1, 1),
            new LootItem("ammo.rocket.seeker",          1, 1),
            new LootItem("ammo.rocket.mlrs",            1, 1),
            new LootItem("explosive.timed",             1, 1),
        };

        private static readonly List<LootItem> BradleyCrateLoot = new List<LootItem>
        {
            // Weapons
            new LootItem("rifle.l96",                   1, 1),
            new LootItem("lmg.m249",                    1, 1),
            new LootItem("rifle.ak",                    1, 1),
            new LootItem("smg.thompson",                1, 1),
            new LootItem("rifle.bolt",                  1, 1),
            new LootItem("smg.2",                       1, 1),
            new LootItem("rifle.lr300",                 1, 1),
            new LootItem("smg.mp5",                     1, 1),
            new LootItem("pistol.m92",                  1, 1),
            new LootItem("shotgun.spas12",              1, 1),
            new LootItem("rifle.m39",                   1, 1),
            new LootItem("pistol.prototype17",          1, 1),
            new LootItem("shotgun.m4",                  1, 1),
            new LootItem("explosive.timed",             1, 2),
            // Ammo
            new LootItem("ammo.rifle",                  8, 20),
            new LootItem("ammo.pistol",                 15, 25),
            new LootItem("ammo.shotgun",                8, 12),
            new LootItem("ammo.shotgun.slug",           8, 12),
            new LootItem("ammo.rocket.fire",            5, 6),
            new LootItem("ammo.rocket.hv",              3, 4),
            new LootItem("ammo.rocket.mlrs",            2, 3),
            new LootItem("ammo.rocket.seeker",          4, 5),
            new LootItem("ammo.rifle.incendiary",       60, 70),
            new LootItem("ammo.rifle.explosive",        30, 40),
            new LootItem("ammo.rifle.hv",               40, 50),
            // Weapon Mods
            new LootItem("weapon.mod.8x.scope",         1, 1),
            new LootItem("weapon.mod.lasersight",       1, 1),
            new LootItem("weapon.mod.small.scope",      1, 1),
            new LootItem("weapon.mod.burstmodule",      1, 1),
            new LootItem("weapon.mod.extendedmags",     1, 1),
            // Components
            new LootItem("techparts",                   10, 14),
            new LootItem("targeting.computer",          1, 2),
            new LootItem("cctv.camera",                 1, 2),
            // Deployables
            new LootItem("door.double.hinged.toptier",  1, 1),
            new LootItem("door.hinged.toptier",         1, 1),
            new LootItem("floor.ladder.hatch",          1, 1),
            new LootItem("autoturret",                  1, 1),
        };

        private static readonly List<LootItem> HeliWeapons = new List<LootItem>
        {
            new LootItem("rifle.ak",                    1, 1),
            new LootItem("smg.thompson",                1, 1),
            new LootItem("rifle.bolt",                  1, 1),
            new LootItem("smg.2",                       1, 1),
            new LootItem("rifle.lr300",                 1, 1),
            new LootItem("smg.mp5",                     1, 1),
            new LootItem("pistol.m92",                  1, 1),
            new LootItem("shotgun.spas12",              1, 1),
            new LootItem("rifle.l96",                   1, 1),
            new LootItem("rifle.m39",                   1, 1),
            new LootItem("pistol.prototype17",          1, 1),
            new LootItem("shotgun.m4",                  1, 1),
            new LootItem("lmg.m249",                    1, 1),
            new LootItem("hmlmg",                       1, 1),
            new LootItem("rifle.sks",                   1, 1),
        };

        private static readonly List<LootItem> HeliAmmo = new List<LootItem>
        {
            new LootItem("ammo.rifle",                  120, 128),
            new LootItem("ammo.rifle.incendiary",       60, 80),
            new LootItem("ammo.rifle.explosive",        30, 40),
            new LootItem("ammo.rifle.hv",               40, 50),
            new LootItem("ammo.pistol",                 15, 20),
            new LootItem("ammo.shotgun",                8, 12),
            new LootItem("ammo.shotgun.slug",           8, 12),
        };

        private static readonly List<LootItem> HeliWeaponMods = new List<LootItem>
        {
            new LootItem("weapon.mod.8x.scope",         1, 1),
            new LootItem("weapon.mod.lasersight",       1, 1),
            new LootItem("weapon.mod.small.scope",      1, 1),
            new LootItem("weapon.mod.burstmodule",      1, 1),
            new LootItem("weapon.mod.extendedmags",     1, 1),
            new LootItem("weapon.mod.silencer",         1, 1),
        };

        private static readonly List<LootItem> HeliOther = new List<LootItem>
        {
            new LootItem("door.double.hinged.toptier",  1, 1),
            new LootItem("door.hinged.toptier",         1, 1),
            new LootItem("wall.window.bars.toptier",    1, 1),
            new LootItem("metal.facemask",              1, 1),
            new LootItem("metal.plate.torso",           1, 1),
            new LootItem("metal.refined",               30, 50),
            new LootItem("ammo.rocket.fire",            5, 6),
            new LootItem("ammo.rocket.hv",              3, 4),
            new LootItem("ammo.rocket.seeker",          4, 5),
        };

        private static readonly List<LootItem> HeliSpecialPool = new List<LootItem>
        {
            new LootItem("ammo.rocket.basic",           1, 1),
            new LootItem("explosive.timed",             1, 1),
            new LootItem("ammo.rocket.mlrs",            1, 2),
            new LootItem("explosives",                  2, 4),
        };

        private static readonly List<LootItem> ChinookWeapons = new List<LootItem>
        {
            new LootItem("smg.mp5",                     1, 1),
            new LootItem("shotgun.spas12",              1, 1),
            new LootItem("pistol.m92",                  1, 1),
            new LootItem("rifle.m39",                   1, 1),
            new LootItem("rifle.lr300",                 1, 1),
            new LootItem("pistol.prototype17",          1, 1),
            new LootItem("rifle.ak",                    1, 1),
            new LootItem("rifle.bolt",                  1, 1),
            new LootItem("hmlmg",                       1, 1),
            new LootItem("rifle.sks",                   1, 1),
        };

        private static readonly List<LootItem> ChinookExplosives = new List<LootItem>
        {
            new LootItem("explosive.satchel",           2, 4),
            new LootItem("explosive.timed",             1, 2),
            new LootItem("explosives",                  1, 5),
        };

        private static readonly List<LootItem> ChinookInstruments = new List<LootItem>
        {
            new LootItem("chainsaw",                    1, 1),
            new LootItem("jackhammer",                  1, 1),
        };

        private static readonly List<LootItem> ChinookAmmo = new List<LootItem>
        {
            new LootItem("ammo.pistol",                 30, 40),
            new LootItem("ammo.shotgun",                8, 12),
            new LootItem("ammo.rifle",                  50, 70),
            new LootItem("ammo.rifle.incendiary",       60, 70),
            new LootItem("ammo.rifle.explosive",        30, 50),
            new LootItem("ammo.rifle.hv",               40, 60),
            new LootItem("grenade.f1",                  2, 5),
        };

        private static readonly List<LootItem> ChinookArmor = new List<LootItem>
        {
            new LootItem("metal.facemask",              1, 1),
            new LootItem("metal.plate.torso",           1, 1),
        };

        private static readonly List<LootItem> ChinookComponents = new List<LootItem>
        {
            new LootItem("gears",                       5, 7),
            new LootItem("metalpipe",                   5, 7),
            new LootItem("sheetmetal",                  5, 7),
            new LootItem("techparts",                   4, 7),
        };

        private static readonly List<LootItem> ChinookDeployables = new List<LootItem>
        {
            new LootItem("door.double.hinged.toptier",      1, 1),
            new LootItem("door.hinged.toptier",             1, 1),
            new LootItem("floor.ladder.hatch",              1, 1),
            new LootItem("wall.window.bars.toptier",        1, 1),
            new LootItem("floor.ladder.hatch.toptier",      1, 1),
            new LootItem("floor.triangle.ladder.hatch.toptier", 1, 1),
            new LootItem("floor.triangle.ladder.hatch",     1, 1),
        };

        private static readonly List<LootItem> ChinookWeaponMods = new List<LootItem>
        {
            new LootItem("weapon.mod.lasersight",       1, 1),
            new LootItem("weapon.mod.small.scope",      1, 1),
            new LootItem("weapon.mod.silencer",         1, 1),
        };

        private static readonly List<LootItem> ChinookAdditional = new List<LootItem>
        {
            new LootItem("aiming.module.mlrs",          1, 1),
            new LootItem("supply.signal",               1, 1),
            new LootItem("autoturret",                  1, 1),
            new LootItem("clothing.mod.armorinsert_metal", 1, 1),
        };

        private static readonly List<LootItem> FoodCacheLoot = new List<LootItem>
        {
            // Teas
            new LootItem("healingtea",          1, 2),
            new LootItem("oretea",              1, 2),
            new LootItem("radiationresisttea",  1, 1),
            new LootItem("scraptea",            1, 2),
            new LootItem("woodtea",             1, 2),
            // Food
            new LootItem("apple",               1, 2),
            new LootItem("bread.loaf",          1, 2),
            new LootItem("can.beans",           1, 2),
            new LootItem("can.tuna",            1, 2),
            new LootItem("chocolate",           1, 2),
            new LootItem("granolabar",          1, 2),
            new LootItem("smallwaterbottle",    1, 1),
            new LootItem("potato",              1, 2),
            new LootItem("corn",                1, 2),
            new LootItem("honeycomb",           1, 2),
            new LootItem("waterjug",            1, 1),
            new LootItem("pumpkin",             1, 2),
            // Cooked Meat
            new LootItem("chicken.cooked",      1, 2),
            new LootItem("deermeat.cooked",     1, 2),
            new LootItem("bearmeat.cooked",     1, 2),
            new LootItem("wolfmeat.cooked",     1, 2),
            new LootItem("meat.pork.cooked",    1, 2),
            // Berries
            new LootItem("green.berry",         1, 2),
            new LootItem("red.berry",           1, 2),
            new LootItem("white.berry",         1, 2),
            new LootItem("yellow.berry",        1, 2),
        };

        private static readonly List<LootItem> BarrelClothing = new List<LootItem>
        {
            new LootItem("burlap.gloves",   1, 1),
            new LootItem("hat.beenie",      1, 1),
            new LootItem("hat.boonie",      1, 1),
            new LootItem("hat.cap",         1, 1),
            new LootItem("mask.balaclava",  1, 1),
            new LootItem("mask.bandana",    1, 1),
            new LootItem("pants.shorts",    1, 1),
        };

        private static readonly List<LootItem> BarrelDeployables = new List<LootItem>
        {
            new LootItem("barricade.wood",      1, 1),
            new LootItem("fireplace.stone",     1, 1),
            new LootItem("spikes.floor",        1, 1),
            new LootItem("water.barrel",        1, 1),
            new LootItem("mailbox",             1, 1),
            new LootItem("planter.large",       1, 1),
            new LootItem("planter.triangle",    1, 1),
            new LootItem("electric.igniter",    1, 1),
            new LootItem("rug.bear",            1, 1),
            new LootItem("rug",                 1, 1),
            new LootItem("shelves",             1, 1),
            new LootItem("sign.wooden.huge",    1, 1),
            new LootItem("sign.wooden.large",   1, 1),
            new LootItem("tool.binoculars",     1, 1),
            new LootItem("paddle",              1, 1),
            new LootItem("bucket.water",        1, 1),
        };

        private static readonly List<LootItem> BarrelComponents = new List<LootItem>
        {
            new LootItem("rope",        1, 4),
            new LootItem("metalblade",  2, 2),
            new LootItem("propanetank", 2, 2),
            new LootItem("tarp",        1, 3),
            new LootItem("sewingkit",   3, 6),
            new LootItem("gears",       2, 4),
            new LootItem("metalpipe",   1, 6),
            new LootItem("metalspring", 1, 3),
            new LootItem("roadsigns",   2, 5),
            new LootItem("sheetmetal",  1, 3),
            new LootItem("semibody",    1, 2),
            new LootItem("fuse",        1, 2),
        };

        // ─── Hook ────────────────────────────────────────────────────────────────

        private void OnLootSpawn(LootContainer container)
        {
            if (container == null) return;

            string prefab = container.PrefabName;
            if (BarrelPrefabs.Contains(prefab))
            {
                PopulateBarrelWeighted(container);
            }
            else if (TrashPilePrefabs.Contains(prefab))
            {
                PopulateTrashPile(container);
            }
            else if (LootTrashPrefabs.Contains(prefab))
            {
                PopulateWithNextTick(container, LootTrashLoot, 1, 0);
            }
            else if (OilBarrelPrefabs.Contains(prefab))
            {
                PopulateOilBarrel(container);
            }
            else if (RoadsignPrefabs.Contains(prefab))
            {
                PopulateRoadsign(container);
            }
            else if (CrateBasicPrefabs.Contains(prefab))
            {
                PopulateWithNextTick(container, CrateBasicLoot, 2, 5);
            }
            else if (CrateBasicJunglePrefabs.Contains(prefab))
            {
                PopulateCrateBasicJungle(container);
            }
            else if (CrateMinePrefabs.Contains(prefab))
            {
                PopulateCrateMine(container);
            }
            else if (CrateNormal2Prefabs.Contains(prefab))
            {
                PopulateCrateNormal2(container);
            }
            else if (CrateNormal2FoodPrefabs.Contains(prefab))
            {
                PopulateCrateNormal2Food(container);
            }
            else if (CrateNormal2MedicalPrefabs.Contains(prefab))
            {
                PopulateCrateNormal2Medical(container);
            }
            else if (CrateNormalPrefabs.Contains(prefab))
            {
                PopulateCrateNormal(container);
            }
            else if (CrateToolsPrefabs.Contains(prefab))
            {
                PopulateCrateTools(container);
            }
            else if (HeliCratePrefabs.Contains(prefab))
            {
                PopulateHeli(container);
            }
            else if (VehiclePartsPrefabs.Contains(prefab))
            {
                PopulateVehiclePartsNextTick(container);
            }
            else if (SupplyDropPrefabs.Contains(prefab))
            {
                PopulateSupplyDrop(container);
            }
        }

        // ─── Trash pile population ──────────────────────────────────────────────────

        private void PopulateTrashPile(LootContainer container)
        {
            if (container.inventory == null) return;

            container.inventory.Clear();
            ItemManager.DoRemoves();

            NextTick(() =>
            {
                if (container == null || container.inventory == null) return;

                container.inventory.Clear();
                ItemManager.DoRemoves();

                var rng = new System.Random();
                var pool = TrashPileLoot.ToList();

                for (int i = 0; i < 3; i++)
                {
                    var pick = pool[rng.Next(pool.Count)];
                    int amount = rng.Next(pick.Min, pick.Max + 1);
                    GiveItem(container, pick.Shortname, amount);
                }

                container.inventory.MarkDirty();
            });
        }

        // ─── Elite crate population ─────────────────────────────────────────────────

        private void PopulateElite(LootContainer container)
        {
            if (container.inventory == null) return;

            container.inventory.Clear();
            ItemManager.DoRemoves();

            var rng = new System.Random();
            var pool = CrateEliteLoot.ToList();

            for (int i = 0; i < 7; i++)
            {
                if (pool.Count == 0) break;
                var pick = pool[rng.Next(pool.Count)];
                int amount = rng.Next(pick.Min, pick.Max + 1);
                GiveItem(container, pick.Shortname, amount);
            }

            GiveItem(container, "scrap", 30);

            if (rng.Next(0, 100) < 10)
                GiveItem(container, "advancedblueprintfragment", 1);

            container.inventory.MarkDirty();
        }

        // ─── Generic population ─────────────────────────────────────────────────────

        private void PopulateGeneric(LootContainer container, List<LootItem> pool, int itemCount, int scrap, int blueprintChance)
        {
            if (container.inventory == null) return;

            container.inventory.Clear();
            ItemManager.DoRemoves();

            var rng = new System.Random();
            var available = pool.ToList();

            for (int i = 0; i < itemCount; i++)
            {
                if (available.Count == 0) break;
                var pick = available[rng.Next(available.Count)];
                int amount = rng.Next(pick.Min, pick.Max + 1);
                GiveItem(container, pick.Shortname, amount);
            }

            if (scrap > 0)
                GiveItem(container, "scrap", scrap);

            if (blueprintChance > 0 && rng.Next(0, 100) < blueprintChance)
            {
                GiveItem(container, "basicblueprintfragment", 1);
            }

            container.inventory.MarkDirty();
        }

        // ─── Barrel population ───────────────────────────────────────────────────────

        private void PopulateBarrelWeighted(LootContainer container)
        {
            if (container.inventory == null) return;

            container.inventory.Clear();
            ItemManager.DoRemoves();

            NextTick(() =>
            {
                if (container == null || container.inventory == null) return;

                container.inventory.Clear();
                ItemManager.DoRemoves();

                var rng = new System.Random();
                int roll = rng.Next(0, 100);
                List<LootItem> pool;

                if (roll < 60)
                    pool = BarrelComponents;
                else if (roll < 80)
                    pool = BarrelDeployables;
                else
                    pool = BarrelClothing;

                var pick = pool[rng.Next(pool.Count)];
                int amount = rng.Next(pick.Min, pick.Max + 1);
                GiveItem(container, pick.Shortname, amount);

                GiveItem(container, "scrap", 4);

                container.inventory.MarkDirty();
            });
        }

        // ─── Crate Tools population ──────────────────────────────────────────────────

        private void PopulateCrateTools(LootContainer container)
        {
            if (container.inventory == null) return;

            container.inventory.Clear();
            ItemManager.DoRemoves();

            NextTick(() =>
            {
                if (container == null || container.inventory == null) return;

                container.inventory.Clear();
                ItemManager.DoRemoves();

                var rng = new System.Random();

                // Roll 1 tool or weapon
                var weaponPick = CrateToolsWeapons[rng.Next(CrateToolsWeapons.Count)];
                GiveItem(container, weaponPick.Shortname, 1);

                if (weaponPick.Shortname == "chainsaw")
                {
                    // Exception: chainsaw gets lowgradefuel instead of second roll
                    int fuel = rng.Next(20, 51);
                    GiveItem(container, "lowgradefuel", fuel);
                }
                else
                {
                    // Roll 1 item from other categories
                    var otherPick = CrateToolsOther[rng.Next(CrateToolsOther.Count)];
                    int amount = rng.Next(otherPick.Min, otherPick.Max + 1);
                    GiveItem(container, otherPick.Shortname, amount);
                }

                // 7-8 scrap
                GiveItem(container, "scrap", rng.Next(7, 9));

                // 2% blueprint fragment
                if (rng.Next(0, 100) < 2)
                    GiveItem(container, "basicblueprintfragment", 1);

                container.inventory.MarkDirty();
            });
        }

        // ─── Oil barrel population ───────────────────────────────────────────────────

        private void PopulateOilBarrel(LootContainer container)
        {
            if (container.inventory == null) return;

            container.inventory.Clear();
            ItemManager.DoRemoves();

            NextTick(() =>
            {
                if (container == null || container.inventory == null) return;

                container.inventory.Clear();
                ItemManager.DoRemoves();

                var rng = new System.Random();
                GiveItem(container, "crude.oil", rng.Next(15, 26));
                GiveItem(container, "lowgradefuel", rng.Next(8, 16));

                container.inventory.MarkDirty();
            });
        }

        // ─── Crate Normal 2 Medical population ──────────────────────────────────────────

        private void PopulateCrateNormal2Medical(LootContainer container)
        {
            if (container.inventory == null) return;

            container.inventory.Clear();
            ItemManager.DoRemoves();

            NextTick(() =>
            {
                if (container == null || container.inventory == null) return;

                container.inventory.Clear();
                ItemManager.DoRemoves();

                var rng = new System.Random();
                var pool = CrateNormal2MedicalLoot.ToList();

                var first = pool[rng.Next(pool.Count)];
                GiveItem(container, first.Shortname, rng.Next(first.Min, first.Max + 1));

                if (first.Shortname == "bandage")
                    pool = pool.Where(x => x.Shortname != "bandage").ToList();

                if (pool.Count > 0)
                {
                    var second = pool[rng.Next(pool.Count)];
                    GiveItem(container, second.Shortname, rng.Next(second.Min, second.Max + 1));
                }

                container.inventory.MarkDirty();
            });
        }

        // ─── Crate Normal 2 Food population ─────────────────────────────────────────────

        private void PopulateCrateNormal2Food(LootContainer container)
        {
            if (container.inventory == null) return;

            container.inventory.Clear();
            ItemManager.DoRemoves();

            NextTick(() =>
            {
                if (container == null || container.inventory == null) return;

                container.inventory.Clear();
                ItemManager.DoRemoves();

                var rng = new System.Random();
                var usedWater = new System.Collections.Generic.HashSet<string>();
                var pool = CrateNormal2FoodLoot.ToList();

                System.Action roll = () =>
                {
                    var available = pool.Where(x => {
                        if (x.Shortname == "smallwaterbottle" || x.Shortname == "waterjug")
                            return !usedWater.Contains(x.Shortname);
                        return true;
                    }).ToList();
                    if (available.Count == 0) return;
                    var pick = available[rng.Next(available.Count)];
                    if (pick.Shortname == "smallwaterbottle" || pick.Shortname == "waterjug")
                        usedWater.Add(pick.Shortname);
                    GiveItem(container, pick.Shortname, rng.Next(pick.Min, pick.Max + 1));
                };

                roll();
                roll();
                if (rng.Next(0, 100) < 50) roll();

                container.inventory.MarkDirty();
            });
        }

        // ─── Crate Normal 2 population ───────────────────────────────────────────────

        private void PopulateCrateNormal2(LootContainer container)
        {
            if (container.inventory == null) return;

            container.inventory.Clear();
            ItemManager.DoRemoves();

            NextTick(() =>
            {
                if (container == null || container.inventory == null) return;

                container.inventory.Clear();
                ItemManager.DoRemoves();

                var rng = new System.Random();

                // 1 guaranteed component
                PickUnique(container, CrateNormal2Components, 1, rng);

                // 1 roll: 80% weapons/grenades/armor/tools, 20% electronics/other
                if (rng.Next(0, 100) < 80)
                {
                    PickUnique(container, CrateNormal2WeaponsArmorTools, 1, rng);
                }
                else
                {
                    PickUnique(container, CrateNormal2ElectronicsOther, 1, rng);
                }

                // Scrap 5-8
                int scrap = rng.Next(5, 9);
                GiveItem(container, "scrap", scrap);

                // 2% basicblueprintfragment
                if (rng.Next(0, 100) < 2)
                {
                    GiveItem(container, "basicblueprintfragment", 1);
                }
                container.inventory.MarkDirty();
            });
        }

        // ─── Crate Normal population ─────────────────────────────────────────────────

        private void PopulateCrateNormal(LootContainer container)
        {
            if (container.inventory == null) return;

            container.inventory.Clear();
            ItemManager.DoRemoves();

            NextTick(() =>
            {
                if (container == null || container.inventory == null) return;

                container.inventory.Clear();
                ItemManager.DoRemoves();

                var rng = new System.Random();

                // 1 guaranteed armor
                PickUnique(container, CrateNormalArmor, 1, rng);

                // 1 guaranteed component
                PickUnique(container, CrateNormalComponents, 1, rng);

                // 1 random from full pool excluding weapons
                var fullPool = new List<LootItem>();
                fullPool.AddRange(CrateNormalArmor);
                fullPool.AddRange(CrateNormalComponents);
                fullPool.AddRange(CrateNormalElectronics);
                fullPool.AddRange(CrateNormalDeployables);
                fullPool.AddRange(CrateNormalTools);
                fullPool.AddRange(CrateNormalAmmo);
                fullPool.AddRange(CrateNormalWeaponMods);
                fullPool.AddRange(CrateNormalOther);
                fullPool.AddRange(CrateNormalUpgrades);
                PickUnique(container, fullPool, 1, rng);

                // 50% chance of 1 weapon
                if (rng.Next(0, 100) < 50)
                    PickUnique(container, CrateNormalWeapons, 1, rng);

                // Scrap 0-30
                int scrap = rng.Next(0, 31);
                if (scrap > 0)
                    GiveItem(container, "scrap", scrap);

                // 10% basicblueprintfragment
                if (rng.Next(0, 100) < 10)
                    GiveItem(container, "basicblueprintfragment", 1);

                container.inventory.MarkDirty();
            });
        }

        // ─── Crate Mine population ───────────────────────────────────────────────────

        private void PopulateCrateMine(LootContainer container)
        {
            if (container.inventory == null) return;

            container.inventory.Clear();
            ItemManager.DoRemoves();

            NextTick(() =>
            {
                if (container == null || container.inventory == null) return;

                container.inventory.Clear();
                ItemManager.DoRemoves();

                var rng = new System.Random();
                var usedTools = new System.Collections.Generic.HashSet<string>();

                for (int i = 0; i < 2; i++)
                {
                    if (rng.Next(0, 100) < 40)
                    {
                        // Tools pool - no duplicates
                        var available = CrateMineTools.Where(x => !usedTools.Contains(x.Shortname)).ToList();
                        if (available.Count > 0)
                        {
                            var pick = available[rng.Next(available.Count)];
                            usedTools.Add(pick.Shortname);
                            GiveItem(container, pick.Shortname, rng.Next(pick.Min, pick.Max + 1));
                        }
                    }
                    else
                    {
                        // Resources pool - duplicates allowed
                        var pick = CrateMineResources[rng.Next(CrateMineResources.Count)];
                        GiveItem(container, pick.Shortname, rng.Next(pick.Min, pick.Max + 1));
                    }
                }

                GiveItem(container, "scrap", 2);
                container.inventory.MarkDirty();
            });
        }

        // ─── Crate Basic Jungle population ───────────────────────────────────────────

        private void PopulateCrateBasicJungle(LootContainer container)
        {
            if (container.inventory == null) return;

            container.inventory.Clear();
            ItemManager.DoRemoves();

            NextTick(() =>
            {
                if (container == null || container.inventory == null) return;

                container.inventory.Clear();
                ItemManager.DoRemoves();

                var rng = new System.Random();

                // Guaranteed blowpipe
                GiveItem(container, "blowpipe", 1);

                // 1 from ammo pool
                PickUnique(container, CrateBasicJungleAmmo, 1, rng);

                // 1 from other/medical pool
                PickUnique(container, CrateBasicJungleOther, 1, rng);

                // 5 guaranteed scrap
                GiveItem(container, "scrap", 5);

                container.inventory.MarkDirty();
            });
        }

        // ─── Generic NextTick population ────────────────────────────────────────────

        private void PopulateWithNextTick(LootContainer container, List<LootItem> pool, int itemCount, int scrap)
        {
            if (container.inventory == null) return;

            container.inventory.Clear();
            ItemManager.DoRemoves();

            NextTick(() =>
            {
                if (container == null || container.inventory == null) return;

                container.inventory.Clear();
                ItemManager.DoRemoves();

                var rng = new System.Random();
                var available = pool.ToList();

                for (int i = 0; i < itemCount; i++)
                {
                    if (available.Count == 0) break;
                    var pick = available[rng.Next(available.Count)];
                    int amount = rng.Next(pick.Min, pick.Max + 1);
                    GiveItem(container, pick.Shortname, amount);
                }

                if (scrap > 0)
                    GiveItem(container, "scrap", scrap);

                container.inventory.MarkDirty();
            });
        }

        // ─── Chinook crate population ────────────────────────────────────────────────

        private void PopulateChinook(LootContainer container)
        {
            if (container.inventory == null) return;

            timer.Once(0.5f, () =>
            {
                if (container == null || container.inventory == null) return;

                container.inventory.Clear();
                ItemManager.DoRemoves();

                var rng = new System.Random();

                PickUnique(container, ChinookWeapons, 2, rng);
                PickUnique(container, ChinookExplosives, 1, rng);
                PickUnique(container, ChinookInstruments, 1, rng);
                PickUnique(container, ChinookAmmo, 1, rng);
                PickUnique(container, ChinookArmor, 1, rng);
                PickUnique(container, ChinookComponents, 1, rng);
                PickUnique(container, ChinookDeployables, 1, rng);
                PickUnique(container, ChinookWeaponMods, 1, rng);
                var fullPool = new List<LootItem>();
                fullPool.AddRange(ChinookWeapons);
                fullPool.AddRange(ChinookExplosives);
                fullPool.AddRange(ChinookInstruments);
                fullPool.AddRange(ChinookAmmo);
                fullPool.AddRange(ChinookArmor);
                fullPool.AddRange(ChinookComponents);
                fullPool.AddRange(ChinookDeployables);
                fullPool.AddRange(ChinookWeaponMods);
                fullPool.AddRange(ChinookAdditional);

                var available = fullPool.ToList();
                for (int i = 0; i < 2; i++)
                {
                    if (available.Count == 0) break;
                    var pick = available[rng.Next(available.Count)];
                    int amount = rng.Next(pick.Min, pick.Max + 1);
                    GiveItem(container, pick.Shortname, amount);
                }
                GiveItem(container, "advancedblueprintfragment", 2);
                GiveItem(container, "scrap", rng.Next(140, 261));
                GiveItem(container, "metal.refined", rng.Next(32, 61));
                if (rng.Next(0, 100) < 2)
                    GiveItem(container, "rifle.l96", 1);

                container.inventory.MarkDirty();
            });
        }

        // ─── Vehicle parts population ────────────────────────────────────────────────

        private void PopulateVehiclePartsNextTick(LootContainer container)
        {
            if (container.inventory == null) return;

            container.inventory.Clear();
            ItemManager.DoRemoves();

            NextTick(() =>
            {
                if (container == null || container.inventory == null) return;

                container.inventory.Clear();
                ItemManager.DoRemoves();

                var rng = new System.Random();
                PickUnique(container, VehiclePartsLoot, 2, rng);

                GiveItem(container, "scrap", rng.Next(8, 11));

                container.inventory.MarkDirty();
            });
        }

        private void PopulateVehicleParts(LootContainer container)
        {
            if (container.inventory == null) return;

            container.inventory.Clear();
            ItemManager.DoRemoves();

            var rng = new System.Random();
            var pool = VehiclePartsLoot.ToList();

            for (int i = 0; i < 2; i++)
            {
                if (pool.Count == 0) break;
                var pick = pool[rng.Next(pool.Count)];
                GiveItem(container, pick.Shortname, 1);
            }

            int scrap = rng.Next(8, 11);
            GiveItem(container, "scrap", scrap);

            container.inventory.MarkDirty();
        }

        // ─── Roadsign population ─────────────────────────────────────────────────────

        private void PopulateRoadsign(LootContainer container)
        {
            if (container.inventory == null) return;

            container.inventory.Clear();
            ItemManager.DoRemoves();

            NextTick(() =>
            {
                if (container == null || container.inventory == null) return;

                container.inventory.Clear();
                ItemManager.DoRemoves();

                var rng = new System.Random();
                var pool = RoadsignLoot.ToList();

                for (int i = 0; i < 2; i++)
                {
                    var pick = pool[rng.Next(pool.Count)];
                    int amount = rng.Next(pick.Min, pick.Max + 1);
                    GiveItem(container, pick.Shortname, amount);
                }

                int scrap = rng.Next(0, 3);
                if (scrap > 0)
                    GiveItem(container, "scrap", scrap);

                container.inventory.MarkDirty();
            });
        }

        // ─── Supply drop population ──────────────────────────────────────────────────

        private void PopulateSupplyDrop(LootContainer container)
        {
            if (container.inventory == null) return;

            container.inventory.Clear();
            ItemManager.DoRemoves();

            NextTick(() =>
            {
                if (container == null || container.inventory == null) return;

                container.inventory.Clear();
                ItemManager.DoRemoves();

                var rng = new System.Random();

                // 2 guaranteed weapons
                PickUnique(container, SupplyDropWeapons, 2, rng);

                // 2 guaranteed clothing
                PickUnique(container, SupplyDropClothing, 2, rng);

                // 1 guaranteed component
                PickUnique(container, SupplyDropComponents, 1, rng);

                // 2 guaranteed ammo
                PickUnique(container, SupplyDropAmmo, 2, rng);

                // 1 guaranteed explosive
                PickUnique(container, SupplyDropExplosives, 1, rng);

                // 1 guaranteed tool
                PickUnique(container, SupplyDropTools, 1, rng);

                // 1 guaranteed weapon mod
                PickUnique(container, SupplyDropWeaponMods, 1, rng);

                // 3 random from full pool
                var fullPool = new List<LootItem>();
                fullPool.AddRange(SupplyDropWeapons);
                fullPool.AddRange(SupplyDropClothing);
                fullPool.AddRange(SupplyDropComponents);
                fullPool.AddRange(SupplyDropAmmo);
                fullPool.AddRange(SupplyDropExplosives);
                fullPool.AddRange(SupplyDropTools);
                fullPool.AddRange(SupplyDropWeaponMods);
                fullPool.AddRange(SupplyDropOther);
                PickUnique(container, fullPool, 3, rng);

                // Guaranteed metal.refined
                int metalRefined = rng.Next(36, 57);
                GiveItem(container, "metal.refined", metalRefined);

                // Guaranteed scrap
                int scrap = rng.Next(140, 261);
                GiveItem(container, "scrap", scrap);

                // Guaranteed blueprint fragments
                int bpAmount = rng.Next(1, 4);
                GiveItem(container, "basicblueprintfragment", bpAmount);

                container.inventory.MarkDirty();
            });
        }

        // ─── Heli crate population ───────────────────────────────────────────────────

        private void PopulateHeli(LootContainer container)
        {
            if (container.inventory == null) return;

            container.inventory.Clear();
            ItemManager.DoRemoves();

            NextTick(() =>
            {
                if (container == null || container.inventory == null) return;

                container.inventory.Clear();
                ItemManager.DoRemoves();

                var rng = new System.Random();

                // 40% chance of 1 special item (rocket/c4/explosives)
                if (rng.Next(0, 100) < 40)
                {
                    var pick = HeliSpecialPool[rng.Next(HeliSpecialPool.Count)];
                    GiveItem(container, pick.Shortname, rng.Next(pick.Min, pick.Max + 1));
                }

                // 1 guaranteed weapon
                PickUnique(container, HeliWeapons, 1, rng);

                // 2 guaranteed ammo
                PickUnique(container, HeliAmmo, 2, rng);

                // 1 random from weapons + weapon mods + other
                var fullPool = new List<LootItem>();
                fullPool.AddRange(HeliWeapons);
                fullPool.AddRange(HeliWeaponMods);
                fullPool.AddRange(HeliOther);
                PickUnique(container, fullPool, 1, rng);

                // 10% chance advancedblueprintfragment
                if (rng.Next(0, 100) < 10)
                    GiveItem(container, "advancedblueprintfragment", 1);

                container.inventory.MarkDirty();
            });
        }

        // ─── Bradley crate population ────────────────────────────────────────────────

        private void PopulateBradley(LootContainer container)
        {
            if (container.inventory == null) return;

            container.inventory.Clear();
            ItemManager.DoRemoves();

            var rng = new System.Random();

            // Guaranteed rocket or c4
            var rocketPick = RocketC4Pool[rng.Next(RocketC4Pool.Count)];
            GiveItem(container, rocketPick.Shortname, rng.Next(rocketPick.Min, rocketPick.Max + 1));

            // 8 random items
            var pool = BradleyCrateLoot.ToList();
            for (int i = 0; i < 8; i++)
            {
                if (pool.Count == 0) break;
                var pick = pool[rng.Next(pool.Count)];
                int amount = rng.Next(pick.Min, pick.Max + 1);
                GiveItem(container, pick.Shortname, amount);
            }

            container.inventory.MarkDirty();
        }



        // ─── Helper ──────────────────────────────────────────────────────────────

        private void PickUnique(LootContainer container, List<LootItem> pool, int count, System.Random rng)
        {
            var available = pool.ToList();
            for (int i = 0; i < count; i++)
            {
                if (available.Count == 0) break;
                int idx = rng.Next(available.Count);
                var pick = available[idx];
                available.RemoveAt(idx);
                int amount = rng.Next(pick.Min, pick.Max + 1);
                GiveItem(container, pick.Shortname, amount);
            }
        }

        private void GiveItem(LootContainer container, string shortname, int amount)
        {
            var item = ItemManager.CreateByName(shortname, amount);
            if (item == null)
            {
                PrintWarning($"[CalmLoot] Could not create item: {shortname}");
                return;
            }
            item.MoveToContainer(container.inventory);
        }
    }
}
