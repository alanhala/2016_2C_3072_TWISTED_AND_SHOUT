using Microsoft.DirectX;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TGC.Core.BoundingVolumes;
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
        private List<Wheel> backWheels;
        private List<Wheel> frontWheels;
        private Vector3 position = new Vector3(0, 20, 0);
        public TgcBoundingOrientedBox boundingBox;

        public Car()
        {
            var loader = new TgcSceneLoader();
            mesh = loader.loadSceneFromFile(Game.Default.MediaDirectory
                + "Auto\\Auto-TgcScene.xml").Meshes[0];
            mesh.AutoTransformEnable = false;
            carMovement = new CarMovement(new Vector3(0, 0, 1), 300, -70, -600);
            boundingBox = TgcBoundingOrientedBox.computeFromPoints(mesh.getVertexPositions());
            boundingBox.move(position);
            createWheels(loader);
        }

        private void createWheels(TgcSceneLoader loader)
        {

            backWheels = new List<Wheel>();
            frontWheels = new List<Wheel>();

            backWheels.Add(new Wheel(false, true));
            backWheels.Add(new Wheel(false, false));

            frontWheels.Add(new Wheel(true, true));
            frontWheels.Add(new Wheel(true, false));
        }

        internal Vector3 getDirection()
        {
            return carMovement.getDirection();
        }

        public void move(TgcD3dInput input, float elapsedTime)
        {
            var previousAngle = carMovement.getRotationAngle();
            carMovement.updateCarPosition(input, elapsedTime);
            position += carMovement.getRelativePosition();
            var carMatrix = Matrix.RotationY(carMovement.getRotationAngle()) * Matrix.Translation(position);
            mesh.Transform = carMatrix;
            foreach (Wheel wheel in backWheels)
            {
                wheel.move(input, carMatrix, carMovement.getVelocity(), elapsedTime, false);
            }

            foreach (Wheel wheel in frontWheels)
            {
                wheel.move(input, carMatrix, carMovement.getVelocity(), elapsedTime, true);
            }
            updateBoundingBox(previousAngle);
        }

        private void updateBoundingBox(float previousAngle)
        {
            boundingBox.move(carMovement.getRelativePosition());
            boundingBox.rotate(new Vector3(0, carMovement.getRotationAngle() - previousAngle, 0));
        }

        public void render()
        {
            mesh.render();
            foreach (var wheel in frontWheels)
            {
                wheel.mesh.render();
            }

            foreach (var wheel in backWheels)
            {
                wheel.mesh.render();
            }
            boundingBox.render();
        }

        public void dispose()
        {
            mesh.dispose();
        }

        public Vector3 getPosition()
        {
            return position;
        }

        public void setPosition(Vector3 newPosition)
        {
            position = newPosition;
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

                foreach (Wheel wheel in backWheels)
                    {
                    wheel.move(previousTransformMatrix, carMovement.getVelocity());
                    }

                foreach (Wheel wheel in frontWheels)
                    {
                    wheel.move(previousTransformMatrix, carMovement.getVelocity());
                    }

            }
        }

    }
}
