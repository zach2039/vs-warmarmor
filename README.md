Warm Armor
=================

Overview
--------

This mod adds warmth to non-metal armors, as well as changes armor condition to match current durability to prevent always max warmth on armors with `"warmth"` attribute.

Changes include:

 * When armors are given warmth attributes, their condition (which affects warmth given) will now match their durability.

 * Leather jerkin body armor provides 2°C of warmth.

 * Leather jerkin leg armor provides 1°C of warmth.

 * Sewn and tailored Gambeson head armor provides 1°C of warmth.

 * Sewn and tailored Gambeson body armor provides 3°C of warmth.

 * Sewn and tailored Gambeson leg armor provides 1.5°C of warmth.


Config Settings (`VintageStoryData/ModConfig/ToolworksAdditions.json`)
--------

 * `PatchEntityBehaviorBodyTemperature`: Enables or disables harmony patch that changes how armor with warmth condition is handled for armor; defaults to `true`.

 * `PatchItemWearable`: Enables or disables harmony patch that changes how condition for armor is generated on craft and item creation; defaults to `true`.


Future Plans
--------

 * None, atm.


Known Issues
--------

 * None, atm.
