using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace Assets.Scripts.DataStructures
{
    //Clase Node genérica, hereda de IComparable para poder ser ordenada automáticamente
    //por una lista genérica al sobreescribir el método CompareTo
    public class Node<T> : IComparable<Node<T>>
    {
        public Node<T> Parent { get; set; } //Nodo padre de este nodo
        public T ProducedBy; //Dato genérico que ha producido la expansión del nodo
        public float Cost { get; set; } //Coste de acceder a un nodo
        public int dir { get; private set; } //Traducción de entero a dato genérico (dirección)

        private float manhDist; //Distancia Manhattan a un nodo (func. heurística)
        
        private CellInfo cell; //Celda asociada al nodo
        private CellInfo[] goals; //Array de metas a seguir
        private BoardInfo board; //Información del tablero

        public Node(){
            this.Parent = null;
            this.cell = null;
            this.board = null;
            this.goals = null;
            this.manhDist = 0.0f;
        }

        public Node(BoardInfo board, CellInfo cell, CellInfo[] goals)
        {
            this.Parent = null;
            this.cell = cell;
            this.board = board;
            this.goals = goals;

            //Calculamos la distancia Manhattan como *m = |x - x0| + |y - y0|*
            manhDist = Mathf.Abs(goals[0].RowId - cell.RowId) + Mathf.Abs(goals[0].ColumnId - cell.ColumnId);
        }

        //Comprueba si dos nodos tienen la misma celda asociada
        public bool isSameNode(CellInfo c)
        {
            bool isEqual = true;
            if (c.RowId != cell.RowId || c.ColumnId != cell.ColumnId) isEqual = false;
            return isEqual;
        }

        //Expande el nodo, comprobando qué direcciones son transitables
        public List<Node<T>> ExpandNode()
        {
            //Devuelve los vecinos del escenario, sean transitables o no
            CellInfo[] neighbours = cell.GetNeighbours(board);
            List<Node<T>> nodes = new List<Node<T>>();

            //Asigna a cada nodo una celda asociada y calcula su coste
            int i = 0;
            foreach (CellInfo c in neighbours)
            {
                if (c == null) //Desechamos los vecinos nulos
                {
                    i++;
                    continue;
                }

                Node<T> newNode = new Node<T>(board, c, goals);
                newNode.Parent = this; //Asignamos como padre de cada nodo expandido, el nodo que los expandió

                var nodeToTraverse = newNode;
                int depth = 0; // Representa el coste G de llegar a un nodo
                while (nodeToTraverse != null)
                {
                    nodeToTraverse = nodeToTraverse.Parent;
                    depth++;
                }

                newNode.Cost = depth + c.WalkCost + manhDist;

                newNode.dir = i;

                nodes.Add(newNode);

                i++;
            }

            return nodes;
        }

        //Sobreescribe el método CompareTo de IComparable<T>
        public int CompareTo(Node<T> node)
        {
            if (node == null) //Asignamos los nodos nulos como más grandes
                return 1; //this node is greater than parameter node
            int c = Cost.CompareTo(node.Cost);

            return c;
        }
    }
}