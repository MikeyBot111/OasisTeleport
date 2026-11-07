# OasisTeleport

> **⚠️ Archived – old project, no longer maintained.**
> Written in 2023 for the **Oasis** Unturned servers. It's kept here for reference only. It may not work with current versions of Unturned or RocketMod.

A RocketMod plugin for **Unturned** that adds **teleport requests between players (TPA)** and **teleport to your bed (`/home`)**.

## What it does

- **`/tpa`**: send, accept, deny or cancel teleport requests to other players
- **`/home`**: teleport to your claimed bed
- Optional **delay** before teleporting, and **cooldown** between uses (set separately for TPA and home)
- Optional cancel if the player is **bleeding** or was **just hurt**
- Blocks teleporting while you or the target are in a vehicle
- Pending requests are cleared when a player joins or leaves

## Commands & permissions

| Command | Permission | Description |
|---|---|---|
| `/tpa <player>` | `OasisTeleport.tpa` | Send a teleport request |
| `/tpa accept` / `deny` / `abort` | `OasisTeleport.tpa` | Accept, deny or cancel a request |
| `/home` | `OasisTeleport.home` | Teleport to your bed |

## Config

`TpaCooldown`, `TpaCooldownSeconds`, `TpaDelay`, `TpaDelaySeconds`, `CancelOnBleeding`, `CancelOnHurt`, `HomeEnabled`, `HomeDelay`, `HomeDelaySeconds`, `HomeCooldown`, `HomeCooldownSeconds`

All player-facing text can be changed in the translation file.

## Tech

- C# / .NET Framework 4.8
- RocketMod
- `lib/` holds the reference assemblies needed to build

## Build

Open `OasisTeleport.sln` in Visual Studio and build it. Then copy the DLL into `Rocket/Plugins`.

## Part of the Oasis project

[OasisEssential](https://github.com/MikeyBot111/OasisEssential) · [OasisTeleport](https://github.com/MikeyBot111/OasisTeleport) · [OasisKits](https://github.com/MikeyBot111/OasisKits) · [OasisRanks](https://github.com/MikeyBot111/OasisRanks) · [OasisVault](https://github.com/MikeyBot111/OasisVault) · [OasisDiscord](https://github.com/MikeyBot111/OasisDiscord) · [OasisCustomCommands](https://github.com/MikeyBot111/OasisCustomCommands) · [WebsiteOasis](https://github.com/MikeyBot111/WebsiteOasis) · [oasis-server-discord](https://github.com/MikeyBot111/oasis-server-discord)
