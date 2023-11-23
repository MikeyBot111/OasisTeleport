using Rocket.API;

namespace OasisTeleport
{
    public class OasisConfiguration : IRocketPluginConfiguration
    {
        public int TpaCooldownSeconds, TpaDelaySeconds, HomeCooldownSeconds, HomeDelaySeconds;
        public bool TpaCooldown, TpaDelay, CancelOnBleeding, CancelOnHurt, HomeEnabled, HomeDelay, HomeCooldown;

        public void LoadDefaults()
        {
            this.TpaCooldown = false;
            this.TpaCooldownSeconds = 10;
            this.TpaDelay = false;
            this.TpaDelaySeconds = 10;
            this.CancelOnBleeding = false;
            this.CancelOnHurt = false;
            this.HomeEnabled = false;
            this.HomeDelay = false;
            this.HomeCooldown = false;
            this.HomeCooldownSeconds = 10;
            this.HomeDelaySeconds = 10;
        }
    }
}
