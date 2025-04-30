namespace zpg;

class TeleportPlatform : Platform
{
    public TeleportPlatform? LinkedTeleportPlatform { get; set; }

    public TeleportPlatform(Shader shader, float width, float height, float depth, Camera camera, string diffuseMap = "./Textures/scaryTeleport.png", string specularMap = "./Textures/scaryTeleport.png") : base(shader, width, height, depth, camera, diffuseMap, specularMap) { }
}
