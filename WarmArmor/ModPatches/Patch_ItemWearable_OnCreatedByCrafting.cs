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
	[HarmonyPatch(typeof(ItemWearable), "OnCreatedByCrafting")]
	class Patch_ItemWearable_OnCreatedByCrafting
	{
		static void Postfix(ItemWearable __instance, ItemSlot[] inSlots, ItemSlot outputSlot, GridRecipe byRecipe)
		{
			if (__instance.IsArmor)
			{	// Condition for armor should match the current durability
				float maxDurability = outputSlot.Itemstack.Collectible.GetMaxDurability(outputSlot.Itemstack);
				float currentDurability = outputSlot.Itemstack.Collectible.GetRemainingDurability(outputSlot.Itemstack);
				float conditionFromDurability = Math.Clamp(currentDurability / maxDurability, 0.0f, 1.0f);
				outputSlot.Itemstack.Attributes.SetFloat("condition", conditionFromDurability);
			}
		}
	}
}
