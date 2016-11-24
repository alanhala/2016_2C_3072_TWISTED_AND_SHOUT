using Microsoft.DirectX;
using TGC.Core.Geometry;

namespace TGC.Group.Model
{
    class CarLight
    {
        private Car car;
        private TgcBox mesh;
        private Vector3 position;

        public CarLight(Car car)
        {
            this.car = car;
            mesh = TgcBox.fromSize(new Vector3(0, 0, 0));
            mesh.AutoTransformEnable = false;
        }

        public void updateLightPosition()
        {
            var matrix = Matrix.Translation(new Vector3(0, 20, -50)) * Matrix.RotationY(car.getRotationAngle())
                         * Matrix.Translation(car.getPosition());
            position = new Vector3(matrix.M41, matrix.M42, matrix.M43);
            mesh.Transform = matrix;
        }

        public Vector3 getPosition()
        {
            return position;
        }

        public void render()
        {
            mesh.render();
        }
    }
}
