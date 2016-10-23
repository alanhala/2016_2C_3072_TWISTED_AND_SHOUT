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
        private TgcMesh mesh;
        private CarMovement carMovement;

        public Car()
        {
            var loader = new TgcSceneLoader();
            mesh = loader.loadSceneFromFile(Game.Default.MediaDirectory
                + "Auto\\Auto-TgcScene.xml").Meshes[0];
            mesh.AutoTransformEnable = false;
            carMovement = new CarMovement(new Vector3(0, 0, 1), 300, -70, -600);
            mesh.move(0, 20, 0);
            
        }

        internal Vector3 getDirection()
        {
            return carMovement.getDirection();
        }

        public void move(TgcD3dInput input, float elapsedTime)
        {
            Matrix matrix = carMovement.move(input, elapsedTime);
            mesh.Transform = matrix;
            mesh.BoundingBox.transform(matrix);
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
            return carMovement.getPosition();
        }

        public void setPosition(Vector3 newPosition)
        {
            carMovement.setPosition(newPosition);
            mesh.Position = newPosition;
        }
        internal float getRotationAngle()
        {
            return carMovement.getRotationAngle();
        }

        public TgcMesh getMesh()
        {
            return mesh;
        }

        public void resetVelocity()
        {
            carMovement.setVelocity(0f);
        }

        public void handleColission(bool hasColissioned, Matrix previousTransformMatrix, Vector3 previousPosition)
        {
            //TODO: la colisión debe tener una reacción
            if (hasColissioned)
            {
                mesh.Transform = previousTransformMatrix;
                mesh.BoundingBox.transform(previousTransformMatrix);
                setPosition(previousPosition);
                resetVelocity();
            }
        }

    }
}
