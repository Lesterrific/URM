using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace CMSC141_MP1 {

    class Program {
        static void Main(string[] args) {

            /* creating a URM program (initial state + instructions) from an external file */
            URMProgram URMP = new URMProgram(Path.Combine(Directory.GetCurrentDirectory(), "mp1.in"));
            URM URMachine = new URM();
            //Console.WriteLine("\n-------------------------------------");

            /* starts executing the program loaded from an external file*/
            URMachine.Execute(URMP);

            /* writing the state history to an external file */
            URMachine.WriteStateHistoryToFile(Path.Combine(Directory.GetCurrentDirectory(), "mp1.out"));
        }
    }

    public class URM {

        /* the program that will be executed by the URM */
        URMProgram Program;

        /* a number that keeps track of the line number to be executed next by the URM; 
        normally increments by 1 after each instruction */
        int InstructionPointer;

        /* a list that contains the state of the URM's registers after each instruction */
        List<string> StateHistory;

        public URM() {
            StateHistory = new List<string>();
        }

        /* resets the URM to prepare it for re-execution/program */
        public void Reset() {
            StateHistory.Clear();
            InstructionPointer = 0;
        }

        public void Execute(URMProgram P) {

            //Console.WriteLine("\nRunning the program...");
            Reset();
            Program = P;
            /* prints the initial state of the URM */
            PrintState();

            /* loops through all the instructions of the URM Program */
            while(InstructionPointer < Program.Instructions.Length) {
                if(InstructionPointer > Program.Instructions.Length) {
                    /* 
                        this is necessary for JUMP instructions that points to a line number beyond the number of instructions
                        - which means to TERMINATE the program.
                    */
                    break;
                }
                ExecuteInstruction(Program.Instructions[InstructionPointer]);
            }
            //Console.WriteLine("END OF PROGRAM.");
        }

        void ExecuteInstruction(string Instruction) {
            /* parsing the instruction */
            bool IJump = false;
            /* splits the instruction into it's pieces separated by the space character */
            string[] ParsedInstruction = Instruction.Split(' ');
            if (ParsedInstruction[0] == "J") {
                int comp1 = Program.State[int.Parse(ParsedInstruction[1])];
                int comp2 = Program.State[int.Parse(ParsedInstruction[2])];
                int jump = int.Parse(ParsedInstruction[3]) - 1;
                if (comp1 == comp2) {
                    /* if the JUMP condition is true, set the instruction pointer to the JUMP line number */
                    InstructionPointer = jump;
                    /* setting this to TRUE means that the Instruction pointer will no be incremented as it's set to a particular line number*/
                    IJump = true;
                }
            }
            else if(ParsedInstruction[0] == "S") {
                Program.State[int.Parse(ParsedInstruction[1])]++;
            }
            else if(ParsedInstruction[0] == "Z") {
                Program.State[int.Parse(ParsedInstruction[1])] = 0;
            }
            else if(ParsedInstruction[0] == "C") {
                Program.State[int.Parse(ParsedInstruction[2])] = Program.State[int.Parse(ParsedInstruction[1])];
            }
            /* incrementing the Instruction pointer by 1 if there is no JUMP */
            if(IJump == false) {
                InstructionPointer++;
            }
            /* print the state of the URM after the execution of the instruction */
            PrintState();
        }

        void PrintState() {
            string CurrentState = "";
            foreach(var Register in Program.State) {
                CurrentState = CurrentState + Register + " ";
            }
            Console.WriteLine(CurrentState);
            /* as the state is printed, you add the state (represented in a string) to the State History list*/
            StateHistory.Add(CurrentState);
        }

        public void WriteStateHistoryToFile(string OutputFilePath) {
            try {
                File.AppendAllLines(@OutputFilePath, StateHistory);
            }
            catch (FileNotFoundException e) {
                Console.WriteLine("Error. Output File not found. \n" + e.Message);
            }
        }
    }

    public class URMProgram {

        /* the instructions of the program read from the file */
        public string[] Instructions;
        /* the initial state of the URM registers */
        public int[] State;

        public URMProgram(String FilePath) {
            this.CreateFromFile(FilePath);
        }
  
        public void CreateFromFile(string FilePath) {
            
            //Console.WriteLine("Creating URM Program from file...");
            string[] FileContent = {}; 
            try {
                FileContent = File.ReadAllLines(@FilePath);
            }
            catch (FileNotFoundException e) {
                //Console.WriteLine("-------------------------------------");
                Console.WriteLine("An error occured while reading the file. Please see error message below...");
                Console.WriteLine(e.Message);
                //Console.WriteLine("Program initialization failed.");
                return;
            }
            //Console.WriteLine("-------------------------------------");
            //Console.WriteLine("File successfully read...");

            /* reading of file is sucessful... * /
            /* initializing the state */
            string[] RawState = FileContent[0].Split(' ');
            List<int> TempL = new List<int>();
            foreach(var Item in RawState) {
                TempL.Add(int.Parse(Item));
            }
            State = TempL.ToArray();
            
            /* Initializing the instructions */
            Instructions = FileContent.Skip(1).ToArray();

            //Console.WriteLine("-------------------------------------");
            //Console.WriteLine("Program sucessfully created from file.");
            //Console.WriteLine("-------------------------------------");
            //Console.WriteLine("PROGRAM INFO: ");
            //Console.Write("Initial State: ");
            /*foreach (var Register in State) {
                Console.Write(Register + " ");
            }*/
            //Console.WriteLine();
            //Console.WriteLine("No. of Instructions: " + Instructions.Length);
        }

    }
}
