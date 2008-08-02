using System;
using Microsoft.Xna.Framework;
using EscherWorld.Objetos;

namespace EscherWorld.Utility
{
    /// <summary>
    /// Clase que contiene ciertas ayudas matemáticas.
    /// </summary>
    static class myMath
    {
        /// <summary>
        /// Mira si un objeto es intersecado por el rayo.
        /// </summary>
        /// <param name="objeto">Objeto que se desea mirar</param>
        /// <param name="r">Rayo del mouse.</param>
        /// <param name="intersectionDistance">Out: Distancia donde se intersecto el objeto.</param>
        /// <returns>True si el objeto es intersectado por el rayo, de lo contrario false.</returns>
        public static bool intersectObject(GameObject objeto, Ray r, out float intersectionDistance)
        {
            //Posiciona el bounding box del objeto en coordenadas del mundo.
            Matrix world = Matrix.CreateScale(objeto.Size) * Matrix.CreateTranslation(objeto.Position);
            BoundingBox box = new BoundingBox();
            box.Max = Vector3.Transform(objeto.BoundingBox.Max, world);
            box.Min = Vector3.Transform(objeto.BoundingBox.Min, world);

            float? d;
            if ((d = r.Intersects(box)).HasValue == false)
            {
                intersectionDistance = 0f;
                return false;
            }
            intersectionDistance = d.Value;
            return true;
        }

        /// <summary>
        /// Determina la distancia entre dos objetos.
        /// </summary>
        /// <param name="o1">Primer objeto a comparar.</param>
        /// <param name="o2">Segundo objeto a comparar.</param>
        /// <returns>Distancia entre los dos.</returns>
        public static double distanciaEntreObjetos(GameObject o1, GameObject o2)
        {
            return Math.Sqrt(Math.Pow((o1.Position.X - o2.Position.X), 2) + Math.Pow((o1.Position.Y - o2.Position.Y), 2) + Math.Pow((o1.Position.Z - o2.Position.Z), 2));
        }

        /// <summary>
        /// Determina la distancia entre dos objetos sin tener en cuenta la coordenada z.
        /// </summary>
        /// <param name="o1">Primer objeto a comparar.</param>
        /// <param name="o2">Segundo objeto a comparar.</param>
        /// <param name="view">matirz de la vista.</param>
        /// <returns>Distancia entre los dos.</returns>
        public static double distanciaAparente(GameObject o1, GameObject o2, Matrix view)
        {
            Vector3 v1, v2;
            //Se transforma la posición de los cubo dependiendo de la posición de la camara.
            v1 = Vector3.Transform(o1.Position, view);
            v2 = Vector3.Transform(o2.Position, view);

            return Math.Sqrt(Math.Pow((v1.X - v2.X), 2) + Math.Pow((v1.Y - v2.Y), 2));
        }

        /// <summary>
        /// Método que redondea un número a el valor mas cercano multiplo de b.
        /// </summary>
        /// <param name="a">Número que se desea redondear.</param>
        /// <param name="b">Número al cual se desea redondear el valor a uno de sus multiplos.</param>
        /// <returns></returns>
        public static int roundTo(double a, int b)
        {
            if (a % b >= b/2)
                a += b - (a % b);
            else
                a -= (a % b);
            return (int)a;
        }
    }
}
