﻿using System.IO;
using Modding;
using UnityEngine.SceneManagement;
using UnityEngine;
using Language;
using On;

using System.Xml.Serialization;
using System;
using System.Linq;
using System.Reflection;
using System.Diagnostics;
using System.Collections;
using System.Collections.Generic;
using Satchel.BetterMenus;

namespace EnemyRandomizerMod
{
    public interface IRandomizerLogic
    {
        /// <summary>
        /// The name of the logic, used to select it from a menu
        /// </summary>
        string Name { get; }

        /// <summary>
        /// A description of the logic for users to understand what this logic should do
        /// </summary>
        string Info { get; }

        /// <summary>
        /// Did the mod get a chance to preload?
        /// </summary>
        bool OnStartGameWasCalled { get; }

        /// <summary>
        /// Is this logic currently enabled?
        /// </summary>
        bool Enabled { get; }

        /// <summary>
        /// After the logic is created, give it a reference to the replacement functionality
        /// </summary>
        void Setup(EnemyRandomizerDatabase database);

        /// <summary>
        /// Return the Menu used by this module
        /// </summary>
        Menu GetSubpage();

        /// <summary>
        /// Will be called once the game starts or if the logic is toggled on mid-game
        /// </summary>
        void Enable();

        /// <summary>
        /// Will be called if the logic is toggled off mid-game
        /// </summary>
        void Disable();

        /// <summary>
        /// Will be called if a new game or save file is loaded while this logic is enabled
        /// </summary>
        void Reset();

        /// <summary>
        /// Invoked when a game is started
        /// </summary>
        void OnStartGame(EnemyRandomizerPlayerSettings settings);

        /// <summary>
        /// Invoked when a game is saved
        /// </summary>
        void OnSaveGame(EnemyRandomizerPlayerSettings settings);

        /// <summary>
        /// Gets the kinds of things that may be used as replacements
        /// </summary>
        List<PrefabObject> GetValidReplacements(ObjectMetadata objectToModify, List<PrefabObject> validReplacementObjects);

        /// <summary>
        /// Configure the RNG/seed for the next thing to be replaced
        /// </summary>
        RNG GetRNG(ObjectMetadata objectToModify, RNG rng, int seed);

        /// <summary>
        /// Use some kind of logic to replace things
        /// </summary>
        ObjectMetadata GetReplacement(ObjectMetadata objectToModify, ObjectMetadata originalObject, List<PrefabObject> validReplacementObjects, RNG rng);
        
        /// <summary>
        /// Use some kind of logic to optionally modify an object 
        /// </summary>
        ObjectMetadata ModifyObject(ObjectMetadata objectToModify, ObjectMetadata originalObject);

        /// <summary>
        /// Use some kind of logic to create/modify persitent bool item's data
        /// For non-boss enemies they should be marked semi persistant
        /// </summary>
        PersistentBoolData ReplacePersistentBoolItemData(PersistentBoolItem other);

        /// <summary>
        /// Use some kind of logic to create/modify persitent bool item's data
        /// For non-boss enemies they should be marked semi persistant
        /// </summary>
        (string ID, string SceneName) ReplacePersistentBoolItemSetMyID(PersistentBoolItem other);
    }

    public abstract class BaseRandomizerLogic : IRandomizerLogic
    {
        public abstract string Name { get; }
        public abstract string Info { get; }
        public virtual bool Enabled { get; protected set; }
        public virtual bool OnStartGameWasCalled { get; protected set; }

        protected virtual EnemyRandomizerDatabase Database { get; set; }
        protected virtual LogicSettings Settings { get => EnemyRandomizer.GlobalSettings.GetLogicSettings(Name); }

        public virtual void Setup(EnemyRandomizerDatabase database)
        {
            Database = database;
        }
        

        public virtual Menu GetSubpage()
        {
            return new Menu(Name,GetEntries().ToArray());
        }

        public virtual void Enable()
        {
            Enabled = true;
        }

        public virtual void Disable()
        {
            Enabled = false;
        }

        public virtual void Reset()
        {
            Enabled = false;
            OnStartGameWasCalled = false;
        }

        public virtual void OnStartGame(EnemyRandomizerPlayerSettings settings)
        {
            OnStartGameWasCalled = true;
        }

        public virtual void OnSaveGame(EnemyRandomizerPlayerSettings settings)
        {
        }

        public virtual List<PrefabObject> GetValidReplacements(ObjectMetadata originalObject, List<PrefabObject> validReplacementObjects)
        {
            return validReplacementObjects;
        }

        public virtual RNG GetRNG(ObjectMetadata objectToModify, RNG rng, int seed)
        {
            return rng;
        }

        public virtual ObjectMetadata GetReplacement(ObjectMetadata newObject, ObjectMetadata originalObject, List<PrefabObject> validReplacements, RNG rng)
        {
            return newObject == null ? originalObject : newObject;
        }

        public virtual ObjectMetadata ModifyObject(ObjectMetadata objectToModify, ObjectMetadata originalObject)
        {
            return objectToModify;
        }

        public virtual PersistentBoolData ReplacePersistentBoolItemData(PersistentBoolItem other)
        {
            return null;
        }

        public virtual (string ID, string SceneName) ReplacePersistentBoolItemSetMyID(PersistentBoolItem other)
        {
            return (null, null);
        }

        //protected const string DefaultRandomizeEnemiesOption = "Randomize Enemies";
        //protected const string DefaultRandomizeHazardsOption = "Randomize Hazards";
        //protected const string DefaultRandomizeEffectsOption = "Randomize Projectiles";

        protected virtual List<(string Name, string Info, bool DefaultState)> ModOptions
        {
            get => new List<(string, string, bool)>();
            //{
            //    (DefaultRandomizeEnemiesOption, "Should this change enemies? (Only one mod may set this to true)", true ),
            //    (DefaultRandomizeHazardsOption, "Should this change hazards? (Only one mod may set this to true)", false ),
            //    (DefaultRandomizeEffectsOption, "Should this change effects? (Only one mod may set this to true)", false ),
            //};
        }

        public virtual List<Element> GetEntries()
        {
            return ModOptions.Select(x => 
                Blueprints.HorizontalBoolOption(
                    name: x.Name, 
                    description: x.Info,
                    applySetting: b => Settings.GetOption(x.Name).value =b ,
                    loadSetting: () => Settings.GetOption(x.Name).value))
                .Select(x => x as Element) // compiler doesn't like implicitly doing this cast
                .ToList();
        }
    }
}





///// <summary>
///// Gets the kinds of hazards that may be used as replacements
///// </summary>
//List<PrefabObject> GetAllowedHazardReplacements(ObjectMetadata sourceData);

///// <summary>
///// Use some kind of logic to replace/modify hazards
///// </summary>
//GameObject ReplaceHazardObject(GameObject other, List<PrefabObject> allowedReplacements, RNG rng);

///// <summary>
///// Use some kind of logic to modify a (potentially) new hazard using the given source data
///// </summary>
//ObjectMetadata ModifyHazardObject(ObjectMetadata other, ObjectMetadata sourceData);

///// <summary>
///// Gets the kinds of effects that may be used as replacements
///// </summary>
//List<PrefabObject> GetAllowedEffectReplacements(ObjectMetadata sourceData);

///// <summary>
///// Use some kind of logic to replace effects/projectiles
///// </summary>
//GameObject ReplacePooledObject(GameObject other, List<PrefabObject> allowedReplacements, RNG rng);

///// <summary>
///// Use some kind of logic to modify a (potentially) new effect/projectile using the given source data
///// </summary>
//ObjectMetadata ModifyPooledObject(ObjectMetadata other, ObjectMetadata sourceData);