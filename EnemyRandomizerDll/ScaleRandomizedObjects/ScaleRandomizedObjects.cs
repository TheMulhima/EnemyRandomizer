﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using System.Linq;
using HutongGames.PlayMaker.Actions;

namespace EnemyRandomizerMod
{
    public class ScaleRandomizedObjects : BaseRandomizerLogic
    {
        public override string Name => "Scale Randomized Objects";

        public override string Info => "Scales randomized objects to match the size of the objects they replaced.";

        List<(string Name, string Info, bool Default)> CustomOptions = new List<(string, string, bool)>()
        {
            ("Scale Enemies", "Should this effect randomized enemies?", true),
            ("Scale Hazards", "Should this effect randomized hazards?", false),
            ("Scale Effects", "Should this effect randomized effects?", false),
            ("Match Scaling", "Should enemies be scaled to a size that \'Makes Sense\' or just to random values? (Default is true)", true),
            ("Match Audio to Scaling", "Should enemies have their sounds changed too?", true),
        };

        protected override List<(string Name, string Info, bool DefaultState)> ModOptions
        {
            get => CustomOptions;
        }

        public override void Setup(EnemyRandomizerDatabase database)
        {
            base.Setup(database);
            EnemyRandomizer.Instance.enemyReplacer.loadedLogics.Add(this);
        }

        public override ObjectMetadata ModifyObject(ObjectMetadata objectToModify, ObjectMetadata originalObject)
        {
            if (objectToModify.ObjectType == PrefabObject.PrefabType.Enemy && Settings.GetOption(CustomOptions[0].Name).value)
            {
                return ScaleObject(objectToModify, originalObject);
            }

            else if (objectToModify.ObjectType == PrefabObject.PrefabType.Hazard && Settings.GetOption(CustomOptions[1].Name).value)
            {
                return ScaleObject(objectToModify, originalObject);
            }

            else if (objectToModify.ObjectType == PrefabObject.PrefabType.Effect && Settings.GetOption(CustomOptions[2].Name).value)
            {
                return ScaleObject(objectToModify, originalObject);
            }

            return objectToModify;
        }

        public virtual ObjectMetadata ScaleObject(ObjectMetadata objectToModify, ObjectMetadata originalObject)
        {
            if(Settings.GetOption(CustomOptions[3].Name).value)
            {
                return ApplySizeScale(objectToModify, originalObject);
            }
            else
            {
                Dev.LogWarning("Using randomized scale. This might create some enemies that are way too big or small");
                RNG prng = new RNG();
                prng.Seed = objectToModify.ObjectName.GetHashCode() + objectToModify.SceneName.GetHashCode();
                float scale = prng.Rand(.2f, 3f);
                objectToModify.ApplySizeScale(scale);
            }
            return objectToModify;
        }

        public virtual ObjectMetadata ApplySizeScale(ObjectMetadata objectToModify, ObjectMetadata originalObject)
        {
            //TODO: maybe apply some default scaling to bigger enemies, for now just return them
            if (originalObject == null)
                return objectToModify;

            float scale = objectToModify.GetRelativeScale(originalObject, .2f);
            Dev.Log($"Relative scale of {objectToModify.DatabaseName} to {originalObject.DatabaseName} is {scale}");
            objectToModify.ApplySizeScale(scale);

            //should we also scale the audio to match?
            if (Settings.GetOption(CustomOptions[4].Name).value)
            {
                objectToModify.SetAudioToMatchScale();
            }

            return objectToModify;
        }
    }
}
