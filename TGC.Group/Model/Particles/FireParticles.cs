using Microsoft.DirectX;
using Microsoft.DirectX.Direct3D;
using TGC.Core.Direct3D;
using TGC.Core.Particle;

namespace TGC.Group.Model.Particles
{
    public class FireParticles
    {
        private ParticleEmitter emitter;
        private Car car;

        public FireParticles(Car car)
        {
            this.car = car;
            emitter = new ParticleEmitter(
                Game.Default.MediaDirectory + "Textures\\Particles\\fuego.png",
                100);
            emitter.MinSizeParticle = 2.5f;
            emitter.MaxSizeParticle = 3;
            emitter.ParticleTimeToLive = 0.4f;
            emitter.CreationFrecuency = 0.1f;
            emitter.Speed = new Vector3(1, 5, 1);
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
            if (car.getEnergy() < 20)
            {
                emitter.Enabled = true;
                var matrix = Matrix.Translation(new Vector3(0, 20, -40)) * Matrix.RotationY(car.getRotationAngle())
                         * Matrix.Translation(car.getPosition());
                emitter.Position = new Vector3(matrix.M41, matrix.M42, matrix.M43);
            }
            else
            {
                emitter.Enabled = false;
            }
        }
    }
}