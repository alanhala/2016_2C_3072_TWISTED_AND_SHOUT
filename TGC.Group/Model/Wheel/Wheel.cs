using Microsoft.DirectX;
using Microsoft.DirectX.DirectInput;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TGC.Core.Input;
using TGC.Core.SceneLoader;
using TGC.Core.Utils;

namespace TGC.Group.Model
{
    class Wheel
    {
        private readonly float DELTA_ROTATION_VELOCITY = 3f;
        private readonly float MAX_DELTA_ROTATION = 0.5f;
        private readonly float MIN_DELTA_ROTATION = -0.5f;

        private Matrix initialPositionMatrix;
        public TgcMesh mesh { get; set; }

        private int xPosition;
        private int zPosition;
        private float angle = 0;

        private float deltaRotation = 0f;

        public Wheel(Boolean isFrontWheel, Boolean isRightWheel)
        {
            var loader = new TgcSceneLoader();
            xPosition = isRightWheel ? -24 : 22;
            zPosition = isFrontWheel ? -32 : 33;
            initialPositionMatrix = Matrix.Translation(new Vector3(xPosition, 7, zPosition));
            mesh = loader.loadSceneFromFile(Game.Default.MediaDirectory + "Rueda\\Rueda-TgcScene.xml").Meshes[0];
            mesh.AutoTransformEnable = false;
            mesh.Transform = initialPositionMatrix;
           
        }

        public void move(TgcD3dInput input, Matrix movementMatrix, float velocity, float elapsedTime, Boolean isFrontWheel)
        {
            angle -= FastMath.QUARTER_PI * 0.002f * velocity;
            var turningVelocity = Matrix.RotationX(angle);
            mesh.Transform = turningVelocity * Matrix.RotationY(isFrontWheel ? getWheelRotation(input, elapsedTime) : 0) *
                initialPositionMatrix * movementMatrix;
        }

        public void move(Matrix movementMatrix, float velocity)
        {
            move(null, movementMatrix, velocity, 0, false);
        }

        private float getWheelRotation(TgcD3dInput input, float elapsedTime)
        {
            if (input.keyDown(Key.Left))
            {
                deltaRotation -= DELTA_ROTATION_VELOCITY * elapsedTime;
            }
            else if (input.keyDown(Key.Right))
            {
                deltaRotation += DELTA_ROTATION_VELOCITY * elapsedTime;
            }
            else
            {
                updateNoInputForRotation(elapsedTime);
            }
            checkDeltaRotationLimits();
            return deltaRotation;
        }

        private void checkDeltaRotationLimits()
        {
            if (deltaRotation > MAX_DELTA_ROTATION)
            {
                deltaRotation = MAX_DELTA_ROTATION;
            }
            else if (deltaRotation < MIN_DELTA_ROTATION)
            {
                deltaRotation = MIN_DELTA_ROTATION;
            }
        }

        private void updateNoInputForRotation(float elapsedTime)
        {
            if (deltaRotation > -0.01f && deltaRotation < 0.01f)
            {
                deltaRotation = 0;
            }
            else if (deltaRotation > 0)
            {
                deltaRotation -= DELTA_ROTATION_VELOCITY * elapsedTime;
            }
            else
            {
                deltaRotation += DELTA_ROTATION_VELOCITY * elapsedTime;
            }
        }

    }
}
