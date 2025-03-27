using OpenTK.Mathematics;

namespace zpg
{
    public class Light
    {
        public Vector3 ambient { get; set; }
        public Vector3 diffuse { get; set; }
        public Vector3 specular { get; set; }

        protected virtual void SpecificUse(Shader shader, string lightName) { }

        public void Use(Shader shader, string lightName)
        {
            shader.SetVector3($"{lightName}.ambient", ambient);
            shader.SetVector3($"{lightName}.diffuse", diffuse);
            shader.SetVector3($"{lightName}.specular", specular);

            SpecificUse(shader, lightName);
        }
    }
}
