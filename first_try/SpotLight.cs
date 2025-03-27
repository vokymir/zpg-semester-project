using OpenTK.Mathematics;

namespace zpg
{
    public class SpotLight : Light
    {
        public Vector3 position { get; set; }
        public Vector3 direction { get; set; }

        public float cutOff { get; set; }
        public float outerCutOff { get; set; }

        public float constant { get; set; }
        public float linear { get; set; }
        public float quadratic { get; set; }

        protected override void SpecificUse(Shader shader, string lightName)
        {
            shader.SetVector3($"{lightName}.position", position);
            shader.SetVector3($"{lightName}.direction", direction);

            shader.SetFloat($"{lightName}.cutOff", cutOff);
            shader.SetFloat($"{lightName}.outerCutOff", outerCutOff);

            shader.SetFloat($"{lightName}.constant", constant);
            shader.SetFloat($"{lightName}.linear", linear);
            shader.SetFloat($"{lightName}.quadratic", quadratic);
        }
    }
}
