using OpenTK.Mathematics;

namespace zpg
{
    /// <summary>
    /// Base Light class provides common properties and function for all specific lights.
    /// </summary>
    public class Light
    {
        public Vector3 Ambient { get; set; }
        public Vector3 Diffuse { get; set; }
        public Vector3 Specular { get; set; }

        /// <summary>
        /// In this function, set everything which isn't set in Use.
        /// Useful for child classes.
        /// </summary>
        protected virtual void SpecificUse(Shader shader, string lightName) { }

        /// <summary>
        /// Set lights parameters into shader - set uniforms.
        /// </summary>
        public void Use(Shader shader, string lightName)
        {
            shader.SetVector3($"{lightName}.ambient", Ambient);
            shader.SetVector3($"{lightName}.diffuse", Diffuse);
            shader.SetVector3($"{lightName}.specular", Specular);

            SpecificUse(shader, lightName);
        }
    }
}
