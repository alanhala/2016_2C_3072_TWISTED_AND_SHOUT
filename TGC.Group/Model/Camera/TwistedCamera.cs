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
using TGC.Core.Input;

namespace TGC.Group.Model.Camera
{
    class TwistedCamera : TgcCamera
    {
        private readonly float DELTA_ROTATION_VELOCITY = 3f;
        private readonly float MAX_DELTA_ROTATION = 0.5f;
        private readonly float MIN_DELTA_ROTATION = -0.5f;

        private Car target;
        private Vector3 offset;
        private TgcD3dInput input;
        private float deltaRotation;

        public TwistedCamera(TgcD3dInput input, Car car, float offsetHeight, float offsetForward)
        {
            target = car;
            offset = new Vector3(0, offsetHeight, offsetForward);
            this.input = input;
            deltaRotation = 0f;
        }

        public override void UpdateCamera(float elapsedTime)
        {
            SetCamera(getCameraPosition(elapsedTime), target.getPosition());
        }

        private Vector3 getCameraPosition(float elapsedTime)
        {
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
