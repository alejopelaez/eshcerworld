using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace EscherWorld.Objetos
{
    /// <summary>
    /// Clase Base para todos los objetos del juego.
    /// </summary>
    public abstract class GameObject : DrawableGameComponent
    {
        /// <summary>
        /// Posición realitva de un punto con este objeto.
        /// </summary>
        public enum RelativePosition{UP, DOWN, LEFT, RIGHT, FRONT, BACK, NONE};
        protected Vector3 position;
        protected BoundingBox boundingBox;
        protected Color color;
        protected float size;

        #region Constructores
        /// <summary>
        /// Crea un objeto tipo GameObject.
        /// </summary>
        /// <param name="effect">Effecto usado para dibujar.</param>
        /// <param name="engine">Clase principal del juego.</param>
        /// <param name="position">Posición del objeto.</param>
        /// <param name="size">Tamaño del objeto.</param>
        public GameObject(Engine engine, Vector3 position, float size)
            :base(engine)
        {
            this.size = size;
            color = Color.LightGray;
            this.position = position;
        }

        /// <summary>
        /// Constructor de la clase GameObject.
        /// </summary>
        /// <param name="effect">Effecto usado para dibujar.</param>
        /// <param name="engine">Clase principal del juego.</param>
        /// <param name="size">Tamaño del objeto.</param>
        /// <param name="position">Posicion x, y, z dada como Vector3.</param>
        /// <param name="color">Color del objeto.</param>
        public GameObject(Engine engine, Vector3 position, Color color, float size)
            : base(engine)
        {
            this.size = size;
            this.color = color;
            this.position = position;
        }
        #endregion

        /// <summary>
        /// Carga el efecto
        /// </summary>
        protected override void LoadContent()
        {
            base.LoadContent();
        }

        /// <summary>
        /// Obtiene el tamaño del objeto.
        /// </summary>
        public float Size
        {
            get { return size; }
        }

        /// <summary>
        /// Obtiene la posición del Objeto.
        /// </summary>
        public Vector3 Position
        {
            get { return position; }
        }

        /// <summary>
        /// Retorna el bounding box de el objeto.
        /// </summary>
        public BoundingBox BoundingBox
        {
            get
            { return boundingBox; }
        }

        public abstract RelativePosition positionRelativeToOBject(Vector3 v);
    }
}