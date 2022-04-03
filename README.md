
# KeyOverlay
 A simple key overlay for osu! streaming
 
 If you're interested in per key coloring and custom width for your keys please check out [Friedchiken-42's version](https://github.com/Friedchicken-42/KeyOverlay) better suited for mania.
To change the keys used please use config.txt
# [Download Link](https://github.com/gxytcgxytc/KeyOverlay/releases/tag/v1.1.0)
IF YOU ARE HAVING PROBLEMS WITH THE PROGRAM WHEN OSU! IS ON FULLSCREEN, TRY USING GAME CAPTURE INSTEAD OF WINDOW CAPTURE IN OBS!


## config.json properties
keyAmount - Calculated via defined and enabled keys

keys => Keys the program should use

they can be:
* Keyboard (for numbers and symbols [please refer to this table](https://www.sfml-dev.org/documentation/2.5.1/classsf_1_1Keyboard.php#acb4cacd7cc5802dec45724cf3314a142)),
* Mouse [mouse button options](https://www.sfml-dev.org/documentation/2.5.1/classsf_1_1Mouse.php#a4fb128be433f9aafe66bc0c605daaa90).

There should be several examples

Hopefully the Config can be easily understood by everyone but if it isn't i'll try to write proper documentation on it

## New Features

* Per-Key Color
* Keycount saving

## Known Issues

* Hot reload not properly working (yet)
* You Tell Me

## Insomnya's note

This is basically a BIG rewrite focused on trying to add features from several different branches.

I'm also trying to achive "hot-reloading" so you can change the config of the program and it updates the ui without needen to close and reopen it.

I'd like to sare this as ``MIT`` or ``ISC`` or ``Beerware`` bit it's sadly ``GNU GPL`` so i basically can't (Maybe in a future if i do a complete rewrite from scratch.)...

However, the ConfigHelper is completly made by me and you can use it as ``MIT`` or ``ISC`` or ``Beerware``, the one you prefer.

Have a great day!!
