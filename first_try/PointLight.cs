using OpenTK.Mathematics;

namespace zpg
{
    public class PointLight : Light
    {
        public Vector3 Position { get; set; }
        public float Constant { get; set; }
        public float Linear { get; set; }
        public float Quadratic { get; set; }

        protected override void SpecificUse(Shader shader, string lightName)
        {
            shader.SetVector3($"{lightName}.position", Position);
            shader.SetFloat($"{lightName}.constant", Constant);
            shader.SetFloat($"{lightName}.linear", Linear);
            shader.SetFloat($"{lightName}.quadratic", Quadratic);
        }
    }
}
