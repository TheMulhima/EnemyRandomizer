﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

using nv;

//disable the unreachable code detected warning for this file
#pragma warning disable 0162

namespace EnemyRandomizerMod
{
    public class EnemyRandomizerDatabase
    {
        public static EnemyRandomizerDatabase Instance { get; private set; }

        CommunicationNode comms;

        public Dictionary<string, List<string>> enemyTypes = new Dictionary<string, List<string>>();
        public List<GameObject> loadedEnemyPrefabs = new List<GameObject>();
        public List<string> loadedEnemyPrefabNames = new List<string>();
        public List<string> uniqueEnemyTypes = new List<string>();

        public Dictionary<string, GameObject> loadedEffectPrefabs = new Dictionary<string, GameObject>();

        public EnemyRandomizerDatabase()
        {
        }

        public void Setup()
        {
            Instance = this;
            comms = new CommunicationNode();
            comms.EnableNode( this );
        }

        public void Unload()
        {
            comms.DisableNode();
            Instance = null;
        }

        /* 
         * Zones by build index__
         * 
         * Tutorial: 7 
         * Crossroads: Scenes 38-80         
         * Ruins_House (city of tears internal): 81-85  
         * Ruins (City of tears): 86 - 116
         * Greenpath/Fungal Wastes/Queens Gardens: 117 - 212
         * Howling Cliffs: 213 - 218
         * Resting Grounds: 219 - 228
         * Crystal Peak Mines: 229 - 256
         * Deepnest: 257 - 302
         * Abyss: 304 - 323
         * Waterways: 326 - 339
         * White Palace: 340 - 358
         * Hive: 359 - 363
         * 
         */

        public const bool USE_TEST_SCENES = true;


        public static List<int> EnemyTypeScenes {
            get {
                if( USE_TEST_SCENES )
                    return testTypeScenes;
                else
                    return enemyTypeScenes;
            }
        }

        static List<int> testTypeScenes = new List<int>()
        {
            //7, //test flame spawn
            241,
            244, //test crystal guardian and Zombie Beam Miner(244)
            34,
            //276, //testing tiny spider
            183, //testing garden zombie

            //test these for mage??
            96  //testing mage?
        };

        static List<int> enemyTypeScenes = new List<int>()
        {
            241,  //LOAD THIS SCENE FIRST -- has laser impact special objects that we want

            7,//tutorial scene

            28,
            33,
            34,
            35,
            36,

            //38,//mender bug
            40,
            41,
            46,
            47,
            49,
            52,
            55,
            58,
            71,
            74,
            76,
            82,
            88,//Ruins Flying Sentry
            90,//(should have Ruins Flying Sentry Javelin (2) )
            97,
            102,
            106,
            115,
            117,
            121,
            126,//(should have Acid Flyer???)
            127,
            135,//should have Fat Fly???
            137,
            142,//lazy flyer enemy
            144,
            146,
            155,
            158,
            164,//(Mantis Flyer Child)
            166,
            167,
            177,
            181,//(fungus flyer)
            183,
            186,//(moss flyer)
            189,
            194,
            204,
            208,
            226,
            //232,//(crystal flyer) -- TEST, don't seem to need this!
            234,

            243,
            259,
            269,
            271,
            276,//(supposed to have spider flyer)
            285,
            302,
            313,
            320,
            321,
            326,//(Inflater)
            327,
            331,
            336,
            340,
            358,
            362
        };



        //TODO: place health cocoons around places
        //for most of these objects, set the "PersistentBoolItem" component to = that of the original
        public static List<string> effectObjectNames = new List<string>()
        {
            //load from crystal guardian scene 241
            "Beam Impact",
            "Beam Ball",
            "Beam Point L",
            "Beam Point R",
            "Beam",
            "Crystal Rain"
        };


        //TODO: place health cocoons around places
        //for most of these objects, set the "PersistentBoolItem" component to = that of the original
        public static List<string> miscObjectNames = new List<string>()
        {
            //"Cocoon Plant 1", //??? also part of health cocoon??? -- OH this is just the butterfly looking plants
            "Health Cocoon", //scene 7, default rotation is hanging from roof
            //check if it has the "HealthCocoon" component
            //does have PersistentBoolItem
        
            //"Chest" //scene 7, by default contains a charm -- check if it has a "Chest Control" FSM
            //mess with this after the content patch, right now it would use FSMs to modify... too much work :P

            "Geo Rock"//scene 7 -- basic crossroads geo rock
            //Example: "Geo Rock 3" is the rock just to the left of start
            //check if it has "Geo Rock" FSM
            //does not have? PersistentBoolItem
        };

        public static List<string> enemyTypeNames = new List<string>()
        {
            //Fungus shaman?
            "Flamebearer Spawn",//7 -- NEEDS TESTING

            "Colosseum_Armoured_Mosquito",//34
            "Mosquito",//28
            
            "Colosseum_Miner",//35

            //Colosseum
            "Giant Buzzer Col",//33
            "Angry Buzzer",//35
            "Buzzer",//74

            "Colosseum_Shield_Zombie",//33
            "Super Spitter Col",//33
            "Super Spitter",//34
            "Spitter", //40

            "Bursting Bouncer",//33
            
            "Ceiling Dropper Col",//34
            "Ceiling Dropper", //??(forgot)
            
            "Giant Fly Col",//34 
            //"Giant Fly",// (gruz mother, it starts sleeping and looks weird...)
            //"Fly Spawn",
            "Fat Fly", //146 --doesn't seem to be here? --try scene 135???
            "Blow Fly", //285
            //"Fluke Fly Spawner", //
            "Fluke Fly", //327
            //"White Palace Fly", //340 (may have to remove randomization for this too)
            "Fly",

            "Grub Mimic",//34
            "Mega Fat Bee",//34
            "Colosseum_Flying_Sentry",//34
            "Spitting Zombie",//34 and 41

            "Colosseum Grass Hopper",//35
            "Giant Hopper",//34
            "Grass Hopper", //208
            "Flip Hopper", //
            "Hopper",//34
            "Blobble",//34

            "Lancer",//35
            "Lobster",//35
            
            //"Mage Balloon Spawner",//35
            //"Mage Knight",//35(96) -- TEMP
            //"Mage Blob",//35(96) -- TEMP
            //"Mage Balloon", //102 -- TEMP
            //"Mage Lord Phase2", //97 (BOSS)
            //"Mage Lord", //97 (BOSS)
            "Electric Mage",//35
            "Mage",//35(96)

            "Lesser Mawlek",//35
            "Mawlek Col",//35 ??? might be same as lesser mawlek
            "Mawlek Body", //46 (BOSS)

            //Crossroads
            "Bursting Zombie",//36
            //"Mender Bug",//38?(not 36?)
            
            "Fung Crawler", //158
            "Mines Crawler", //243
            //"Crystal Crawler", //243 -- TEMP
            "Abyss Crawler", //320
            "Crawler", //40

            "Climber", //40
            
            "Zombie Hornhead Sp", //271
            "Zombie Hornhead", //41 (Giant Fly = boss)


            // "Head",//turns out this is just a head....
            "Prayer Slug", //47 prayer slugs = maggots
                        
            "Colosseum_Worm",//34
            "Worm", //49
            
            //"Hatcher Baby Spawner", //55
            "Hatcher",//52

            "Zombie Shield", //52
            "Zombie Leaper",//52
            "Zombie Myla", //71 -- seems to be killing enemies it replaces?? maybe remove from replacement list
            "Blocker",//74 (baulder shell)

            "Zombie Guard", //76 (big enemy)
            
            "Colosseum_Armoured_Roller",//33
            "Mushroom Roller", //
            "Roller", //74

            //City of tears
            "Gorgeous Husk", //82
            
            "Zombie Runner Sp", //271 ???
            "Zombie Runner", //90

            "Zombie Barger", //90
            
            "Ruins Sentry Fat", //90
            "Ruins Sentry", //90


            "Great Shield Zombie", //106

            "Royal Zombie Coward", //106
            "Royal Zombie Fat", //106
            "Royal Zombie", //106

            "Ruins Flying Sentry Javelin", //106
            "Ruins Flying Sentry", //106
            
            //"Jar Collector", //115 (boss)

            //Greenapth
            "Moss Walker", //117
            "Mossman_Shaker", //117
            "Pigeon", //117 (don't randomize, while technically enemies, kinda ends up being lame)
            "Plant Trap", //117
            "Mossman_Runner",//117

            "Acid Flyer", //126
            "Shell", //126 (don't randomize this, since it's key to a couple encounters)
            
            "Plant Turret", //127
            //"Acid Walker", //127 (don't randomize this, since it's fairly key to getting around a few places)
            

            "Moss Knight Fat", //204
            "Moss Knight", //137            

            //"Mega Moss Charger", //144 (subboss thing, doesn't work when spawned somewhere else)
            
            
            //Fungal Waste
            "Zombie Fungus", //155
            "Fungus Flyer", //
            "Mushroom Turret", //
            "Fungoon Baby", //



            
            "Mantis Traitor Lord", //194 (BOSS)          needs lots of room or will fall through the floor  
            "Mantis Heavy Flyer",//35
            "Mantis Heavy",//35
            //"Mantis Lord Temp", //167 (BOSSES)
            //"Mantis Lord S1", // (static clinging mantis bosses, don't load these)
            //"Mantis Lord S2", // (static clinging mantis bosses, don't load these)
            //"Mantis Lord", // (boss)
            "Mantis Flyer Child", //166
            "Gate Mantis", // --kinda boring, let's not randomize him, also might break something in mantis village
            "Mantis", //

            "Mushroom Baby", //177
            "Mushroom Brawler", //

            //Queens Garden
            //"Jellyfish Baby", //183 -- TEMP
            //"Jellyfish", // -- TEMP

            "Moss Flyer", //
            "Garden Zombie", //183 -- has idle issues, does not wake up like it should

            
            //"Lazy Flyer Enemy", // -- TEMP

            //Resting Grounds
            "Grave Zombie", //226

            //Crystal Peak
            "Laser Turret Frames", //234

            //"Mega Zombie Beam Miner", //241 --  TEMP 

            //"Zombie Miner", //243 -- TEMP
            //"Crystallised Lazer Bug", //243 -- TEMP
            //"Crystal Flyer", //243 -- TEMP

            "Zombie Beam Miner", //244 -- has nullref issues, don't load for now until we work on a fix

            //Deepnest
            "Baby Centipede", //259
            "Centipede Hatcher", //???? (find scene for this)

            //"Slash Spider", //271 -- TEMP
            "Spider Mini", //271
            //"Zombie Spider 1", //271 TEMP -- TODO: put a filter in to NOT use him a replacement
            //"Zombie Spider 2", //271 TEMP

            "Tiny Spider", //276
            //"Spider Flyer", //--TEMP
            //"Deep Spikes", //
            
            //Kingdom's Edge
            "Bee Hatchling Ambient", //

            //Abyss
            "Shade Sibling", //313
            
            "Parasite Balloon", //320
            
            "Mawlek Turret Ceiling", //321
            "Mawlek Turret", //321

            //Waterways
            "Inflater", //
            "Flukeman", //
            
            //White Palace
            "Royal Gaurd", //358

            //Hive
            "Big Bee", //362
            "Bee Stinger", //   
            "Zombie Hive", //   
            //"Hiveling Spawner", // don't want this

            
            //"Mimic Spider", //269 (Nosk, BOSS) -- has a memory leak, don't load
            
            
            "Zote Boss"//33 (BOSS???)
        };


        //==============================================================================================================
        //==============================================================================================================
        //==============================================================================================================


        public static List<string> flyerEnemyTypeNames = new List<string>()
        {
            "Colosseum_Armoured_Mosquito",//34
            "Mosquito",//28
            "Giant Buzzer Col",//33
            "Angry Buzzer",//35
            "Buzzer",//74
            "Super Spitter Col",//33
            "Super Spitter",//34
            "Spitter", //40
            "Bursting Bouncer",//33
            "Giant Fly Col",//34 
            //"Giant Fly",//(gruz mother?)
            //"Fly Spawn",
            "Fat Fly", //146 
            "Blow Fly", //285
            //"Fluke Fly Spawner", //
            "Fluke Fly", //327
            //"White Palace Fly", //340
            "Fly",
            "Mega Fat Bee",//34
            "Colosseum_Flying_Sentry",//34
            "Blobble",//34
            "Mage Balloon", //102
            "Flamebearer Spawn",
            "Electric Mage",//35
            "Mage",//35
            "Hatcher",//52
            "Ruins Flying Sentry Javelin", //106
            "Ruins Flying Sentry", //106
            "Acid Flyer", //126
            "Fungus Flyer", //
            "Fungoon Baby", //
            "Mantis Heavy Flyer",//35
            "Mantis Flyer Child", //166
            "Jellyfish Baby", //183
            "Jellyfish", //
            "Moss Flyer", //
            "Lazy Flyer Enemy", //
            "Crystal Flyer", //243
            "Spider Flyer", //
            "Bee Hatchling Ambient", //
            "Inflater", //
            "Bee Stinger", // 
            "Shade Sibling",
            "Big Bee", //362 
            "Centipede Hatcher",
            "Parasite Balloon"
        };

        public static List<string> groundEnemyTypeNames = new List<string>()
        {
            "Colosseum_Miner",//35
            "Colosseum_Worm",//34
            "Colosseum_Shield_Zombie",//33
            "Grub Mimic",//34
            "Spitting Zombie",//34 and 41
            "Colosseum Grass Hopper",//35
            "Giant Hopper",//34
            "Grass Hopper", //208
            "Flip Hopper", //
            "Hopper",//34
            "Lancer",//35
            "Lobster",//35
            "Pigeon",
            "Mage Blob",//35 (the "mistake" enemy)
            "Mage Knight",//35
            "Lesser Mawlek",//35
            "Mawlek Col",//35 ??? might be same as lesser mawlek
            "Crystal Crawler", //243
            "Mawlek Body", //46 (BOSS)
            "Bursting Zombie",//36
            //"Mender Bug",//38?(not 36?)
            "Zombie Hornhead Sp", //271
            "Zombie Hornhead", //41 (Giant Fly = boss)
            "Prayer Slug", //47 prayer slugs = maggots
            "Zombie Shield", //52
            "Zombie Leaper",//52
            "Zombie Myla", //71
            "Blocker",//74
            "Zombie Guard", //76 (big enemy)
            "Colosseum_Armoured_Roller",//33
            "Mushroom Roller", //
            "Roller", //74
            "Gorgeous Husk", //82
            "Zombie Runner Sp", //271
            "Zombie Runner", //90
            "Zombie Barger", //90
            "Ruins Sentry Fat", //90
            "Ruins Sentry", //90
            "Great Shield Zombie", //106
            "Royal Zombie Coward", //106
            "Royal Zombie Fat", //106
            "Royal Zombie", //106
            "Moss Walker", //117
            "Mossman_Shaker", //117
            "Mossman_Runner",//117
            //"Moss Knight Fat", //204
            "Moss Knight", //137        
            "Fung Crawler", //158    
            "Zombie Fungus", //155
            "Mantis Heavy",//35
            "Mantis", //
            "Mushroom Baby", //177
            "Shell",
            "Mushroom Brawler", //
            "Garden Zombie", //
            "Grave Zombie", //226
            "Zombie Miner", //243
            "Zombie Beam Miner", //244
            "Baby Centipede", //259
            "Crawler", //40
            "Slash Spider", //271
            "Zombie Spider 1", //271
            "Zombie Spider 2", //271
            "Tiny Spider", //276
            "Flukeman", //
            "Royal Gaurd", //358
            "Zombie Hive", //   
            "Gate Mantis",
            "Mega Zombie Beam Miner", //241
            "Mimic Spider", //269 (Nosk, BOSS)
            "Mantis Traitor Lord", //194 (BOSS)          needs lots of room or will fall through the floor  
            "Zote Boss"//33 (BOSS???)
        };

        public static List<string> smallEnemyTypeNames = new List<string>()
        {
            "Colosseum_Armoured_Mosquito",//34
            "Mosquito",//28
            "Buzzer",//74
            "Shell",
            "Pigeon",
            "Super Spitter Col",//33
            "Super Spitter",//34
            "Spitter", //40
            "Bursting Bouncer",//33
            "Ceiling Dropper Col",//34
            "Ceiling Dropper", //??(forgot)
            //"Fluke Fly Spawner", //
            "Moss Flyer", //
            "Fluke Fly", //327
            //"White Palace Fly", //340
            "Fly",
            "Blobble",//34
            "Mage Blob",//35
            "Mage Balloon", //102
            "Fung Crawler", //158
            "Mines Crawler", //243
            "Abyss Crawler", //320
            "Crawler", //40
            "Climber", //40
            "Colosseum_Armoured_Roller",//33
            "Roller", //74
            "Mossman_Shaker", //117
            "Plant Turret", //127
            "Zombie Fungus", //155
            "Mushroom Turret", //
            "Fungoon Baby", //
            "Mushroom Baby", //177
            "Jellyfish Baby", //183
            "Garden Zombie", //
            "Laser Turret Frames", //234
            "Crystallised Lazer Bug", //243
            "Crystal Flyer", //243
            "Spider Mini", //271
            "Tiny Spider", //276
            "Bee Hatchling Ambient", //
            "Inflater", //
            "Parasite Balloon",
            "Mawlek Turret Ceiling", //321
            "Mawlek Turret", //321
            "Worm", //49
            "Plant Trap", //117
            
            //old BIG flyer types
            "Shade Sibling",
            "Blow Fly", //285
            "Giant Fly Col",//34 
            "Fat Fly", //146 
            //"Giant Fly",//(gruz mother?)
            //"Fly Spawn",
            "Mega Fat Bee",//34
            "Electric Mage",//35
            "Mage",//35     
            "Fungus Flyer", //
            "Jellyfish", //
            "Lazy Flyer Enemy", //
            "Big Bee", //362
            

            //old med enemy types --testing to see how this plays out
            "Colosseum_Miner",//35
            "Giant Buzzer Col",//33
            "Angry Buzzer",//35
            "Colosseum_Shield_Zombie",//33
            "Grub Mimic",//34
            "Colosseum_Flying_Sentry",//34
            "Spitting Zombie",//34 and 41
            "Colosseum Grass Hopper",//35
            "Giant Hopper",//34
            "Grass Hopper", //208
            "Flip Hopper", //
            "Hopper",//34
            "Lesser Mawlek",//35
            "Mawlek Col",//35 ??? might be same as lesser mawlek
            "Bursting Zombie",//36
            //"Mender Bug",//38?(not 36?)
            "Crystal Crawler", //243
            "Zombie Hornhead Sp", //271
            "Zombie Hornhead", //41 (Giant Fly = boss)
            "Prayer Slug", //47 prayer slugs = maggots
            "Hatcher",//52
            "Zombie Shield", //52
            "Zombie Leaper",//52
            "Zombie Myla", //71
            //"Blocker",//74
            "Mushroom Roller", //
            "Gorgeous Husk", //82
            "Zombie Runner Sp", //271
            "Zombie Runner", //90
            "Zombie Barger", //90
            "Ruins Sentry", //90
            "Royal Zombie Coward", //106
            "Royal Zombie Fat", //106
            "Royal Zombie", //106
            "Ruins Flying Sentry Javelin", //106
            "Ruins Flying Sentry", //106
            "Moss Walker", //117
            "Mossman_Runner",//117
            "Acid Flyer", //126
            "Gate Mantis",
            "Mantis Heavy Flyer",//35
            "Mantis Heavy",//35
            "Mantis Flyer Child", //166
            "Mantis", //
            "Zombie Miner", //243
            "Zombie Beam Miner", //244
            "Baby Centipede", //259
            "Zombie Spider 1", //271
            "Zombie Spider 2", //271
            "Spider Flyer", //
            "Flukeman", //
            "Zombie Hive", //   
            "Bee Stinger", //   



            "Zote Boss"//33 (BOSS???)            
        };

        public static List<string> mediumEnemyTypeNames = new List<string>()
        {
            "Colosseum_Miner",//35
            "Giant Buzzer Col",//33
            "Angry Buzzer",//35
            "Colosseum_Shield_Zombie",//33
            "Grub Mimic",//34
            "Colosseum_Flying_Sentry",//34
            "Spitting Zombie",//34 and 41
            "Colosseum Grass Hopper",//35
            "Giant Hopper",//34
            "Grass Hopper", //208
            "Flip Hopper", //
            "Hopper",//34
            "Lesser Mawlek",//35
            "Mawlek Col",//35 ??? might be same as lesser mawlek
            "Bursting Zombie",//36
            //"Mender Bug",//38?(not 36?)
            "Crystal Crawler", //243
            "Zombie Hornhead Sp", //271
            "Zombie Hornhead", //41 (Giant Fly = boss)
            "Prayer Slug", //47 prayer slugs = maggots
            "Hatcher",//52
            "Zombie Shield", //52
            "Zombie Leaper",//52
            "Zombie Myla", //71
            "Blocker",//74
            "Mushroom Roller", //
            "Gorgeous Husk", //82
            "Zombie Runner Sp", //271
            "Zombie Runner", //90
            "Zombie Barger", //90
            "Ruins Sentry", //90
            "Royal Zombie Coward", //106
            "Royal Zombie Fat", //106
            "Royal Zombie", //106
            "Ruins Flying Sentry Javelin", //106
            "Ruins Flying Sentry", //106
            "Moss Walker", //117
            "Mossman_Runner",//117
            "Acid Flyer", //126
            "Gate Mantis",
            "Mantis Heavy Flyer",//35
            "Mantis Heavy",//35
            "Mantis Flyer Child", //166
            "Mantis", //
            "Zombie Miner", //243
            "Zombie Beam Miner", //244
            "Baby Centipede", //259
            "Zombie Spider 1", //271
            "Zombie Spider 2", //271
            "Spider Flyer", //
            "Flukeman", //
            "Zombie Hive", //   
            "Bee Stinger", //   

            ////original "big enemy" list
            //"Blow Fly", //285
            //"Ruins Sentry Fat", //90
            //"Giant Fly Col",//34 
            ////"Giant Fly",//(gruz mother?)
            ////"Fly Spawn",
            //"Fat Fly", //146 
            //"Mega Fat Bee",//34
            //"Lancer",//35
            //"Lobster",//35
            //"Mage Knight",//35
            //"Electric Mage",//35
            //"Mage",//35
            //"Mawlek Body", //46 (BOSS)
            //"Colosseum_Worm",//34
            //"Worm", //49
            //"Zombie Guard", //76 (big enemy)
            //"Great Shield Zombie", //106
            //"Moss Knight Fat", //204
            //"Moss Knight", //137            
            //"Fungus Flyer", //
            //"Mushroom Brawler", //
            //"Jellyfish", //
            //"Lazy Flyer Enemy", //
            //"Grave Zombie", //226
            //"Slash Spider", //271   -- doesn't seem to randomize well, needs more research
            //"Royal Gaurd", //358
            //"Big Bee", //362
            //"Colosseum_Worm",//34
            //"Mimic Spider", //269 (Nosk, BOSS)
            //"Mantis Traitor Lord" //194 (BOSS)          needs lots of room or will fall through the floor  



        };

        //TODO: testing....
        public static List<string> bigEnemyTypeNames = new List<string>()
        {
            //"NONE",
            //"NONE"
            
            //these were doubled into the small list so they could be scaled down when replacing small enemies
            "Blow Fly", //285
            "Giant Fly Col",//34 
            "Fat Fly", //146 
            //"Giant Fly",//(gruz mother?)
            //"Fly Spawn",
            "Mega Fat Bee",//34
            "Electric Mage",//35
            "Mage",//35     
            "Fungus Flyer", //
            "Jellyfish", //
            "Lazy Flyer Enemy", //
            "Big Bee", //362
            "Flamebearer Spawn",

            "Centipede Hatcher",
            "Ruins Sentry Fat", //90
            "Lancer",//35
            "Lobster",//35
            "Mage Knight",//35
            "Mawlek Body", //46 (BOSS)
            "Colosseum_Worm",//34
            "Worm", //49
            "Zombie Guard", //76 (big enemy)
            "Great Shield Zombie", //106
            "Moss Knight Fat", //204
            "Moss Knight", //137       
            "Mushroom Brawler", //
            "Grave Zombie", //226
            "Slash Spider", //271   -- doesn't seem to randomize well, needs more research
            "Royal Gaurd", //358
            "Mimic Spider", //269 (Nosk, BOSS)
            "Mantis Traitor Lord" //194 (BOSS)          needs lots of room or will fall through the floor  
        };

        //doubled the occurence of some types in here to even out the replacements
        public static List<string> wallEnemyTypeNames = new List<string>()
        {
            "Ceiling Dropper Col",//34
            "Ceiling Dropper", //??(forgot)
            "Plant Trap", //117
            "Plant Trap", //117
            "Plant Turret", //127
            "Plant Turret", //127
            "Laser Turret Frames", //234
            "Laser Turret Frames", //234
            "Mawlek Turret Ceiling", //321
            "Mawlek Turret", //321
            "Worm", //49
            "Worm", //49
            "Mushroom Turret",
            "Mushroom Turret"
        };


        public static List<string> crawlerEnemyTypeNames = new List<string>()
        {
            "Mines Crawler", //243
            "Abyss Crawler", //320
            "Climber", //40
            "Crystallised Lazer Bug", //243 (this was removed from the list for some reason???)
            "Spider Mini" //271
        };


        public static List<string> excludeFromBattleArenaZones = new List<string>()
        {
            //until these 3 are fixed, do not spawn them in battle areas
            //"Mage Knight",//35
            "Electric Mage",//35 //TODO: needs to be moved down? (by 20?)
            "Mage",//35 //TODO: still broken
            "Mantis Traitor Lord",
            "Flamebearer Spawn",
            "Centipede Hatcher",
            "Mossman_Shaker",
            "Mage Blob",//35
            "Mage Balloon", //102
            "Zombie Spider 1", //271
            "Zombie Spider 2", //271
            "Parasite Balloon",
            "Mega Zombie Beam Miner", //241
            //"Mage Knight",//35
            "Lancer",//35
            "Lobster",//35
            "Colosseum_Worm",//34
            "Laser Turret Frames", //234
            "Worm", //49
            "Baby Centipede", //259
            "Zote Boss"//33 (BOSS???)
        };


        public static List<string> colloseumEnemyTypes = new List<string>()
        {
            "Colosseum_Armoured_Mosquito",//34
            
            "Colosseum_Miner",//35

            "Giant Buzzer Col",//33
            
            "Colosseum_Shield_Zombie",//33
            "Super Spitter Col",//33

            "Bursting Bouncer",//33
            
            "Ceiling Dropper Col",//34
            "Giant Fly Col",//34 
            
            "Grub Mimic",//34
            "Mega Fat Bee",//34
            "Colosseum_Flying_Sentry",//34

            "Colosseum Grass Hopper",//35
            "Giant Hopper",//34
            "Hopper",//34
            "Blobble",//34

            "Lancer",//35
            "Lobster",//35
            
            "Mage Knight",//35
            "Mage Blob",//35
            "Electric Mage",//35
            "Mage",//35
            "Mantis Heavy Flyer",//35
            "Mantis Heavy",//35
            
            "Mawlek Col"//35 ??? might be same as lesser mawlek
        };


        public static List<string> hardEnemyTypeNames = new List<string>()
        {
            "Centipede Hatcher",
            "Gorgeous Husk", //82 (for fun)
            "Electric Mage",//35
            "Mage Knight",//35
            "Giant Buzzer Col",//33
            "Giant Fly Col",//34 
            "Lancer",//35
            "Lobster",//35
            "Mawlek Body", //46 (mini boss)
            "Moss Knight", //137            
            "Mushroom Brawler", //
            "Royal Gaurd", //358
            "Mimic Spider", //269 (Nosk, BOSS)
            "Mega Zombie Beam Miner", //241
            "Mantis Traitor Lord" //194 (BOSS)          needs lots of room or will fall through the floor  
        };

        //"Giant Fly",//(gruz mother?)
        //"Great Shield Zombie", //106
        //"Moss Knight Fat", //204
        //public static List<string> bossTypeNames = new List<string>()
        //{
        //    "Jar Collector", //115 (boss)
        //    //"Mushroom Turret", //
        //    "Mantis Traitor Lord", //194 (BOSS)          needs lots of room or will fall through the floor  
        //    "Mantis Lord Temp", //167 (BOSSES)
        //    "Mantis Lord", //
        //    "Mega Zombie Beam Miner", //241
        //    //"Zote Boss",//33 (BOSS???)
        //    "Infected Knight", //320 (boss)
        //    "Hornet Boss 1",//121     (BOSS)       
        //    "Hornet Boss 2", //302 (BOSS)
        //    "Mimic Spider", //269 (Nosk, BOSS)
        //    "Dung Defender", //331  (BOSS)
        //    "Fluke Mother" //336 (BOSS)

        //    //"PLACEHOLDER"
        //};
    }
}


#pragma warning restore 0162
