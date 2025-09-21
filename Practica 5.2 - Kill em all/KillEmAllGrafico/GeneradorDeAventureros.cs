using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using System.Reflection;

namespace KillEmAll
{
    public class GeneradorDeAventureros
    {
        //Pon aquí los nombres de los Aventureros que están en la carpeta AventurerosDePrueba, junto
        //con su namespace, como en el ejemplo
        /*private static string[] s_aventureros = new string[] {
            "Practica5_2.AventureroSoloMovimiento",
            "Practica5_2.AventureroSoloPercepcion"};*/

        private static string[] s_aventureros = new string[] {
                //Aqui iran los aventuraros a cargar
                //ejemplo: "Practica5_2.AventureroMago"
            
             };


        public static Dictionary<int, Aventurero> GenerarAventureros()
        {
            Dictionary<int, Aventurero> lista = new Dictionary<int, Aventurero>();
            Aventurero aventurero;
            for (int i = 0; i < s_aventureros.Length; ++i) {
                Type tipo = Type.GetType(s_aventureros[i]);
                ConstructorInfo constructor = tipo.GetConstructor(new Type[0]);
                aventurero = (Aventurero)constructor.Invoke(null);
                aventurero.Vida = ReglasDelJuego.s_vidaPorClase[aventurero.Clase];
                lista.Add(aventurero.Id, aventurero);
            }
            return lista;
        }

        public static Dictionary<int, EstadoAventurero> GenerarEstadosIniciales(Dictionary<int, Aventurero> listaAventureros)
        {
            Random generador = new Random();
            Dictionary<int, EstadoAventurero> estados = new Dictionary<int, EstadoAventurero>();
            foreach (Aventurero aventurero in listaAventureros.Values) {
                Point posicionAleatoria = new Point(generador.Next(0, 800), generador.Next(0, 800));
                EstadoAventurero estado = new EstadoAventurero(aventurero.Id, aventurero.Nombre, aventurero.Vida, posicionAleatoria, aventurero.Clase);
                aventurero.Situacion = estado.Situacion;
                estados[aventurero.Id] = estado;
            }
            return estados;
        }
    }
}
