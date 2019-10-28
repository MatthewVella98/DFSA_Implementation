using System;

namespace DFSadjCSharp
{
    class Program
    {
    [STAThread]
    static void Main()
    {
      Console.WriteLine("1. DFSA 'A' Generation.");
      DFSadj dfsA = new DFSadj();

      Console.WriteLine("\n2. Depth of DFSA 'A'.");
      Console.WriteLine("  - Number of States in 'A': " + dfsA.states.Count);
      Console.WriteLine("  - Depth of 'A': " + dfsA.GetDepth());

      Console.WriteLine("\n3. DFSA 'A' Minimisation.");
      DFSadj dfsM = dfsA.GetMinimalDFSadj();

      Console.WriteLine("\n4. Depth of DFSA 'M'.");
      Console.WriteLine("  - Number of States in 'M': " + dfsM.states.Count);
      Console.WriteLine("  - Depth of 'M': " + dfsM.GetDepth());

      Console.WriteLine("\n5. Generating 100 random strings [Accept/Reject]"); 


      Console.WriteLine("\n6. Strongly Connected Components in DFSA 'M':");
      dfsM.GetSCCs();
      Console.WriteLine("  - Number of Strongly Connected Components in 'M': " + dfsM.SCCs.Count);
      dfsM.MaxminCC();
      Console.WriteLine("  - Number of States in the largest SCC: " + dfsM.maxSCC);
      Console.WriteLine("  - Number of States in the smallest SCC: " + dfsM.minSCC);
    }
    }
}



