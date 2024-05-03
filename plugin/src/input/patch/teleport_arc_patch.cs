using HarmonyLib;
using Valve.VR.InteractionSystem;

namespace PiVrLoader.Input;

[HarmonyPatch]
class TeleportArcPatch
{
	[HarmonyPatch(typeof(TeleportArc), "Update")]
	[HarmonyPrefix]
	static bool Update()
	{
		return false;
	}
}
