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

        int _currentTick = 0;
        public bool isEnabled;
        int TickFade = 60;
        List<Ability> abilitiesDisplayed = new List<Ability>();

        void OnGUI()
        {
            for(int i = 0; i < abilitiesDisplayed.Count; i++)
            {
                GUI.Label(new Rect((float)Screen.width-(30*(i+1)), (float)Screen.height-30 , 30, 30), abilitiesDisplayed[i].name, abilitiesDisplayed[i].textStyle);
            }
        }


        void Start()
        {
            Debugger.Log("Ability Logger");
            isEnabled = true;
            abilitiesDisplayed.Add(new Ability("T", _currentTick));
            StartCoroutine(UpdateCommandFrame());
            
        }

        IEnumerator UpdateCommandFrame()
        {
            while (isEnabled)
            {
                this._currentTick = NetworkTime.SimulationNetworkClock.Tick;
                CommandFrame currentFrame = GetCommandFrame(this._currentTick);

                if (currentFrame.AcrobaticPressed)
                {
                    Debugger.Log("A");
                    abilitiesDisplayed.Add(new Ability("A", _currentTick));
                }
                if (currentFrame.BasicAttackPressed)
                {
                    Debugger.Log("BA");
                    abilitiesDisplayed.Add(new Ability("BA", _currentTick));
                }
                if (currentFrame.TimewinderPressed)
                {
                    Debugger.Log("TW");
                    abilitiesDisplayed.Add(new Ability("TW", _currentTick));
                }
                Debugger.Log(this._currentTick.ToString());
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
            readonly Font font = AssetBundleMap.Instance.LoadAsset<Font>("Assets/DS.Game.Updraft/UI/Fonts_RestOfWorld/FontAssets/FSLola-Medium.otf");

            public string name;
            public GUIStyle textStyle;
            int tick;

            public Ability(string name, int tickWhenPressed)
            {
                Debugger.Log("New Ability created");
                this.textStyle = new GUIStyle();
                this.textStyle.font = font;
                this.textStyle.fontSize = 30;
                this.textStyle.normal.textColor = Color.white;
                this.name = name;
                this.tick = tickWhenPressed;
            }
             
        }
    }
}