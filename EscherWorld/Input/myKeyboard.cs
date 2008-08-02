using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace EscherWorld.Input
{
    /// <summary>
    /// Clase que controla el teclado.
    /// </summary>
    class myKeyboard
    {
        private KeyboardState currentState, previousState;

        /// <summary>
        /// Crea un teclado y lo inicializa.
        /// </summary>
        public myKeyboard()
        {
            previousState = Keyboard.GetState();
            currentState = Keyboard.GetState();
        }
        
        /// <summary>
        /// Retorna las teclas actualmente oprimidas en el teclado.
        /// </summary>
        /// <returns></returns>
        public Keys[] getCurrentKeys()
        {
            return currentState.GetPressedKeys();
        }

        /// <summary>
        /// Retorna las teclas previamente oprimidas en el teclado.
        /// </summary>
        /// <returns></returns>
        public Keys[] getPreviousKeys()
        {
            return previousState.GetPressedKeys();
        }

        /// <summary>
        /// Mira si una tecla esta presionada.
        /// </summary>
        /// <param name="k">Tecla que se desea mirar.</param>
        /// <returns>True si esta presionada, falso si no.</returns>
        public bool isKeyPressed(Keys k)
        {
            return currentState.IsKeyDown(k);
        }

        /// <summary>
        /// Mira si una tecla estaba presionada.
        /// </summary>
        /// <param name="k">La tecla que se desea mirar.</param>
        /// <returns>True si estaba presionada, falso si no.</returns>
        public bool wasKeyPressed(Keys k)
        {
            return previousState.IsKeyDown(k);
        }

        /// <summary>
        /// Actualiza el estado del teclado.
        /// </summary>
        public void processKeyboard()
        {
            previousState = currentState;
            currentState = Keyboard.GetState();
        }
    }
}