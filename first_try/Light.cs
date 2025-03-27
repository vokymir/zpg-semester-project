using OpenTK.Mathematics;

namespace zpg
{
    public class Light
    {
        public Vector3 Ambient { get; set; }
        public Vector3 Diffuse { get; set; }
        public Vector3 Specular { get; set; }

        protected virtual void SpecificUse(Shader shader, string lightName) { }

        public void Use(Shader shader, string lightName)
        {
            shader.SetVector3($"{lightName}.ambient", Ambient);
            shader.SetVector3($"{lightName}.diffuse", Diffuse);
            shader.SetVector3($"{lightName}.specular", Specular);

            SpecificUse(shader, lightName);
        }
    }
}
