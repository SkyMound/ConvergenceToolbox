# ConvergenceToolbox
ConvergenceToolbox is an open-source project dedicated to enhancing the speedrunning experience of Convergence : A League of Legends Story by providing a collection of tools and utilities


# Installation

- Download CTB binaries right [here](https://github.com/SkyMound/ConvergenceToolbox/releases)
- Download LiveSplit [here](https://livesplit.org/downloads/)
- Download LiveSplit Server [here](https://github.com/LiveSplit/LiveSplit.Server/releases) and place the content of this zip into "LiveSplit/Components"


# Setup

Start by executing Convergence and LiveSplit.  
(**If you just installed LiveSplit Server**, LS Server wont appear under Control, so lets add it : go to Edit Layout&rarr;click on the '+' icon&rarr;Control&rarr;LS Server)

Now you can right click on LS&rarr;Control&rarr;Start Server.

You also need to import the splits template (lss file) in LiveSplit.

Finally, you can execute CTB. If all works well, you should see your CTB version in the top left corner.


# Features

## Implemented
- Autosplit
    - Automatic start, split and stop (Focus on your run and nothing else)
    - Synchronize with the in-game pause menu (It's good now, you can go open to the postmen)
    - Pause when loading screen (Having a wooden pc is no longer an excuse)

## Upcoming
- Gizmos (Checkpoint, Combat/Cutscene Trigger)
- Save/Checkpoint Manager
- In-game Tips/Tricks
- Modding
- Camera control


# Contributing

We highly appreciate contributions from the community! If you encounter any issues, have ideas for improvements, or simply want to ask questions, we encourage you to engage with us in the following ways.
1. **GitHub Issues**: Feel free to open a GitHub issue in our [issue tracker](https://github.com/SkyMound/ConvergenceToolbox/issues) to report a bug, request a new feature, or provide feedback. 
2. **Discord Community**: For more interactive discussions, assistance, or to share your ideas directly with the community, you are welcome to join our Speedrunning Discord server. Use this [invitation link](https://discord.gg/FXame4kQ7h) to join the server.
3. **Pull Request**: If you would like to contribute directly to the codebase, you can submit a pull request. Simply fork the repository, make your changes, and submit a pull request. We will review your changes and merge them if they align with the project's goals.

## Build the project

```
git clone https://github.com/SkyMound/ConvergenceToolbox.git
```

Resolve dependencies of Tools project by referencing the dll located in the Convergence folder. No need to add them all, prioritize the one starting with DS and Unity.

```
...\Steam\steamapps\common\Convergence\Convergence_Data\Managed
```

Build the project with [.NET Framework 4.8 SDK](https://dotnet.microsoft.com/en-us/download/dotnet-framework/net48)

And you're ready to go.
