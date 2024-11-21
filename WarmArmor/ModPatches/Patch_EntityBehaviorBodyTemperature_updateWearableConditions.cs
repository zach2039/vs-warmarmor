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
	[HarmonyPatch(typeof(EntityBehaviorBodyTemperature), "updateWearableConditions")]
	class Patch_EntityBehaviorBodyTemperature_updateWearableConditions
	{
		static void Postfix(EntityBehaviorBodyTemperature __instance, ref float ___clothingBonus, ref ICoreAPI ___api)
		{
			EntityAgent entityAgent = __instance.entity as EntityAgent;
			EntityBehaviorPlayerInventory bh = (entityAgent != null) ? entityAgent.GetBehavior<EntityBehaviorPlayerInventory>() : null;
			if (((bh != null) ? bh.Inventory : null) != null)
			{
				foreach (ItemSlot slot in bh.Inventory)
				{
					ItemStack itemstack = slot.Itemstack;
					ItemWearable wearableItem = ((itemstack != null) ? itemstack.Collectible : null) as ItemWearable;
					if (wearableItem != null && wearableItem.IsArmor)
					{
						___clothingBonus += wearableItem.GetWarmth(slot);

						// Condition for armor should match the current durability
						float maxDurability = wearableItem.GetMaxDurability(itemstack);
						float currentDurability = wearableItem.GetRemainingDurability(itemstack);
						float conditionFromDurability = Math.Clamp(currentDurability / maxDurability, 0.0f, 1.0f);
						WarmArmorModSystem.SetSlotCondition(___api, slot, conditionFromDurability);
					}
				}
			}
		}
	}
}
