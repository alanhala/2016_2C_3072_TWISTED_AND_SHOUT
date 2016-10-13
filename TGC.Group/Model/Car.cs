using Microsoft.DirectX;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TGC.Core.Example;
using TGC.Core.Input;
using TGC.Core.SceneLoader;
using TGC.Core.Utils;

namespace TGC.Group.Model
{
    public class Car
    {
        private float velocity;
        private float acceleration;
        private float deceleration;
        private float brakeAcceleration;
        private Vector3 position;
        private Matrix translation;
        private TgcMesh mesh;
        private float angle;
        private Vector3 direction;

        public Car()
        {
            var loader = new TgcSceneLoader();
            mesh = loader.loadSceneFromFile(Game.Default.MediaDirectory
                + "Auto\\Auto-TgcScene.xml").Meshes[0];
            mesh.AutoTransformEnable = false;
            initializeMovement();
        }

        private void initializeMovement()
        {
            acceleration = 300;
            deceleration = -70;
            brakeAcceleration = -600;
            translation = Matrix.Identity;
            position = new Vector3(0, 0, 0);
            direction = new Vector3(0, 0, -1);
            angle = 0f;
        }

        public void move(TgcD3dInput input, float elapsedTime)
        {
            if (input.keyDown(Microsoft.DirectX.DirectInput.Key.Up))
            {
                velocity = velocity + acceleration * elapsedTime;
                System.Diagnostics.Debug.WriteLine("up");
            }
            if (input.keyDown(Microsoft.DirectX.DirectInput.Key.Down))
            {
                velocity = velocity + brakeAcceleration * elapsedTime;
                System.Diagnostics.Debug.WriteLine("down");
            }
            if (input.keyDown(Microsoft.DirectX.DirectInput.Key.Left))
            {
                angle -= 2f * elapsedTime;
                System.Diagnostics.Debug.WriteLine("left");
            }
            if (input.keyDown(Microsoft.DirectX.DirectInput.Key.Right))
            {
                angle += 2f * elapsedTime;
                System.Diagnostics.Debug.WriteLine("right");
            }
            //else
           // {
             //   decelerate(elapsedTime);
            //}
            Vector4 asd = Vector3.Transform(direction, Matrix.RotationY(angle));
            position.Z += asd.Z * velocity * elapsedTime;
            position.X += asd.X * velocity * elapsedTime;
            mesh.Transform = Matrix.RotationY(angle) * Matrix.Translation(position);
        }

        private void decelerate(float elapsedTime)
        {
            if (velocity <= 0)
            {
                velocity = 0;
            } else
            {
                velocity = velocity + deceleration * elapsedTime;
            }
        }

        public void render()
        {
            mesh.render();
        }

        public void dispose()
        {
            mesh.dispose();
        }

        public Vector3 getPosition()
        {
            return position;
        }

        public Vector3 getDirection()
        {
            return direction;
        }
    }
}
