﻿using HutongGames.PlayMaker;
using HutongGames.PlayMaker.Actions;
using System.Collections;
using System.Linq;
using UnityEngine;

namespace EnemyRandomizerMod
{
    public class ZoteThwompControl : DefaultSpawnedEnemyControl
    {
        public PlayMakerFSM control;
        public Vector3 spawnLocation;

        public float aggroRange = 250f;
        public float xRange = 113.85f - 92.43f;
        public float yHeight;

        PlayMakerFSM fsm;

        FsmFloat xpos;
        FsmFloat ypos;

        public override void Setup(ObjectMetadata other)
        {
            base.Setup(other);
        }

        protected virtual void OnEnable()
        {
            spawnLocation = transform.position;
        }

        IEnumerator Start()
        {
            Dev.Where();
            fsm = gameObject.LocateMyFSM("Control");
            var posState = fsm.GetState("Set Pos");
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
                        var rng = (new RNG(hero.transform.position.x.GetHashCode()));
                        xpos.Value = spawnLocation.x + rng.Rand(-xRange * .5f, xRange * .5f);

                        var roof = gameObject.GetPointOn(Vector2.up, yHeight);

                        if (roof.y - spawnLocation.y < yHeight)
                        {
                            ypos.Value = roof.y;
                        }
                        else
                        {
                            ypos.Value = spawnLocation.y + yHeight;
                        }

                        fsm.SendEvent("GO");
                    }
                }
                else
                {
                    if (fsm.ActiveStateName == "Out")
                        fsm.SendEvent("PLAYER_FAR");
                }

                yield return new WaitForEndOfFrame();
            }

            yield break;
        }
    }

    public class ZoteThwompSpawner : DefaultSpawner<ZoteThwompControl>
    {
        public override GameObject Spawn(PrefabObject p, ObjectMetadata source)
        {
            var go = base.Spawn(p, source);
            var fsm = go.GetComponent<ZoteThwompControl>();
            fsm.control = go.LocateMyFSM("Control");

            if (source.IsBoss)
            {
                //TODO:
            }
            else
            {
                //var hm = go.GetComponent<HealthManager>();
                //hm.hp = source.MaxHP;
            }

            return go;
        }
    }

    public class ZoteThwompPrefabConfig : DefaultPrefabConfig<ZoteThwompControl>
    {
        public override void SetupPrefab(PrefabObject p)
        {
            base.SetupPrefab(p);

            {
                var fsm = p.prefab.LocateMyFSM("Control");
                //remove the transitions related to chain spawning zotes for the event
                fsm.RemoveTransition("Break", "FINISHED");
                fsm.AddTransition("Out", "PLAYER_FAR", "Dormant");
                //fsm.ChangeTransition("Init", "FINISHED", "Teleport");
                //fsm.ChangeTransition("Init", "GG BOSS", "Teleport");
            }
        }
    }































}
