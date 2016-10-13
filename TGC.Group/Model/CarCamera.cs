using Microsoft.DirectX;
using Microsoft.DirectX.DirectInput;
using System.Drawing;
using System.Windows.Forms;
using TGC.Core.Camara;
using TGC.Core.Direct3D;
using TGC.Core.Input;
using TGC.Core.Utils;

namespace TGC.Group.Model
{
    public class CarCamera : TgcCamera
    {
        private Car car;

        public CarCamera(Car car)
        {
            this.car = car;
        }
        public void UpdateCamera(Car car) {
            this.car = car;
        }

        public override void UpdateCamera(float elapsedTime)
        {
            Vector3 carPosition = car.getPosition();
            Vector3 carDirection = car.getDirection();
            carDirection.Normalize();
            Vector3 cameraPosition = new Vector3(carPosition.X + 0 /*calcular distancia en X segun la direccion del auto*/, 100, carPosition.Z + /*calcular distancia en Z segun la direccion del auto*/);
            base.SetCamera(cameraPosition, new Vector3(carDirection.X, 100, carDirection.Z));
        }
    }
}
