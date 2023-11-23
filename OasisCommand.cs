using Rocket.API;
using Rocket.Core.Utils;
using Rocket.Unturned.Chat;
using Rocket.Unturned.Player;
using SDG.Unturned;
using Steamworks;
using System;
using System.Collections.Generic;
using UnityEngine;


namespace OasisTeleport
{
    public class OasisCommandTeleport : IRocketCommand
    {
        #region Declarations
        public static Dictionary<CSteamID, CSteamID> Requests = new Dictionary<CSteamID, CSteamID>();
        private Dictionary<CSteamID, DateTime> Cooldown = new Dictionary<CSteamID, DateTime>();
        private Dictionary<CSteamID, byte> Health = new Dictionary<CSteamID, byte>();
        public List<string> Permissions => new List<String>() { "OasisTeleport.tpa" };
        public AllowedCaller AllowedCaller => AllowedCaller.Player;
        public string Name => "tpa";
        public string Syntax => "tpa (hráč/accept/deny/ignore)";
        public string Help => "Příkaz pro teleport na hráče.";
        public List<string> Aliases => new List<string>();
        #endregion


        public void Execute(IRocketPlayer caller, string[] command)
        {

            #region Help
            UnturnedPlayer player = (UnturnedPlayer)caller;
            if (command.Length < 1)
            {
                UnturnedChat.Say(caller, OasisPlugin.Instance.Translate("tpa_send_help"), Color.yellow);
                UnturnedChat.Say(caller, OasisPlugin.Instance.Translate("tpa_accept_help"), Color.yellow);
                UnturnedChat.Say(caller, OasisPlugin.Instance.Translate("tpa_deny_help"), Color.yellow);
                UnturnedChat.Say(caller, OasisPlugin.Instance.Translate("tpa_abort_help"), Color.yellow);
                return;
            }
            #endregion

            #region Accept

            if (command[0].ToString().ToLower() == "accept" || command[0].ToString().ToLower() == "a" || command[0].ToString().ToLower() == "yes")
            {
                if (!player.HasPermission("tpa.accept") && !player.IsAdmin)
                {
                    UnturnedChat.Say(player, OasisPlugin.Instance.Translate("tpa_nopermission"), Color.red);
                    return;
                }

                if (Requests.ContainsKey(player.CSteamID))
                {
                    UnturnedPlayer teleporter = UnturnedPlayer.FromCSteamID(Requests[player.CSteamID]);

                    if (teleporter == null || !CheckPlayer(Requests[player.CSteamID]))
                    {
                        UnturnedChat.Say(caller, OasisPlugin.Instance.Translate("tpa_playerNotFound"), Color.red);
                        Requests.Remove(player.CSteamID);
                        return;
                    }

                    if (teleporter.Stance == EPlayerStance.DRIVING || teleporter.Stance == EPlayerStance.SITTING)
                    {
                        UnturnedChat.Say(teleporter, OasisPlugin.Instance.Translate("tpa_playerInCar"), Color.red);
                        UnturnedChat.Say(caller, OasisPlugin.Instance.Translate("tpa_targetInCar"), Color.red);
                        Requests.Remove(player.CSteamID);
                        return;
                    }

                    if (OasisPlugin.Instance.Configuration.Instance.TpaDelay)
                    {
                        DelayTP(player, teleporter, OasisPlugin.Instance.Configuration.Instance.TpaDelaySeconds);
                        return;
                    }

                    if (OasisPlugin.Instance.Configuration.Instance.CancelOnBleeding && teleporter.Bleeding)
                    {
                        UnturnedChat.Say(teleporter, OasisPlugin.Instance.Translate("tpa_cooldown_error"), Color.red);
                        Requests.Remove(player.CSteamID);
                        return;
                    }

                    UnturnedChat.Say(player, OasisPlugin.Instance.Translate("request_accepted", teleporter.CharacterName), Color.yellow);
                    UnturnedChat.Say(teleporter, OasisPlugin.Instance.Translate("request_accepted_1", player.CharacterName), Color.yellow);
                    //teleporter.Teleport(player);
                    teleporter.Teleport(player);
                    Requests.Remove(player.CSteamID);
                    return;
                }
                else
                {
                    UnturnedChat.Say(caller, OasisPlugin.Instance.Translate("request_none"), Color.red);
                    return;
                }
            }

            #endregion

            #region Deny

            if (command[0].ToString() == "deny" || command[0].ToString().ToLower() == "d" || command[0].ToString().ToLower() == "no")
            {
                if (Requests.ContainsKey(player.CSteamID))
                {
                    UnturnedPlayer teleporter = UnturnedPlayer.FromCSteamID(Requests[player.CSteamID]);
                    Requests.Remove(player.CSteamID);
                    UnturnedChat.Say(caller, OasisPlugin.Instance.Translate("request_denied", teleporter.CharacterName), Color.yellow);
                    UnturnedChat.Say(teleporter, OasisPlugin.Instance.Translate("request_denied_1", player.CharacterName), Color.red);
                    return;
                }
                else
                {
                    UnturnedChat.Say(caller, OasisPlugin.Instance.Translate("request_none"), Color.red); return;
                }
            }
            #endregion

            #region Abort

            if (command[0].ToString() == "abort")
            {
                bool flag = false;
                foreach (Steamworks.CSteamID id in OasisCommandTeleport.Requests.Keys)
                {
                    if (OasisCommandTeleport.Requests[id] == player.CSteamID)
                    {
                        OasisCommandTeleport.Requests.Remove(id);
                        UnturnedChat.Say(player, OasisPlugin.Instance.Translate("request_abort"), Color.yellow);
                        flag = true;
                        break;
                    }
                }
                if (!flag)
                {
                    UnturnedChat.Say(caller, OasisPlugin.Instance.Translate("request_none"), Color.red);
                }
                return;
            }
            #endregion

            #region Send

            if (!player.HasPermission("tpa.send"))
            {
                UnturnedChat.Say(player, OasisPlugin.Instance.Translate("tpa_nopermission"), Color.red);
                return;
            }

            UnturnedPlayer requestTo = UnturnedPlayer.FromName(command[0].ToString());

            if (requestTo == null || requestTo.CSteamID == player.CSteamID)
            {
                UnturnedChat.Say(caller, OasisPlugin.Instance.Translate("tpa_playerNotFound"), Color.red);
                return;
            }

            if (OasisPlugin.Instance.Configuration.Instance.TpaCooldown)
            {
                if (Cooldown.ContainsKey(player.CSteamID))
                {
                    int timeLeft = Convert.ToInt32(System.Math.Abs((DateTime.Now - Cooldown[player.CSteamID]).TotalSeconds));
                    if (timeLeft < OasisPlugin.Instance.Configuration.Instance.TpaCooldownSeconds)
                    {
                        UnturnedChat.Say(caller, OasisPlugin.Instance.Translate("tpa_cooldown_error", OasisPlugin.Instance.Configuration.Instance.TpaCooldownSeconds), Color.red);
                        UnturnedChat.Say(caller, OasisPlugin.Instance.Translate("tpa_cooldown", (OasisPlugin.Instance.Configuration.Instance.TpaCooldownSeconds - timeLeft), Color.yellow));
                        return;
                    }
                    Cooldown.Remove(player.CSteamID);
                }
            }

            if (Requests.ContainsKey(requestTo.CSteamID))
            {
                if (Requests[requestTo.CSteamID] == player.CSteamID)
                {
                    UnturnedChat.Say(caller, OasisPlugin.Instance.Translate("request_pending"), Color.red);
                    return;
                }
            }

            if (Requests.ContainsKey(requestTo.CSteamID))
                Requests[requestTo.CSteamID] = player.CSteamID;
            else
                Requests.Add(requestTo.CSteamID, player.CSteamID);

            UnturnedChat.Say(caller, OasisPlugin.Instance.Translate("request_sent", requestTo.CharacterName), Color.yellow);
            UnturnedChat.Say(requestTo, OasisPlugin.Instance.Translate("request_sent_1", player.CharacterName), Color.yellow);


            if (Cooldown.ContainsKey(player.CSteamID))
            {
                Cooldown[player.CSteamID] = DateTime.Now;
            }
            else
            {
                Cooldown.Add(player.CSteamID, DateTime.Now);
            }
            #endregion

        }

        private void DelayTP(UnturnedPlayer player, UnturnedPlayer teleporter, int delay)
        {
            UnturnedChat.Say(teleporter, OasisPlugin.Instance.Translate("request_accepted_2", player.CharacterName, delay), Color.yellow);
            UnturnedChat.Say(teleporter, OasisPlugin.Instance.Translate("tpa_delay", delay), Color.yellow);
            UnturnedChat.Say(player, OasisPlugin.Instance.Translate("request_accepted_3", teleporter.CharacterName, delay), Color.yellow);

            if (Health.ContainsKey(teleporter.CSteamID))
                Health[teleporter.CSteamID] = teleporter.Health;
            else
                Health.Add(teleporter.CSteamID, teleporter.Health);

            TaskDispatcher.QueueOnMainThread(() =>
            {

                if (OasisPlugin.Instance.Configuration.Instance.CancelOnBleeding && teleporter.Bleeding)
                {
                    UnturnedChat.Say(teleporter, OasisPlugin.Instance.Translate("tpa_bleeding_error"), Color.red);
                    Requests.Remove(player.CSteamID);
                }
                else if (OasisPlugin.Instance.Configuration.Instance.CancelOnHurt && Health.ContainsKey(teleporter.CSteamID) && Health[teleporter.CSteamID] > teleporter.Health)
                {
                    UnturnedChat.Say(teleporter, OasisPlugin.Instance.Translate("tpa_hurt_error"), Color.red);
                    Requests.Remove(player.CSteamID);
                    Health.Remove(teleporter.CSteamID);
                }
                else
                {
                    UnturnedChat.Say(teleporter, OasisPlugin.Instance.Translate("request_success"), Color.yellow);
                    //teleporter.Teleport(player);
                    teleporter.Teleport(player);
                    Requests.Remove(player.CSteamID);
                    Health.Remove(teleporter.CSteamID);
                }

            }, (float)delay);
        }

        private bool CheckPlayer(Steamworks.CSteamID plr)
        {
            bool flag = false;
            foreach (SteamPlayer sp in Provider.clients)
            {
                if (sp.playerID.steamID == plr)
                {
                    flag = true;
                }
            }

            return flag;
        }
    }
    public class OasisCommandHome : IRocketCommand
    {
        #region Declarations
        private Dictionary<CSteamID, DateTime> Cooldown = new Dictionary<CSteamID, DateTime>();
        private Dictionary<CSteamID, byte> Health = new Dictionary<CSteamID, byte>();
        public List<string> Permissions => new List<String>() { "OasisTeleport.home" };
        public AllowedCaller AllowedCaller => AllowedCaller.Player;
        public string Name => "home";
        public string Syntax => "home";
        public string Help => "Příkaz pro teleport na postel.";
        public List<string> Aliases => new List<string>();
        #endregion
        public void Execute(IRocketPlayer caller, string[] command)
        {
            UnturnedPlayer player = (UnturnedPlayer)caller;

            if (!OasisPlugin.Instance.Configuration.Instance.HomeEnabled)
            {
                UnturnedChat.Say(player, OasisPlugin.Instance.Translate("home_disabled"), Color.red);
                return;
            }

            if (!player.HasPermission("home") && !player.IsAdmin)
            {
                UnturnedChat.Say(player, OasisPlugin.Instance.Translate("home_nopermission"), Color.red);
                return;
            }

            if (!BarricadeManager.tryGetBed(player.CSteamID, out var bedPos, out var bedRot))
            {
                UnturnedChat.Say(player, OasisPlugin.Instance.Translate("home_no_bed"), Color.red);
                return;
            }

            if (player.Stance == EPlayerStance.DRIVING || player.Stance == EPlayerStance.SITTING)
            {
                UnturnedChat.Say(player, OasisPlugin.Instance.Translate("home_in_vehicle"), Color.red);
                return;
            }

            if (Health.ContainsKey(player.CSteamID))
            {
                UnturnedChat.Say(player, OasisPlugin.Instance.Translate("tpa_already_teleporting"), Color.red);
                return;
            }

            if (OasisPlugin.Instance.Configuration.Instance.HomeCooldown)
            {
                if (Cooldown.ContainsKey(player.CSteamID))
                {
                    int timeLeft = Convert.ToInt32(System.Math.Abs((DateTime.Now - Cooldown[player.CSteamID]).TotalSeconds));
                    if (timeLeft < OasisPlugin.Instance.Configuration.Instance.HomeCooldownSeconds)
                    {
                        UnturnedChat.Say(player, OasisPlugin.Instance.Translate("home_cooldown", (OasisPlugin.Instance.Configuration.Instance.TpaCooldownSeconds - timeLeft), Color.yellow));
                        return;
                    }
                    Cooldown.Remove(player.CSteamID);
                }
            }

            if (OasisPlugin.Instance.Configuration.Instance.HomeDelay)
            {
                DelayTP(player, OasisPlugin.Instance.Configuration.Instance.HomeDelaySeconds);
                return;
            }

            UnturnedChat.Say(player, OasisPlugin.Instance.Translate("home_success"), Color.green);

            if (Cooldown.ContainsKey(player.CSteamID))
            {
                Cooldown[player.CSteamID] = DateTime.Now;
            }
            else
            {
                Cooldown.Add(player.CSteamID, DateTime.Now);
            }
        }

        private void DelayTP(UnturnedPlayer player, int delay)
        {
            UnturnedChat.Say(player, OasisPlugin.Instance.Translate("home_bed_found", delay), Color.yellow);

            if (Health.ContainsKey(player.CSteamID))
                Health[player.CSteamID] = player.Health;
            else
                Health.Add(player.CSteamID, player.Health);

            TaskDispatcher.QueueOnMainThread(() =>
            {

                if (OasisPlugin.Instance.Configuration.Instance.CancelOnHurt && Health.ContainsKey(player.CSteamID) && Health[player.CSteamID] > player.Health)
                {
                    UnturnedChat.Say(player, OasisPlugin.Instance.Translate("home_hurt"), Color.red);
                    Health.Remove(player.CSteamID);
                }
                else if (player.Dead)
                {
                    UnturnedChat.Say(player, OasisPlugin.Instance.Translate("home_died"), Color.red);
                    Health.Remove(player.CSteamID);
                }
                else
                {
                    Health.Remove(player.CSteamID);
                    bool gotBed = BarricadeManager.tryGetBed(player.CSteamID, out Vector3 bedPos, out Byte bedRot);
                    Vector3 _bedPos = Vector3.up + bedPos + new Vector3(0f, 0.5f, 0f); ;
                    Byte _bedRot = bedRot;
                    if (gotBed)
                    {
                        UnturnedChat.Say(player, OasisPlugin.Instance.Translate("home_success"), Color.green);
                        player.Teleport(_bedPos, _bedRot);
                        return;
                    }
                    else
                    {
                        UnturnedChat.Say(player, OasisPlugin.Instance.Translate("home_no_bed"), Color.red);
                        return;
                    }
                }

            }, (float)delay);
        }
    }
}