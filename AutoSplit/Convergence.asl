state("Convergence") { }

startup
{
    Assembly.Load(File.ReadAllBytes("Components/asl-help")).CreateInstance("Unity");
    vars.Helper.GameName = "Convergence: A League Of Legends Story";

}

init
{
    vars.Helper.TryLoad = (Func<dynamic,bool>)(mono=>{
        var nhm = mono["NetworkHeroManager"];
        var lvl = mono["Level"];
        var ug = mono["UpdraftGame"];

        // Lensable castable to standard ?
        vars.Helper["hasTW"] = nhm.Make<bool>("Instance","NetworkHero","SimulationHero","HasTimewinderAbility");
        vars.Helper["hasPC"] = nhm.Make<bool>("Instance","NetworkHero","SimulationHero","HasParallelConvergenceAbility");
        vars.Helper["hasPD"] = nhm.Make<bool>("Instance","NetworkHero","SimulationHero","HasPhaseDiveAbility");
        vars.Helper["hasTP"] = nhm.Make<bool>("Instance","NetworkHero","SimulationHero","HasTemporalPulseAbility");
        vars.Helper["hasDJ"] = nhm.Make<int>("Instance","NetworkHero","SimulationHero",1,1,"PlatformerController","AirJumpCount");
        vars.Helper["hasDash"] = nhm.Make<int>("Instance","NetworkHero","SimulationHero","Dodger","HasDashAbility");

        // Call Unity Function ?
        var func
        vars.Helper["gadgetSlots"] = ug.
        // Enum castable to string ?
        vars.Helper["worldZone"] = lvl.MakeString("Instance","LevelData","WorldZone");
        
        vars.GetMostRecentAbility = (Func<string>)(()=>{
            
        });

    });
}


shutdown
{ 
  vars.Helper.Dispose();
}