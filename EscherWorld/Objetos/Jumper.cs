using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace EscherWorld.Objetos
{
    /// <summary>
    /// Clase jumper.
    /// </summary>
    public class Jumper : GameObject
    {
        Model model;
        Matrix[] boneTransforms;
        /// <summary>
        /// Crea un jumper en la posición y del tamaño especificado.
        /// </summary>
        /// <param name="engine">Clase principal del juego.</param>
        /// <param name="position">Posición del jumper.</param>
        /// <param name="size">Tamaño del jumper.</param>
        public Jumper(Engine engine, Vector3 position, float size)
            : base(engine, position, size * 0.035f)
        {
            Visible = true;
        }

        /// <summary>
        /// Carga el modelo del jumper.
        /// </summary>
        protected override void LoadContent()
        {
            base.LoadContent();
            model = ((Engine)Game).getModel(2);
            boneTransforms = new Matrix[model.Bones.Count];

            //Crea la bounding box
            BoundingSphere bs = model.Meshes[0].BoundingSphere;
            for (int i = 1; i < model.Meshes.Count; i++)
            {
                bs = BoundingSphere.CreateMerged(bs, model.Meshes[i].BoundingSphere);
            }
            boundingBox = BoundingBox.CreateFromSphere(bs);
        }

        public override GameObject.RelativePosition positionRelativeToOBject(Vector3 v)
        {
            throw new System.Exception("The method or operation is not implemented.");
        }

        /// <summary>
        /// Dibuja el jumper.
        /// </summary>
        /// <param name="gameTime">Tiempo transcurrido del juego.</param>
        public override void Draw(GameTime gameTime)
        {
            model.CopyAbsoluteBoneTransformsTo(boneTransforms);
            foreach (ModelMesh mesh in model.Meshes)
            {
                foreach (BasicEffect effect in mesh.Effects)
                {
                    boneTransforms[mesh.ParentBone.Index] *= Matrix.CreateScale(size);
                    boneTransforms[mesh.ParentBone.Index] *= Matrix.CreateTranslation(position);
                    effect.World = boneTransforms[mesh.ParentBone.Index] * ((Engine)Game).WorldMatrix;
                    effect.View = ((Engine)Game).ViewMatrix;
                    effect.Projection = ((Engine)Game).ProjectionMatrix;
                    effect.LightingEnabled = false;
                    effect.CommitChanges();
                }

                //Dibuja la parte solida del mesh.
                GraphicsDevice.RenderState.DepthBias = -0.0000001f;
                mesh.Draw();
                GraphicsDevice.RenderState.DepthBias = 0;
            }
        }
    }
}