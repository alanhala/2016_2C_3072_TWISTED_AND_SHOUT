using System;
using Microsoft.DirectX.Direct3D;
using System.Drawing;
using Microsoft.DirectX;
using TGC.Core.Collision;
using TGC.Core.Example;
using TGC.Core.Geometry;
using TGC.Core.Input;
using TGC.Core.SceneLoader;
using TGC.Core.Shaders;
using TGC.Core.Utils;
using TGC.Group.Model.Camera;
using TGC.Group.Model.Particles;
using TGC.Group.Model.HUD;
using TGC.Core.Sound;

namespace TGC.Group.Model
{
    class Scenario : TgcExample
    {
        private TgcScene scene;
        private Car car;
        private TwistedCamera camera;
        private Velocimetro velocimetro;
        private SmokeParticle smokeParticles;
        private FireParticles fireParticles;
        private Energy energy;
        private TgcMp3Player mp3Player = new TgcMp3Player();
        private TgcPickingRay pickingRay;
        private Bullet bullet;
        private Car enemy;
        private FireParticles enemyFireParticles;

        public Scenario(string mediaDir, string shadersDir) : base(mediaDir, shadersDir)
        {
            Category = Game.Default.Category;
            Name = Game.Default.Name;
            Description = Game.Default.Description;
        }

        public override void Init()
        {
            var loader = new TgcSceneLoader();
            scene = loader.loadSceneFromFile(MediaDir + "city-TgcScene.xml");
            var carMovement = new CarMovement(new Vector3(0, 0, 1), 300, -70, -600);
            enemy = new Car(scene.Meshes, new EnemyMovement(carMovement));
            car = new Car(scene.Meshes, carMovement);
            enemy.position = new Vector3(0, 20, 50);
            camera = new TwistedCamera(Input, car, scene, 50f, 200f);
            Camara = camera;
            velocimetro = new Velocimetro();
            smokeParticles = new SmokeParticle(car);
            fireParticles = new FireParticles(car);
            enemyFireParticles = new FireParticles(enemy);
            energy = new Energy();
            mp3Player.FileName = MediaDir + "demo.mp3";
            mp3Player.play(true);
            pickingRay = new TgcPickingRay(Input);
            bullet = new Bullet();
        }

        public override void Update()
        {
            PreUpdate();
            car.move(Input, ElapsedTime);
            smokeParticles.update();
            fireParticles.update();
            if (Input.buttonPressed(TgcD3dInput.MouseButtons.BUTTON_LEFT))
            {
                pickingRay.updateRay();
                Vector3 newPosition;
                foreach (var mesh in scene.Meshes)
                {
                    if (TgcCollisionUtils.intersectRayAABB(pickingRay.Ray, mesh.BoundingBox, out newPosition))
                    {
                        bullet.enable();
                        bullet.init(car.getPosition(), newPosition, enemy);
                        break;
                    }
                }
            }
            bullet.update(ElapsedTime);
            enemy.move(Input, ElapsedTime);
            enemyFireParticles.update();
        }

        public override void Render()
        {
            PreRender();
            Effect effect = TgcShaders.Instance.TgcMeshShader;
            effect.SetValue("cameraPosition", TgcParserUtils.vector3ToFloat4Array(camera.getPosition()));
            effect.SetValue("CarLightPosition", TgcParserUtils.vector3ToFloat4Array(car.getLightPosition()));
            effect.SetValue("SpotLightDir", TgcParserUtils.vector3ToFloat4Array(car.getDirection()));
            effect.SetValue("SpotLightAngleCos", FastMath.ToRad((float)20));
            effect.SetValue("carDamaged", car.isDamaged());
            foreach (var mesh in scene.Meshes)
            {
                if (mesh.Enabled)
                {
                    var r = TgcCollisionUtils.classifyFrustumAABB(Frustum, mesh.BoundingBox);
                    if (r != TgcCollisionUtils.FrustumResult.OUTSIDE)
                    {
                        mesh.Effect = effect;
                        mesh.Technique = "Light";
                        mesh.render();
                    }
                }
            }
            enemy.getMesh().Effect = effect;
            enemy.getMesh().Technique = "ColissionAndLight";
            enemy.render();
            car.getMesh().Effect = effect;
            car.getMesh().Technique = "ColissionAndLight";
            car.render();
            velocimetro.render(DrawText, car.getVelocity());
            energy.render(car.getEnergy());
            smokeParticles.render(ElapsedTime);
            fireParticles.render(ElapsedTime);
            enemyFireParticles.render(ElapsedTime);
            bullet.render();
            DrawText.drawText("ENERGIA DEL ENEMIGO: " + (Math.Max(enemy.getEnergy(), 0)), 300, 200, Color.Yellow);
            PostRender();
        }

        public override void Dispose()
        {
            scene.disposeAll();
            car.dispose();
            smokeParticles.dispose();
            fireParticles.dispose();
        }
    }
}

