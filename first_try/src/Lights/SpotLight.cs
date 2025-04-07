using OpenTK.Mathematics;

namespace zpg
{
    // player's flashlight
    public class SpotLight : Light
    {
        public Vector3 Position { get; set; }
        public Vector3 Direction { get; set; }

        public float CutOff { get; set; }
        public float OuterCutOff { get; set; }

        public float Constant { get; set; }
        public float Linear { get; set; }
        public float Quadratic { get; set; }

        protected override void SpecificUse(Shader shader, string lightName)
        {
            shader.SetVector3($"{lightName}.position", Position);
            shader.SetVector3($"{lightName}.direction", Direction);

            shader.SetFloat($"{lightName}.cutOff", CutOff);
            shader.SetFloat($"{lightName}.outerCutOff", OuterCutOff);

            shader.SetFloat($"{lightName}.constant", Constant);
            shader.SetFloat($"{lightName}.linear", Linear);
            shader.SetFloat($"{lightName}.quadratic", Quadratic);
        }
    }
}
