using System;
using System.Collections.Generic;

namespace minesweeper_console
{
    //--------------------------------------------------------------------------
    // Author: Tom S
    // Description: The minefield itself
    //--------------------------------------------------------------------------

    /// <summary>
    /// Mine field.
    /// </summary>
    public class Minefield
    {
        // Member Vars
        private Application m_app;
        private uint m_fieldCols;
        private uint m_fieldRows;
        private uint m_mineCount;
        private Cell[,] m_minefield;

        /// <summary>
        /// Initializes a new instance of the MineField class.
        /// </summary>
        /// <param name="_app">App.</param>
        /// <param name="_fieldWidth">Field width.</param>
        /// <param name="_fieldHeight">Field height.</param>
        /// <param name="_mineCount">Mine count.</param>
        /// <param name="_seed">Seed.</param>
        public Minefield(Application _app, uint _fieldWidth, uint _fieldHeight, uint _mineCount, int _seed = default(int))
        {
            m_app = _app;
            GenerateMinefield(_fieldWidth, _fieldHeight, _mineCount, _seed);
        }

        /// <summary>
        /// Returns the minefield array
        /// </summary>
        /// <returns>The field.</returns>
        public Cell[,] GetField() { return m_minefield; }

        /// <summary>
        /// Gets the minefield rows.
        /// </summary>
        /// <returns>The rows.</returns>
        public uint GetRows() { return m_fieldRows; }

        /// <summary>
        /// Gets the minefield cols.
        /// </summary>
        /// <returns>The cols.</returns>
        public uint GetCols() { return m_fieldCols; }

        /// <summary>
        /// Generates a new mine field.
        /// </summary>
        /// <param name="_fieldWidth">Field width.</param>
        /// <param name="_fieldHeight">Field height.</param>
        /// <param name="_mineCount">Mine count.</param>
        /// <param name="_seed">Seed.</param>
        private void GenerateMinefield(uint _fieldWidth, uint _fieldHeight, uint _mineCount, int _seed = default(int))
        {
            m_fieldCols = _fieldWidth;
            m_fieldRows = _fieldHeight;
            m_mineCount = _mineCount;

            // Check there are less mines than cells
            if (m_mineCount >= m_fieldCols * m_fieldRows)
                Console.WriteLine("Error: You can't have more mines than tiles available");

            // Initialising a new empty minefield
            m_minefield = new Cell[m_fieldRows, m_fieldCols];
            for (int row = 0; row < m_fieldRows; row++)
                for (int col = 0; col < m_fieldCols; col++)
                    m_minefield[row, col] = new Cell(row, col);

            // Place mines randomly using a seed if one was given
            Random rand = _seed != default(int) ? new Random(_seed) : new Random();
            for (int placed = 0; placed < m_mineCount;)
            {
                int col = rand.Next(0, (int)m_fieldCols);
                int row = rand.Next(0, (int)m_fieldRows);

                // If this cell already has a mine
                if (m_minefield[row, col].GetHasMine())
                    continue;

                // If there isnt already a mine here add one to the field
                m_minefield[row, col].SeState(CellState.Mine);

                // Update surrounding cells
                foreach (Cell cell in GetSurroundingCells(row, col))
                {
                    if (!cell.GetHasMine())
                        cell.IncrementNearbyMineCount();
                }
                placed++;
            }
        }

        /// <summary>
        /// Gets the surrounding cells of a specific cell.
        /// </summary>
        /// <returns>The surrounding cells.</returns>
        /// <param name="_row">Row.</param>
        /// <param name="_col">Col.</param>
        public List<Cell> GetSurroundingCells(int _row, int _col)
        {
            List<Cell> surrounding = new List<Cell>();

            //Top Left
            if (_row - 1 >= 0 && _col - 1 >= 0)
                surrounding.Add(m_minefield[_row - 1, _col - 1]);

            //Top Middle
            if (_row - 1 >= 0)
                surrounding.Add(m_minefield[_row - 1, _col]);

            //Top Right
            if (_row - 1 >= 0 && _col + 1 < m_fieldCols)
                surrounding.Add(m_minefield[_row - 1, _col + 1]);

            //Middle Left
            if (_col - 1 >= 0)
                surrounding.Add(m_minefield[_row, _col - 1]);

            //Middle Right
            if (_col + 1 < m_fieldCols)
                surrounding.Add(m_minefield[_row, _col + 1]);

            //Bottom Left
            if (_row + 1 < m_fieldRows && _col - 1 >= 0)
                surrounding.Add(m_minefield[_row + 1, _col - 1]);

            //Bottom Middle
            if (_row + 1 < m_fieldRows)
                surrounding.Add(m_minefield[_row + 1, _col]);

            //Bottom Right
            if (_row + 1 < m_fieldRows && _col + 1 < m_fieldCols)
                surrounding.Add(m_minefield[_row + 1, _col + 1]);

            return surrounding;
        }

        /// <summary>
        /// Opens all connected empty cells, after opening a cell
        /// </summary>
        /// <param name="_surrounding">Surrounding.</param>
        private void RevealNearbyCells(List<Cell> _surrounding)
        {
            foreach (Cell cell in _surrounding)
            {
                if (!cell.GetIsOpen())
                {
                    cell.SetIsOpen(CellState.Open);
                    if (cell.GetState() == CellState.Empty)
                        RevealNearbyCells(GetSurroundingCells(cell.GetPosition()[0], cell.GetPosition()[1]));
                }
            }
        }

        /// <summary>
        /// Games over.
        /// </summary>
        private void GameOver()
        {
            // Reveal all cells
            for (int row = 0; row < m_fieldRows; row++)
                for (int col = 0; col < m_fieldCols; col++)
                    m_minefield[row, col].SetIsOpen(CellState.Open);
            m_app.SetGameState(GameState.GameOver);
        }

        /// <summary>
        /// Checks if there are anymore cells to open or if the game has been won
        /// </summary>
        /// <returns><c>true</c>, if no more cells can be opened, <c>false</c> otherwise.</returns>
        private bool CheckHasWon()
        {
            // Count closed cells
            uint closedCellsCount = 0;
            for (int row = 0; row < m_fieldRows; row++)
                for (int col = 0; col < m_fieldCols; col++)
                    if (!m_minefield[row, col].GetIsOpen())
                        closedCellsCount++;
            // Are there anymore cells without mines
            if (closedCellsCount <= m_mineCount)
            {
                m_app.SetGameState(GameState.GameWin);
                return true;
            }
            return false;
        }

        /// <summary>
        /// Opens a specific cell.
        /// </summary>
        /// <param name="_row">Row.</param>
        /// <param name="_col">Col.</param>
        public void OpenCell(uint _row, uint _col)
        {
            Cell cell = m_minefield[_row, _col];

            // If a mine was opened
            if (cell.GetHasMine())
            {
                GameOver();
                return;
            }

            cell.SetIsOpen(CellState.Open);
            if (cell.GetState() == CellState.Empty)
                RevealNearbyCells(GetSurroundingCells((int)_row, (int)_col));

            if (CheckHasWon())
                return;
        }

        /// <summary>
        /// Displays the minefield in the console
        /// </summary>
        public void Draw()
        {
            for (int row = 0; row < m_fieldRows; row++)
            {
                for (int col = 0; col < m_fieldCols; col++)
                {
                    m_minefield[row, col].SetConsoleColor();
                    Console.Write(" {0} ", (char)m_minefield[row, col].GetDisplay());
                    Console.ResetColor();
                }
                Console.Write(Environment.NewLine);
            }
        }
    }
}
