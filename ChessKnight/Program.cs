using System;
using System.Collections.Generic;

namespace ChessKnight
{
    class Program
    {
        /// <summary>
        /// позиция описывает координаты фигуры на доске
        /// </summary>
        public struct Point
        {
            /// <summary>
            /// позиция по ширине доски
            /// </summary>
            public int W;

            /// <summary>
            /// позиция по высоте доски
            /// </summary>
            public int H;

            /// <summary>
            /// конструктор позиции
            /// </summary>
            /// <param name="w">ширина</param>
            /// <param name="h">высота</param>
            public Point( int w, int h )
            {
                this.W = w;
                this.H = h;
            }
        }

        /// <summary>
        /// шахматная фигура
        /// </summary>
        abstract public class ChessMan
        {
            /// <summary>
            /// массив описывающий возможные ходы фигуры в относительных координатах
            /// за центр [0, 0] принимается позиция самой фигуры
            /// оси  (декартовы, прямоугольные) проходящие через центр, делят плоскость  на 4 квадранта
            /// Левый Верхний - координата по ширине - отрицательна,  координата по высоте - отрицательна
            /// Правый Верхний - координата по ширине - положительна,  координата по высоте - отрицательна
            /// Левый Нижний - координата по ширине - отрицательна,  координата по высоте - положительна
            /// Правый Нижний  - координата по ширине - положительна,  координата по высоте - положительна
            /// </summary>
            static Point[] moveslib;

            /// <summary>
            /// позиция фигуры на доске
            /// </summary>
            Point currpos;

            /// <summary>
            /// массив описывающий возможные ходы фигуры в относительных координатах
            /// </summary>
            public static Point[] MovesLib
            {
                get { return moveslib; }
                set { moveslib = value; }
            }

            /// <summary>
            /// позиция фигуры на доске
            /// </summary>
            public Point CurrentPosition
            {
                get { return this.currpos; }
                set { this.currpos = value; }
            }
        }

        /// <summary>
        /// Конь: шахматная фигура
        /// </summary>
        public class ChessKnight : ChessMan
        {
            /// <summary>
            /// статический конструктор объектов Конь
            /// </summary>
            static ChessKnight()
            {
                MovesLib = new Point[8];
                MovesLib[0] = new Point( -2, -1 );
                MovesLib[1] = new Point( -1, -2 );
                MovesLib[2] = new Point( 1, -2 );
                MovesLib[3] = new Point( 2, -1 );
                MovesLib[4] = new Point( 2, 1 );
                MovesLib[5] = new Point( 1, 2 );
                MovesLib[6] = new Point( -1, 2 );
                MovesLib[7] = new Point( -2, 1 );
            }

            /// <summary>
            /// конструктор объекта Конь
            /// </summary>
            /// <param name="w">позиция по ширине</param>
            /// <param name="h">позиция по высоте</param>
            public ChessKnight( int w, int h )
            {
                this.CurrentPosition = new Point( w, h );
            }
        }

        /// <summary>
        /// Доска (шахматная)
        /// </summary>
        public struct ChessBoard
        {
            /// <summary>
            /// ширина доски
            /// </summary>
            public int Width;

            /// <summary>
            /// высота доски
            /// </summary>
            public int Height;

            /// <summary>
            /// элементы (ячейки) доски
            /// </summary>
            public int[,] Board;

            /// <summary>
            /// стек - история ходов
            /// </summary>
            public Stack<Point> Moves;

            /// <summary>
            /// конструктор объекта Доска
            /// </summary>
            /// <param name="w">ширина</param>
            /// <param name="h">высота</param>
            public ChessBoard( int w, int h )
            {
                this.Width = w;
                this.Height = h;
                this.Board = new int[this.Width, this.Height];
                this.Moves = new Stack<Point>();
            }

            /// <summary>
            /// вычисление новой позиции фигуры
            /// </summary>
            /// <param name="figure">шахматная фигура</param>
            /// <param name="move">предполагаемый ход (в относительных координатах)</param>
            /// <returns>новая позиция фигуры</returns>
            private Point CalculateFigurePosition( ChessMan figure, Point move )
            {
                return new Point( figure.CurrentPosition.W + move.W, figure.CurrentPosition.H + move.H );
            }

            /// <summary>
            /// проверка возможности предполагаемого хода
            /// </summary>
            /// <param name="figure">шахматная фигура</param>
            /// <param name="move">предполагаемый ход (в относительных координатах)</param>
            /// <param name="newPos">новая позиция фигуры</param>
            /// <returns>предполагаемый ход возможен - true/false</returns>
            public bool IsPosibleMove( ChessMan figure, Point move, out Point newPos )
            {
                newPos = CalculateFigurePosition( figure, move );

                //новая позиция фигуры вписывается в габариты доски и эта позиция еще не занята
                if (newPos.W >= 0 && newPos.W < this.Width && newPos.H >= 0 && newPos.H < this.Height && !( this.Moves.Contains( newPos ) )) return true;
                else return false;
            }

            /// <summary>
            /// записать ход
            /// </summary>
            /// <param name="figure"></param>
            public void AddFigureMove( ChessMan figure )
            {
                this.Moves.Push( figure.CurrentPosition );
                this.Board[figure.CurrentPosition.W, figure.CurrentPosition.H] = this.Moves.Count;
            }

            /// <summary>
            /// вывести доску с позициями ходов
            /// </summary>
            public void PrintBoard()
            {
                for (int j = 0; j < this.Height; j++)
                {
                    for (int i = 0; i < this.Width; i++)
                    {
                        Console.Write( $"\t{this.Board[i, j]}" );
                    }
                    Console.WriteLine();
                    Console.WriteLine();
                }
            }
        }

        /// <summary>
        /// добавить фигуру на доску
        /// </summary>
        /// <param name="figure">фигура</param>
        /// <param name="board">доска</param>
        static void AddFigureToBoard( ChessMan figure, ChessBoard board )
        {
            if (board.Moves.Count >= ( board.Width * board.Height )) return;

            foreach (Point libNextMove in ChessKnight.MovesLib)
            {
                if (board.Moves.Count >= ( board.Width * board.Height )) return;

                if (!( board.Moves.Peek().Equals( figure.CurrentPosition ) ))
                {
                    board.Moves.Pop();
                }

                bool IsPosibleMove = board.IsPosibleMove( figure, libNextMove, out Point newPosition );

                if (IsPosibleMove)
                {
                    ChessKnight knight = new ChessKnight( newPosition.W, newPosition.H );
                    board.AddFigureMove( knight );

                    AddFigureToBoard( knight, board );
                }
            }
        }

        static void Main( string[] args )
        {

            ChessBoard cBrd = new ChessBoard( 5, 5 );
            ChessKnight cKnt = new ChessKnight( 0, 0 );

            cBrd.AddFigureMove( cKnt );

            AddFigureToBoard( cKnt, cBrd );

            cBrd.PrintBoard();

            Console.ReadKey();
        }
    }
}
