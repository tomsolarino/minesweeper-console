using System;
namespace minesweeper_console
{
    //--------------------------------------------------------------------------
    // Author: Tom S
    // Description: Each cell in the minefield
    //--------------------------------------------------------------------------

    /// <summary>
    /// Various cell states
    /// </summary>
    public enum CellState
    {
        Empty = '.',
        M1 = '1',
        M2 = '2',
        M3 = '3',
        M4 = '4',
        M5 = '5',
        M6 = '6',
        M7 = '7',
        M8 = '8',
        Open = 'O',
        Closed = '?',
        Flagged = '!',
        Mine = 'M'
    }

    /// <summary>
    /// Cell Object.
    /// </summary>
    public class Cell
    {
        CellState m_state;
        CellState m_open;
        int[] m_gridPos;

        /// <summary>
        /// Initializes a new instance of the Cell class.
        /// </summary>
        /// <param name="_row">Row.</param>
        /// <param name="_col">Col.</param>
        public Cell(int _row, int _col)
        {
            m_gridPos = new int[] { _row, _col };
            m_state = CellState.Empty;
            m_open = CellState.Closed;
        }

        /// <summary>
        /// Gets the cells grid/field position.
        /// </summary>
        /// <returns>The position.</returns>
        public int[] GetPosition() { return m_gridPos; }

        /// <summary>
        /// Gets the current state of the cell.
        /// </summary>
        /// <returns>The state.</returns>
        public CellState GetState() { return m_state; }

        /// <summary>
        /// Ses the state of the cell.
        /// </summary>
        /// <param name="_state">State.</param>
        public void SeState(CellState _state) { m_state = _state; }

        /// <summary>
        /// Gets the is open bool.
        /// </summary>
        /// <returns><c>true</c>, if cell is open, <c>false</c> otherwise.</returns>
        public bool GetIsOpen() { return m_open == CellState.Open || m_open == CellState.Flagged ? true : false; }

        /// <summary>
        /// Sets the is open bool.
        /// </summary>
        /// <param name="_state">State.</param>
        public void SetIsOpen(CellState _state) { m_open = _state; }

        /// <summary>
        /// Gets the has mine.
        /// </summary>
        /// <returns><c>true</c>, if cell has mine, <c>false</c> otherwise.</returns>
        public bool GetHasMine() { return m_state == CellState.Mine ? true : false; }

        /// <summary>
        /// Returns how the cell should look to the user in the console
        /// </summary>
        /// <returns>The display.</returns>
        public CellState GetDisplay()
        {
            if (m_open == CellState.Open)
                return m_state;
            else if (m_open == CellState.Closed && m_state == CellState.Flagged)
                return m_state;

            return m_open;
        }

        /// <summary>
        /// Increments the nearby mine count or state of the cell.
        /// </summary>
        public void IncrementNearbyMineCount()
        {
            switch (m_state)
            {
                case CellState.Empty:
                    m_state = CellState.M1;
                    break;
                case CellState.M1:
                    m_state = CellState.M2;
                    break;
                case CellState.M2:
                    m_state = CellState.M3;
                    break;
                case CellState.M3:
                    m_state = CellState.M4;
                    break;
                case CellState.M4:
                    m_state = CellState.M5;
                    break;
                case CellState.M5:
                    m_state = CellState.M6;
                    break;
                case CellState.M6:
                    m_state = CellState.M7;
                    break;
                case CellState.M7:
                    m_state = CellState.M8;
                    break;
                default:
                    Console.WriteLine("Error: Cannot increment mine count");
                    break;
            }
        }

        /// <summary>
        /// Sets the color of the console depending on state.
        /// </summary>
        public void SetConsoleColor()
        {
            // Default
            Console.ForegroundColor = ConsoleColor.Black;

            // If the cell is closed just set background to white
            if (m_open == CellState.Closed)
            {
                Console.BackgroundColor = ConsoleColor.White;

                if (m_state == CellState.Flagged)
                    Console.ForegroundColor = ConsoleColor.Red;

                return;
            }

            switch (m_state)
            {
                case CellState.Mine:
                    Console.BackgroundColor = ConsoleColor.Red;
                    return;
                case CellState.Empty:
                    Console.BackgroundColor = ConsoleColor.DarkGray;
                    return;
                case CellState.M1:
                    Console.BackgroundColor = ConsoleColor.Blue;
                    return;
                case CellState.M2:
                    Console.BackgroundColor = ConsoleColor.Green;
                    return;
                case CellState.M3:
                    Console.BackgroundColor = ConsoleColor.Yellow;
                    return;
                case CellState.M4:
                    Console.BackgroundColor = ConsoleColor.Cyan;
                    return;
                case CellState.M5:
                    Console.BackgroundColor = ConsoleColor.DarkGreen;
                    return;
                case CellState.M6:
                    Console.BackgroundColor = ConsoleColor.DarkYellow;
                    return;
                case CellState.M7:
                    Console.BackgroundColor = ConsoleColor.DarkMagenta;
                    return;
                case CellState.M8:
                    Console.BackgroundColor = ConsoleColor.Magenta;
                    return;
                default:
                    Console.BackgroundColor = ConsoleColor.White;
                    break;
            }
        }
    }
}
