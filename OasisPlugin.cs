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
        public static string version = "1.0.0.1";
        public static OasisPlugin Instance;

        public override TranslationList DefaultTranslations
        {
            get
            {
                return new TranslationList
                {
                    { "tpa_send_help", "[Oasis] /tpa (playerName) - Odešle žádost o teleport." },
                    { "tpa_accept_help", "[Oasis] /tpa accept - Přijme žádost o teleport." },
                    { "tpa_deny_help", "[Oasis] /tpa deny - Zamítne žádost o teleport." },
                    { "tpa_abort_help", "[Oasis] /tpa abort - Zruší žádost o teleport." },
                    { "tpa_nopermission", "[Oasis] Nemáš oprávnění na tento příkaz." },
                    { "tpa_playerNotFound", "[Oasis] Tento hráč nebyl nalezen!" },
                    { "tpa_targetInCar", "[Oasis] Teleport zrušen, hráč je v autě." },
                    { "tpa_playerInCar", "[Oasis] Teleport zrušen, jsi v autě." },
                    { "tpa_cooldown", "[Oasis] Můžeš se zase teleportovat za {0} sekund." }, 
                    { "tpa_delay", "[Oasis] Teleportnuti za {0} sekund." }, 
                    { "tpa_cooldown_error", "[Oasis] Žádost můžeš odesílat každých {0} sekund." }, 
                    { "tpa_bleeding_error", "[Oasis] Krvacíš až přestaneš můžeš se zase teleportovat!" },
                    { "tpa_hurt_error", "[Oasis] Byl si zrovna zraněn nemůžeš se teleportovat!" },
                    { "tpa_help", "[Oasis] Pomocí TPA se můžeš teleportnou na jiného hráče." },
                    { "tpa_already_teleporting", "[Oasis] Už se teleportuješ." },
                    { "request_success", "[Oasis] Teleportaval jsi se úspěšně!" },
                    { "request_accepted", "[Oasis] Přijmul jsi žádost od {0}!" }, 
                    { "request_denied", "[Oasis] Zamítnul jsi žádost od {0}!" },
                    { "request_accepted_1", "[Oasis] {0} přijmul tvojí žádost!" }, 
                    { "request_accepted_2", "[Oasis] {0} přijmul tvojí žádost a budeš teleportnut za {1} sekund." },
                    { "request_accepted_3", "[Oasis] Přijmul jsi žádost od {0}, bude teleportován za {1} sekund." }, 
                    { "request_denied_1", "[Oasis] {0} odmítnul tvoji žádost!" },
                    { "request_sent", "[Oasis] Odeslal jsi žádost hráči {0}" },
                    { "request_sent_1", "[Oasis] {0} ti odelslal žádost o teleport, /tpa accept pro přijmutí!" },
                    { "request_pending", "[Oasis] Už si jednou odeslal žádost tomuto hráči." },
                    { "request_none", "[Oasis] Nemáš žádné žádosti o teleport!" },
                    { "request_abort", "[Oasis] Zrušil si žádost o teleport." },
                    { "home_disabled", "[Oasis] Tento příkaz je vypnut." },
                    { "home_no_bed", "[Oasis] Nemáš žádnou postel kam se můžeš teleportovat." },
                    { "home_in_vehicle", "[Oasis] V autě se teleportovat nemůžeš." },
                    { "home_success", "[Oasis] Byl si teleportován na postel." },
                    { "home_bed_found", "[Oasis] Postel nalezena, teleportuji za {0} sekund." },
                    { "home_cooldown", "[Oasis] Teleport můžeš zase použít za {0} sekund." },
                    { "home_died", "[Oasis] Zemřel si dřív než jsem tě mohl teleportovat." },
                    { "home_hurt", "[Oasis] Teleport zrušen, zranil si se mezitím co si se teleportovat." },
                    { "home_nopermission", "[Oasis] Nemáš oprávnění na tento příkaz." },
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

