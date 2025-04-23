namespace zpg;

class TeleportPlatform : Platform
{
    public TeleportPlatform? LinkedTeleportPlatform { get; set; }
    public bool IsActive { get; set; } = true;

    public TeleportPlatform(Shader shader, float width, float height, float depth, Camera camera, string diffuseMap = "./Textures/scaryTeleport.png", string specularMap = "./Textures/scaryTeleport.png") : base(shader, width, height, depth, camera, diffuseMap, specularMap) { }

    public void StartCooldown(int miliseconds = 1000)
    {
        IsActive = false;
        if (LinkedTeleportPlatform != null) LinkedTeleportPlatform.IsActive = false;
        System.Timers.Timer aTimer = new System.Timers.Timer();
        aTimer.Elapsed += (s, e) =>
        {
            IsActive = true;
            if (LinkedTeleportPlatform != null) LinkedTeleportPlatform.IsActive = true;
            aTimer.Dispose();
        };
        aTimer.Interval = miliseconds;
        aTimer.AutoReset = false;
        aTimer.Enabled = true;
    }
}
