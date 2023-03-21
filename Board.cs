public interface IDeepCloneable<TSelf>
{
    public TSelf DeepClone();
}

class Board : IDeepCloneable<Board>
{

    //cell struct and flag enum
    public struct Cell
    {
        public int Value; // value 1-9, 0 indicates an empty cell
        public List<int> PossibleValues; // an array of all currently known possible values for this cell
        public Flag Status;
        public int Sector;
        public List<(int X, int Y)> Affects;

        public Cell(int value, int xcoord, int ycoord /*only here to calculate sector*/)
        {
            Value = value;
            PossibleValues = new List<int>() {1, 2, 3, 4, 5, 6, 7, 8, 9};
    
            xcoord = (int)Math.Ceiling((double)(xcoord / 3));
            ycoord = (int)Math.Ceiling((double)(ycoord / 3));
            Sector = xcoord + 3 * (ycoord - 1); // algorithm to get the sector # given sector coordinates
            
            Affects = new List<(int, int)>();
            Status = Flag.Empty;
        }
    }

    public enum Flag
    {
        Empty, // for cells which are empty and have not yet been touched by the algorithm
        Preplaced, //for cells which have a value that was written in the starting conditions
        Pruned, // for cells which have a value that was not written in by the user
        Guessed // for cells which are in progress being solved
    }

    //field
    public Cell[,] Grid;


    //constants
    private const string TOP = " ___1___2___3___ ___4___5___6___ ___7___8___9___\n|               |               |               |";
    private const string HORIZ_BARS_MAJOR = "\n   |_______________|_______________|_______________|\n   |               |               |               |";
    private const string HORIZ_BARS_MINOR = "\n   |   —   —   —   |   —   —   —   |   —   —   —   |";
    private const string BOTTOM = "\n   |_______________|_______________|_______________|";
    

    //constructors

    public Board(Cell[,] grid)
    {
        Grid = grid;
    }
    /*
    //AI code I wanted to see how well it can write code
    public Board(Board boardToClone)
    {
        Grid = new Cell[9, 9];

        for (int i = 0; i < 9; i++)
        {
            for (int j = 0; j < 9; j++)
            {
                Grid[i, j] = new Cell(boardToClone.Grid[i, j].Value, i, j);

                Grid[i, j].PossibleValues = new List<int>();
                foreach (int possibleValue in boardToClone.Grid[i, j].PossibleValues)
                {
                    Grid[i, j].PossibleValues.Add(possibleValue);
                }

                Grid[i, j].Status = boardToClone.Grid[i, j].Status;
                Grid[i, j].Sector = boardToClone.Grid[i, j].Sector;

                Grid[i, j].Affects = new List<(int, int)>();
                foreach ((int x, int y) in boardToClone.Grid[i, j].Affects)
                {
                    Grid[i, j].Affects.Add((x, y));
                }
            }
        }
    }
    */


    public Board()
    {
        Grid = new Cell[9,9];
        
        for ( int i = 0; i < 9; i++)  //fill the 2d array with cells
        {
            for ( int j = 0; j < 9; j++)
            {
                Grid[i,j] = new Cell(0, i, j);
            }
        }


        for (int x1 = 0; x1 < 9; x1++)
        {
            for (int y1 = 0; y1 < 9; y1++)
            {
                ////////
                for (int x2 = 0; x2 < 9; x2++)
                {
                    for (int y2 = 0; y2 < 9; y2++)
                    {
                        if ((Grid[x2, y2].Sector == Grid[x1,y1].Sector && x2 != x1 && y2 != y1) ^ (x2 == x1) ^ (y2 == y1))
                        { //TRUE only if the given cell is in the same sector but not the same row or column OR the same row OR the same column
                            Grid[x1,y1].Affects.Add((x2, y2));
                        }  
                    }
                }
                ////////
            }
        }
  
    }

    public Board DeepClone()
    {
        Cell[,] Grid = new Cell[9, 9];

        for ( int y = 0; y < 9; y++ )
        {
            for ( int x = 0; x < 9; x++ )
            {
                Grid[x, y] = new Cell(this.Grid[x, y].Value, x, y);

                Grid[x, y].PossibleValues = new List<int>();
                foreach ( int PVal in this.Grid[x, y].PossibleValues )
                {
                    Grid[x, y].PossibleValues.Add(PVal);
                }

                foreach ( (int ax, int ay) in this.Grid[x, y].Affects )
                {
                    Grid[x, y].Affects.Add((ax, ay));
                }

                Grid[x, y].Status = this.Grid[x, y].Status;
            }
        }

        return new Board(Grid);
    }

    ////////////////////////////////////////////////////////////////////////////////////////////
    //code


    public void PrintBoard()
    {
        Dictionary<Board.Flag, ConsoleColor> FlagLookup = new Dictionary<Board.Flag, ConsoleColor>()
        {
            [Board.Flag.Empty] = ConsoleColor.White,
            [Board.Flag.Preplaced] = ConsoleColor.Blue,
            [Board.Flag.Pruned] = ConsoleColor.Green,
            [Board.Flag.Guessed] = ConsoleColor.Red
        };
        
        Console.Write("    ");
        for ( int i = 1; i <= 9; i++ )  //write the top beam in with colored numbers
        {
            Console.Write("___");
            Program.ColorWrite($"{i}", ConsoleColor.Yellow);
            if(i == 3 || i == 6)
            {
                Console.Write("___ ");
            }
        }
        Console.Write("___\n   |               |               |               |\n");

        for ( int y = 1; y <= 9; y++ ) //increments y value
        {
            Program.ColorWrite($"   {y}  ", ConsoleColor.Yellow);
            for ( int x = 1; x <= 9; x++ ) //increments x value
            {
                if(this.Grid[x-1, y-1].Value == 0)
                {
                    Program.ColorWrite(" + ", FlagLookup[this.Grid[x-1, y-1].Status]); // print a `+` to indicate an empty cell, coloring it according to its this.Grid.Flag
                } else
                {
                    Program.ColorWrite($" {this.Grid[x-1, y-1].Value} ", FlagLookup[this.Grid[x-1, y-1].Status]); // print the cell's current value, coloring it according to its Board.Flag
                }

                if (x % 3 == 0)
                {
                    Console.Write("  |  ");
                    // If the current x value is a multple of 3, or in other words, is 3, 6, or 9,
                    // give it an extra space to indicate the sector border.
                } else 
                {
                    Console.Write("|");
                    // Otherwise, give it no extra space to indicate the cell border.
                }   
            }  

            ///////

            if ( y % 3 == 0 && y != 9)
            {
                Console.WriteLine(HORIZ_BARS_MAJOR); // print the big sector borders @ 3 and 6
            } else if ( y == 9)
            {
                Console.WriteLine(BOTTOM); // print the big sector border minus the extra bit at the bottom @ 9
            }else
            {
                Console.WriteLine(HORIZ_BARS_MINOR); // print the little cell borders everywhere else
            }

        }
    }

    public void TakeInput()
    {
        while (true)
        {
            Console.Clear();
            Program.ColorWrite("     Please input the starting conditions...\n", ConsoleColor.Green);
            Console.WriteLine(Program.INSTRUCTION_SCHPIEL);

            PrintBoard();

            Console.Write("\n\n\n\n > ");
            string[] commands = (Console.ReadLine() ?? "").Split('/');
            if( commands[0] == "q" || commands[0] == "" )
            {
                break;
            }

            foreach( string cmd in commands )
            {
                try {
                    int x = Convert.ToInt32(cmd[0].ToString()); Console.WriteLine(x);
                    int y = Convert.ToInt32(cmd[1].ToString()); Console.WriteLine(y);
                    int v = Convert.ToInt32(cmd[2].ToString()); Console.WriteLine(v);

                    this.Grid[x-1, y-1].Value = v; // make the cell at the specified coordinate have the specified value
                    this.Grid[x-1, y-1].Status = Flag.Preplaced;
                    if(v == 0) { this.Grid[x-1,y-1].Status = Flag.Empty;}
                } catch (Exception e) {
                    Console.WriteLine($"Exception: {e.Message}");
                    Console.WriteLine("\nIncorrect command format. Try again.");
                    Environment.Exit(0);
                }
                    
                
            }
        }
    }

    public bool IsDone()
    {
        foreach (Cell cell in this.Grid)
        {
            if (cell.Value == 0)
            {
                return false;
            }
        }

        return true;
    }

    public bool Prune()
    { // prune the board oh yeah
        Stack<(int x, int y)> queue = new Stack<(int x, int y)>();

        //start the procedure

        //add to the queue
        for ( int x = 0; x < 9; x++ )
        {
            for ( int y = 0; y < 9; y++ )
            {
                queue.Push((x, y));
            }
        }

        while ( queue.Count > 0 )
        {
            var input = queue.Pop();
            int x = input.x; int y = input.y;
            this.Grid[x, y].PossibleValues = new List<int>() {1, 2, 3, 4, 5, 6, 7, 8, 9};

            if( this.Grid[x, y].Value != 0)
            {
                continue; //cancel the evaluation if the given cell already has a value
            }

            Program.ColorWrite($"Evaluating cell ({x+1}, {y+1})...    ", ConsoleColor.Blue);

            foreach ( (int x, int y) compareCell in this.Grid[x, y].Affects ) // pick a new compared cell from the list of cells that the checking cell affects
            {
                int cmpCellVal = this.Grid[compareCell.x, compareCell.y].Value; // set cmpCellVal to the value of the compared cell

                if ( cmpCellVal != 0 && this.Grid[x, y].PossibleValues.Contains(cmpCellVal) ) // if the given cell both has a value that is not zero and has a value that is currently in the checking cell's possible values list, then...
                {
                    Console.Write($"[({compareCell.x+1}, {compareCell.y+1}) --> {cmpCellVal}]   ");
                    this.Grid[x, y].PossibleValues.Remove( cmpCellVal ); // remove the compared cell's value from the list of possible values for the checking cell
                } // if statement #1
            } Console.WriteLine("");
            
            if ( this.Grid[x, y].PossibleValues.Count == 1 ) // if there is only one possible value left, then
            {
                this.Grid[x, y].Value = this.Grid[x, y].PossibleValues[0]; // set the value of the cell to that value
                this.Grid[x, y].Status = Flag.Pruned;

                Program.ColorWrite($"Value found for the current cell!! ({this.Grid[x, y].PossibleValues[0]})\n\n", ConsoleColor.Green);
                foreach ( (int x, int y) cell in this.Grid[x, y].Affects ) // then, take every cell that this one affects...
                {
                    if ( this.Grid[cell.x, cell.y].Value == 0 ) // check if it has a value of zero...
                    {
                        queue.Push(cell); // and if so, add it to the queue
                    } // if statement #3
                }  // foreach

            } else
            if (this.Grid[x, y].PossibleValues.Count == 0) // if this cell has no possible values
            {
                Console.WriteLine($"\n\nNODE FAILED WITH CELL ({x+1}, {y+1})");
                return false; // this node failed
            }
        } // while loop

        return true; // no errors were found

    } // Prune()
} // class