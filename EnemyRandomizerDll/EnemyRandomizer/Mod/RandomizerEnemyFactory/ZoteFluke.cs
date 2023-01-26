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
    public class ZoteFlukeController : MonoBehaviour
    {
        public Vector3 spawnLocation;

        public float aggroRange = 250f;
        public float xRange = 115.64f - 90.45f;
        public float yHeight;

        PlayMakerFSM fsm;

        FsmFloat xpos;
        FsmFloat ypos;

        void Awake()
        {
            spawnLocation = transform.position;
        }


        IEnumerator Start()
        {
            Dev.Where();
            fsm = gameObject.LocateMyFSM("Control");
            var posState = fsm.GetState("Pos");
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

                        var roof = gameObject.GetPointOn(Vector2.up, float.MaxValue);

                        ypos.Value = roof.y;

                        fsm.SendEvent("GO");
                    }
                }
                else
                {
                    fsm.SendEvent("PLAYER_FAR");
                }

                yield return new WaitForEndOfFrame();
            }

            yield break;
        }
    }

    public class ZoteFluke : DefaultEnemy
    {
        public override void SetupPrefab()
        {
            Dev.Where();
            var fsm = Prefab.LocateMyFSM("Control");

            //remove the transitions related to chain spawning zotes for the event
            //fsm.RemoveTransition("Sleeping", "F");
            fsm.RemoveTransition("Death", "FINISHED");

            //change the start transition to just begin the spawn antics
            //fsm.ChangeTransition("Init", "FINISHED", "Pos");

            //remove the states that were also part of that
            fsm.AddGlobalTransition("PLAYER_FAR", "Dormant");
            //fsm.Fsm.RemoveState("Dormant");

            //base.Setup(enemy, knownEnemyTypes, EnemyObject);
            var controller = Prefab.AddComponent<ZoteFlukeController>();
        }
    }
}