using DS.Game.Luna;
using DS.Tech.App;
using System;
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

        int horizontalMargin = 50;
        
        void OnGUI()
        {
            for(int i = 0; i < abilitiesDisplayed.Count; i++)
            {
                GUI.Label(new Rect((float)Screen.width-(horizontalMargin * (i+1)), (float)Screen.height-30 , 30, 30), abilitiesDisplayed[i].name, abilitiesDisplayed[i].textStyle);
            }
        }


        void Start()
        {
            Debugger.Log("Ability Logger");
            isEnabled = true;
            StartCoroutine(UpdateCommandFrame());

        }

        void UpdateAbilitiesDisplayed(List<Ability> abilitiesFrame) {
            abilitiesFrame.ForEach(ability =>
            {
                if (ability.holding)
                {
                    int index = abilitiesDisplayed.IndexOf(ability);
                    if(index != -1)
                    {
                        if (!abilitiesDisplayed[index].holding)
                        {
                            abilitiesDisplayed.Insert(0,ability);
                        }
                    }
                    else
                    {
                        abilitiesDisplayed.Insert(0,ability);
                    }
                }else{
                    int index = abilitiesDisplayed.IndexOf(ability);
                    if (index != -1 && abilitiesDisplayed[index].holding)
                    {
                        abilitiesDisplayed[index].released();
                    }
                }
            });
        }

        IEnumerator UpdateCommandFrame()
        {
            while (isEnabled)
            {
                try
                {

                    this._currentTick = NetworkTime.SimulationNetworkClock.Tick;
                    CommandFrame currentFrame = GetCommandFrame(this._currentTick);

                    List<Ability> abilitiesFrame = new List<Ability>();
                    abilitiesFrame.Add(new Ability("BA", currentFrame.BasicAttackHeld, _currentTick));
                    abilitiesFrame.Add(new Ability("A", currentFrame.AcrobaticHeld, _currentTick));
                    abilitiesFrame.Add(new Ability("TW", currentFrame.TimewinderHeld, _currentTick));
                    abilitiesFrame.Add(new Ability("PD", currentFrame.PhaseDiveHeld, _currentTick));
                    abilitiesFrame.Add(new Ability("TP", currentFrame.TemporalPulseHeld, _currentTick));
                    abilitiesFrame.Add(new Ability("R", currentFrame.RewindHeld, _currentTick));
                    abilitiesFrame.Add(new Ability("J", currentFrame.JumpHeld, _currentTick));
                    abilitiesFrame.Add(new Ability("PC", currentFrame.ParallelConvergenceHeld, _currentTick));
                    abilitiesFrame.Add(new Ability("CB", currentFrame.ChronobreakPressed, _currentTick));
                    abilitiesFrame.Add(new Ability("D", currentFrame.DodgeHeld, _currentTick));


                    UpdateAbilitiesDisplayed(abilitiesFrame);
                }catch(Exception ex)
                {
                    Debugger.Log(ex.Message);
                }
                //Debugger.Log(this._currentTick.ToString());
                yield return null;
            }
        }
        void LogAbilites(List<Ability> abilities)
        {
            Debugger.Log("Abilities :" + abilities.Count.ToString());
            for (int i = 0; i < abilities.Count; i++)
            {
                Debugger.Log(abilities[i].ToString());
            }
        }

        CommandFrame GetCommandFrame(int tick)
        {
            try
            {
                return NetworkHeroManager.Instance.NetworkHero.Player.NetworkPlayerInput.ServerCommandFrameReceiver.GetCommandFrameAtTick(tick);
            }catch(Exception ex)
            {
                return default;
            }
        }

        // Update is called once per frame
        void Update()
        {
           
        }



        class Ability : IEquatable<Ability>
        {
            readonly Font font = AssetBundleMap.Instance.LoadAsset<Font>("Assets/DS.Game.Updraft/UI/Fonts_RestOfWorld/FontAssets/FSLola-Medium.otf");

            public string name;
            public GUIStyle textStyle;
            int tick;
            public bool holding;

            public Ability(string name, bool pressed ,int tickWhenPressed)
            {
                this.textStyle = new GUIStyle();
                this.textStyle.font = font;
                this.textStyle.fontSize = 30;
                this.textStyle.normal.textColor = pressed ? Color.white : Color.gray;
                this.name = name;
                this.holding = pressed;
                this.tick = tickWhenPressed;
            }

            public bool Equals(Ability other)
            {
                return this.name == other.name;
            }

            public void released()
            {
                this.holding = false;
                this.textStyle.normal.textColor = Color.gray;
            }

            public override string ToString()
            {
                return this.name + ':'+ this.holding.ToString();
            }
        }
    }
}