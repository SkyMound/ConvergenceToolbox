using DS.Game.Luna;
using DS.Tech.Util;
using DS.Game.Updraft;
using System.Collections;
using UnityEngine;

namespace Tools
{
    public class PersistentCheckpoint : MonoBehaviour
    {
        public UpdraftRoomDoor persistentCheckpoint;


        void SetCurrentToPersistent()
        {
            persistentCheckpoint.DoorNode = ServiceLocator.Instance.GetService<ServerLevelFlowScope>().SpawningDoorNode;
        }

        void LoadPersistent()
        {
            ServiceLocator.Instance.GetService<ServerLevelFlowScope>().OnCheckpointReached(persistentCheckpoint, true);

        }

        // Use this for initialization
        void Start()
        {
            persistentCheckpoint = new UpdraftRoomDoor();
        }

        // Update is called once per frame
        void Update()
        {

        }
    }
}