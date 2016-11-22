using System;
using Microsoft.DirectX.Direct3D;
using System.Drawing;
using Microsoft.DirectX;
using TGC.Core.Example;
using TGC.Core.SceneLoader;
using TGC.Core.Shaders;
using TGC.Core.Utils;
using TGC.Group.Model.Camera;
using TGC.Group.Model.Particles;

namespace TGC.Group.Model
{
    class Scenario : TgcExample
    {
        private TgcScene scene;
        private Car car;
        private TwistedCamera camera;
        private Velocimetro velocimetro;
        private SmokeParticle emitter;
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
            car = new Car(scene);
            camera = new TwistedCamera(Input, car, scene, 100f, 250f);
            Camara = camera;
            velocimetro = new Velocimetro();
            emitter = new SmokeParticle(car);
        }

        public override void Update()
        {
            PreUpdate();
            car.move(Input, ElapsedTime);
            if (emitter != null) {
                emitter.update();
            }
            
        }

        public override void Render()
        {
            PreRender();
            Effect effect = TgcShaders.Instance.TgcMeshShader;
            effect.SetValue("cameraPosition", TgcParserUtils.vector3ToFloat4Array(camera.getPosition()));
            effect.SetValue("CarLightPosition", TgcParserUtils.vector3ToFloat4Array(car.getLightPosition()));
            effect.SetValue("SpotLightDir", TgcParserUtils.vector3ToFloat4Array(car.getDirection()));
            effect.SetValue("SpotLightAngleCos", FastMath.ToRad((float)36));
            effect.SetValue("carDamaged", car.isDamaged());
            foreach (var mesh in scene.Meshes)
            {
                mesh.Effect = effect;
                mesh.Technique = "Light";
                mesh.render();
            }
            car.getMesh().Effect = effect;
            car.getMesh().Technique = "ColissionAndLight";
            car.render();
            velocimetro.render(DrawText, car.getVelocity());
            DrawText.drawText("Energy: " + car.getEnergy(), 800, 600, Color.Yellow);
            emitter.render(ElapsedTime);
            PostRender();
        }

        public override void Dispose()
        {
            scene.disposeAll();
            car.dispose();
        }
    }
}
