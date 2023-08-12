using Chutpot.FPSParty.Persistent;
using KinematicCharacterController;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using Unity.Netcode.Components;
using UnityEngine;

namespace Chutpot.FPSParty.Game
{
    [DisallowMultipleComponent]
    public class PlayerTransform : NetworkTransform
    {
        private KinematicCharacterMotor _characterMotor;
        private Transform[] _points;

        protected override void Awake()
        {
            base.Awake();
            _characterMotor = GetComponent<KinematicCharacterMotor>();

            _points = FindObjectOfType<GameContextView>().GetComponent<SpawnPoints>().Points;
        }

        protected override bool OnIsServerAuthoritative()
        {
            return false;
        }

        public override void OnNetworkSpawn()
        {
            base.OnNetworkSpawn();
            if (IsOwner)
            {
                var point = _points[Random.Range(0, _points.Length)];
                PlayerTeleport(point.position, point.rotation);
            }
        }

        private void PlayerTeleport(Vector3 position, Quaternion rotation)
        {
            _characterMotor.SetPositionAndRotation(position, rotation);
            Teleport(position, Quaternion.identity, Vector3.one);
        }

        [ClientRpc(Delivery = RpcDelivery.Reliable)]
        public void SpawnClientRpc(ClientRpcParams clientRpcParams = default)
        {
            
        }
    }
}
