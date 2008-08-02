using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace EscherWorld.Objetos
{
    /// <summary>
    /// Clase escalera.
    /// </summary>
    class Stair : GameObject
    {
        private Model model;
        Matrix[] boneTransforms;
        private float rotation;

        /// <summary>
        /// Crea una escalera en la posición y del tamaño deseado.
        /// </summary>
        /// <param name="engine">Clase principal del juego.</param>
        /// <param name="position">Psotición de la escalera.</param>
        /// <param name="size">Tamaño de la escalera.</param>
        /// <param name="rotation">Rotación inicial de la escalera.</param>
        public Stair(Engine engine, Vector3 position, float size, float rotation)
            : base(engine, position, size*0.02f)
        {
            Visible = true;
            this.rotation = rotation;
        }
        
        /// <summary>
        /// Carga el modelo de la escalera.
        /// </summary>
        protected override void LoadContent()
        {
            base.LoadContent();
            model = ((Engine)Game).getModel(0);
            boneTransforms = new Matrix[model.Bones.Count];
            //Crea la bounding box
            BoundingSphere bs = model.Meshes[0].BoundingSphere;
            for (int i = 1 ; i < model.Meshes.Count; i++)
            {
                bs = BoundingSphere.CreateMerged(bs, model.Meshes[i].BoundingSphere);    
            }
            boundingBox = BoundingBox.CreateFromSphere(bs);
        }

        /// <summary>
        /// Rota la escalera 90 grados a la derecha.
        /// </summary>
        public void rotate()
        {
            rotation += MathHelper.PiOver2;
        }

        public override GameObject.RelativePosition positionRelativeToOBject(Vector3 v)
        {
            throw new System.Exception("The method or operation is not implemented.");
        }

        /// <summary>
        /// Dibuja la escalera.
        /// </summary>
        /// <param name="gameTime">Tiempo transucrrido en el juego.</param>
        public override void Draw(GameTime gameTime)
        {
            model.CopyAbsoluteBoneTransformsTo(boneTransforms);

            foreach (ModelMesh mesh in model.Meshes)
            {
                foreach (BasicEffect effect in mesh.Effects)
                {
                    boneTransforms[mesh.ParentBone.Index] *= Matrix.CreateScale(size);
                    boneTransforms[mesh.ParentBone.Index] *= Matrix.CreateRotationY(rotation);
                    boneTransforms[mesh.ParentBone.Index] *= Matrix.CreateTranslation(position);
                    effect.World = boneTransforms[mesh.ParentBone.Index] * ((Engine)Game).WorldMatrix;
                    effect.View = ((Engine)Game).ViewMatrix;
                    effect.Projection = ((Engine)Game).ProjectionMatrix;
                    effect.LightingEnabled = false;
                    effect.CommitChanges();
                }

                
                //Dibuja la parte solida del mesh.
                GraphicsDevice.RenderState.DepthBias = -0.001f;
                mesh.Draw();
                GraphicsDevice.RenderState.DepthBias = 0;
            }
        }
    }
}