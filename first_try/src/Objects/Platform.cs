namespace zpg;

class Platform : Cube
{
    public Platform(Shader shader, float width, float height, float depth, Camera camera, string diffuseMap = "./Textures/Floor.png", string specularMap = "./Textures/FloorSpecular.png") : base(shader, width, height, depth, camera, diffuseMap, specularMap) { }
    ///
    /// <summary>
    /// Overload for existing texutures.
    /// </summary>
    public Platform(Shader shader, float width, float height, float depth, Camera camera, Texture diffuseTex, Texture specularTex) : base(shader, width, height, depth, camera, diffuseTex, specularTex) { }
}
