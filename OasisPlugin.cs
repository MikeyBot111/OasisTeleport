using Rocket.API.Collections;
using Rocket.Core.Logging;
using Rocket.Core.Plugins;
using Rocket.Unturned;
using Rocket.Unturned.Player;
using static Rocket.Unturned.Events.UnturnedEvents;

namespace OasisTeleport
{
    public class OasisPlugin : RocketPlugin<OasisConfiguration>
    {
        public static string version = "1.0.0.2";
        public static OasisPlugin Instance;

        public override TranslationList DefaultTranslations
        {
            get
            {
                return new TranslationList
                {
                    { "tpa_send_help": "[Oasis] /tpa (playerName) - Sends a teleport request." },
                    { "tpa_accept_help": "[Oasis] /tpa accept - Accepts a teleport request." },
                    { "tpa_deny_help": "[Oasis] /tpa deny - Denies a teleport request." },
                    { "tpa_abort_help": "[Oasis] /tpa abort - Cancels a teleport request." },
                    { "tpa_nopermission": "[Oasis] You don't have permission for this command." },
                    { "tpa_playerNotFound": "[Oasis] This player was not found!" },
                    { "tpa_targetInCar": "[Oasis] Teleport canceled, player is in a car." },
                    { "tpa_playerInCar": "[Oasis] Teleport canceled, you are in a car." },
                    { "tpa_cooldown": "[Oasis] You can teleport again in {0} seconds." },
                    { "tpa_delay": "[Oasis] Teleporting in {0} seconds." },
                    { "tpa_cooldown_error": "[Oasis] You can send a request every {0} seconds." },
                    { "tpa_bleeding_error": "[Oasis] You are bleeding, you can't teleport until you stop!" },
                    { "tpa_hurt_error": "[Oasis] You were just injured, you can't teleport!" },
                    { "tpa_help": "[Oasis] With TPA, you can teleport to another player." },
                    { "tpa_already_teleporting": "[Oasis] You are already teleporting." },
                    { "request_success": "[Oasis] You have successfully teleported!" },
                    { "request_accepted": "[Oasis] You have accepted a request from {0}!" },
                    { "request_denied": "[Oasis] You have denied a request from {0}!" },
                    { "request_accepted_1": "[Oasis] {0} accepted your request!" },
                    { "request_accepted_2": "[Oasis] {0} accepted your request and you will be teleported in {1} seconds." },
                    { "request_accepted_3": "[Oasis] You accepted a request from {0}, and will be teleported in {1} seconds." },
                    { "request_denied_1": "[Oasis] {0} denied your request!" },
                    { "request_sent": "[Oasis] You have sent a request to the player {0}" },
                    { "request_sent_1": "[Oasis] {0} sent you a teleport request, /tpa accept to accept!" },
                    { "request_pending": "[Oasis] You have already sent a request to this player once." },
                    { "request_none": "[Oasis] You have no teleport requests!" },
                    { "request_abort": "[Oasis] You have canceled the teleport request." },
                    { "home_disabled": "[Oasis] This command is disabled." },
                    { "home_no_bed": "[Oasis] You have no bed to teleport to." },
                    { "home_in_vehicle": "[Oasis] You cannot teleport while in a vehicle." },
                    { "home_success": "[Oasis] You have been teleported to your bed." },
                    { "home_bed_found": "[Oasis] Bed found, teleporting in {0} seconds." },
                    { "home_cooldown": "[Oasis] You can teleport again in {0} seconds." }
                };
            }
        }

        protected override void Load()
        {
            OasisPlugin.Instance = this;

            Logger.Log("OasisTeleport, Version " + OasisPlugin.version);
            Logger.Log("Checking errors... ");
            Logger.Log("No errors have been found!");

            U.Events.OnPlayerConnected += PlayerJoined;
            U.Events.OnPlayerDisconnected += PlayerDisconnect;
        }

        protected override void Unload()
        {
            U.Events.OnPlayerConnected -= PlayerJoined;
            U.Events.OnPlayerDisconnected -= PlayerDisconnect;
        }

        private void PlayerDisconnect(UnturnedPlayer player)
        {
            if (OasisCommandTeleport.Requests.ContainsKey(player.CSteamID))
            {
                OasisCommandTeleport.Requests.Remove(player.CSteamID);
            }
        }

        private void PlayerJoined(UnturnedPlayer player)
        {
            if (OasisCommandTeleport.Requests.ContainsKey(player.CSteamID))
            {
                OasisCommandTeleport.Requests.Remove(player.CSteamID);
            }
        }
    }
}

