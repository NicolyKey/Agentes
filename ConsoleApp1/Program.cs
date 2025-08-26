class Program
{
    static void Main()
    {
        Console.WriteLine("SIMULADOR DE CAMINHO DE ROBO");

        int n = 10;
        var start = (2, 3);
        var end = (8, 8);

        var finder = new RobotPathFinder(n, start, end);
        finder.PrintMatrix();
        Console.WriteLine();
        finder.PrintResults();
    }
}