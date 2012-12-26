﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TicTacToeSignalR
{
    /// <summary>
    /// Game entity that holds the game context : players, moves, board.
    /// </summary>
    public class Game
    {
        public const int Dimension = 3;

        #region Private Members
        private Guid _gameId;
        private Player _player1;
        private Player _player2;
        private Player _winner;
        private char[,] _board;
        private SortedList<int, Movement> _moves;
        #endregion

        public Guid GameId
        {
            get { return _gameId; }
            set { _gameId = value; }
        }
        public Player Player1
        {
            get { return _player1; }
            set { _player1 = value; }
        }
        public Player Player2
        {
            get { return _player2; }
            set { _player2 = value; }
        }
        public Player Winner
        {
            get { return _winner; }
            set { _winner = value; }
        }
        public char[,] Board
        {
            get { return _board; }
            set { _board = value; }
        }
        public SortedList<int, Movement> Moves
        {
            get { return _moves; }
        }

        public event EventHandler<NotificationEventArgs<Movement>> PlayerHasMovedPiece;
        public event EventHandler<NotificationEventArgs<string>> ErrorOccurred;
        public event EventHandler<NotificationEventArgs<Game>> UpdateSummary;
       
        #region Fire
        private void RaisePlayerHasMoved(NotificationEventArgs<Movement> e)
        {
            var handler = PlayerHasMovedPiece;
            if (handler != null)
            {
                PlayerHasMovedPiece(this,e);
            }
        }
        private void RaiseErrorOccurred(NotificationEventArgs<string> e)
        {
            var handler = this.ErrorOccurred;
            if (handler != null)
            {
                handler(this, e);
            }
        }
        private void RaiseUpdateSummary(NotificationEventArgs<Game> e)
        {
            var handler = this.UpdateSummary;
            if (handler != null)
            {
                handler(this, e);
            }
        }
        #endregion Fire

        #region Constructors
        public Game()
            : this(Guid.Empty, new char[Game.Dimension, Game.Dimension], Player.Null, Player.Null, new SortedList<int,Movement>())
        {
        }
        public Game(Player p1, Player p2)
            : this(Guid.NewGuid(), new char[Game.Dimension, Game.Dimension], p1, p2, new SortedList<int,Movement>())
        {
        }
        public Game (Guid gameId,char[,] board,Player p1, Player p2, SortedList<int,Movement> moves)
	    {
            GameId = gameId;
            Board = board;
            Player1 = p1;
            Player2 = p2;
            Winner = null;
            _moves = moves;
	    }
        #endregion

        #region NullObject Pattern
        private static readonly Game _nullGame = new Game();

        public static Game Null
        {
            get { return Game._nullGame; }
        }         
        #endregion

        public bool AddMove(Movement move,string playerId)
        {
            Player replyTo = null;
            Player from = null;
            if (this.Player1.Id != playerId)
            {
                replyTo = this.Player1;
                from = this.Player2;
            }
            else
            {
                replyTo = this.Player2;
                from = this.Player1;
            }

            if (!isValidMove(move, playerId))
            {
                NotificationEventArgs<string> args = new NotificationEventArgs<string>();
                args.Message = "Impossible. Invalid move!";
                RaiseErrorOccurred(args);
                return false;
            }
            else
            {
                move.Player = from;
                Board[move.X, move.Y] = move.Piece;
                _moves.Add(_moves.Count, move);

                RaisePlayerHasMoved(new NotificationEventArgs<Movement>(replyTo.Id, from.Nick  +move.ToString(), move));
                RaiseUpdateSummary(new NotificationEventArgs<Game>(this));
                return true;
            }
        }
        
        private bool IsWon()
        {
            char x = 'x';
            char o = 'o';
            //its turn-based so you can get a 3rd piece to win the earliest at the 5th turn
            if (_moves.Count<=4)
            {
                return false;
            }

            //rows
            for (int i = 0; i < Game.Dimension; i++)
            {
                if (Board[i,0] == x && Board[i,1] == x && Board[i,2] == x)
                {

                }
                if (Board[i, 0] == o && Board[i, 1] == o && Board[i, 2] == o)
                {

                }
            }

            //columns
            for (int j = 0; j < Game.Dimension; j++)
            {
                if(Board[0,j] == x && Board[1, j] == x && Board[2, j] == x)
                {

                }

                if (Board[0, j] == o && Board[1, j] == o && Board[2, j] == o)
                {

                }
            }

            //diagonals
            return false;
        }
        private bool isValidMove(Movement move, string playerId)
        {
            //if (_moves.Count == 0)
            //{
            //    return true;
            //}
            //else
            //{
            //    if (_moves[_moves.Count - 1].Piece == move.Piece)
            //    {
            //        return false;
            //    }
            //    else
            //    {
            //        return true;
            //    }
            //}
            return true;
        }

        [Obsolete]
        private void FillBoard()
        {
            char[] pieces = new char[3]{char.MinValue,'x','o'};
            Random r = new Random();

            for (int i = 0; i < Game.Dimension; i++)
            {
                for (int j = 0; j < Game.Dimension; j++)
                {
                    Board[i, j] = pieces[r.Next(0, Game.Dimension)];
                }
            }
        }

    }
}