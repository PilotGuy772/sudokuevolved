/*
using System.Threading;
using System;
using System.Diagnostics;

class Solver_old
{
    static Stack<(int X, int Y)> Queue = new Stack<(int X, int Y)>();
    
    public static void StartSolving() //start solving the sudoku
    {
        Console.Clear();
        Stopwatch SW = new Stopwatch();
        SW.Start();

        for (int x = 0; x < 9; x++) //add all cells to the stack
        {
            for (int y = 0; y < 9; y++)
            {
                Queue.Push((x, y));
            }
        }
        ProcessStack(true);

        

        SW.Stop();
        TimeSpan ts = SW.Elapsed;
        Program.ColorWrite("\nSolving complete!\n\n", ConsoleColor.Green);
        Console.WriteLine($"Time: {ts.Milliseconds}ms");
        return;
    }

    
    
    private static void EvaluateCellWithoutAdding((int x, int y) input) // go through the full evaluation process for a single cell
    {
        int x = input.x; int y = input.y; // assign the values in the input tuple to new local variables for convenience

        if( Program.Board[x, y].Value != 0)
        {
            return; //cancel the evaluation if the given cell already has a value
        }

        Program.ColorWrite($"Evaluating cell ({x + 1}, {y + 1}) (first pass)...\n\n", ConsoleColor.Blue);

        foreach ( (int x, int y) compareCell in Program.Board[x, y].Affects ) // pick a new compared cell from the list of cells that the checking cell affects
        {
            int cmpCellVal = Program.Board[compareCell.x, compareCell.y].Value; // set cmpCellVal to the value of the compared cell
            

            if ( cmpCellVal != 0 && Program.Board[x, y].PossibleValues.Contains(cmpCellVal) ) // if the given cell both has a value that is not zero and has a value that is currently in the checking cell's possible values list, then...
            {
                Console.WriteLine($"Conflicting cell found at ({compareCell.x + 1}, {compareCell.y + 1}) with value {cmpCellVal}");
                Program.Board[x, y].PossibleValues.Remove( cmpCellVal ); // remove the compared cell's value from the list of possible values for the checking cell

                if ( Program.Board[x, y].PossibleValues.Count == 1 ) // if there is only one possible value left, then
                {
                    Program.Board[x, y].Value = Program.Board[x, y].PossibleValues[0]; // set the value of the cell to that value
                    Program.Board[x, y].Status = Program.Flag.Solved;
                    Program.ColorWrite($"Value found for the current cell!! ({Program.Board[x, y].PossibleValues[0]})\n\n", ConsoleColor.Green);

                    //IsImpossible = false; //the sudoku is now possibly possible

                    return; //all done
                }
            }
        }
    }

    private static void EvaluateCell((int x, int y) input) // go through the full evaluation process for a single cell with adding affected cells to the queue
    {

        int x = input.x; int y = input.y; // assign the values in the input tuple to new local variables for convenience

        if( Program.Board[x, y].Value != 0)
        {
            return; //cancel the evaluation if the given cell already has a value
        }

        Program.ColorWrite($"Evaluating cell ({x+1}, {y+1})...\n\n", ConsoleColor.Blue);

        foreach ( (int x, int y) compareCell in Program.Board[x, y].Affects ) // pick a new compared cell from the list of cells that the checking cell affects
        {
            int cmpCellVal = Program.Board[compareCell.x, compareCell.y].Value; // set cmpCellVal to the value of the compared cell

            if ( cmpCellVal != 0 && Program.Board[x, y].PossibleValues.Contains(cmpCellVal) ) // if the given cell both has a value that is not zero and has a value that is currently in the checking cell's possible values list, then...
            {
                Console.WriteLine($"Conflicting cell found at ({compareCell.x+1}, {compareCell.y+1}) with value {cmpCellVal}");
                Program.Board[x, y].PossibleValues.Remove( cmpCellVal ); // remove the compared cell's value from the list of possible values for the checking cell

                if ( Program.Board[x, y].PossibleValues.Count == 1 ) // if there is only one possible value left, then
                {
                    Program.Board[x, y].Value = Program.Board[x, y].PossibleValues[0]; // set the value of the cell to that value
                    Program.Board[x, y].Status = Program.Flag.Solved;

                    Program.ColorWrite($"Value found for the current cell!! ({Program.Board[x, y].PossibleValues[0]})\n\n", ConsoleColor.Green);
                    foreach ( (int x, int y) cell in Program.Board[x, y].Affects ) // then, take every cell that this one affects...
                    {
                        if ( Program.Board[cell.x, cell.y].Value == 0 ) // check if it has a value of zero...
                        {
                            Queue.Push(cell); // and if so, add it to the queue
                        }
                    }  

                    return; //all done
                }
            }
        }
    }

    private static void ProcessStack(bool doAdd)
    {
        //go through everything in the stack 
        while (Queue.Count > 0)
        {     
            EvaluateCell(Queue.Pop());
        }
    }
}
*/