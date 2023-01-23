﻿using System.Collections;
using HutongGames.PlayMaker.Actions;
using UnityEngine;
using Vasi;

namespace EnemyRandomizerMod.Behaviours
{
    public class BrokenVessel : EnemyBehaviour
    {
        private PlayMakerFSM _control;
        private PlayMakerFSM _spawn;

        private void Awake()
        {
            _control = gameObject.LocateMyFSM("IK Control");
            _spawn = gameObject.LocateMyFSM("Spawn Balloon");
        }

        public override void AdjustPosition()
        {
            transform.position = new Vector3(transform.position.x, transform.position.y - 4, transform.position.z);
        }

        private IEnumerator Start()
        {
            _control.SetState("Pause");

            _control.Fsm.GetFsmFloat("Air Dash Height").Value = bounds.yMin + 3;
            _control.Fsm.GetFsmFloat("Left X").Value = bounds.xMin;
            _control.Fsm.GetFsmFloat("Min Dstab Height").Value = bounds.yMin + 5;
            _control.Fsm.GetFsmFloat("Right X").Value = bounds.xMax;

            _control.GetAction<RandomFloat>("Aim Jump 2").min = bounds.center.x - 1;
            _control.GetAction<RandomFloat>("Aim Jump 2").max = bounds.center.x + 1;
            _control.GetAction<SetPosition>("Set Pos").x = transform.position.x;
            _control.GetAction<SetPosition>("Set Pos").y = transform.position.y;

            _spawn.Fsm.GetFsmFloat("X Min").Value = bounds.xMin + 1;
            _spawn.Fsm.GetFsmFloat("X Max").Value = bounds.xMax - 1;
            _spawn.Fsm.GetFsmFloat("Y Min").Value = bounds.yMin + 1;
            _spawn.Fsm.GetFsmFloat("Y Max").Value = bounds.yMin + 5;

            yield return new WaitUntil(() => _control.ActiveStateName == "Intro Fall");

            _control.SetState("Roar End");
        }
    }
}