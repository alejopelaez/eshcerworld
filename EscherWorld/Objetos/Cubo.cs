using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using EscherWorld.Utility;

namespace EscherWorld.Objetos
{
    public class Cubo : GameObject
    {
        private const int NUM_VERTICES = 8;

        private Hole hole;
        private Jumper jumper;
        private Vector3[] vertices;
        private VertexBuffer vBufferCubo, vBufferLineas;
        private IndexBuffer iBufferCubo, iBufferLineas;
        private int[] indiceCubo;
        private List<int> indiceLineas;
        //Arreglo que guarda los cubos cercanos a este, para aumentar la eficiencia en ciertos calculos.
        private List<Cubo> cubosAdyacentes;

        #region Constructores
        /// <summary>
        /// Constructor de la clase Cubo.
        /// </summary>
        /// <param name="engine">Clase principal del juego.</param>
        /// <param name="position">Posición del cubo.</param>
        /// <param name="size">Tamaño del cubo. (1 es lo normal)</param>
        public Cubo(Engine engine, Vector3 position, float size)
            : base(engine, position, size)
        {
            Visible = true;
        }

        /// <summary>
        /// Constructor de la clase cubo
        /// </summary>
        /// <param name="engine">Clase principal del juego.</param>
        /// <param name="position">Posición del centro del cubo.</param>
        /// <param name="size">Tamaño del cubo (Lo normal es 1)</param>
        /// <param name="color">Color del cubo</param>
        public Cubo(Engine engine, Vector3 position, float size, Color color)
            : base(engine, position, color, size)
        {
            Visible = true;
        }
        #endregion

        #region Properties (Gets y sets)
        /// <summary>
        /// Obtiene o asigna el hueco de este cubo.
        /// </summary>
        public Hole Hole
        {
            get { return hole; }
            set { hole = value; }
        }

        /// <summary>
        /// Obtiene o asigna el jumper de este cubo.
        /// </summary>
        public Jumper Jumper
        {
            get { return jumper; }
            set { jumper = value; }
        }

        /// <summary>
        /// Obtiene los vertices Superiores del cubo.
        /// </summary>
        public Vector3[] VerticesSuperiores
        {
            get
            {
                Vector3[] vert = new Vector3[vertices.Length / 2];
                Matrix world = Matrix.CreateScale(size) * Matrix.CreateTranslation(position);
                int j = 0;
                for (int i = 0; i < vertices.Length; i++)
                {
                    if (vertices[i].Y == 1)
                    {
                        vert[j] = Vector3.Transform(vertices[i], world);
                        j++;
                    }
                }
                return vert;
            }
        }
        /// <summary>
        /// Obtiene los vertices Iuperiores del cubo.
        /// </summary>
        public Vector3[] VerticesInferiores
        {
            get
            {
                Vector3[] vert = new Vector3[vertices.Length / 2];
                Matrix world = Matrix.CreateScale(size) * Matrix.CreateTranslation(position);
                for (int i = 0; i < vert.Length; i++)
                {
                    if (vertices[i].Y == -1)
                        vert[i] = Vector3.Transform(vertices[i], world);
                }
                return vert;
            }
        }

        /// <summary>
        /// Obtiene el indice de los vertex del cubo.
        /// </summary>
        public int[] IndiceCubo
        {
            get { return indiceCubo; }
        }

        /// <summary>
        /// Obtiene los cubos adyacentes a este.
        /// </summary>
        public List<Cubo> CubosAdyacentes
        {
            get { return cubosAdyacentes; }
        }

        #endregion

        /// <summary>
        /// Carga el contenido inicial del cubo.
        /// </summary>
        public override void  Initialize()
        {
            base.Initialize();

            //El cubo empieza sin hueco ni jumper.
            hole = null;
            jumper = null;
            //Se crean los vertices iniciales del cubo
            vertices = new Vector3[8];
            //Top Left front
            vertices[0] = new Vector3(-1, 1, 1);
            //Top Right front
            vertices[1] = new Vector3(1, 1, 1);
            //Down Right front
            vertices[2] = new Vector3(1, -1, 1);
            //Down Left front
            vertices[3] = new Vector3(-1, -1, 1);
            //Down Right back
            vertices[4] = new Vector3(-1, -1, -1);
            //Top Right back
            vertices[5] = new Vector3(-1, 1, -1);
            //Top Left back
            vertices[6] = new Vector3(1, 1, -1);
            //Down Left back
            vertices[7] = new Vector3(1, -1, -1);

            //Bounding box del cubo.
            boundingBox = BoundingBox.CreateFromPoints(vertices);
            
            //Lista con los cubos adyacentes a este, ayuda a opimizar calculos.
            cubosAdyacentes = new List<Cubo>();

            setVertices();
        }

        #region Métodos para dibujar el contorno de los cubos
        /// <summary>
        /// Asigna los vertices del cubo al igual que los indices para dibujarlo.
        /// </summary>
        private void setVertices()
        {
            //Vertex del cubo
            VertexPositionColor[] vertexCubo = new VertexPositionColor[vertices.Length];
            for (int i = 0; i < vertices.Length; i++)
            {
                vertexCubo[i] = new VertexPositionColor(vertices[i], color);    
            }

            //Se crean los indices.
            indiceCubo = new int[36] { 0, 3, 1, 1, 3, 2, 5, 0, 6, 6, 0, 1, 1, 2, 6, 6, 2, 7, 5, 4, 0, 0, 4, 3, 3, 4, 2, 2, 4, 7, 6, 7, 5, 5, 7, 4 };

            //Vertex buffer e indice del cubo.
            vBufferCubo = new VertexBuffer(GraphicsDevice, VertexPositionColor.SizeInBytes * vertexCubo.Length, BufferUsage.WriteOnly);
            vBufferCubo.SetData<VertexPositionColor>(vertexCubo);
            iBufferCubo = new IndexBuffer(GraphicsDevice, typeof(int), indiceCubo.Length, BufferUsage.WriteOnly);
            iBufferCubo.SetData<int>(indiceCubo);
            
            //Crea el contorno del cubo.
            setContorno();
        }

        /// <summary>
        /// Asigna los vertices del contorno del cubo al igual que los indices para dibujarlo.
        /// </summary>
        public void setContorno()
        {
            //Indice de las lineas
            indiceLineas = new List<int>();

            //Vertex de las lineas
            List<VertexPositionColor> vertexLineas = new List<VertexPositionColor>(vertices.Length);
            for (int i = 0; i < vertices.Length; i++)
            {
                vertexLineas.Add(new VertexPositionColor(vertices[i], Color.Black));
            }

            //Crea el indice de los vertices de las lineas.
            for(int i = 0; i < NUM_VERTICES; i++)
            {
                for (int j = i; j < NUM_VERTICES; j++)
                {
                    if (vertexLineas[i] != vertexLineas[j])
                    {
                        if (vertexLineas[i].Position.X == vertexLineas[j].Position.X && vertexLineas[i].Position.Y == vertexLineas[j].Position.Y)
                            dibujarLinea(vertexLineas, i, j);
                        
                        else if (vertexLineas[i].Position.X == vertexLineas[j].Position.X && vertexLineas[i].Position.Z == vertexLineas[j].Position.Z)
                            dibujarLinea(vertexLineas, i, j);

                        else if (vertexLineas[i].Position.Y == vertexLineas[j].Position.Y && vertexLineas[i].Position.Z == vertexLineas[j].Position.Z)
                            dibujarLinea(vertexLineas, i, j);
                    }
                }
            }
            if (indiceLineas.Count == 0)
            {
                indiceLineas.Add(0);
                indiceLineas.Add(0);
            }

            //Vertex buffer e indice de las lineas.
            vBufferLineas = new VertexBuffer(GraphicsDevice, VertexPositionColor.SizeInBytes * vertexLineas.Count, BufferUsage.WriteOnly);
            vBufferLineas.SetData<VertexPositionColor>(vertexLineas.ToArray());
            iBufferLineas = new IndexBuffer(GraphicsDevice, typeof(int), indiceLineas.Count, BufferUsage.WriteOnly);
            iBufferLineas.SetData<int>(indiceLineas.ToArray());
        }

        /// <summary>
        /// Dibuja una linea entre dos vertices, siempre y cuando no halla ningun cubo que choque con esta.
        /// </summary>
        /// <param name="vertexLineas">Lista de los vertices de las lineas.</param>
        /// <param name="i">Indice del pirmer vertice.</param>
        /// <param name="j">Indice del segundo vertice.</param>
        public void dibujarLinea(List<VertexPositionColor> vertexLineas, int i, int j)
        {
            //Distancia a la cual se choca el rayo con el cubo
            float distancia = 0;

            foreach (Cubo c in ((Engine)Game).Cubos)
            {
                if (c != this && myMath.distanciaEntreObjetos(this, c) <= 4)
                {
                    //Posiciona los dos vertices en espacio del mundo.
                    Matrix world = Matrix.CreateScale(size) * Matrix.CreateTranslation(position);
                    Vector3 v1 = Vector3.Transform(vertexLineas[i].Position, world);
                    Vector3 v2 = Vector3.Transform(vertexLineas[j].Position, world);

                    //Crea un rayo desde el vertice 1 al 2.
                    //El segundo rayo es para comprobar que si choque el rayo y no solo un punto
                    //(es el mismo rayo pero empezando un poco mas adelante.
                    Ray r1 = new Ray(v1, Vector3.Normalize(v2 - v1));
                    Ray r2 = new Ray(r1.Position + Vector3.Normalize(v2 - v1) / 100f, Vector3.Normalize(v2 - v1));

                    //Añade el cubo a la lista, si este no estaba ya.
                    if (!cubosAdyacentes.Contains(c))
                        cubosAdyacentes.Add(c);

                    //Mira si los vertices intersecan al otro cubo.
                    if (myMath.intersectObject(c, r2, out distancia) && myMath.intersectObject(c, r1, out distancia) && distancia >= 0f && distancia < 4f)
                    {
                        Vector3 nuevo = r1.Position + (r1.Direction * distancia);
                        nuevo = Vector3.Transform(nuevo, Matrix.Invert(world));
                        //Crea el vertice donde choco el rayo y lo añade a la lista.
                        VertexPositionColor vertex = new VertexPositionColor(nuevo, Color.Black);
                        if (!vertexLineas.Contains(vertex))
                            vertexLineas.Add(vertex);

                        //indice del vertice donde choca el rayo 
                        //Se compara para mirar si es el mismo ke el vertice de origen.
                        int h = vertexLineas.IndexOf(vertex);
                        if (i != h)
                        {
                            indiceLineas.Add(i);
                            indiceLineas.Add(h);
                        }
                        return;
                    }
                }
            }
            indiceLineas.Add(i);
            indiceLineas.Add(j);
        }
        #endregion

        /// <summary>
        /// Remueve el cubo deseado de la lista de cubos adyacentes.
        /// </summary>
        /// <param name="c"></param>
        public void removeCuboFromAdyacentes(Cubo c)
        {
            if (cubosAdyacentes.Contains(c))
                cubosAdyacentes.Remove(c);
        }

        /// <summary>
        /// Método que determina si un punto esta dentro del cubo.
        /// </summary>
        /// <param name="point">Punto que se desea chequear.</param>
        /// <returns>Verdadero si esta adentro, de lo contrario falso.</returns>
        public bool isPointInside(Vector3 point)
        {
            if (boundingBox.Contains(point) != ContainmentType.Disjoint)
                return true;
            return false;
        }

        /// <summary>
        /// Método que determina en que cara del cubo esta el punto dado.
        /// </summary>
        /// <param name="v">Punto que se desea chequear.</param>
        /// <returns>Cara del Cubo relativa al punto.</returns>
        public override RelativePosition positionRelativeToOBject(Vector3 v)
        {
            //Posiciona el bounding box del objeto en coordenadas del mundo.
            Matrix world = Matrix.CreateScale(Size) * Matrix.CreateTranslation(Position);
            BoundingBox box = new BoundingBox();
            box.Max = Vector3.Transform(BoundingBox.Max, world);
            box.Min = Vector3.Transform(BoundingBox.Min, world);
            
            if (v.X >= box.Max.X)
                return RelativePosition.RIGHT;
            if (v.X <= box.Min.X)
                return RelativePosition.LEFT;
            if (v.Y >= box.Max.Y)
                return RelativePosition.UP;
            if (v.Y <= box.Min.Y)
                return RelativePosition.DOWN;
            if (v.Z >= box.Max.Z)
                return RelativePosition.FRONT;
            if (v.Z <= box.Min.Z)
                return RelativePosition.BACK;
            return RelativePosition.NONE;
        }

        /// <summary>
        /// Dibuja el objeto.
        /// </summary>
        /// <param name="gameTime">Tiempo que lleva el juego.</param>
        public override void Draw(GameTime gameTime)
        {
            //Guarda la matriz original
            Matrix original = ((Engine)Game).WorldMatrix;
            BasicEffect effect = ((Engine)Game).Effect;

            //Prepara el device
            GraphicsDevice.VertexDeclaration = new VertexDeclaration(GraphicsDevice, VertexPositionColor.VertexElements);

            effect.World = Matrix.CreateScale(size) * Matrix.CreateTranslation(position) * ((Engine)Game).WorldMatrix;
            effect.Begin();
            foreach (EffectPass pass in effect.CurrentTechnique.Passes)
            {
                pass.Begin();
                //Contorno de lineas
                GraphicsDevice.Indices = iBufferLineas;
                GraphicsDevice.Vertices[0].SetSource(vBufferLineas, 0, VertexPositionColor.SizeInBytes);
                GraphicsDevice.DrawIndexedPrimitives(PrimitiveType.LineList, 0, 0, 17, 0, indiceLineas.Count / 2);
                
                
                //Parte solida
                GraphicsDevice.RenderState.DepthBias = 0.0012f;
                GraphicsDevice.Indices = iBufferCubo;
                GraphicsDevice.Vertices[0].SetSource(vBufferCubo, 0, VertexPositionColor.SizeInBytes);
                GraphicsDevice.DrawIndexedPrimitives(PrimitiveType.TriangleList, 0, 0, 36, 0, indiceCubo.Length / 3);
                GraphicsDevice.RenderState.DepthBias = 0f;
                pass.End();
                
            }
            effect.End();
            //effect.World = original;
        }
    }
}