﻿using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using System.Linq;
using HutongGames.PlayMaker.Actions;
using HutongGames.PlayMaker;

using UniRx;

namespace EnemyRandomizerMod
{
    public class ManagedObject : MonoBehaviour
    {
        public ObjectMetadata myMetaData;
        public string originalGameObjectPath;
        public string sourceDatabaseKey;
        public string myDatabaseKey;
        public bool replaced;
        public ObjectMetadata replacedMetaData;

        /// <summary>
        /// True if this has not been replaced
        /// </summary>
        public bool ThisIsSourceObject { get; protected set; }

        /// <summary>
        /// Pass "null" for the source if setting up a custom enemy
        /// </summary>
        public virtual void Setup(ObjectMetadata source)
        {
            ThisIsSourceObject = (source.Source == gameObject);
            this.originalGameObjectPath = source.ScenePath;
            sourceDatabaseKey = source.DatabaseName;

            myDatabaseKey = EnemyRandomizerDatabase.ToDatabaseKey(name);
            replacedMetaData = source;

            myMetaData = new ObjectMetadata();
            myMetaData.Setup(gameObject, EnemyRandomizerDatabase.GetDatabase());

            if (!ThisIsSourceObject)
                replaced = true;
        }
    }

    public class BattleManagedObject : ManagedObject
    {
        public override void Setup(ObjectMetadata source)
        {
            base.Setup(source);

            RegisterWithBattleManager();
        }

        public virtual void RegisterWithBattleManager()
        {
            if (BattleManager.StateMachine.Value == null)
            {
                BattleManager.DoSceneCheck(gameObject);
            }

            if(BattleManager.StateMachine.Value == null)
            {
                StartCoroutine(FindStateMachine());
            }
            else
            {
                BattleManager.StateMachine.Value.RegisterEnemy(this);
            } 
        }

        IEnumerator FindStateMachine()
        {
            while(BattleManager.StateMachine.Value == null)
            {
                BattleManager.DoSceneCheck(gameObject);
                yield return new WaitForSeconds(1f);
            }

            BattleManager.StateMachine.Value.RegisterEnemy(this);
        }

        //public virtual void UnregisterWithBattleManager()
        //{
        //    BattleManager.StateMachine.Value.UnregisterEnemy(this);
        //}

        //public virtual void UnregisterWithBattleManager(int oldWave)
        //{
        //    BattleManager.StateMachine.Value.UnregisterEnemy(this, oldWave);
        //}

        //public bool IsWaveInName()
        //{
        //    if (string.IsNullOrEmpty(originalGameObjectPath))
        //        return false;

        //    return originalGameObjectPath.Contains("Battle Scene/Wave");
        //}

        //public int GetBattleWaveFromName()
        //{
        //    var wavePart = originalGameObjectPath.Split('/').FirstOrDefault(x => x.Contains("Wave"));
        //    var endPart = wavePart.Split(' ').Last();
        //    int numberPart = int.Parse(endPart);
        //    return numberPart;
        //}

        //public virtual int GetMyWave()
        //{
        //    var fullPath = originalGameObjectPath.Split('/');
        //    if(fullPath.Length > 1)
        //    {
        //        if(IsWaveInName())
        //        {
        //            return GetBattleWaveFromName();
        //        }

        //        List<string> pathRemaining = fullPath.ToList();

        //        string myPath = originalGameObjectPath;
        //        int wave = -1;
        //        //try partial match if full match doesn't work
        //        while (!StateMachines.BattleWaveMap.TryGetValue(myPath, out wave))
        //        {
        //            wave = -1;
        //            pathRemaining.RemoveAt(pathRemaining.Count - 1);
        //            if (pathRemaining.Count <= 0)
        //                break;

        //            myPath = string.Join(@"/", pathRemaining);
        //        }

        //        if (wave >= 0)
        //            return wave;

        //        bool hasWave = StateMachines.BattleWaveMap.ContainsKey(myPath);

        //        if (hasWave && wave < 0)
        //        {
        //            return wave;
        //        }
        //    }
        //    else
        //    {
        //        string myPath = originalGameObjectPath;
        //        if (StateMachines.BattleWaveMap.TryGetValue(myPath, out int wave))
        //        {
        //            return wave;
        //        }
        //    }

        //    return 0;
        //}
    }
}
