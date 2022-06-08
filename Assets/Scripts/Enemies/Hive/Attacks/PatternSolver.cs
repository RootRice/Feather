using System.Collections;
using System.Collections.Generic;
using System;
using System.Diagnostics;
using UnityEngine;

public class PatternSolver
{
    public PatternSolver(int[][] _enemyAttacks)
    {
        enemyAttacks = _enemyAttacks;
    }
    int[][] enemyAttacks;

    List<List<int[]>> combinations;
    List<List<int[]>> permutations;

    public void FindCombinations()
    {
        combinations = new List<List<int[]>>();
        FillList(0, new List<int[]>());
        UnityEngine.Debug.Log(combinations.Count);
        RemoveDuplicateCombos();
        UnityEngine.Debug.Log(combinations.Count);
        FindPermutations(combinations);
        RemoveUnit(3);
    }

    void FindPermutations(List<List<int[]>> list)
    {
        Stopwatch timer = new Stopwatch();
        timer.Start();
        permutations = new List<List<int[]>>();
        foreach(List<int[]> l in list)
        {
            Permute(l, new List<int[]>());
        }
        timer.Stop();
        UnityEngine.Debug.Log(timer.ElapsedMilliseconds);
        UnityEngine.Debug.Log(permutations.Count);
    }

    void RemoveUnit(int index)
    {
        Stopwatch timer = new Stopwatch();
        timer.Start();
        foreach (List<int[]> l in permutations)
        {
            for(int i = 0; i < l.Count; i++)
            {
                if(l[i][0] == index)
                {
                    l.RemoveAt(i);
                    i--;
                }
            }
        }
        timer.Stop();
        UnityEngine.Debug.Log(timer.ElapsedMilliseconds);
    }

    void Permute(List<int[]> choices, List<int[]> workingSet)
    {
        if(choices.Count == 0)
        {
            permutations.Add(new List<int[]>(workingSet));
        }

        for(int i = 0; i < choices.Count; i++)
        {
            int[] hold = choices[i];
            workingSet.Add(hold);
            choices.RemoveAt(i);
            Permute(choices, workingSet);

            choices.Insert(i, hold);
            workingSet.Remove(hold);
        }
    }

    void FillList(int index, List<int[]> currentList)
    {
        if(index >= enemyAttacks.GetLength(0))
        {
            combinations.Add(currentList);
            return;
        }
        for (int i = 1; i < enemyAttacks[index].Length; i++)
        {
            List<int[]> newList = new List<int[]>(currentList);
            newList.Add(new int[2] { index, enemyAttacks[index][i] });
            FillList(index + 1, newList);
        }
        currentList.Add(new int[2] { index, enemyAttacks[index][0] });
        FillList(index + 1, currentList);
        
        
    }

    void RemoveDuplicateCombos()
    {
        foreach (List<int[]> l in combinations)
        {
            SortArrListByElement(l, 1);
        }
        for (int i = 0; i < combinations.Count-1; i++)
        {
            for(int ii = i+1; ii < combinations.Count; ii++)
            {
                if (CompareListContents(combinations[i], combinations[ii]))
                {
                    combinations.RemoveAt(ii);
                    ii--;
                }
            }
            
        }
    }

    bool CompareListContents(List<int[]> l1, List<int[]> l2)
    {
        bool r = true;
        for(int i = 0; i < l1.Count; i++)
        {
            if(l1[i][1] != l2[i][1])
            {
                r = false;
            }
        }
        return r;
    }

    void SortArrListByElement(List<int[]> list, int element)
    {
        for (int i = 0; i < list.Count; i++)
        {
            for(int ii = 0; ii < list.Count-1; ii++)
            {
                if (list[i][element] > list[ii + 1][element])
                {
                    int[] temp = list[ii+1];
                    list[ii+1] = list[ii];
                    list[ii] = temp;
                }
            }
            
        }
    }

}
