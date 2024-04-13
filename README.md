# Eosin_VRRenderer for MMDShow and mmd2timeline Player

## Overview

This is an extended version of [Eosin's VRRenderer Plugin](https://hub.virtamate.com/resources/video-renderer-for-3d-vr180-vr360-and-flat-2d-audio-bvh-animation-recorder.11994/) for Virt-A-Mate. See that link for further information.

In order to better support MMD recording, I made some modifications previously. Later, I discovered [yunidatsu's Modified Version](https://github.com/yunidatsu/Eosin_VRRenderer), which includes a highly useful multi-threaded rendering feature. I forked that version and merged my previous changes with it, resulting in this version.

**This version is recommended to be used in conjunction with MMDShow (version 3.0.0 or higher) and mmd2timeline Player (version 1.5 or higher).**

## License

Eosin released the plugin under CC BY-SA.

## Credits

Credit for this plugin goes mainly to Eosin. Further credits from the original release:

* Thanks to **MacGruber** for his previous work which this plugin builds and heavily relies upon!
* Thanks to **Élie Michel** for his LilyRender360 shader which is responsible for the 15x performance gain compared to a CPU-based implementation!
* Thanks to **kuler** for contributing the correct method to do transparent render in VaM!
* Thanks to **ragingsimian**, **morkork**, **VAMguy**, **3115062**, **Cleo** and **Vezezepu** for improvement suggestions!

And

* Thanks to **yunidatsu** for developing the multi-threaded rendering feature!
