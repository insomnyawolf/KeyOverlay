
# KeyOverlay
 A simple multipropouse key overlay (pss, works for osu! too)

# [Old Version Download Link](https://github.com/insomnyawolf/KeyOverlay/releases/)

New version is not ready yet

IF YOU ARE HAVING PROBLEMS WITH FULLSCREEN, TRY USING GAME CAPTURE INSTEAD OF WINDOW CAPTURE IN OBS!

## Insomnya's note

This is basically a full rewrite focused on trying to add features from several different branches and optimize what we had already (bruh linq is cool but wa completly stupid to use it here, changing ``foreach`` into ``for`` is faster and also give you the index that they were using on linq to find things, i know it ain't much but each bit counts).

↑ I've already suffered this problem in the past, one learn something new and tries to use it everywhere... it happens sometimes ¯\\_(ツ)_/¯ ↑

I'm also trying to achive "hot-reloading" so you can change the config of the program and it updates the ui without needen to close and reopen it.

I'll release this under ``zlib License`` mixed with ``Emailware``, ``LinkWare`` and ``Beerware``, which basically means that you can do whatever you want with this but if you think this software is useful, let me know (we all like to know if what we make helps someone), add a link to places where you use it (if you want, but it will help oter people find this software), and if we ever met irl, talking a bit while we share some cold drinks may be awesome (dw, i'll continue with the second round)

Have a great day!!

## Progress

Rewrite started, i'm using veldrid to manage the output so it may be possible to port it to other operating systems in a future.

It even let you to chose the GraphicsBackend between ``Direct3D11, Vulkan, OpenGL, Metal, OpenGL, OpenGLES```

This is basically a raw rendering engine so it gives me control of everything, and that means a lot of work to get anything done but it even let us to use custom shaders for things.

At the same time i'm updating and creating another 2 libraries that will help with the configuration management and git things (like helping the user opening error reports, autoupdating etc)

### Too lazy, didn't read

This is harder than i expected D:

### Done

* Basic Output
* Imgui Render (to prepare live controls \\:D/)
* Global Keyboar Event Listener (Windows only for now)

## Known Issues

* Keeping pressed keys on keyboard returns unknown event
