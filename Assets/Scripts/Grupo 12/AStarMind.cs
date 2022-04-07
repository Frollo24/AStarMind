using System.Collections.Generic;
using System.Linq;
using Assets.Scripts.DataStructures;
using UnityEngine;

namespace Assets.Scripts.SampleMind
{
    public class AStarMind : AbstractPathMind {
        private Stack<Locomotion.MoveDirection> currentPlan = new Stack<Locomotion.MoveDirection>();

        public override void Repath()
        {
            currentPlan.Clear();
        }

        public override Locomotion.MoveDirection GetNextMove(BoardInfo boardInfo, CellInfo currentPos, CellInfo[] goals)
        {
            //Si aún quedan pasos en el plan, los devuelve
            if (currentPlan.Count > 0) 
            {
                return currentPlan.Pop();
            }

            //Busca un plan para seguir
            var searchResult = Search(boardInfo, currentPos, goals);

            //Si no se encuentra el plan, no solicita ningún movimiento
            if (searchResult == null)
            {
                Debug.Log("Cannot find a path");
                return Locomotion.MoveDirection.None;
            }

            //Si encuentra un plan, lo va añadiendo a la pila desde la meta hasta el inicio
            while (searchResult.Parent != null)
            {
                currentPlan.Push(searchResult.ProducedBy);
                searchResult = searchResult.Parent;
            }

            //Al sacar los elementos en orden inverso, realiza el plan desde el inicio
            if (currentPlan.Count > 0)
                return currentPlan.Pop();

            return Locomotion.MoveDirection.None;
        }

        private Node<Locomotion.MoveDirection> Search(BoardInfo board, CellInfo start, CellInfo[] goals)
        {
            //Crea una lista vacía de nodos
            var open = new List<Node<Locomotion.MoveDirection>>();

            //Se crea el nodo inicial y se añade a la lista abierta
            var n = new Node<Locomotion.MoveDirection>(board, start, goals);
            open.Add(n);

            //Mientras la lista no esté vacía
            while (open.Any())
            {
                //Se mira el primer nodo de la lista, se saca de la lista y se comprueba que
                //sea el nodo destino
                var node = open[0];
                open.RemoveAt(0);

                //Si es el nodo destino, se devuelve el nodo que contiene el plan
                if (node.isSameNode(goals[0]))
                    return node;

                if (node.Cost == float.MaxValue)
                    return null;

                //Expande el nodo a sus vecinos (calcula coste de cada uno, etc) y los añade en la lista
                var nodesToAdd = node.ExpandNode();
                for (int i = 0; i < nodesToAdd.Count; i++)
                {
                    if (nodesToAdd[i] == null) continue; //ExpandNode puede devolver nulos, los desechamos

                    //Asignamos la dirección que debe seguir y lo añadimos a la lista
                    if (nodesToAdd[i].dir == 0)
                        nodesToAdd[i].ProducedBy = Locomotion.MoveDirection.Up;
                    if (nodesToAdd[i].dir == 1)
                        nodesToAdd[i].ProducedBy = Locomotion.MoveDirection.Right;
                    if (nodesToAdd[i].dir == 2)
                        nodesToAdd[i].ProducedBy = Locomotion.MoveDirection.Down;
                    if (nodesToAdd[i].dir == 3)
                        nodesToAdd[i].ProducedBy = Locomotion.MoveDirection.Left;
                    open.Add(nodesToAdd[i]);
                }

                //Ordena la lista para expandir el nodo más prometedor
                open.Sort();
            }

            //Si se vacía la lista y no ha encontrado el nodo destino
            //quiere decir que no se ha encontrado un plan
            return null;
        }
    }
}
