# ConvergenceToolbox
ConvergenceToolbox is an open-source project dedicated to enhancing the speedrunning experience of Convergence : A League of Legends Story by providing a collection of tools and utilities


# Installation

- Download CTB binaries right [here](https://github.com/SkyMound/ConvergenceToolbox/releases)
- Download LiveSplit [here](https://livesplit.org/downloads/)
- Download LiveSplit Server [here](https://github.com/LiveSplit/LiveSplit.Server/releases) and place the content of this zip into "LiveSplit/Components"


# Setup

Let's now see how to setup the Autsplit with LiveSplit.

1. Run LiveSplit.  

2. Right click on LS&rarr;Open Splits&rarr;From File...&rarr;Select 'Splits_CTB.lss'

3. Right click on LS&rarr;Open Layout&rarr;From File...&rarr;Select 'Layout_CTB.lsl'

4. Right click on LS&rarr;Control&rarr;Start Server

5. Run Convergence. Wait until you're on the main menu.

6. Run CTB, you should see your CTB version in the top left corner of Convergence. Check 'Enable AutoSplit' and start a new game.

>**Note**
>For all the other tools except Autosplit, you just need Convergence to be up and running before running CTB.

# Features

## Implemented
- Autosplit
    - Automatic start, split and stop (Focus on your run and nothing else)
    - Handle Real Time (with loads) and Game Time (without loads) (Having a wooden pc is no longer an excuse)
    - Synchronize with the in-game pause menu (It's good now, you can go open to the postmen)
- Gizmos 
    - Hitbox trigger for Checkpoint, Battle, Cutscene and Level Loader

## Upcoming
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
