state("Convergence") { }

startup
{
    // vars.Log = (Action<object>)((output) => print("[Process ASL] " + output));

    Assembly.Load(File.ReadAllBytes("Components/asl-help")).CreateInstance("Unity");
    vars.Helper.GameName = "Convergence: A League Of Legends Story";

    vars.SplitCounter = 0;

}

init
{
    vars.Helper.TryLoad = (Func<dynamic, bool>)(mono =>
    {
        // vars.Helper["MissionState"] = mono.Make<int>("Game", "mission", "state");
        // vars.Helper["MissionTime"] = mono.Make<float>("Game", "mission", "rawResults", "time");

        return true;
    });
}


shutdown
{ 
  vars.Helper.Dispose();
}