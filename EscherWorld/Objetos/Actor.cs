using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace EscherWorld.Objetos
{
    /// <summary>
    /// Clase actor.
    /// </summary>
    class Actor : GameObject
    {
        /// <summary>
        /// Crea un jumper en la posici�n y del tama�o especificado.
        /// </summary>
        /// <param name="engine">Clase principal del juego.</param>
        /// <param name="position">Posici�n del actor.</param>
        /// <param name="size">Tama�o del actor.</param>
        public Actor(Engine engine, Vector3 position, float size)
            : base(engine, position, size)
        {
            Visible = true;
        }

        public override GameObject.RelativePosition positionRelativeToOBject(Vector3 v)
        {
            throw new System.Exception("The method or operation is not implemented.");
        }
    }
}