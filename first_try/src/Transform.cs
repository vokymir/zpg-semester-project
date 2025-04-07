using OpenTK.Mathematics;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace zpg
{

    /// <summary>
    /// Encapsulates pos, rot, scale of anything and generates matrix for it.
    /// </summary>
    class Transform : INotifyPropertyChanged
    {
        // this can be further shortened when using .NET 9.0 - in 8.0 it is what it is
        protected Vector3 _position = Vector3.Zero;
        public Vector3 Position { get => _position; set => SetField(ref _position, value); }
        protected Vector3 _rotation = Vector3.Zero;
        public Vector3 Rotation { get => _rotation; set => SetField(ref _position, value); }
        protected Vector3 _scale = Vector3.One;
        public Vector3 Scale { get => _scale; set => SetField(ref _scale, value); }

        public Matrix4 GetMatrix()
        {
            return Matrix4.CreateScale(_scale) *
                   Matrix4.CreateRotationX(_rotation.X) *
                   Matrix4.CreateRotationY(_rotation.Y) *
                   Matrix4.CreateRotationZ(_rotation.Z) *
                   Matrix4.CreateTranslation(_position);
        }

        // Property changed is used on Camera - it's collision cube changes position based on the Camera transform
        public event PropertyChangedEventHandler? PropertyChanged;

        protected void OnPropertyChanged(string propertyName) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

        protected bool SetField<T>(ref T field, T value, [CallerMemberName] string propertyName = "")
        {
            if (EqualityComparer<T>.Default.Equals(field, value)) return false;
            field = value;
            OnPropertyChanged(propertyName);
            return true;
        }
    }
}
