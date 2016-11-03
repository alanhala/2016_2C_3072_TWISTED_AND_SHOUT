using Microsoft.DirectX;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TGC.Core.Collision;
using TGC.Core.Example;
using TGC.Core.SceneLoader;
using TGC.Group.Model.Camera;

namespace TGC.Group.Model
{
    class Scenario : TgcExample
    {
        private TgcScene scene;
        private Car car;
        private TwistedCamera camera;
        private Velocimetro velocimetro;

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
            camera = new TwistedCamera(Input, car, 200f, 300f);
            Camara = camera;
            velocimetro = new Velocimetro();
        }

        public override void Update()
        {
            PreUpdate();
            car.move(Input, ElapsedTime);
        }

        public override void Render()
        {
            PreRender();
            car.render();
            scene.renderAll(true);
            velocimetro.render(DrawText, car.getVelocity());
            PostRender();
        }

        public override void Dispose()
        {
            scene.disposeAll();
            car.dispose();
        }
    }
}
