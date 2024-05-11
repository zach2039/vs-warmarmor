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
	[HarmonyPatch(typeof(ItemWearable), "ensureConditionExists")]
	class Patch_ItemWearable_ensureConditionExists
	{
		static void Postfix(ItemWearable __instance, ItemSlot slot, ref ICoreAPI ___api)
		{
			if (__instance.IsArmor && ___api.Side == EnumAppSide.Server)
			{	
				JsonObject itemAttributes = slot.Itemstack.ItemAttributes;
				if (itemAttributes != null && itemAttributes["warmth"].Exists)
				{
					JsonObject itemAttributes2 = slot.Itemstack.ItemAttributes;
					if (itemAttributes2 == null || itemAttributes2["warmth"].AsFloat(0f) != 0f)
					{
						// Condition for armor should match the current durability
						float maxDurability = slot.Itemstack.Collectible.GetMaxDurability(slot.Itemstack);
						float currentDurability = slot.Itemstack.Collectible.GetRemainingDurability(slot.Itemstack);
						float conditionFromDurability = Math.Clamp(currentDurability / maxDurability, 0.0f, 1.0f);
						slot.Itemstack.Attributes.SetFloat("condition", conditionFromDurability);
					}
				}
			}
		}
	}
}
