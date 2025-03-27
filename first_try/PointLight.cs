using OpenTK.Mathematics;

namespace zpg
{
    public class PointLight : Light
    {
        public Vector3 position { get; set; }
        public float constant { get; set; }
        public float linear { get; set; }
        public float quadratic { get; set; }

        protected override void SpecificUse(Shader shader, string lightName)
        {
            shader.SetVector3($"{lightName}.position", position);
            shader.SetFloat($"{lightName}.constant", constant);
            shader.SetFloat($"{lightName}.linear", linear);
            shader.SetFloat($"{lightName}.quadratic", quadratic);
        }
    }
}
