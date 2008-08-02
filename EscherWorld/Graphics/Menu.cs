using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace EscherWorld.Graphics
{
    /// <summary>
    /// Clase que contiene toda la información de la barra de menú.
    /// </summary>
    class Menu 
    {
        public enum Iconos { BLOCK, HOLE, JUMP, STAIR, ACTOR, DELETE, EXIT, NONE };
        public const int NUMEROICONOS = 7;
        private Texture2D[] texturasOff, texturasOn;
        private Rectangle[] rectangulos;
        private Vector2 position, posClicked;
        private Viewport viewport;
        private bool[] iluminados, clicked;
        private bool isSelected, vertical;

        /// <summary>
        /// Contructor de la clase menú, define las posiciones de los iconos.
        /// </summary>
        /// <param name="vieport">Area donde se dibuja le juego.</param>
        public Menu(Viewport viewport)
        {
            this.viewport = viewport;
            //Define la posicion y distribución del menú
            position = new Vector2(0, viewport.Height/(NUMEROICONOS+2));
            posClicked = new Vector2(0, 0);
            rectangulos = new Rectangle[NUMEROICONOS];
            setRectangles();

            texturasOff = new Texture2D[NUMEROICONOS];
            texturasOn = new Texture2D[NUMEROICONOS];

            //Cuales iconos comienzan iluminados.
            iluminados = new bool[NUMEROICONOS];
            clicked = new bool[NUMEROICONOS];
            clicked[0] = true;
            
            //Indica si el menu esta seleccionado y la forma de distribución.
            isSelected = false;
            vertical = true;
        }

        #region LoadTextures
        /// <summary>
        /// Carga las texturas usadas en el menu.
        /// </summary>
        /// <param name="blockOff">Textura del icono bloque sin seleccionar.</param>
        /// <param name="holeOff">Textura del icono hueco sin seleccionar.</param>
        /// <param name="jumpOff">Textura del icono salto sin seleccionar.</param>
        /// <param name="deleteOff">Textura del icono borrar sin seleccionar.</param>
        /// <param name="exitOff">Textura de salir sin seleccionar.</param>
        /// <param name="actorOff">Textura del icono actor sin seleccionar.</param>
        /// <param name="stairsOff">Textura de las escaleras sin seleccionar.</param>
        /// <param name="blockOn">Textura del icono bloque seleccionado.</param>
        /// <param name="holeOn">Textura del icono hueco seleccionado.</param>
        /// <param name="jumpOn">Textura del icono salto seleccionado.</param>
        /// <param name="deleteOn">Textura del icono borrar seleccionado.</param>
        /// <param name="exitOn">Textura de salir seleccionada.</param>
        /// <param name="actorOn">Textura del icono actor seleccionado.</param>
        /// <param name="stairsOn">Textura de las escaleras seleccionada.</param>
        public void loadTextures(Texture2D blockOff, Texture2D holeOff, Texture2D jumpOff, Texture2D deleteOff, Texture2D exitOff, Texture2D actorOff, Texture2D stairsOff,
                                 Texture2D blockOn, Texture2D holeOn, Texture2D jumpOn, Texture2D deleteOn, Texture2D exitOn, Texture2D actorOn, Texture2D stairsOn)
        {
            texturasOff[(int)Iconos.ACTOR] = actorOff;
            texturasOff[(int)Iconos.BLOCK] = blockOff;
            texturasOff[(int)Iconos.DELETE] = deleteOff;
            texturasOff[(int)Iconos.EXIT] = exitOff;
            texturasOff[(int)Iconos.HOLE] = holeOff;
            texturasOff[(int)Iconos.JUMP] = jumpOff;
            texturasOff[(int)Iconos.STAIR] = stairsOff;
            texturasOn[(int)Iconos.ACTOR] = actorOn;
            texturasOn[(int)Iconos.BLOCK] = blockOn;
            texturasOn[(int)Iconos.DELETE] = deleteOn;
            texturasOn[(int)Iconos.EXIT] = exitOn;
            texturasOn[(int)Iconos.HOLE] = holeOn;
            texturasOn[(int)Iconos.JUMP] = jumpOn;
            texturasOn[(int)Iconos.STAIR] = stairsOn;
        }
        #endregion

        /// <summary>
        /// Reotna el rectangulo del icono especificado.
        /// </summary>
        /// <param name="icono">Icono al cual se le desea hallar el rectángulo.</param>
        /// <returns>Rectánuglo del icono seleccionado.</returns>
        public Rectangle getRectangle(Iconos icono)
        {
            return rectangulos[(int)icono];
        }

        /// <summary>
        /// Define la posición y distribución del menú.
        /// </summary>
        /// <param name="viewport">Area donde se dibuja le juego.</param>
        private void setRectangles()
        {
            float spacingX, spacingY;
            spacingX = viewport.Width / (float)(NUMEROICONOS+5);
            spacingY = viewport.Height / (float)(NUMEROICONOS+2);

            for (int i = 0; i < rectangulos.Length; i++)
            {
                rectangulos[i] = new Rectangle((int)(position.X), (int)spacingY * i + (int)(position.Y), (int)spacingX, (int)spacingY);
            }
        }

        /// <summary>
        /// Retorna o pone la posición donde fue clickeado el menú relativo a este.
        /// </summary>
        public Vector2 PosClicked
        {
            get { return posClicked; }
            set 
            { 
                posClicked = value;
                posClicked.X -= position.X;
                posClicked.Y -= position.Y;
            }
        }

        /// <summary>
        /// Mueve el menu a la posición deseada .
        /// </summary>
        /// <param name="position">Posición a donde se desea mover el menú.</param>
        public void moveMenu(Vector2 pos)
        {
            position.X = pos.X - posClicked.X;
            position.Y = pos.Y - posClicked.Y;

            //Limita el menu a los bordes de la ventana y decide cual debe ser su forma de organización.
            if (position.X < 0)
                position.X = 0;
            if (position.X > viewport.Width - rectangulos[0].Width)
                position.X = viewport.Width - rectangulos[0].Width;
            if (position.Y < 0)
                position.Y = 0;
            if (position.Y > viewport.Height - rectangulos[0].Height * 7)
                position.Y = viewport.Height - rectangulos[0].Height * 7;

            setRectangles();
        }

        /// <summary>
        /// Indica si el menú esta seleccionado.
        /// </summary>
        public bool IsSelected
        {
            get { return isSelected; }
            set { isSelected = value; }
        }

        /// <summary>
        /// Retorna el icono actualmente seleccionado.
        /// </summary>
        /// <returns>Icono que esta seleccionado.</returns>
        public Iconos getIconClicked()
        {
            for (int i = 0; i < clicked.Length; i++)
            {
                if (clicked[i])
                    return (Iconos)i;
            }
            return Iconos.NONE;
        }

        /// <summary>
        /// Retorna el icono sobre el cual esta el mouse.
        /// </summary>
        /// <param name="x">Coordenada en x.</param>
        /// <param name="y">Coordenada en y.</param>
        /// <returns>el icono sobre el cual esta.</returns>
        private Iconos mouseOverIcon(int x, int y)
        {
            for (int i = 0; i < rectangulos.Length; i++)
            {
                if (rectangulos[i].Contains(x, y))
                    return (Iconos)i;
            }
            return Iconos.NONE;
        }

        /// <summary>
        /// Ilumina el icono en las coordenadas especificadas.
        /// </summary>
        /// <param name="x">Coordenada x.</param>
        /// <param name="y">Coordenada y.</param>
        public void iluminateIcon(int x, int y)
        {
            //Se mira sobre cual icono esta el mouse.
            Iconos icono = mouseOverIcon(x, y);
            if (icono != Iconos.NONE)
            {
                for (int i = 0; i < iluminados.Length; i++)
                {
                    if (iluminados[i])
                        iluminados[i] = false;
                }
                iluminados[(int)icono] = true;
            }
            else
            {
                for (int i = 0; i < iluminados.Length; i++)
                {
                    iluminados[i] = false;
                }
            }
        }

        /// <summary>
        /// Mira a cual icono le fue dado click.
        /// </summary>
        /// <param name="x">Coordenada x del mouse.</param>
        /// <param name="y">Coordenada y del mouse.</param>
        /// <returns>El icono al que le fue dado click.</returns>
        public Iconos iconClicked(int x, int y)
        {
            Iconos icono = mouseOverIcon(x, y);
            if (icono != Iconos.NONE)
            {
                for (int i = 0; i < clicked.Length; i++)
                {
                    if (clicked[i])
                        clicked[i] = false;
                }
                clicked[(int)icono] = true;
            }
            return icono;
        }

        /// <summary>
        /// Dibuja el menu.
        /// </summary>
        /// <param name="spriteBathc">SpriteBatch usado para dibujar sprites.</param>
        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Begin();
            for (int i = 0; i < texturasOff.Length; i++)
            {
                if (iluminados[i] || clicked[i])
                    spriteBatch.Draw(texturasOn[i], rectangulos[i], Color.White);
                else
                    spriteBatch.Draw(texturasOff[i], rectangulos[i], Color.White);    
            }
            spriteBatch.End();
        }
    }
}