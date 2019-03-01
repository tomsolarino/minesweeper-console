using System;
using System.Collections.Generic;

namespace minesweeper_console
{
    //--------------------------------------------------------------------------
    // Author: Tom S
    // Description: The 'AI' if you will for deciding the next cell to open
    //--------------------------------------------------------------------------

    /// <summary>
    /// Metal detector class.
    /// </summary>
    public class MetalDetector
    {
        private Minefield m_minefield;
        private int m_fieldCols;
        private int m_fieldRows;

        /// <summary>
        /// Initializes a new instance of the metal detector class.
        /// </summary>
        /// <param name="_mineField">Mine field.</param>
        public MetalDetector(Minefield _mineField)
        {
            m_minefield = _mineField;
            m_fieldRows = m_minefield.GetField().GetLength(0);
            m_fieldCols = m_minefield.GetField().GetLength(1);
        }

        /// <summary>
        /// Decides the next cell to open without knowing the mine locations
        /// </summary>
        /// <returns>The next cell to open.</returns>
        public uint[] DecideNextCellToOpen()
        {
            // If a mine is found, search for a potentialy empty cell to click
            if (FlagClosedCellsIfPossible())
            {
                for (int row = 0; row < m_fieldRows; row++)
                {
                    for (int col = 0; col < m_fieldCols; col++)
                    {
                        // if the current cell is closed or empty it of no use here
                        if (!m_minefield.GetField()[row, col].GetIsOpen() || m_minefield.GetField()[row, col].GetState() == CellState.Empty)
                            continue;

                        // For this cell, collect all nearby closed and flagged cells
                        List<Cell> closedAndFlagged = new List<Cell>();
                        List<Cell> closedNotFlagged = new List<Cell>();
                        foreach (Cell cell in m_minefield.GetSurroundingCells(row, col))
                        {
                            if (cell.GetIsOpen())
                                continue;
                            if (cell.GetState() == CellState.Flagged)
                                closedAndFlagged.Add(cell);
                            else
                                closedNotFlagged.Add(cell);
                        }

                        // If the amount of flagged cells are greater or = to the surrounding mine value
                        //  it means that any closed cells that are not flagged cannot be hiding a mine
                        int mineCount = (int)Char.GetNumericValue((char)m_minefield.GetField()[row, col].GetState());
                        if (closedAndFlagged.Count >= mineCount && closedNotFlagged.Count > 0)
                        {
                            uint nextRow = (uint)closedNotFlagged[0].GetPosition()[0];
                            uint nextCol = (uint)closedNotFlagged[0].GetPosition()[1];
                            return new uint[] { nextRow, nextCol };
                        }
                    }
                }
                return FindRandomCellToOpen();
            }
            else             {                 return FindRandomCellToOpen();             }
        }

        /// <summary>
        /// Finds the random closed cell to open.
        /// </summary>
        /// <returns>The random cell to open.</returns>
        public uint[] FindRandomCellToOpen()
        {
            bool found = false;
            uint randRow = 0;
            uint randCol = 0;

            while (!found)
            {
                Random rand = new Random();
                randRow = (uint)rand.Next(0, (int)m_minefield.GetRows());
                randCol = (uint)rand.Next(0, (int)m_minefield.GetCols());

                // Checking the random cell is a closed one and not flagged
                if (!m_minefield.GetField()[randRow, randCol].GetIsOpen() &&
                    m_minefield.GetField()[randRow, randCol].GetState() != CellState.Flagged)
                    found = true;
            }
            return new uint[] { randRow, randCol };
        }

        /// <summary>
        /// Flags the closed cells if possible.
        /// </summary>
        /// <returns><c>true</c>, if closed cells were flaged, <c>false</c> otherwise.</returns>
        private bool FlagClosedCellsIfPossible()
        {
            bool flaggedCell = false;
            // Loop through every cell and see if any closed ones can be flagged with a mine
            for (int row = 0; row < m_fieldRows; row++)
            {
                for (int col = 0; col < m_fieldCols; col++)
                {
                    // If the current cell is closed or empty it can't be near a 
                    // 'clickable/potential' cell
                    if (!m_minefield.GetField()[row, col].GetIsOpen() || m_minefield.GetField()[row, col].GetState() == CellState.Empty)
                        continue;

                    // For this cell, collect all nearby closed cells
                    List<Cell> surroundingClosed = new List<Cell>();
                    foreach (Cell cell in m_minefield.GetSurroundingCells(row, col))
                        if (!cell.GetIsOpen())
                            surroundingClosed.Add(cell);

                    // If the neaby mine count matches the amount of closed 
                    //  cells around a particular cell, the closed cells must be mines
                    //  so they are flagged
                    int mineCount = (int)Char.GetNumericValue((char)m_minefield.GetField()[row, col].GetState());
                    if (surroundingClosed != default(List<Cell>) && mineCount == surroundingClosed.Count)
                    {
                        foreach (Cell c in surroundingClosed)
                        {
                            c.SeState(CellState.Flagged);
                            flaggedCell = true;
                        }
                    }
                }
            }
            return flaggedCell;
        }
    }
}
