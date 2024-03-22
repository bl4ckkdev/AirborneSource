// Copyright © bl4ck & XDev, 2022-2024
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//i fucking hate you

namespace goddamnit // don't remove the namespace everything will break
{
    public class Physics : MonoBehaviour
    {
        public bool isEnabled;
        public float mass;
        public bool lockX, lockY, lockZ;
        public bool restartOnDeath;

        public Vector3 startPos, startRot;
        [HideInInspector] public Rigidbody rigid;

        private void Awake() => Init();
        public void Init()
        {
            startPos = transform.position;
            startRot = transform.eulerAngles;
            if (rigid == null) rigid = gameObject.AddComponent<Rigidbody>();

            rigid.mass = mass;

            rigid.constraints = RigidbodyConstraints.None;
            
            rigid.constraints |= RigidbodyConstraints.FreezePositionZ | RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationY;
            if (lockX) rigid.constraints |= RigidbodyConstraints.FreezePositionX;
            if (lockY) rigid.constraints |= RigidbodyConstraints.FreezePositionY;
            if (lockZ) rigid.constraints |= RigidbodyConstraints.FreezeRotationZ;
        }
    }

}
