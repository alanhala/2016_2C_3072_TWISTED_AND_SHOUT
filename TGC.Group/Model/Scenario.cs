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
            car = new Car();
            camera = new TwistedCamera(Input, car, 200f, 300f);
            Camara = camera;

        }

        public override void Update()
        {
            PreUpdate();
            var currentTransform = car.getMesh().Transform;
            var currentPosition = car.getPosition();
            car.move(Input, ElapsedTime);

            var collisionResult = false;

            foreach (var mesh in scene.Meshes)
            {
                var sceneMeshBoundingBox = mesh.BoundingBox;

                collisionResult = TgcCollisionUtils.testObbAABB(car.boundingBox, sceneMeshBoundingBox);

                if (collisionResult) break;
            }
            car.handleColission(collisionResult, currentTransform, currentPosition);
        }

        public override void Render()
        {
            PreRender();
            car.render();
            scene.renderAll(true);

            PostRender();
        }

        public override void Dispose()
        {
            scene.disposeAll();
            car.dispose();
        }
    }
}
