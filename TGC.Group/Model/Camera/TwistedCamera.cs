using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using Microsoft.DirectX;
using Microsoft.DirectX.DirectInput;
using TGC.Core.Camara;
using TGC.Core.Collision;
using TGC.Core.Input;
using TGC.Core.SceneLoader;
using TGC.Core.Utils;

namespace TGC.Group.Model.Camera
{
    class TwistedCamera : TgcCamera
    {
        private readonly float DELTA_ROTATION_VELOCITY = 3f;
        private readonly float MAX_DELTA_ROTATION = 0.5f;
        private readonly float MIN_DELTA_ROTATION = -0.5f;

        private Car target;
        private TgcD3dInput input;
        private float deltaRotation;
        private float offsetHeight;
        private float offsetForward;
        private float currentOffsetHeight;
        private float currentOffsetForward;
        private TgcScene scene;

        public TwistedCamera(TgcD3dInput input, Car car, TgcScene scene, float offsetHeight, float offsetForward)
        {
            target = car;
            this.offsetHeight = offsetHeight;
            this.offsetForward = offsetForward;
            this.input = input;
            this.scene = scene;
            deltaRotation = 0f;
        }

        public override void UpdateCamera(float elapsedTime)
        {
            currentOffsetForward = offsetForward;
            var position = getCameraPosition(elapsedTime);
            //Detectar colisiones entre el segmento de recta camara-personaje y todos los objetos del escenario
            Vector3 intersectionPoint;
            var minDistSq = FastMath.Pow2(offsetForward);
            foreach (var obstaculo in scene.Meshes)
            {
                //Hay colision del segmento camara-personaje y el objeto
                if (TgcCollisionUtils.intersectSegmentAABB(target.getPosition(), position, 
                    obstaculo.BoundingBox, out intersectionPoint))
                {
                    //Si hay colision, guardar la que tenga menor distancia
                    var distSq = Vector3.Subtract(intersectionPoint, target.getPosition()).LengthSq();
                    //Hay dos casos singulares, puede que tengamos mas de una colision hay que quedarse con el menor offset.
                    //Si no dividimos la distancia por 2 se acerca mucho al target.
                    minDistSq = FastMath.Min(distSq / 2, minDistSq);
                }
            }

            var newOffsetForward = FastMath.Sqrt(minDistSq);
            currentOffsetForward = newOffsetForward;

            SetCamera(getCameraPosition(elapsedTime), target.getPosition());
        }

        private Vector3 getCameraPosition(float elapsedTime)
        {
            var offset = new Vector3(0, offsetHeight, currentOffsetForward);
            var matrix = Matrix.Translation(offset) * Matrix.RotationY(getCameraRotationAngle(elapsedTime))
                         * Matrix.Translation(target.getPosition());
            return new Vector3(matrix.M41, matrix.M42, matrix.M43);
        }

        private float getCameraRotationAngle(float elapsedTime)
        {
            if (input.keyDown(Key.D))
            {
                deltaRotation -= DELTA_ROTATION_VELOCITY*elapsedTime;
            } else if (input.keyDown(Key.A))
            {
                deltaRotation += DELTA_ROTATION_VELOCITY*elapsedTime;
            }
            else
            {
                updateNoInputForRotation(elapsedTime);
            }
            checkDeltaRotationLimits();
            return target.getRotationAngle() + deltaRotation;
        }

        private void checkDeltaRotationLimits()
        {
            if (deltaRotation > MAX_DELTA_ROTATION)
            {
                deltaRotation =  MAX_DELTA_ROTATION;
            } else if (deltaRotation < MIN_DELTA_ROTATION)
            {
                deltaRotation = MIN_DELTA_ROTATION;
            }
        }

        private void updateNoInputForRotation(float elapsedTime)
        {
            if (deltaRotation > -0.01f && deltaRotation < 0.01f)
            {
                deltaRotation = 0;
            } else if (deltaRotation > 0)
            {
                deltaRotation -= DELTA_ROTATION_VELOCITY*elapsedTime;
            }
            else
            {
                deltaRotation += DELTA_ROTATION_VELOCITY*elapsedTime;
            }
        }
    }
}
