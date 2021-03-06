﻿using Microsoft.DirectX;
using TGC.Core.Direct3D;
using TGC.Core.Particle;

namespace TGC.Group.Model.Particles
{
    class SmokeParticle
    {
        private ParticleEmitter emitter;
        private Vector3 position;
        private Car car;

        public SmokeParticle(Car car)
        {
            this.car = car;
            emitter = new ParticleEmitter(Game.Default.MediaDirectory + "Textures\\Particles\\humo.png", 1000);
            emitter.MinSizeParticle = 0.5f;
            emitter.MaxSizeParticle = 0.5f;
            emitter.ParticleTimeToLive = 1f;
            emitter.CreationFrecuency = 0.01f;
            emitter.Speed = new Vector3(1, 1, 100);
        }

        public void render(float elapsedTime)
        {
            D3DDevice.Instance.ParticlesEnabled = true;
            D3DDevice.Instance.EnableParticles();
            emitter.render(elapsedTime);
        }

        public void dispose()
        {
            emitter.dispose();
        }

        public void update()
        {
            if (car.getVelocity() >= 0)
            {
                emitter.Enabled = true;
            }
            else
            {
                emitter.Enabled = false;
            }
            var matrix = Matrix.Translation(new Vector3(10, 2, 32)) * Matrix.RotationY(car.getRotationAngle())
                         * Matrix.Translation(car.getPosition());
            emitter.Position = new Vector3(matrix.M41, matrix.M42, matrix.M43);
        }
    }
}
