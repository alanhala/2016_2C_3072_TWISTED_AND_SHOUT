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
            var collisionFound = false;

            foreach (var mesh in scene.Meshes)
            {
                var mainMeshBoundingBox = car.getMesh().BoundingBox;
                var sceneMeshBoundingBox = mesh.BoundingBox;

                //TODO: No es el algoritmo definitivo
                var collisionResult = TgcCollisionUtils.classifyBoxBox(mainMeshBoundingBox, sceneMeshBoundingBox);

                if (collisionResult != TgcCollisionUtils.BoxBoxResult.Afuera)
                {
                    collisionFound = true;
                    break;
                }
            }

            car.handleColission(collisionFound, currentTransform, currentPosition);
        }

        public override void Render()
        {
            PreRender();
            car.render();
            scene.renderAll();

            //En este ejemplo a modo de debug vamos a dibujar los BoundingBox de todos los objetos.
            //Asi puede verse como se efectúa el testeo de colisiones.
            car.getMesh().BoundingBox.render();
            foreach (var mesh in scene.Meshes)
            {
                mesh.BoundingBox.render();
            }

            PostRender();
        }

        public override void Dispose()
        {
            scene.disposeAll();
            car.dispose();
        }
    }
}
