using System;
using System.Collections.Generic;

class RobotPathFinder
{
    private int n;
    private int[,] matrix;
    private (int, int) start;
    private (int, int) end;
    private (int, int)[] directions = new (int, int)[]
    {
        (0, 1), (1, 0), (0, -1), (-1, 0)
    };

    public RobotPathFinder(int n, (int, int) start, (int, int) end)
    {
        this.n = n;
        this.start = start;
        this.end = end;
        this.matrix = GenerateRandomMatrix(n);
    }

    private int[,] GenerateRandomMatrix(int n)
    {
        var rand = new Random();
        var mat = new int[n, n];
        for (int i = 0; i < n; i++)
            for (int j = 0; j < n; j++)
                mat[i, j] = rand.Next(1, 4);
        return mat;
    }

    private bool IsValidPosition(int row, int col)
    {
        return row >= 0 && row < n && col >= 0 && col < n;
    }

    private bool IsCorner(int row, int col)
    {
        return (row == 0 && col == 0) ||
               (row == 0 && col == n - 1) ||
               (row == n - 1 && col == 0) ||
               (row == n - 1 && col == n - 1);
    }

    private List<(int, int, int)> GetNeighbors(int row, int col)
    {
        var neighbors = new List<(int, int, int)>();
        foreach (var (dr, dc) in directions)
        {
            int newRow = row + dr;
            int newCol = col + dc;
            if (IsValidPosition(newRow, newCol))
            {
                int cost = matrix[newRow, newCol];
                neighbors.Add((newRow, newCol, cost));
            }
        }
        return neighbors;
    }

    public (List<(int, int)>, int, List<(int, int)>) FindOptimalPath()
    {
        var dist = new Dictionary<(int, int), int>();
        var prev = new Dictionary<(int, int), (int, int)?>();
        var heap = new PriorityQueue<(int, int), int>();
        var visitedCorners = new List<(int, int)>();

        dist[start] = 0;
        prev[start] = null;
        heap.Enqueue(start, 0);

        while (heap.Count > 0)
        {
            var currentPos = heap.Dequeue();
            int currentCost = dist[currentPos];

            if (currentPos == end)
                break;

            if (IsCorner(currentPos.Item1, currentPos.Item2) &&
                currentPos != start &&
                !visitedCorners.Contains(currentPos))
            {
                visitedCorners.Add(currentPos);
            }

            foreach (var (nr, nc, moveCost) in GetNeighbors(currentPos.Item1, currentPos.Item2))
            {
                var neighbor = (nr, nc);
                int newCost = currentCost + moveCost;

                if (!dist.ContainsKey(neighbor) || newCost < dist[neighbor])
                {
                    dist[neighbor] = newCost;
                    prev[neighbor] = currentPos;
                    heap.Enqueue(neighbor, newCost);
                }
            }
        }

        if (!prev.ContainsKey(end))
        {
            return (new List<(int, int)>(), int.MaxValue, visitedCorners);
        }

        var path = new List<(int, int)>();
        (int, int)? current = end;
        while (current is not null)
        {
            path.Add(current.Value);
            current = prev[current.Value];
        }
        path.Reverse();

        return (path, dist[end], visitedCorners);
    }

    public void PrintMatrix()
    {
        Console.WriteLine("Matriz de Custos:");
        for (int i = 0; i < n; i++)
        {
            for (int j = 0; j < n; j++)
            {
                if ((i, j) == start)
                    Console.Write($"S({matrix[i, j]}) ");
                else if ((i, j) == end)
                    Console.Write($"E({matrix[i, j]}) ");
                else
                    Console.Write($"{matrix[i, j]} ");
            }
            Console.WriteLine();
        }
    }

    public void PrintResults()
    {
        var (path, totalCost, corners) = FindOptimalPath();

        Console.WriteLine(new string('=', 50));
        Console.WriteLine("ROBO CAMINHO MAIS EFICIENTE");
        Console.WriteLine(new string('=', 50));
        Console.WriteLine($"Matriz: {n}x{n}");
        Console.WriteLine($"Partida: {start}");
        Console.WriteLine($"Destino: {end}");

        if (totalCost == int.MaxValue)
        {
            Console.WriteLine("NAO FOI POSSIVEL ENCONTRAR UM CAMINHO!");
            return;
        }

        Console.WriteLine($"Custo total: {totalCost}");
        Console.WriteLine($"Numero de passos: {path.Count - 1}");

        Console.WriteLine("\nCaminho percorrido:");
        for (int i = 0; i < path.Count; i++)
        {
            var pos = path[i];
            string corner = IsCorner(pos.Item1, pos.Item2) && pos != start ? " (QUINA)" : "";
            string startMark = pos == start ? " (INICIO)" : "";
            string endMark = pos == end ? " (FIM)" : "";
            Console.WriteLine($"Passo {i:00}: {pos}{corner}{startMark}{endMark}");
        }

        Console.WriteLine($"\nQuinas visitadas: [{string.Join(", ", corners)}]");
    }
}
