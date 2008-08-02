using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Graphics;
using EscherWorld.Objetos;
using EscherWorld.Utility;

namespace EscherWorld.Input
{
    /// <summary>
    /// Clase que controla el input del mouse.
    /// </summary>
    class myMouse
    {
        /// <summary>
        /// Botones del mouse.
        /// </summary>
        public enum MouseButtons { LeftClick, RightClick, MiddleButton };
        
        private MouseState currentState, previousState;
        private Texture2D cursorTexture, cursorRotateTexture, currentTexture;
        private int staticX, staticY;
        private bool isVisible;

        /// <summary>
        /// Crea un mouse y lo inicializa.
        /// </summary>
        public myMouse()
        {
            previousState = Mouse.GetState();
            currentState = Mouse.GetState();
            isVisible = true;
        }

        #region Properties (Gets y sets)
        /// <summary>
        /// Estado actual del mouse.
        /// </summary>
        public MouseState CurrentState
        {
            get
            {
                currentState = Mouse.GetState();
                return currentState;
            }
        }

        /// <summary>
        /// Estado pasado del mouse.
        /// </summary>
        public MouseState PreviousState
        {
            get { return previousState; }
        }

        /// <summary>
        /// Retorna la posición actual de x
        /// </summary>
        public int X
        {
            get
            { return currentState.X; }
            set
            {
                Mouse.SetPosition(value, Y);
            }
        }
        /// <summary>
        /// Retorna la posición actual de y
        /// </summary>
        /// 
        public int Y
        {
            get
            { return currentState.Y; }
            set
            {
                Mouse.SetPosition(X, value);
            }
        }
        /// <summary>
        /// Retorna la coordenad pasada del mouse en X
        /// </summary>
        public int LastX
        {
            get
            { return previousState.X; }
        }
        /// <summary>
        /// Rotorna la coordenada pasada del mouse en Y
        /// </summary>
        public int LastY
        {
            get
            { return previousState.Y; }
        }

        /// <summary>
        /// Coordenada X usada cuando el cursor cmabia a la forma de mover mundo.
        /// </summary>
        public int StaticX
        {
            get { return staticX; }
            set { staticX = value; }
        }


        /// <summary>
        /// Coordenada Y usada cuando el cursor cmabia a la forma de mover mundo.
        /// </summary>
        public int StaticY
        {
            get { return staticY; }
            set { staticY = value; }
        }
        #endregion

        /// <summary>
        /// Asigna las texturas del mouse.
        /// </summary>
        /// <param name="cursor">Textura del cursor.</param>
        /// <param name="cursorRotate">Textura del cursor cuando la camara rota.</param>
        public void loadTextures(Texture2D cursor, Texture2D cursorRotate)
        {
            cursorTexture = cursor;
            cursorRotateTexture = cursorRotate;
            currentTexture = cursorTexture;
        }

        /// <summary>
        /// Mira si el boton deseado esta presionado o no.
        /// </summary>
        /// <param name="button">Boton que se desea chequear</param>
        /// <returns>True si esta presionado, false si no</returns>
        public bool isPressed(MouseButtons button)
        {
            return (buttonState(button) == ButtonState.Pressed);
        }

        /// <summary>
        /// Mira si el boton deseado estaba presionado o no.
        /// </summary>
        /// <param name="button">Boton que se desea chequear</param>
        /// <returns>True si estaba presionado, false si no</returns>
        public bool wasPressed(MouseButtons button)
        {
            return (previousButtonState(button) == ButtonState.Pressed);
        }

        /// <summary>
        /// Mira el estado del boton deseado.
        /// </summary>
        /// <param name="button">Boton que se desea chequear.</param>
        /// <returns>El estado del boton.</returns>
        public ButtonState buttonState(MouseButtons button)
        {
            if (button == MouseButtons.LeftClick)
                return currentState.LeftButton;
            if (button == MouseButtons.RightClick)
                return currentState.RightButton;
            if (button == MouseButtons.MiddleButton)
                return currentState.MiddleButton;
            return ButtonState.Released;
        }

        /// <summary>
        /// Mira el estado pasado del boton deseado.
        /// </summary>
        /// <param name="button">Boton que se desea chequear.</param>
        /// <returns>El estado pasado del boton.</returns>
        public ButtonState previousButtonState(MouseButtons button)
        {
            if (button == MouseButtons.LeftClick)
                return previousState.LeftButton;
            if (button == MouseButtons.RightClick)
                return previousState.RightButton;
            if (button == MouseButtons.MiddleButton)
                return previousState.MiddleButton;
            return ButtonState.Released;
        }

        /// <summary>
        /// Obtiene el rayo de la posición del mouse.
        /// </summary>
        /// <param name="projection">Matriz de projeccion.</param>
        /// <param name="view">Matriz de posición de la camar.</param>
        /// <param name="viewPort">Cuadro de dimensiones donde se renderiza.</param>
        /// <returns>Rayo con la posición del mouse.</returns>
        public Ray getRay(Matrix projection, Matrix view,  Matrix world, Viewport viewPort)
        {
            Vector3 farPoint = viewPort.Unproject(new Vector3(X, Y, 1), projection, view, world);
            Vector3 nearPoint = viewPort.Unproject(new Vector3(X, Y, 0), projection, view, world);

            Ray r = new Ray(nearPoint, Vector3.Normalize(farPoint - nearPoint));
            return r;
        }

        /// <summary>
        /// Obtiene las coordenadas 3D del mouse con un z arbitrario(basado en las coordenadas X y Y).
        /// </summary>
        /// <param name="projection">Matriz de projeccion.</param>
        /// <param name="view">Matriz de posición de la camar.</param>
        /// <param name="viewPort">Cuadro de dimensiones donde se renderiza.</param>
        /// <returns>Vector de posición 3D del mouse.</returns>
        public Vector3 get3DPosition(Matrix projection, Matrix view, Matrix world, Viewport viewPort)
        {
            Ray r = getRay(projection, view, world, viewPort);
            Plane p = new Plane(new Vector4(r.Direction, -((X * 60 / viewPort.Width) - (Y * 60 / viewPort.Height))));
            float? distance = r.Intersects(p);
            if (distance != null)
            {
                Vector3 position = r.Position + distance.Value * r.Direction;
                return position;
            }
            return Vector3.Zero;
        }

        /// <summary>
        /// Resetea las coordenadas del mouse a las de los x y y estáticos.
        /// </summary>
        public void resetCoordinates()
        {
            Mouse.SetPosition(StaticX, StaticY);
        }

        /// <summary>
        /// Cambia la textura del mouse.
        /// </summary>
        public void switchTextures()
        {
            if (currentTexture == cursorTexture)
                currentTexture = cursorRotateTexture;
            else
                currentTexture = cursorTexture;
        }

        /// <summary>
        /// Indica si el mouse debe dibujarse o no.
        /// </summary>
        public bool IsVisible
        {
            get { return isVisible; }
            set { isVisible = value; }
        }

        /// <summary>
        /// Procesa los datos del mouse.
        /// </summary>
        public void processMouse()
        {
            previousState = currentState;
            currentState = Mouse.GetState();
        }

        /// <summary>
        /// Dibuja el cursor del mouse, dependiendo de que textura sea la actual.
        /// </summary>
        public void Draw(SpriteBatch spriteBatch)
        {
            //Coordenadas donde se dibujara el cursor.
            int x, y;
            if (currentTexture == cursorTexture)
            {
                x = X - 4;
                y = Y - 3;
            }
            else
            {
                x = staticX;
                y = staticY;
            }

            if (isVisible)
            {
                spriteBatch.Begin();
                spriteBatch.Draw(currentTexture, new Rectangle(x, y, cursorTexture.Width, cursorTexture.Height), Color.White);
                spriteBatch.End();
            }
        }
    }
}