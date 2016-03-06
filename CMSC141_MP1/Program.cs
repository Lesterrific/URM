using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace CMSC141_MP1 {

    class Program {
        static void Main(string[] args) {
            URMProgram URMP = new URMProgram(Path.Combine(Directory.GetCurrentDirectory(), "mp1.in"));
            URM URMachine = new URM();
            //Console.WriteLine("\n-------------------------------------");
            URMachine.Execute(URMP);
        }
    }

    public class URM {

        URMProgram Program;
        int InstructionPointer;

        public void Execute(URMProgram P) {
            
            //Console.WriteLine("\nRunning the program...");
            Program = P;
            InstructionPointer = 0;
            PrintState();

            while(InstructionPointer < Program.Instructions.Length) {
                if(InstructionPointer > Program.Instructions.Length) {
                    break;
                }
                ExecuteInstruction(Program.Instructions[InstructionPointer]);
            }
            //Console.WriteLine("END OF PROGRAM.");
        }

        void ExecuteInstruction(string Instruction) {
            /* parsing the instruction */
            bool IJump = false;
            string[] ParsedInstruction = Instruction.Split(' ');
            if (ParsedInstruction[0] == "J") {
                int comp1 = Program.State[int.Parse(ParsedInstruction[1])];
                int comp2 = Program.State[int.Parse(ParsedInstruction[2])];
                int jump = int.Parse(ParsedInstruction[3]) - 1;
                if (comp1 == comp2) {
                    InstructionPointer = jump;
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
            if(IJump == false) {
                InstructionPointer++;
            }
            PrintState();
        }

        void PrintState() {
            foreach(var Register in Program.State) {
                Console.Write(Register + " ");
            }
            Console.WriteLine("");
        }
    }

    public class URMProgram {

        public string[] Instructions;
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
