using DS.Tech.App;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tools
{
    class GodMode
    {
        DSApplicationBuildSettings defaultSettings;
        DSApplicationBuildSettings godModeSettings;
        public bool isEnabled;
        public bool isActive;

        GodMode()
        {
            defaultSettings = DSApplicationBuildConfig.Instance.DSApplicationBuildSettings;
            godModeSettings = new DSApplicationBuildSettings();
            isEnabled = false;
        }

        public void ToggleGodMode()
        {
            try{
                isActive = isEnabled;
                if (isEnabled)
                {
                    DSApplicationBuildConfig.Instance.WriteBuildSettings(godModeSettings);
                }
                else
                {
                    DSApplicationBuildConfig.Instance.WriteBuildSettings(defaultSettings);
                }
            }catch(Exception ex){
                Debugger.Log(ex.Message);
            }
            
        }
    }
}
