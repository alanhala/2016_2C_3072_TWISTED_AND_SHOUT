using Microsoft.DirectX;
using Microsoft.DirectX.Direct3D;
using System.Drawing;
using TGC.Core.Example;
using TGC.Core.Geometry;
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
            camera = new TwistedCamera(Input, car, 100f, 250f);
            Camara = camera;
            velocimetro = new Velocimetro();
            emitter = new SmokeParticle(car);
        }

        public override void Update()
        {
            PreUpdate();
            car.move(Input, ElapsedTime);
            emitter.update();
        }

        public override void Render()
        {
            Effect currentShader = TgcShaders.Instance.TgcMeshPointLightShader;
            foreach (var mesh in scene.Meshes)
            {
                mesh.Effect = currentShader;
                mesh.Technique = TgcShaders.Instance.getTgcMeshTechnique(mesh.RenderType);

                mesh.Effect.SetValue("lightColor", ColorValue.FromColor(car.getLight().Color));
                mesh.Effect.SetValue("lightPosition", TgcParserUtils.vector3ToFloat4Array(car.getPosition()));
                mesh.Effect.SetValue("eyePosition", TgcParserUtils.vector3ToFloat4Array(Camara.Position));
                mesh.Effect.SetValue("lightIntensity", 100);
                mesh.Effect.SetValue("lightAttenuation", 1f);

                //Cargar variables de shader de Material. El Material en realidad deberia ser propio de cada mesh. Pero en este ejemplo se simplifica con uno comun para todos
                mesh.Effect.SetValue("materialEmissiveColor", ColorValue.FromColor(Color.Black));
                mesh.Effect.SetValue("materialAmbientColor", ColorValue.FromColor(Color.White));
                mesh.Effect.SetValue("materialDiffuseColor", ColorValue.FromColor(Color.White));
                mesh.Effect.SetValue("materialSpecularColor", ColorValue.FromColor(Color.White));
                mesh.Effect.SetValue("materialSpecularExp", 19f);
            }

            PreRender();
            car.render();
            scene.renderAll();
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
