using System;
using System.Collections.Generic;
using System.Linq;
using HarmonyLib;
using Vintagestory.API.Common;
using Vintagestory.API.Common.Entities;
using Vintagestory.API.Datastructures;
using Vintagestory.API.MathTools;
using Vintagestory.GameContent;

namespace WarmArmor.ModPatches
{
	[HarmonyPatch(typeof(ItemWearable), "GetMergableQuantity")]
	class Patch_ItemWearable_GetMergableQuantity
	{
		static bool Prefix(ItemWearable __instance, ref int __result, ItemStack sinkStack, ItemStack sourceStack, EnumMergePriority priority)
		{
			
			if (priority == EnumMergePriority.DirectMerge && __instance.IsArmor)
			{
				JsonObject itemAttributes = sinkStack.ItemAttributes;
				if (itemAttributes != null && itemAttributes["warmth"].Exists)
				{
					// Prevent armor from getting its condition repaired from repair items like twine
					__result = 0;
					return false; // skip original method
				}
			}

			return true; // resume original method
		}
	}
}
