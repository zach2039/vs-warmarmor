using ProtoBuf;

namespace WarmArmor.ModNetwork
{
	[ProtoContract(ImplicitFields = ImplicitFields.AllPublic)]
	public class SyncConfigClientPacket
	{
		public bool PatchEntityBehaviorBodyTemperature;

		public bool PatchItemWearable;
	}
}