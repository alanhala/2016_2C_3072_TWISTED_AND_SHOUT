using Microsoft.DirectX;
using System;
using System.Collections.Generic;
using TGC.Core.BoundingVolumes;
using TGC.Core.Input;
using TGC.Core.SceneLoader;

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
        private CarCollisionDetection carCollisionDetection;
        private double energy;
        private float elapsedTime;
        private CarLight light;

        public Car(TgcScene scene)
        {
            var loader = new TgcSceneLoader();
            mesh = loader.loadSceneFromFile(Game.Default.MediaDirectory
                + "Auto\\Auto-TgcScene.xml").Meshes[0];
            mesh.AutoTransformEnable = false;
            carMovement = new CarMovement(new Vector3(0, 0, 1), 300, -70, -600);
            boundingBox = TgcBoundingOrientedBox.computeFromPoints(mesh.getVertexPositions());
            boundingBox.move(position);
            createWheels(loader);
            carCollisionDetection = new CarCollisionDetection(this, scene);
            energy = 100;
            light = new CarLight(this);
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
            this.elapsedTime = elapsedTime;
            carMovement.updateCarPosition(input, elapsedTime);
            updateBoundingBox();
            handleColission(input);
        }

        private void updateBoundingBox()
        {
            boundingBox.move(carMovement.getPositionDiff());
            boundingBox.rotate(new Vector3(0, carMovement.getRotationAngleDiff(), 0));
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
            light.render();
        }

        public void dispose()
        {
            mesh.dispose();
        }

        public Vector3 getPosition()
        {
            return position + carMovement.getRelativePosition();
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

        public void handleColission(TgcD3dInput input)
        {
            if (carCollisionDetection.hasCollisioned())
            {
                boundingBox.move(-carMovement.getPositionDiff());
                boundingBox.rotate(new Vector3(0, -carMovement.getRotationAngleDiff(), 0));
                energy -= (Math.Abs(Math.Round(carMovement.getVelocity() / 40)));
                if (carMovement.getVelocity() > 0)
                    carMovement.setVelocity(-100);
                else
                    carMovement.setVelocity(100);
                carMovement.rollbackMovement();
            }
            else
            {
                var carMatrix = Matrix.RotationY(carMovement.getRotationAngle()) * Matrix.Translation(getPosition());
                mesh.Transform = carMatrix;
                light.updateLightPosition();
                foreach (Wheel wheel in backWheels)
                {
                    wheel.move(input, carMatrix, carMovement.getVelocity(), elapsedTime, false);
                }
                
                foreach (Wheel wheel in frontWheels)
                {
                    wheel.move(input, carMatrix, carMovement.getVelocity(), elapsedTime, true);
                }
            }
        }

        public float getVelocity()
        {
            return carMovement.getVelocity();
        }

        public double getEnergy()
        {
            return energy;
        }

        public Vector3 getLightPosition()
        {
            return light.getPosition();
        }

        public bool isDamaged()
        {
            if(energy < 70)
            {
                return true;
            }

            return false;
        }
    }
}
