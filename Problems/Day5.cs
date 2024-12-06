using System.Collections;

namespace advent_2024.problems;

public class Day5 : Day<int, int>
{
    protected override TaskState Task1State => TaskState.Final;
    protected override TaskState Task2State => TaskState.Final;
    
    protected override int ExecuteTask1(string input)
    {
        int result = 0;
        ParseInput(input, out var constraints, out var reports);
        foreach (var report in reports)
        {
            result += report.IsReportValid(constraints) ? report.GetReportValue() : 0;
        }
        return result;
    }

    protected override int ExecuteTask2(string input)
    {
        int result = 0;
        ParseInput(input, out var constraints, out var reports);
        foreach (var report in reports)
        {
            result += report.FixIfBroken(constraints) ? report.GetReportValue() : 0;
        }
        return result;
    }

    private class Report
    {
        private List<int> m_Pages;
        private Dictionary<int, int> m_PageIndexes;

        public Report(List<int> pages)
        {
            m_Pages = pages;
            m_PageIndexes = new Dictionary<int, int>();
            for (int i = 0; i < pages.Count; i++)
            {
                m_PageIndexes[pages[i]] = i;
            }
        }

        public int GetReportValue()
        {
            return m_Pages[(m_Pages.Count - 1) / 2];
        }

        public bool IsReportValid(Dictionary<int, List<int>> constraints)
        {
            foreach (var pageKvp in m_PageIndexes)
            {
                var pageNumber = pageKvp.Key;
                if (!constraints.ContainsKey(pageNumber))
                    continue;

                var pageIndex = pageKvp.Value;
                foreach (var constraint in constraints[pageNumber])
                {
                    if (!m_PageIndexes.TryGetValue(constraint, out var constraintIndex))
                        continue;

                    if (pageIndex > constraintIndex)
                        return false;
                }
            }

            return true;
        }

        public bool FixIfBroken(Dictionary<int, List<int>> constraints)
        {
            bool hasBeenFixed = false;
            
            for (int i = 0; i < m_Pages.Count; i++)
            {
                var pageNumber = m_Pages[i];
                if (!constraints.ContainsKey(pageNumber))
                    continue;

                foreach (var constraint in constraints[pageNumber])
                {
                    if (!m_PageIndexes.TryGetValue(constraint, out var constraintIndex))
                        continue;

                    if (i > constraintIndex)
                    {
                        hasBeenFixed = true;
                        
                        // Swap both pages
                        m_Pages[i] = constraint;
                        m_PageIndexes[constraint] = i;

                        m_Pages[constraintIndex] = pageNumber;
                        m_PageIndexes[pageNumber] = constraintIndex;

                        // And start over... this is the worst thing I've ever written haha
                        i = -1;
                    }
                }
            }

            return hasBeenFixed;
        }
    }
    
    private void ParseInput(string input, out Dictionary<int, List<int>> pageConstraints, out List<Report> reports)
    {
        pageConstraints = new Dictionary<int, List<int>>();
        reports = new List<Report>();
        
        var lines = input.Split(Environment.NewLine);
        foreach (var line in lines)
        {
            if (line.Contains('|'))
            {
                var parts = line.Split("|");
                var constrainedPage = int.Parse(parts[0]);
                var pageConstraint = int.Parse(parts[1]);
                if (!pageConstraints.ContainsKey(constrainedPage))
                    pageConstraints.Add(constrainedPage, new List<int>());
                
                pageConstraints[constrainedPage].Add(pageConstraint);
            }
            
            else if (line.Contains(','))
            {
                // Split the line and parse each part to an int
                var parts = line.Split(",");
                List<int> report = parts.Select(part => int.Parse(part)).ToList();
                reports.Add(new Report(report));
            }
        }
    }
}