using System;

namespace minesweeper_console
{
    //--------------------------------------------------------------------------
    // Author: Tom S
    // Description: The main game play loop
    //--------------------------------------------------------------------------

    /// <summary>
    /// Entry Point
    /// </summary>
    class MainClass
    {
        public static void Main(string[] args)
        {
            Application app = new Application();
            app.Run();
        }
    }

    /// <summary>
    /// Various Game state.
    /// </summary>
    public enum GameState
    {
        Playing,
        GameOver,
        GameWin
    }

    /// <summary>
    /// Application.
    /// </summary>
    public class Application
    {
        private Minefield m_minefield;
        private MetalDetector m_metalDetector;
        private GameState m_gameState;
        private uint m_fieldWidth = 20;
        private uint m_fieldHeight = 20;
        private uint m_mineCount = 50;
        private int m_seed = 100;

        /// <summary>
        /// Initializes a new instance of the Application class.
        /// </summary>
        public Application()
        {
            m_gameState = GameState.Playing;

            m_minefield = new Minefield(this, m_fieldWidth, m_fieldHeight, m_mineCount);
            //m_mineField = new MineField(m_fieldWidth, m_fieldHeight, m_mineCount, m_seed);

            m_metalDetector = new MetalDetector(m_minefield);

            Console.Write("Begin!\n");
            m_minefield.Draw();
        }

        /// <summary>
        /// Main gameplay loop
        /// </summary>
        public void Run()
        {
            // initial click is random
            Random rand = new Random();
            uint randRow = (uint)rand.Next(0, (int)m_minefield.GetRows());
            uint randCol = (uint)rand.Next(0, (int)m_minefield.GetCols());
            uint[] nextCellToOpen = new uint[] { randRow, randCol };

            // main gameplay loop
            while (m_gameState == GameState.Playing)
            {
                // Prompt user
                Console.Write("Press Enter for next step!\n");
                // Wait for enter to proceed
                Console.ReadKey();
                // Display the cell opened next
                Console.Write("Opened Row: " + (int)nextCellToOpen[0] + ", Col: " + (int)nextCellToOpen[1] + '\n');
                // Open the cell if failed, game over
                m_minefield.OpenCell(nextCellToOpen[0], nextCellToOpen[1]);
                // Figure out the next cell to open if the game isn't over
                if (m_gameState == GameState.Playing)
                    nextCellToOpen = m_metalDetector.DecideNextCellToOpen();
                // Display updated minefield
                m_minefield.Draw();
            }

            // End game
            switch (m_gameState)
            {
                case GameState.GameOver:
                    Console.WriteLine("Game Over");
                    break;
                case GameState.GameWin:
                    Console.WriteLine("You Win!");
                    break;
                default:
                    Console.WriteLine("Something went wrong...");
                    break;
            }
        }

        /// <summary>
        /// Sets the state of the game.
        /// </summary>
        /// <param name="_state">State.</param>
        public void SetGameState(GameState _state) { m_gameState = _state; }
    }
}
