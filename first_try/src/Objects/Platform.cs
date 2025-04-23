namespace zpg;

class Platform : Cube
{
    public Platform(Shader shader, float width, float height, float depth, Camera camera, string diffuseMap = "./Textures/container2.png", string specularMap = "./Textures/container2_specular.png") : base(shader, width, height, depth, camera, diffuseMap, specularMap) { }
}
