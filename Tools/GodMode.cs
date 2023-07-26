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
        static DSApplicationBuildSettings defaultSettings;
        static DSApplicationBuildSettings godModeSettings;
        public static bool isEnabled;
        public static bool isActive;

        public static void Init()
        {
            defaultSettings = DSApplicationBuildConfig.Instance.DSApplicationBuildSettings;
            godModeSettings = new DSApplicationBuildSettings();
            isEnabled = false;
        }

        public static void ToggleGodMode()
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
