using DS.Game.Luna;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

namespace Tools
{
    public class AbilityLogger : MonoBehaviour
    {
        // Use this for initialization

        void Start()
        {
            Debugger.Log("Ability Logger");
        }

        CommandFrame GetCommandFrame()
        {
            int tick = NetworkTime.SimulationNetworkClock.Tick;
            return NetworkHeroManager.Instance.NetworkHero.Player.NetworkPlayerInput.ServerCommandFrameReceiver.GetCommandFrameAtTick(tick);
        }

        // Update is called once per frame
        void Update()
        {
            
        }
    }
}