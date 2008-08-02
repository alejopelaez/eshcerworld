#region Usings
using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Net;
using Microsoft.Xna.Framework.Storage;
using EscherWorld.Graphics;
using EscherWorld.Input;
using EscherWorld.Objetos;
using EscherWorld.Utility;
#endregion

namespace EscherWorld
{
    /// <summary>
    /// Clase principal del juego.
    /// </summary>
    public class Engine : Microsoft.Xna.Framework.Game
    {
        #region Atributos
        //Constantes que definen el tamaño de los objetos.
        private const int SIZE_ESCALERA = 1;
        private const int SIZE_CUBO = 2;
        //Controlo si el juego esta pausado o no.
        private bool paused;

        List<Cubo> cubos;
        BasicEffect effect;
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        Model[] models;
        string[] modelNames;
        Menu menu;
        Camera camera;
        
        //Inputs
        myMouse mouse;
        myKeyboard keyboard;

        //Mide el tiempo para saber si el menú puede ser movido o no.
        double tiempo;
        #endregion

        public Engine()
        {
            Window.Title = "Escher's World";
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            string a = Content.RootDirectory;
            a += "d";
        }

        #region Carga e incialización (Initialize() carga el contenido no gráfico y LoadContent() carga el contenido gráfico)
        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            //Crea la lista de cubos.
            cubos = new List<Cubo>();

            //El juego empieza sin pausa
            paused = false;
            
            //Numero de modelos utilizados
            models = new Model[3];
            modelNames = new string[3] { @"resources/models/stair", @"resources/models/hole", @"resources/models/jump" };
            //Tiempo usado en el método uptdate
            tiempo = 0;
            //Los triangulos deben ser dibujados al contrario de las manecillas del reloj.
            GraphicsDevice.RenderState.CullMode = CullMode.CullClockwiseFace;
            
            //Se inicializa el effecto con algunas propiedades
            effect = new BasicEffect(graphics.GraphicsDevice, null);
            effect.Alpha = 1.0f;
            effect.LightingEnabled = false;
            effect.VertexColorEnabled = true;

            //Se crean los input
            mouse = new myMouse();
            keyboard = new myKeyboard();

            //Se crea la camara
            camera = new Camera(effect, graphics.GraphicsDevice);

            //Se crea le menú
            menu = new Menu(GraphicsDevice.Viewport);

            crearCubo(0, 0, 0);
            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            //Carga los modelos escaleras = 0, hueco = 1, salto = 2, actor = 3.
            for (int i = 0; i < models.Length; i++)
            {
                models[i] = Content.Load<Model>(modelNames[i]);    
            }
            //Carga las texturas del mouse
            mouse.loadTextures(Content.Load<Texture2D>(@"resources/textures/cursor"), Content.Load<Texture2D>(@"resources/textures/cursor_rotate"));
            
            //Carga las texturas del menu
            menu.loadTextures(Content.Load<Texture2D>(@"resources/textures/block"), Content.Load<Texture2D>(@"resources/textures/hole"), Content.Load<Texture2D>(@"resources/textures/jump"), Content.Load<Texture2D>(@"resources/textures/delete")
                             , Content.Load<Texture2D>(@"resources/textures/exit"), Content.Load<Texture2D>(@"resources/textures/actor"), Content.Load<Texture2D>(@"resources/textures/stairsOff")
                             , Content.Load<Texture2D>(@"resources/textures/blockOn"), Content.Load<Texture2D>(@"resources/textures/holeOn"), Content.Load<Texture2D>(@"resources/textures/jumpOn"), Content.Load<Texture2D>(@"resources/textures/deleteOn")
                             , Content.Load<Texture2D>(@"resources/textures/exitOn"), Content.Load<Texture2D>(@"resources/textures/actorOn"), Content.Load<Texture2D>(@"resources/textures/stairsOn"));
            
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);
        }
        #endregion

        /// <summary>
        /// Obtiene el modelo deseado.
        /// </summary>
        /// <param name="indice">Indice del modelo que se desea obtener, escaleras = 0, hueco = 1, salto = 2, actor = 3.</param>
        /// <returns>El modelo deseado, si no existe retorna null.</returns>
        public Model getModel(int indice)
        {
            if (indice >= 0 && indice < models.Length)
                return models[indice];
            return null;
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
        /// </summary>
        protected override void UnloadContent()
        {
            spriteBatch.Dispose();
            Content.Unload();
            Content.Dispose();
        }

        #region Creación de GameObjects
        /// <summary>
        /// Crea un nuevo cubo.
        /// </summary>
        /// <param name="x">Coordenada en x.</param>
        /// <param name="y">Coordenada en y.</param>
        /// <param name="z">Coordenada en z.</param>
        /// <param name="size">Tamaño del cubo.</param>
        public void crearCubo(int x, int y, int z)
        {
            Vector3 position = new Vector3(myMath.roundTo(x, SIZE_CUBO * 2), myMath.roundTo(y, SIZE_CUBO * 2), myMath.roundTo(z, SIZE_CUBO * 2));
            foreach (GameObject o in Components)
            {
                if (o.Position == position)
                    return;
            }
            Cubo c = new Cubo(this, position, SIZE_CUBO);
            cubos.Add(c);
            Components.Add(c);

            foreach (GameObject o in Components)
            {
                if (o is Cubo && ((Cubo)o) != c)
                    ((Cubo)o).setContorno();
            }
        }

        /// <summary>
        /// Crea una nueva escalera.
        /// </summary>
        /// <param name="x">Coordenada en x.</param>
        /// <param name="y">Coordenada en y.</param>
        /// <param name="z">Coordenada en z.</param>
        /// <param name="rotation">Rotacion inicial de la escalera.</param>
        public void crearEscalera(int x, int y, int z, float rotation)
        {
            Vector3 position = new Vector3(x, y, z);
            foreach (GameObject o in Components)
            {
                if (o.Position == position)
                    return;
            }
            Stair s = new Stair(this, position, SIZE_ESCALERA, rotation);
            Components.Add(s);
        }

        /// <summary>
        /// Crea un nuevo hueco.
        /// </summary>
        /// <param name="x">Coordenada en x.</param>
        /// <param name="y">Coordenada en y.</param>
        /// <param name="z">Coordenada en z.</param>
        /// <param name="c">Cubo en el cual esta el hueco.</param>
        public void crearHueco(int x, int y, int z, Cubo c)
        {
            //Si el cubo contiene un jumper lo borra.
            if (c.Jumper != null)
            {
                Components.Remove(c.Jumper);
                c.Jumper = null;
            }

            Vector3 position = new Vector3(x, y, z);
            foreach (GameObject o in Components)
            {
                if (o.Position == position)
                    return;
            }
            Hole h = new Hole(this, position, SIZE_ESCALERA);
            Components.Add(h);
            //Añade el hueco al cubo.
            c.Hole = h;
        }

        /// <summary>
        /// Crea un nuevo jumper.
        /// </summary>
        /// <param name="x">Coordenada en x.</param>
        /// <param name="y">Coordenada en y.</param>
        /// <param name="z">Coordenada en z.</param>
        /// <param name="c">Cubo en el cual esta el jumnper.</param>
        public void crearJumper(int x, int y, int z, Cubo c)
        {
            //Si el cubo contiene un hueco lo borra.
            if (c.Hole != null)
            {
                Components.Remove(c.Hole);
                c.Hole = null;
            }
            Vector3 position = new Vector3(x, y, z);
            foreach (GameObject o in Components)
            {
                if (o.Position == position)
                    return;
            }
            Jumper j = new Jumper(this, position, SIZE_ESCALERA);
            Components.Add(j);
            //Añade el hueco al cubo.
            c.Jumper = j;
        }
        #endregion

        #region Get de las matrices
        /// <summary>
        /// Obtiene la matriz de proyección.
        /// </summary>
        public Matrix ProjectionMatrix
        {
            get { return camera.ProjectionMatrix; }
        }
        /// <summary>
        /// Obtiene la matriz del mundo.
        /// </summary>
        public Matrix WorldMatrix
        {
            get { return camera.WorldMatrix; }
        }
        /// <summary>
        /// Obtiene la matriz de visión.
        /// </summary>
        public Matrix ViewMatrix
        {
            get { return camera.ViewMatrix; }
        }
        #endregion

        #region Properties (Get Y sets)
        /// <summary>
        /// Obtiene el effecto usado.
        /// </summary>
        public BasicEffect Effect
        {
            get { return effect; }
        }

        /// <summary>
        /// Obtiene los cubos que estan en el juego.
        /// </summary>
        public Cubo[] Cubos
        {
            get { return cubos.ToArray(); }
        }
        #endregion

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            //Solo se refresca el juego cuando la ventana esta activa.
            if (this.IsActive)
            {
                #region Keyboard Input
                //Para salir
                if (keyboard.isKeyPressed(Keys.Escape) && !keyboard.wasKeyPressed(Keys.Escape))
                    this.Exit();

                //Rotar la camara
                if (keyboard.isKeyPressed(Keys.Up) || keyboard.isKeyPressed(Keys.W))
                    camera.girarCamara(Camera.Direccion.UP, -0.005f);
                if (keyboard.isKeyPressed(Keys.Down) || keyboard.isKeyPressed(Keys.S))
                    camera.girarCamara(Camera.Direccion.DOWN, 0.005f);
                if (keyboard.isKeyPressed(Keys.Left) || keyboard.isKeyPressed(Keys.A))
                    camera.girarCamara(Camera.Direccion.LEFT, -0.005f);
                if (keyboard.isKeyPressed(Keys.Right) || keyboard.isKeyPressed(Keys.D))
                    camera.girarCamara(Camera.Direccion.RIGHT, 0.005f);

                keyboard.processKeyboard();
                #endregion

                //El input del mouse no se analiza si el juego esta pausado.
                if (!paused)
                {
                    #region MouseInput
                    //Guarda el estado actual y anteriro del mouse.
                    MouseState currentState = mouse.CurrentState;
                    MouseState pastState = mouse.PreviousState;

                    if (!mouse.isPressed(myMouse.MouseButtons.RightClick))
                        menu.iluminateIcon(mouse.X, mouse.Y);

                    //Mueve el menu, solo sucede si se oprime el botón mas de 300 milisegundos.
                    if (mouse.isPressed(myMouse.MouseButtons.LeftClick) && mouse.X > 0 && mouse.X < GraphicsDevice.Viewport.Width
                                        && mouse.Y > 0 && mouse.Y < GraphicsDevice.Viewport.Height)
                    {
                        if (menu.iconClicked(mouse.X, mouse.Y) != Menu.Iconos.NONE)
                        {
                            //Se va sumando el tiempo, cuando supere los 300 milisegundos el menu puede ser movido.
                            tiempo += gameTime.ElapsedGameTime.TotalMilliseconds;

                            if (!mouse.wasPressed(myMouse.MouseButtons.LeftClick))
                            {
                                menu.PosClicked = new Vector2(mouse.X, mouse.Y);
                                //Coje el menu
                                menu.IsSelected = true;
                            }
                        }

                        if (mouse.wasPressed(myMouse.MouseButtons.LeftClick) && menu.IsSelected && (tiempo > 300))
                        {
                            //Limita el mouse a la pantalla.
                            if (menu.getRectangle((Menu.Iconos)0).Left <= 0)
                                mouse.X = (int)menu.PosClicked.X + menu.getRectangle((Menu.Iconos)0).Left;

                            if (menu.getRectangle((Menu.Iconos)Menu.NUMEROICONOS - 1).Right >= GraphicsDevice.Viewport.Width)
                                mouse.X = (int)menu.PosClicked.X + menu.getRectangle((Menu.Iconos)0).Left;

                            if (menu.getRectangle((Menu.Iconos)0).Top <= 0)
                                mouse.Y = (int)menu.PosClicked.Y + menu.getRectangle((Menu.Iconos)0).Top;

                            if (menu.getRectangle((Menu.Iconos)Menu.NUMEROICONOS - 1).Bottom >= GraphicsDevice.Viewport.Height)
                                mouse.Y = (int)menu.PosClicked.Y + menu.getRectangle((Menu.Iconos)0).Top;

                            menu.moveMenu(new Vector2(mouse.X, mouse.Y));
                        }
                    }
                    //Procesa el clic del mouse.
                    if (!mouse.isPressed(myMouse.MouseButtons.LeftClick) && mouse.wasPressed(myMouse.MouseButtons.LeftClick)
                                         && mouse.X > 0 && mouse.X < GraphicsDevice.Viewport.Width && mouse.Y > 0 && mouse.Y < GraphicsDevice.Viewport.Height)
                    {
                        //Restea el tiempo
                        tiempo = 0;

                        //Suelta el menu
                        menu.IsSelected = false;

                        //Primero se mira si se le dio click a un icono del menu, de lo contrario se añade un cubo.
                        Menu.Iconos icono;
                        if ((icono = menu.iconClicked(mouse.X, mouse.Y)) != Menu.Iconos.NONE && !menu.IsSelected)
                        {
                            //Ejemplo
                            if (icono == Menu.Iconos.EXIT)
                                this.Exit();
                        }
                        else
                        {
                            Ray r = mouse.getRay(ProjectionMatrix, ViewMatrix, WorldMatrix, GraphicsDevice.Viewport);

                            //El objeto mas cercano que intersecta el rayo del mouse.
                            GameObject seleccionado = null;

                            //Punto en donde interseca el rayo al cubo.
                            Vector3 intersectionPoint = Vector3.Zero;

                            //Distancia a la cual interseca el cubo, y la menor distancia de intersección.
                            float distanciaInterseccion = 0f;
                            float menorDistancia = float.MaxValue;

                            //Mira con que objetos del mundo choca el rayo del mouse y selecciona el mas cercano de ellos.
                            foreach (GameObject o in Components)
                            {
                                if (myMath.intersectObject(o, r, out distanciaInterseccion))
                                {
                                    if (distanciaInterseccion < menorDistancia)
                                    {
                                        seleccionado = o;
                                        menorDistancia = distanciaInterseccion;
                                        intersectionPoint = r.Position + (r.Direction * distanciaInterseccion);
                                    }
                                }
                            }

                            //Si el objeto selecionado es un cubo...
                            if (seleccionado is Cubo)
                            {
                                //Obtiene la posición del objeto.
                                Vector3 position = seleccionado.Position;

                                GameObject.RelativePosition relativePosition = seleccionado.positionRelativeToOBject(intersectionPoint);

                                //Si el icono de crear cubo o crear escalera esta on, se crea un cubo o una escalera(Exepto si se undio click abajo del cubo) al lado de este.
                                if (menu.getIconClicked() == Menu.Iconos.BLOCK || menu.getIconClicked() == Menu.Iconos.STAIR || menu.getIconClicked() == Menu.Iconos.HOLE
                                    || menu.getIconClicked() == Menu.Iconos.JUMP)
                                {
                                    if (relativePosition == GameObject.RelativePosition.FRONT)
                                    {
                                        if (menu.getIconClicked() == Menu.Iconos.BLOCK)
                                            crearCubo((int)position.X, (int)position.Y, (int)position.Z + 2 * (int)seleccionado.Size);
                                        else if (menu.getIconClicked() == Menu.Iconos.STAIR)
                                        {
                                            crearEscalera((int)position.X, (int)position.Y, (int)position.Z + 2 * (int)seleccionado.Size, MathHelper.PiOver2);
                                        }
                                    }

                                    if (relativePosition == GameObject.RelativePosition.BACK)
                                    {
                                        if (menu.getIconClicked() == Menu.Iconos.BLOCK)
                                            crearCubo((int)position.X, (int)position.Y, (int)position.Z - 2 * (int)seleccionado.Size);
                                        else if (menu.getIconClicked() == Menu.Iconos.STAIR)
                                            crearEscalera((int)position.X, (int)position.Y, (int)position.Z - 2 * (int)seleccionado.Size, MathHelper.PiOver2 * 3);
                                    }

                                    if (relativePosition == GameObject.RelativePosition.LEFT)
                                    {
                                        if (menu.getIconClicked() == Menu.Iconos.BLOCK)
                                            crearCubo((int)position.X - 2 * (int)seleccionado.Size, (int)position.Y, (int)position.Z);
                                        else if (menu.getIconClicked() == Menu.Iconos.STAIR)
                                            crearEscalera((int)position.X - 2 * (int)seleccionado.Size, (int)position.Y, (int)position.Z, 0);
                                    }

                                    if (relativePosition == GameObject.RelativePosition.RIGHT)
                                    {
                                        if (menu.getIconClicked() == Menu.Iconos.BLOCK)
                                            crearCubo((int)position.X + 2 * (int)seleccionado.Size, (int)position.Y, (int)position.Z);
                                        else if (menu.getIconClicked() == Menu.Iconos.STAIR)
                                            crearEscalera((int)position.X + 2 * (int)seleccionado.Size, (int)position.Y, (int)position.Z, MathHelper.Pi);
                                    }

                                    if (relativePosition == GameObject.RelativePosition.UP)
                                    {
                                        if (menu.getIconClicked() == Menu.Iconos.BLOCK)
                                            crearCubo((int)position.X, (int)position.Y + 2 * (int)seleccionado.Size, (int)position.Z);
                                        else if (menu.getIconClicked() == Menu.Iconos.STAIR)
                                            crearEscalera((int)position.X, (int)position.Y + 2 * (int)seleccionado.Size, (int)position.Z, 0);
                                        else if (menu.getIconClicked() == Menu.Iconos.HOLE)
                                            crearHueco((int)position.X, (int)position.Y + (int)seleccionado.Size, (int)position.Z, (Cubo)seleccionado);
                                        else if (menu.getIconClicked() == Menu.Iconos.JUMP)
                                            crearJumper((int)position.X, (int)position.Y + (int)seleccionado.Size, (int)position.Z, (Cubo)seleccionado);
                                    }

                                    if (relativePosition == GameObject.RelativePosition.DOWN)
                                        crearCubo((int)position.X, (int)position.Y - 2 * (int)seleccionado.Size, (int)position.Z);
                                }
                            }
                            //Si el objeto seleccionado es una escalera la rota.
                            else if (seleccionado is Stair)
                            {
                                ((Stair)seleccionado).rotate();
                            }
                            //Si se señalo un hueco y esta la opción de crear jumper on, se borra el hueco y se crea el jumper.
                            else if (seleccionado is Hole && menu.getIconClicked() == Menu.Iconos.JUMP)
                            {
                            }
                            //Si no hay cubo seleccionado pero el icono de crear cubo esta on, se crea un cubo en la posición del mouse.
                            else if (seleccionado == null && menu.getIconClicked() == Menu.Iconos.BLOCK)
                            {
                                Vector3 position = mouse.get3DPosition(ProjectionMatrix, ViewMatrix, WorldMatrix, GraphicsDevice.Viewport);
                                crearCubo((int)position.X, (int)position.Y, (int)position.Z);
                            }
                            //Si el icono de borrar esta on, se borra el Objeto.
                            if (seleccionado != null && menu.getIconClicked() == Menu.Iconos.DELETE)
                            {
                                Components.Remove(seleccionado);
                                //En el caso que sea un cubo el objeto removido...
                                if (seleccionado is Cubo)
                                {
                                    cubos.Remove((Cubo)seleccionado);
                                    if (((Cubo)seleccionado).Hole != null)
                                        Components.Remove(((Cubo)seleccionado).Hole);
                                    if (((Cubo)seleccionado).Jumper != null)
                                        Components.Remove(((Cubo)seleccionado).Jumper);
                                    //Vuelve a establecer el contorno de los demas cubos.
                                    foreach (GameObject o in Components)
                                    {
                                        if (o is Cubo)
                                            ((Cubo)o).setContorno();
                                    }
                                }
                            }
                        }
                    }

                    //Rota la camara
                    if (mouse.isPressed(myMouse.MouseButtons.RightClick))
                    {
                        //Cambia la textura del mouse.
                        if (!mouse.wasPressed(myMouse.MouseButtons.RightClick))
                        {
                            mouse.switchTextures();
                            mouse.StaticX = mouse.X;
                            mouse.StaticY = mouse.Y;
                        }
                        float deltaX = (float)(currentState.X - pastState.X) / (float)(float)graphics.GraphicsDevice.Viewport.Width;
                        float deltaY = (float)(currentState.Y - pastState.Y) / (float)(float)graphics.GraphicsDevice.Viewport.Height;

                        if (deltaX > 0)
                            camera.girarCamara(Camera.Direccion.RIGHT, deltaX);
                        if (deltaX < 0)
                            camera.girarCamara(Camera.Direccion.LEFT, deltaX);
                        if (deltaY > 0)
                            camera.girarCamara(Camera.Direccion.UP, deltaY);
                        if (deltaY < 0)
                            camera.girarCamara(Camera.Direccion.DOWN, deltaY);
                        //Si el mouse e sale de la venteana lo posiciona al otro lado, de donde se salio.
                        if (mouse.CurrentState.X > GraphicsDevice.Viewport.Width)
                            mouse.X = 0;
                        if (mouse.CurrentState.X < 0)
                            mouse.X = GraphicsDevice.Viewport.Width;
                        if (mouse.CurrentState.Y > GraphicsDevice.Viewport.Height)
                            mouse.Y = 0;
                        if (mouse.CurrentState.Y < 0)
                            mouse.Y = GraphicsDevice.Viewport.Height;
                    }
                    //Retorna el cursor al estado del cursor al soltar click derecho.
                    if (!mouse.isPressed(myMouse.MouseButtons.RightClick) && mouse.wasPressed(myMouse.MouseButtons.RightClick))
                    {
                        mouse.resetCoordinates();
                        mouse.switchTextures();
                    }
                    //Renueva los estados del mouse.
                    mouse.processMouse();
                    #endregion
                }
            }

            //Para pausar
            if (keyboard.isKeyPressed(Keys.P) && !keyboard.wasKeyPressed(Keys.P))
                paused = !paused;

            foreach (GameObject o1 in Components)
            {
                foreach (GameObject o2 in Components)
                {
                    if (o1 != o2)
                        myMath.distanciaAparente(o1, o2, ViewMatrix);
                }
            }

            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            RenderState state = GraphicsDevice.RenderState;
            graphics.GraphicsDevice.Clear(Color.CornflowerBlue);

            //Dibuja los componentes (GameObjects)
            base.Draw(gameTime);

            //Se prepara para dibujar sprites.
            GraphicsDevice.Clear(ClearOptions.DepthBuffer, Color.Black, 0, 0);
            state.DepthBufferEnable = false;
            state.DepthBufferWriteEnable = false;

            //Dibuja el menú
            menu.Draw(spriteBatch);
            //Dibuja el mouse.
            mouse.Draw(spriteBatch);

            //Restaura valores del estado de renderización.
            state.DepthBufferWriteEnable = true;
            state.AlphaBlendEnable = false;
            state.DepthBufferEnable = true;
            state.CullMode = CullMode.CullClockwiseFace;
        }
    }  
}