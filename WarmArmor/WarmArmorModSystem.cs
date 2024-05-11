using HarmonyLib;
using Vintagestory.API.Client;
using Vintagestory.API.Common;
using Vintagestory.API.Config;
using Vintagestory.API.Datastructures;
using Vintagestory.API.MathTools;
using Vintagestory.API.Server;
using Vintagestory.GameContent;
using WarmArmor.ModNetwork;
using WarmArmor.ModPatches;

namespace WarmArmor
{
	public class WarmArmorModSystem : ModSystem
	{
		private IServerNetworkChannel serverChannel;
		
		private ICoreAPI api;

		public Harmony harmony;
		
	   public override void StartPre(ICoreAPI api)
		{
			string cfgFileName = "WarmArmor.json";

			try 
			{
				WarmArmorConfig cfgFromDisk;
				if ((cfgFromDisk = api.LoadModConfig<WarmArmorConfig>(cfgFileName)) == null)
				{
					api.StoreModConfig(WarmArmorConfig.Loaded, cfgFileName);
				}
				else
				{
					WarmArmorConfig.Loaded = cfgFromDisk;
				}
			} 
			catch 
			{
				api.StoreModConfig(WarmArmorConfig.Loaded, cfgFileName);
			}

			base.StartPre(api);
		}

		public override void Start(ICoreAPI api)
		{
			this.api = api;

			if (!Harmony.HasAnyPatches(Mod.Info.ModID)) {
				harmony = new Harmony(Mod.Info.ModID);

				if (WarmArmorConfig.Loaded.PatchEntityBehaviorBodyTemperature)
				{
					var original = typeof(EntityBehaviorBodyTemperature).GetMethod("updateWearableConditions", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
					var postfix = typeof(Patch_EntityBehaviorBodyTemperature_updateWearableConditions).GetMethod("Postfix", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Static);
					
					harmony.Patch(original, null, new HarmonyMethod(postfix));			

					api.Logger.Notification("Applied patch to VintageStory's EntityBehaviorBodyTemperature.updateWearableCondition from Warm Armor!");
				}

				if (WarmArmorConfig.Loaded.PatchItemWearable)
				{
					var original0 = typeof(ItemWearable).GetMethod("ensureConditionExists", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
					var postfix0 = typeof(ModPatches.Patch_ItemWearable_ensureConditionExists).GetMethod("Postfix", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Static);
					
					harmony.Patch(original0, null, new HarmonyMethod(postfix0));			

					api.Logger.Notification("Applied patch to VintageStory's ItemWearable.ensureConditionExists from Warm Armor!");

					var original1 = typeof(ItemWearable).GetMethod("OnCreatedByCrafting", System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance);
					var postfix1 = typeof(Patch_ItemWearable_OnCreatedByCrafting).GetMethod("Postfix", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Static);
					
					harmony.Patch(original1, null, new HarmonyMethod(postfix1));			

					api.Logger.Notification("Applied patch to VintageStory's ItemWearable.OnCreatedByCrafting from Warm Armor!");
				}
			}

			base.Start(api);

			api.Logger.Notification("Loaded Warm Armor!");
		}

		private void OnPlayerJoin(IServerPlayer player)
		{
			// Send connecting players config settings
			this.serverChannel.SendPacket(
				new SyncConfigClientPacket {
					PatchEntityBehaviorBodyTemperature = WarmArmorConfig.Loaded.PatchEntityBehaviorBodyTemperature,
					PatchItemWearable = WarmArmorConfig.Loaded.PatchItemWearable,
				}, player);
		}

		public override void StartServerSide(ICoreServerAPI sapi)
		{
			sapi.Event.PlayerJoin += this.OnPlayerJoin; 
			
			// Create server channel for config data sync
			this.serverChannel = sapi.Network.RegisterChannel(Mod.Info.ModID)
				.RegisterMessageType<SyncConfigClientPacket>()
				.SetMessageHandler<SyncConfigClientPacket>((player, packet) => {});
		}

		public override void StartClientSide(ICoreClientAPI capi)
		{
			// Sync config settings with clients
			capi.Network.RegisterChannel(Mod.Info.ModID)
				.RegisterMessageType<SyncConfigClientPacket>()
				.SetMessageHandler<SyncConfigClientPacket>(p => {
					this.Mod.Logger.Event("Received config settings from server");
					WarmArmorConfig.Loaded.PatchEntityBehaviorBodyTemperature = p.PatchEntityBehaviorBodyTemperature;
					WarmArmorConfig.Loaded.PatchItemWearable = p.PatchItemWearable;
				});
		}
		
		public override void Dispose()
		{
			if (this.api is ICoreServerAPI sapi)
			{
				sapi.Event.PlayerJoin -= this.OnPlayerJoin;
			}
		}

		/// <summary>
		/// Copied from ItemWearable
		/// </summary>
		public static void EnsureConditionExistsForSlot(ICoreAPI api, ItemSlot slot)
		{
			if (slot is DummySlot || slot.Itemstack.Attributes.HasAttribute("condition") || api.Side != EnumAppSide.Server)
			{
				return;
			}

			JsonObject itemAttributes = slot.Itemstack.ItemAttributes;
			if (itemAttributes == null || !itemAttributes["warmth"].Exists)
			{
				return;
			}

			JsonObject itemAttributes2 = slot.Itemstack.ItemAttributes;
			if (itemAttributes2 == null || itemAttributes2["warmth"].AsFloat() != 0f)
			{
				if (slot is ItemSlotTrade)
				{
					slot.Itemstack.Attributes.SetFloat("condition", (float)api.World.Rand.NextDouble() * 0.25f + 0.75f);
				}
				else
				{
					slot.Itemstack.Attributes.SetFloat("condition", (float)api.World.Rand.NextDouble() * 0.4f);
				}

				slot.MarkDirty();
			}
		}

		/// <summary>
		/// Copied from ItemWearable
		/// </summary>
		public static void SetSlotCondition(ICoreAPI api, ItemSlot slot, float setVal)
		{
			EnsureConditionExistsForSlot(api, slot);
			slot.Itemstack.Attributes.SetFloat("condition", GameMath.Clamp(setVal, 0f, 1f));
			slot.MarkDirty();
		}
	}
}
