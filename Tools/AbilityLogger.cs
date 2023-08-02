using DS.Game.Luna;
using DS.Tech.App;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

namespace Tools
{
    public class AbilityLogger : MonoBehaviour
    {
        GUIStyle _textStyle;
        int _currentTick;
        public bool isEnabled;
        int TickFade = 60;

        void OnGUI()
        {
            GUI.Label(new Rect((float)Screen.width-150, (float)Screen.height-25 , 150, 25), "ABCDE", this._textStyle);

        }


        void Start()
        {
            Debugger.Log("Ability Logger");
    
            Font font = AssetBundleMap.Instance.LoadAsset<Font>("Assets/DS.Game.Updraft/UI/Fonts_RestOfWorld/FontAssets/FSLola-Medium.otf");
            this._textStyle = new GUIStyle();
            this._textStyle.font = font;
            this._textStyle.fontSize = 30;
            this._textStyle.normal.textColor = Color.white;
        }

        IEnumerator UpdateCommandFrame()
        {
            while (isEnabled)
            {
                this._currentTick = NetworkTime.SimulationNetworkClock.Tick;
                CommandFrame currentFrame = GetCommandFrame(this._currentTick);
                yield return null;
            }
        }

        CommandFrame GetCommandFrame(int tick)
        {
            return NetworkHeroManager.Instance.NetworkHero.Player.NetworkPlayerInput.ServerCommandFrameReceiver.GetCommandFrameAtTick(tick);
        }

        // Update is called once per frame
        void Update()
        {
            
        }

        class Ability
        {
            string name;
            GUIStyle _textStyle;
        }
    }
}