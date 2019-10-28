using System;
using System.Collections.Generic;
using System.Linq;

namespace DFSadjCSharp
{
  public class DFSadj
  {
    //Bool array: is state adjacent or not
    private bool[] adjState;

    //Initial state id storage
    private int startStateID;

    // Storing the set of states
    public List<State> states;
    // An Adjacency Matrix was used so that 'a' and 'b' transitions, can be checked in 0(1). 
    // n rows and 2 columns. n being the number of states in the DFSadj
    // Col 0 shows the outgoing 'a' transitions and Col 1 shows the outgoing 'b' transitions
    // Example; finding out which state is reachable by following an 'a' from state 10, check: adj[10][0]
    private int[,] adj;

    /** Tarjan's Algorithm - Strongly Connected Component (SCC) finder **/
    //List of SCCs. Each element in the List is an List of StateIDs (int).
    public List<List<int>> SCCs;
    // Last used index (smallest unused index)
    private int lastIndex;
    // Index storage for each state
    private int[] stateIndex;
    // lowlink storage for each state
    private int[] lowlink;
    // stack storing the IDs of the states which make up the current SCC
    private Stack<int> stateIDStack;
    // Wthether a state is on the stack or not
    private bool[] onStack;
    //the largest SCC
    public int maxSCC;
    //the smallest SCC
    public int minSCC;

    public DFSadj()
    {
      Random random = new Random();
      //set the number of states of the DFSadj to a random number between 16 and 64 (inclusive)
      int n = random.Next(48+1) + 16;

      states = new List<State>();
      adj = new int[n, 2];

      // For every state which needs to be generated:
      for (int i = 0; i < n; i++)
      {
        // initialise a new State object
        State state = new State();

        state.stateID = i;

        // Randomly label state [Accepting/Rejecting]
        state.finalState = random.Next(2) == 1;

        // Randomly current transition to a'
        adj[i, 0] = random.Next(n);

        // Randomly current transition to b'
        adj[i, 1] = random.Next(n);

        states.Add(state);
      }

      // setting the start state of the DFSadj to a random state
      startStateID = random.Next(n);
    }

    // Constructor for a new DFSadj, takes the number of states as a parameter
    public DFSadj(int numberOfStates)
    {
      states = new List<State>();
      adj = new int[numberOfStates, 2];
    }

    public int GetDepth()
    {
      //State IDs storage of the states in the current 'level' of the DFS
      List<int> currLevelStates = new List<int>();
      // add the start state of the DFSadj to the current level
      currLevelStates.Add(startStateID);

      // bool array: a state has been visited or not
      bool[] visited = new bool[states.Count];

      // depth of DFSA initialised 
      int depth = 0;

     
      while (true)
      {
        // storing the reachable states from the current level. The stateID is stored.
        List<int> nextLevelStates = new List<int>();

        // Current level states are marked visited
        for (int i = 0; i < currLevelStates.Count; i++)
        {
          int currentLevelState = currLevelStates[i];
          visited[currentLevelState] = true;
        }

        // getting the states reachable from current level
        for (int i = 0; i < currLevelStates.Count; i++)
        {
          int currentLevelState = currLevelStates[i];
          // if The state reachable by trnaistion 'a', has not been visited yet
          if (!visited[adj[currentLevelState, 0]])
          {
            //The reachable state by transition a is added  to the nextLevelStates list
            nextLevelStates.Add(adj[currentLevelState, 0]);
          }
          // if the state rwachable by transition 'b' has not been visited yet. 
          if (!visited[adj[currentLevelState, 1]])
          {
            //The reachable state by transition b is added  to the nextLevelStates list
            nextLevelStates.Add(adj[currentLevelState, 1]);
          }
        }

                // The nextLevelStates is copied to the currentLevelStates, to be ready for the next iteration
                currLevelStates = new List<int>(nextLevelStates);

        // When there are no nextStates: 
        // i) all the states of the DFSadj have been visited
        // ii) all the states which can be visited starting from the Start State have been visited, therefore some states
        // cannot be reached)
        if (nextLevelStates.Count == 0)
        {
            // The contents of the visited array are copied to the reachableState array.
            // the unreachable states are those states which were not visited after running this function. 
            // This is later used in the minimisation of the DFAadj. 
        
            adjState = new bool[visited.Length];
            Array.Copy(visited, adjState, visited.Length);

          return depth;
        }
        else
        {
          depth++;
        }
      }
    }

    public DFSadj GetMinimalDFSadj()
    {
      //States are divided into accepting states and rejecting states
      HashSet<int> acceptingStates = new HashSet<int>();
      HashSet<int> rejectingStates = new HashSet<int>();

      foreach (State state in states)
      {
        // If the current state could be reached 
        if (adjState[state.stateID])
        {
          // Current state = accepting state 
          if (state.finalState)
          {
            // Added int the set of accepting states. 
            acceptingStates.Add(state.stateID);
          }
          else
          {
            // Else it's added in the set of rejecting states 
            rejectingStates.Add(state.stateID);
          }
        }
      }

      // The hashsetP := {F, Q \ F}
      HashSet<HashSet<int>> hashSetP = new HashSet<HashSet<int>>(HashSet<int>.CreateSetComparer());
      hashSetP.Add(acceptingStates);
      hashSetP.Add(rejectingStates);

      // The hashsetW := {F} 
      HashSet<HashSet<int>> hashSetW = new HashSet<HashSet<int>>(HashSet<int>.CreateSetComparer());
      hashSetW.Add(acceptingStates);

      // whilst hashSetW is not empty 
      while (hashSetW.Count != 0)
      {
        // a hashset A is removed from hashset W  
        HashSet<int> hashSetA = hashSetW.ElementAt(0);
        hashSetW.Remove(hashSetA);

        // for each c in Σ do
        for (int ii = 0; ii < adj.GetLength(1); ii++)
        {

          // All the transitions c which lead to state a, are stored in hashset X
          HashSet<int> hashSetX = new HashSet<int>();
          foreach (State state in states)
          {
            if (adjState[state.stateID])
            {
              if (hashSetA.Contains(adj[state.stateID, ii]))
              {
                hashSetX.Add(state.stateID);
              }
            }
          }

          // When X Intercetion Y and Y set difference X is non empty, in P is hashsetY.  
          HashSet<HashSet<int>> Pcopy = new HashSet<HashSet<int>>(hashSetP, HashSet<int>.CreateSetComparer());
          foreach (HashSet<int> setInP in Pcopy)
          {
            HashSet<int> hashSetY = new HashSet<int>(setInP);

            HashSet<int> intersection = new HashSet<int>(hashSetX);
            intersection.IntersectWith(hashSetY);

            HashSet<int> difference = new HashSet<int>(hashSetY);
            difference.ExceptWith(hashSetX);

            if (intersection.Count > 0 && difference.Count > 0)
            {
              // hashsetY is replaced in hashsetp by the 2 sets:  X ∩ Y & Y \ X
              hashSetP.Remove(hashSetY);
              hashSetP.Add(intersection);
              hashSetP.Add(difference);

              // if hashsetY is in hashetW
              if (hashSetW.Contains(hashSetY))
              {
                // hashsetY is replaced in hashsetW by the 2 sets:  X ∩ Y & Y \ X
                hashSetW.Remove(hashSetY);
                hashSetW.Add(intersection);
                hashSetW.Add(difference);
              }
              else
              {
                // else
                // if |X ∩ Y| <= |Y \ X|
                if (intersection.Count <= difference.Count)
                {
                  // X ∩ Y is added to hashsetW
                  hashSetW.Add(intersection);
                }
                else
                {
                    // else
                    // Y \ X is added to hashsetW
                    hashSetW.Add(difference);
                }
              }
            }
          }
        }
      }

      // DFSadj M
      DFSadj dfsaM = new DFSadj(hashSetP.Count);

      // So that the elements have a fixed order, the hashsetP was changed into a list.
      List<HashSet<int>> arrayP = new List<HashSet<int>>(hashSetP);

      // Iterating throughout the list P
      int i = 0;
      for (int k = 0; k < arrayP.Count; k++)
      {
        HashSet<int> setInP = arrayP[k];
        // A new state is cleared for each element 
        State state = new State();

        // An id is assigned to the state 
        state.stateID = i;
                
        // if the start state of DFSadj A is contained 
        if (setInP.Contains(startStateID))
        {
          // the state is set as start state in DFSadj M
          dfsaM.startStateID = i;
        } 

        // if the states are final states, the set represting the partion
        // is set as an accepting state
        // Else, the state is set as a rejecting state. 
        if (setInP.Count > 0 && states[0].finalState)
          state.finalState = true;
        else state.finalState = false; 

        // the states in this partition of DFSadj A, are checked were they go to. 
        int oldA = -1, oldB = -1;
        for (int j = 0; j < states.Count; j++)
        {
          // When the state is found, the transitions are stored. 
          if (setInP.Contains(states[j].stateID))
          {
            oldA = adj[j, 0];
            oldB = adj[j, 1];
            break; 
          }
        }

        // when it is known which states a and b lead to in DFSadj A, make these transitions to show where transitions a and be had led to, from the current state. 
        for (int j = 0; j < arrayP.Count; j++)
        {
          if (arrayP[j].Contains(oldA))
          {
            dfsaM.adj[i, 0] = j;
          }

          if (arrayP[j].Contains(oldB))
          {
            dfsaM.adj[i, 1] = j;
          }
        }

        dfsaM.states.Add(state);

        i++;
      }

      return dfsaM;
    }

    public List<List<int>> GetSCCs()
    {
      SCCs = new List<List<int>>();

      //smallest unused index is set to 0
      lastIndex = 0;

      stateIndex = Enumerable.Repeat(-1, states.Count).ToArray();

      lowlink = Enumerable.Repeat(-1, states.Count).ToArray();                                                      
                                                                                                  
      stateIDStack = new Stack<int>();
      onStack = new bool[states.Count];

      // loops through every state in the DFSadj
      for (int i = 0; i < states.Count; i++)
      {
        // if node index is undefined
        if (stateIndex[i] == -1)
        {
          // the strongConnect method is called and passed the current node 
          StrongConnect(states[i].stateID);
        }
      }

      return SCCs;
    }

    public void StrongConnect(int stateID)
    { 
      // the index and lowlink are set for the current state to the smallest unused index
      stateIndex[stateID] = lastIndex;
      lowlink[stateID] = lastIndex;

      // incrementing this index. 
      lastIndex++;

      // the current state is added to the state stack. 
      stateIDStack.Push(stateID);
      // This is marked as being on the stack. 
      onStack[stateID] = true;

      // for all the states wgucg can be reached from the current state by an a/b transition. 
      for (int i = 0; i < adj.GetLength(1); i++)
      {
        int successorID = adj[stateID, i];
            // a recursive call with that state is made if the index of the next state is still undefined  
            if (stateIndex[successorID] == -1)
        {
          StrongConnect(successorID);
          // The lowlink of the current state is set as the lowlink of the next or current state, depending on which is smaller. 
          lowlink[stateID] = (lowlink[successorID] < lowlink[stateID]) ? lowlink[successorID] : lowlink[stateID];
        }
        else if (onStack[successorID])
        {
          // if  next state is on the stack,  it is part of the current SCC
          // The lowlink of the current state is set as the lowlink of the next or current state, depending on which is smaller.
          lowlink[stateID] = (lowlink[stateID] < stateIndex[successorID]) ? lowlink[stateID] : stateIndex[successorID];
        }
      }

      // if the current node is a root node, a SCC has been found 
      if (lowlink[stateID] == stateIndex[stateID])
      {
        List<int> SCC = new List<int>();

        // the states are popped from the stack until the current state is popped. 
        // every popped state is added to the current SCC
        int w;
        do
        {
          w = stateIDStack.Pop();
          onStack[w] = false;
          SCC.Add(w);
        } while (w != stateID);

        // The newly created SCC is then returned. 
        SCCs.Add(SCC);
      }
    }

    //Finding the largest and smallest in the SCC. 
    public void MaxminCC()
    {
      maxSCC = SCCs[0].Count;
      minSCC = SCCs[0].Count;
      for (int i = 1; i < SCCs.Count; i++)
      {
        if (SCCs[i].Count > maxSCC)
        {
          maxSCC = SCCs[i].Count;
        }
        if (SCCs[i].Count < minSCC)
        {
          minSCC = SCCs[i].Count;
        }
      }
    }
  }

  public struct State
  {
    // The ID of the current state. Range from 0 to n-1; where n is the number of states in the DFSadj
    public int stateID;

    
    // 1 = accepting (final), 0 = rejecting
    public bool finalState;
  }
}
