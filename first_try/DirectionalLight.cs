using OpenTK.Mathematics;

namespace zpg
{
    public class DirectionalLight : Light
    {
        public Vector3 Direction { get; set; }

        protected override void SpecificUse(Shader shader, string lightName)
        {
            shader.SetVector3($"{lightName}.direction", Direction);
        }
    }
}
