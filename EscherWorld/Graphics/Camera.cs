using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace EscherWorld.Graphics
{
    /// <summary>
    /// Clase que controla la camara
    /// </summary>
    class Camera
    {
        /// <summary>
        /// Dirección hacia donde se desea hacer un giro, rotaci´´on de mundo o translación de la camara.
        /// </summary>
        public enum Direccion {UP, DOWN, LEFT, RIGHT};     
        GraphicsDevice device;
        BasicEffect effect;
        Matrix worldMatrix, viewMatrix, projection;
        Vector3 posicion, destino;
        Quaternion rotation;
        double angleXZ, angleYZ;
        float yaw, pitch, roll;

        /// <summary>
        /// Constructor de la clase camara, se inicializan las matrices y se añaden al effect.
        /// </summary>
        /// <param name="effect">Effecto que se usa para dibujar.</param>
        /// <param name="device">El device actual.</param>
        public Camera(BasicEffect effect, GraphicsDevice device)
        {
            this.effect = effect;
            this.device = device;

            //Angulos inciales de la camara
            angleXZ = MathHelper.Pi / 4;
            angleYZ = MathHelper.Pi / 6;

            yaw = 0;
            pitch = 0;
            roll = 0;

            rotation = Quaternion.Identity;

            //Posicicón y destino inicial de la camara
            posicion.Y = 20 * (float)Math.Sin(angleYZ);
            posicion.X = 20 * (float)Math.Cos(angleYZ) * (float)Math.Sin(angleXZ);
            posicion.Z = 20 * (float)Math.Cos(angleYZ) * (float)Math.Cos(angleXZ);
            destino = new Vector3(0, 0, 0);
            
            //Se definen las matrices
            setCamera();
            worldMatrix = Matrix.Identity;
            projection = Matrix.CreateOrthographic(device.Viewport.Width / 8, device.Viewport.Height / 8, -200.0f, 200.0f);
            //projection = Matrix.CreatePerspectiveFieldOfView(MathHelper.PiOver4, device.Viewport.AspectRatio, 1.0f, 200.0f);

            //Se añaden al effect
            effect.World = worldMatrix;
            effect.View = viewMatrix;
            effect.Projection = projection;
        }

        /// <summary>
        /// Pone la camara en el lugar seleccionada mirando hacia el destino deseado.
        /// </summary>
        /// <param name="posicion">Posicion de la camara.</param>
        /// <param name="destino">Hacia donde mira la camara.</param>
        public void setCamera()
        {
            viewMatrix = Matrix.CreateLookAt(posicion, destino, new Vector3(0, 1, 0));
            effect.View = viewMatrix;
        }

        /// <summary>
        /// Mueve la camara en la dirección deseada.
        /// </summary>
        /// <param name="d">Dirección hacia donde se desea mover la camara.</param>
        public void moverCamara(Direccion d)
        {
            /*if (d == Direccion.UP) ;
            if (d == Direccion.DOWN) ;
            if (d == Direccion.LEFT) ;
            if (d == Direccion.RIGHT) ;*/
        }

        /// <summary>
        /// Gira la camara en la dirección deseada.
        /// </summary>
        /// <param name="d">Dirección hacia donde se quiere girar la camara.</param>
        /// <param name="amount">Cantidad que se desea rotar, donde 0 es nada, y 1 es una vuelta entera.</param>
        public void girarCamara(Direccion d, float amount)
        {
            if (d == Direccion.UP || d == Direccion.DOWN)
            {
                angleYZ += (amount * MathHelper.TwoPi);
                if (angleYZ > (MathHelper.PiOver2 - 0.1))
                    angleYZ = MathHelper.PiOver2 - 0.1;
                if (angleYZ < -MathHelper.PiOver2 + 0.1)
                    angleYZ = -MathHelper.PiOver2 + 0.1;

                posicion.Y = 20 * (float)Math.Sin(angleYZ);
                posicion.X = 20 * (float)Math.Cos(angleYZ) * (float)Math.Sin(angleXZ);
                posicion.Z = 20 * (float)Math.Cos(angleYZ) * (float)Math.Cos(angleXZ);
                setCamera();
            }

            if (d == Direccion.RIGHT || d == Direccion.LEFT)
            {
                angleXZ -= (amount * MathHelper.TwoPi);
                posicion.X = 20 * (float)Math.Cos(angleYZ) * (float)Math.Sin(angleXZ);
                posicion.Z = 20 * (float)Math.Cos(angleYZ) * (float)Math.Cos(angleXZ);
                setCamera();
            }
        }

        /// <summary>
        /// Rota el mundo en la dirección deseada.
        /// </summary>
        /// <param name="d">Dirección hacia donde se quiere girar el mundo</param>
        public void rotarMundo(Direccion d, float amount)
        {
            if (d == Direccion.UP || d == Direccion.DOWN)
            {
                pitch += (amount*MathHelper.TwoPi);
                
                if (pitch > MathHelper.PiOver2)
                    pitch = MathHelper.PiOver2;
                if (pitch < -MathHelper.PiOver2)
                    pitch = -MathHelper.PiOver2;

                rotation = Quaternion.CreateFromYawPitchRoll(yaw, pitch, roll);
                worldMatrix = Matrix.CreateFromQuaternion(rotation);
            }
            if (d == Direccion.LEFT || d == Direccion.RIGHT)
            {
                yaw += amount * MathHelper.TwoPi;
                rotation = Quaternion.CreateFromYawPitchRoll(yaw, pitch, roll);
                worldMatrix = Matrix.CreateFromQuaternion(rotation);
            }
            effect.World = worldMatrix;
        }

        /// <summary>
        /// Obtiene o asigna la matriz de proyección.
        /// </summary>
        public Matrix ProjectionMatrix
        {
            get { return projection; }
            set { projection = value; }
        }
        /// <summary>
        /// Obtiene o asigna la matriz del mundo.
        /// </summary>
        public Matrix WorldMatrix
        {
            get { return worldMatrix; }
            set { WorldMatrix = value; }
        }
        /// <summary>
        /// Obtiene o asigna la matriz de visión.
        /// </summary>
        public Matrix ViewMatrix
        {
            get { return viewMatrix; }
            set { viewMatrix = value; }
        }
    }
}