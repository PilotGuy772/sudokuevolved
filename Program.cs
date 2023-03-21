using System.Diagnostics;

public class Program
{
    
    

    // CONSTANTS
    public const string INSTRUCTION_SCHPIEL = "Commands take the format of \"(x-coord)(y-coord)(value).\"\n  e.g. \"257\"\n\nCommands may be chained as many times as you like.\n  e.g. \"537/423/826/231/839.\" Never start or end with a slash.\n\nTo finish, send the letter \"q\"\n\n";
    private static Board? FinishedBoard;
    private static int NodeCounter = 0;
    

    

    // VARIABLES

    //////////
    public static void Main(String[] args)
    {
        Board masterBoard = new Board();

        masterBoard.TakeInput();
        Console.Clear();
        Console.WriteLine("This is your completed Sudoku board. Press any key to send it to the solving algorithm.\n\n");
        masterBoard.PrintBoard();
        
        Interrupt(" ");
        //start the process
        Stopwatch SW = new Stopwatch();
        SW.Start();
        bool success = Solve(masterBoard, 0, 0, (0, 0, 0));
        SW.Stop();
        TimeSpan time = SW.Elapsed;

        ColorWrite($"\n\n\n ALGORITHM COMPLETE!\n\n", ConsoleColor.Green);

        if ( success )
        {
            Console.WriteLine($"The algorithm was able to solve your sudoku.\nTime: {time.TotalMilliseconds}ms\nNodes: {NodeCounter}");
        } else
        {
            ColorWrite($"The algorithm was NOT able to solve your sudoku.\nThis means you either provided an invalid puzzle or there was an error with the algorithm.\nTime: {time.TotalMilliseconds}ms\n\n\n", ConsoleColor.Red);
        }

        (FinishedBoard ?? new Board()).PrintBoard();
        Interrupt(" ");
    }


    private static bool Solve(Board board, int xstart, int ystart, (int x, int y, int val) whatToChange)
    {
        NodeCounter++;


        if ( whatToChange.val != 0)
        {
            board.Grid[whatToChange.x, whatToChange.y].Value = whatToChange.val;
            board.Grid[whatToChange.x, whatToChange.y].Status = Board.Flag.Guessed;

            
            //board.PrintBoard();
            //Interrupt("A value was just changed.");            
        }
            
        if ( board.IsDone() )
        {
            FinishedBoard = board;
            return true;
        }
        
        if ( !board.Prune() ) // if the pruning algorithm decides that this iteration is impossible
        {
            ColorWrite("\n\nNODE FAILED\n\n", ConsoleColor.Red);
            FinishedBoard = board;
            return false;
        }
        //board.PrintBoard();
        //Interrupt("Pruning just finished with no problems.");

        if ( board.IsDone() )
        {
            FinishedBoard = board;
            return true;
        }
        
        ColorWrite("\n\n\nPRUNING COMPLETED - BOARD IS NOT DONE\n\n\n", ConsoleColor.Green);
        
        for ( int y = ystart; y < 9; y++ )
        {
            for ( int x = xstart; x < 9; x++ ) // iterate through every cell
            {
                if ( board.Grid[x,y].Value == 0 ) // if that cell does not have a value, then
                {
                    foreach ( int val in board.Grid[x, y].PossibleValues ) // comb through its list of possible values
                    {                     
                       ColorWrite($"\n\nNEW NODE CREATED\nWith value {val} at cell ({x+1}, {y+1})\n\n", ConsoleColor.Green);
                       if ( Solve(board.DeepClone(), x, y, (x, y, val)) )  // and try the whole thing again
                       {
                            return true; // if anything down the chain from here completed it, then follow it up the chain
                       }else
                       {
                            //board.PrintBoard();
                            //Interrupt("A node just failed.");
                       }
                    }
                    // every cell is tried and none of the nodes below it are found to have any possible solutions, so the iteration is impossible? as in, no children nodes have solutions
                    ColorWrite("\n\nThis node is unviable\n\n", ConsoleColor.Yellow);
                    FinishedBoard = board;
                    return false;
                    // therefore if it makes it through the entire foreach, there are no possible solutions branching from this node and therefore the node is unviable     
                }
            }

            xstart = 0; //change xstart to zero to make sure that the x cursor gets through the whole thing
        }
        
        ColorWrite("\n\nNODE FAILED (this should be illegal but I have to put it here)\n\n", ConsoleColor.Red);
        return false;       

    }

    public static void ColorWrite(string text, ConsoleColor color)
    {
        var oldcolor = Console.ForegroundColor;
        Console.ForegroundColor = color;
        Console.Write(text);
        Console.ForegroundColor = oldcolor;
    }

    public static void Interrupt(string message = "The process has been interrupted for debugging")
    {
        //interrupts the stream for debugging
        Console.WriteLine(message);
        ConsoleKey key = Console.ReadKey().Key;

        if (key == ConsoleKey.Escape)
        {
            Environment.Exit(0);
        }    
    }
}