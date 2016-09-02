using Microsoft.DirectX;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TGC.Core.Example;
using TGC.Core.SceneLoader;

namespace TGC.Group.Model
{
    class Scenario : TgcExample
    {
        private TgcScene scene;
        private TgcMesh car;
        private TgcFpsCamera camera;

        public Scenario(string mediaDir, string shadersDir) : base(mediaDir, shadersDir)
        {
            Category = Game.Default.Category;
            Name = Game.Default.Name;
            Description = Game.Default.Description;
        }

        public override void Init()
        {
            var loader = new TgcSceneLoader();
            scene = loader.loadSceneFromFile(MediaDir + "adoquines-TgcScene.xml");
            car = loader.loadSceneFromFile(MediaDir + "Auto\\Auto-TgcScene.xml").Meshes[0];
            camera = new TgcFpsCamera(Input);
            Camara = camera;
        }

        public override void Update()
        {
            PreUpdate();
            var movement = new Vector3(0, 0, 0);

            if (Input.keyDown(Microsoft.DirectX.DirectInput.Key.Up))
            {
                movement.Z = -1;
            }
            if (Input.keyDown(Microsoft.DirectX.DirectInput.Key.Down))
            {
                movement.Z = 1;
            }
            if (Input.keyDown(Microsoft.DirectX.DirectInput.Key.Left))
            {
                movement.X = 1;
            }
            if (Input.keyDown(Microsoft.DirectX.DirectInput.Key.Right))
            {
                movement.X = -1;
            }

            movement *= 200f * ElapsedTime;
            car.move(movement);
        }

        public override void Render()
        {
            PreRender();
            car.render();
            scene.renderAll();
            PostRender();
        }

        public override void Dispose()
        {
            scene.disposeAll();
            car.dispose();
        }
    }
}
