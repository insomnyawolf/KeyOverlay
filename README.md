
# KeyOverlay
 A simple multipropouse key overlay (pss, works for osu! too)

# [Download Link]() Not ready yet
IF YOU ARE HAVING PROBLEMS WITH FULLSCREEN, TRY USING GAME CAPTURE INSTEAD OF WINDOW CAPTURE IN OBS!


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
* Per-Key HitCount saving

## Known Issues

* Hot reload not properly working (yet)
* You Tell Me

## Insomnya's note

This is basically a BIG rewrite focused on trying to add features from several different branches and optimize what we had already (bruh linq is cool but wa completly stupid to use it here, changing ``foreach`` into ``for`` is faster and also give you the index that they were using on linq to find things, i know it ain't much but each bit counts).

↑ I've already suffered this problem in the past, one learn something new and tries to use it everywhere... it happens sometimes ¯\_(ツ)_/¯ ↑

I'm also trying to achive "hot-reloading" so you can change the config of the program and it updates the ui without needen to close and reopen it.

I'd like to sare this as ``MIT`` or ``ISC`` or ``Beerware`` bit it's sadly ``GNU GPL`` so i basically can't (Maybe in a future if i do a complete rewrite from scratch.)...

However, the ConfigHelper is completly made by me and you can use it as ``MIT`` or ``ISC`` or ``Beerware``, the one you prefer.

Have a great day!!
