﻿using System.IO;
using Modding;
using UnityEngine.SceneManagement;
using UnityEngine;
using Language;
using On;
using nv;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Serialization;
using System;
using HutongGames.PlayMaker;
using HutongGames.PlayMaker.Actions;
using System.Collections;

namespace EnemyRandomizerMod
{
    public class ZoteCrewNormalController : MonoBehaviour
    {
        public Vector3 spawnLocation;

        public float aggroRange = 250f;
        public float xRange = 114.56f - 91.54f;
        public float yHeight;

        PlayMakerFSM fsm;

        FsmFloat xpos;
        FsmFloat ypos;

        public bool hasSpawned;

        void Awake()
        {
            spawnLocation = transform.position;
        }

        IEnumerator Start()
        {
            Dev.Where();
            hasSpawned = false;
            fsm = gameObject.LocateMyFSM("Control");
            var posState = fsm.GetState("Spawn Antic");
            {
                var setPosition = posState.Actions.FirstOrDefault(x => typeof(SetPosition).IsAssignableFrom(x.GetType()));

                xpos = setPosition.GetFieldValue<FsmFloat>("x");
                ypos = setPosition.GetFieldValue<FsmFloat>("y");

                yHeight = ypos.Value;

                //check how much room the thowmp has
                var left = gameObject.GetPointOn(Vector2.left, float.MaxValue);
                var right = gameObject.GetPointOn(Vector2.right, float.MaxValue);

                xRange = right.x - left.x;
                aggroRange = xRange * 2f;
            }

            //check distance and move us into idle if player is far from spawn
            var hero = HeroController.instance;
            while (gameObject.SafeIsActive())
            {
                if ((hero.transform.position - spawnLocation).magnitude < aggroRange)
                {
                    if (fsm.ActiveStateName == "Dormant")
                    {
                        xpos.Value = spawnLocation.x + EnemyRandomizer.pRNG.Rand(-xRange * .5f, xRange * .5f);

                        var roof = gameObject.GetPointOn(Vector2.up, yHeight);

                        if (roof.y - spawnLocation.y < yHeight)
                        {
                            ypos.Value = roof.y;
                        }
                        else
                        {
                            ypos.Value = spawnLocation.y + yHeight;
                        }

                        hasSpawned = true;
                        fsm.SendEvent("SPAWN");
                    }
                }
                else
                {
                    if (hasSpawned)
                        fsm.SendEvent("PLAYER_FAR");
                }

                yield return new WaitForEndOfFrame();
            }

            yield break;
        }
    }

    public class ZoteCrewNormal : DefaultEnemy
    {
        public override void SetupPrefab()
        {
            Dev.Where();
            base.SetupPrefab();
            var fsm = Prefab.LocateMyFSM("Control");

            //var initState = fsm.Fsm.GetState("Init");
            //var spawnState = fsm.Fsm.GetState("Spawn Antic");
            //var activateState = fsm.Fsm.GetState("Activate");

            //remove the transitions related to chain spawning zotes for the event
            //fsm.RemoveTransition("Death", "FINISHED");
            fsm.RemoveTransition("Death Reset", "FINISHED");
            fsm.ChangeTransition("Dormant", "SPAWN", "Spawn Antic");
            fsm.RemoveTransition("Multiply", "FINISHED");

            //change the start transition to just begin the spawn antics
            //fsm.ChangeTransition("Init", "FINISHED", "Spawn Antic");

            //remove the states that were also part of that
            //fsm.Fsm.RemoveState("Death Reset");
            //fsm.Fsm.RemoveState("Dormant");
            fsm.Fsm.RemoveState("Multiply");

            fsm.AddGlobalTransition("PLAYER_FAR", "Idle");

            //base.Setup(enemy, knownEnemyTypes, EnemyObject);
            var controller = Prefab.AddComponent<ZoteCrewNormalController>();
        }
    }
}