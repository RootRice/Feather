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
    List<List<int[]>>[] permutations;

    int[,] comparisonMatrix;

    public void FindCombinations()
    {
        combinations = new List<List<int[]>>();
        FillList(0, new List<int[]>());
        RemoveDuplicateCombos();
        FindPermutations(combinations);
    }

    void FindPermutations(List<List<int[]>> list)
    {
        permutations = new List<List<int[]>>[list.Count];
        for(int i = 0; i < permutations.Length; i++)
        {
            permutations[i] = new List<List<int[]>>();
        }
        for(int i = 0; i < list.Count; i++) 
        {
            Permute(list[i], new List<int[]>(), i);
        }
    }

    void RemoveUnit(int index)
    {
        for (int i = 0; i < combinations.Count; i++)
        {
            for(int ii = 0; ii < combinations[i].Count; ii++)
            {
                if ((combinations[i][ii][0]) == index)
                {
                    combinations[i].RemoveAt(ii);
                    ii--;
                }
            }
        }
        for (int i = 0; i < permutations.Length; i++)
        {
            foreach (List<int[]> l in permutations[i])
            {
                for (int ii = 0; ii < l.Count; ii++)
                {
                    if (l[ii][0] == index)
                    {
                        l.RemoveAt(ii);
                        ii--;
                    }
                }
            }
        }
    }

    void Permute(List<int[]> choices, List<int[]> workingSet, int combID)
    {
        if(choices.Count == 0)
        {
            permutations[combID].Add(new List<int[]>(workingSet));
        }

        for(int i = 0; i < choices.Count; i++)
        {
            int[] hold = choices[i];
            workingSet.Add(hold);
            choices.RemoveAt(i);
            Permute(choices, workingSet, combID);

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

    public int[][] FindCombo(int[] combo)
    {
        int[] matches = new int[combinations.Count];
        List<int> maxMatches = new List<int>();
        List<int> validPerms = new List<int>();
        int highestNumMatches = 0;
        for (int i = 0; i < matches.Length; i++)
        {
            matches[i] = FindMatchingContentCount(combo, combinations[i]);
            highestNumMatches = Mathf.Max(highestNumMatches, matches[i]);
        }
        for (int i = 0; i < matches.Length; i++)
        {
            if (matches[i] == highestNumMatches)
            {
                maxMatches.Add(i);
            }
        }

        comparisonMatrix = new int[combo.Length + 1, permutations[maxMatches[0]][0].Count + 1];
        int matrixX = combo.Length + 1;
        int matrixY = permutations[maxMatches[0]][0].Count + 1;
        int n = permutations[maxMatches[0]].Count;
        int ak = UnityEngine.Random.Range(0, maxMatches.Count);
        for (int ii = 0; ii < n; ii++)
        {
            if (FindLCS(combo, permutations[maxMatches[ak]][ii], highestNumMatches, matrixX, matrixY))
            {
                validPerms.Add(ii);
            }
        }
        int aj = UnityEngine.Random.Range(0, validPerms.Count);
        List<int[]> selected = permutations[maxMatches[ak]][validPerms[aj]];
        int[] attackerIndices = GetLCS(combo, selected, highestNumMatches, matrixX, matrixY);
        List<int[]> attackerData = new List<int[]>();
        foreach(int i in attackerIndices)
        {
            attackerData.Add(selected[i]);
        }
        return attackerData.ToArray();
    }

    bool FindLCS(int[] combo, List<int[]> sequence, int expectedLength, int m, int n)
    {
        int x = combo[m - 2];
        int y = sequence[n - 2][1];
        for (int i = 1; i < m; i++)
        {
            int comboPiece = combo[i - 1];
            for (int ii = 1; ii < n; ii++)
            {
                if (comboPiece == sequence[ii - 1][1])
                {
                    comparisonMatrix[i, ii] = comparisonMatrix[i - 1, ii - 1] + 1;
                }
                else
                {
                    comparisonMatrix[i, ii] = Math.Max(comparisonMatrix[i - 1, ii], comparisonMatrix[i, ii - 1]);
                }
            }
        }
        return comparisonMatrix[m-1, n-1] >= expectedLength;
    }

    int[] GetLCS(int[] combo, List<int[]> sequence, int expectedLength, int m, int n)
    {
        int x = combo[m - 2];
        int y = sequence[n - 2][1];
        for (int i = 1; i < m; i++)
        {
            int comboPiece = combo[i - 1];
            for (int ii = 1; ii < n; ii++)
            {
                if (comboPiece == sequence[ii - 1][1])
                {
                    comparisonMatrix[i, ii] = comparisonMatrix[i - 1, ii - 1] + 1;
                }
                else
                {
                    comparisonMatrix[i, ii] = Math.Max(comparisonMatrix[i - 1, ii], comparisonMatrix[i, ii - 1]);
                }
            }
        }
        int[] indices = new int[expectedLength];
        int j = m - 1;
        int jj = n - 1;
        int k = expectedLength-1;
        while (comparisonMatrix[j,jj] != 0)
        {
            int index = comparisonMatrix[j, jj];
            int indexMX = comparisonMatrix[j - 1, jj];
            int indexMY = comparisonMatrix[j, jj - 1];
            if (index > Math.Max(indexMX, indexMY))
            {
                indices[k] = jj-1;
                k--;
                j--;
                jj--;
            }
            else if (index == indexMX)
            {
                j--;
            }
            else
            {
                jj--;
            }
        }
        return indices;
    }

    int FindMatchingContentCount(int[] a, List<int[]> b)
    {
        List<int> aList = new List<int>(a);
        List<int[]> bList = new List<int[]>(b);
        int j = 0;
        for(int i = 0; i < aList.Count; i++)
        {
            for(int ii = 0; ii < bList.Count; ii++)
            {
                if(aList[i] == bList[ii][1])
                {
                    j++;
                    bList.RemoveAt(ii);
                    break;
                }
            }
        }
        return j;
    }
}
