# SwarmUI-AdaptiveProjectedGuidance

A [SwarmUI](https://github.com/mcmonkeyprojects/SwarmUI/) extension that adds parameters for MythicalChu's [ComfyUI-APG_ImYourCFGNow](https://github.com/MythicalChu/ComfyUI-APG_ImYourCFGNow) custom node to the generate tab. Retain the quality-boosting advantages of CFG while enabling the use of higher guidance scales without oversaturation.

## Changelog
<details>
  <summary>16 November 2024</summary>

* Initial Release
</details>

## Installation
1. Update SwarmUI first, if you have an old version this extension may not work
2. Shutdown SwarmUI
3. Open a cmd/terminal window in `SwarmUI\src\Extensions`
4. Run `git clone https://github.com/Quaggles/SwarmUI-AdaptiveProjectedGuidance.git`
5. Run `SwarmUI\update-windows.bat` or `SwarmUI\update-linuxmac.sh` to recompile SwarmUI
6. If the `ComfyUI-APG_ImYourCFGNow` custom node is not installed in the backend an install button will be shown in the parameter group, install it and follow the prompts
7. Now the parameters should appear and you are good to go

## Updating
1. Update SwarmUI first, if you have an old version this extension may not work
2. In SwarmUI go to the Server/Extensions tab
3. Click the update button for 'AdaptiveProjectedGuidanceExtension'

## Usage

I'd recommend first experimenting by comparing a generation with and without the Adaptive Projected Guidance, then experiment with turning up your CFG Scale to 8-12 and decreasing `Momentum` and `Norm Threshold` params to control the saturation/burn.
