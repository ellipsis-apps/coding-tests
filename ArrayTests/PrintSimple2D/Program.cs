namespace PrintSimple2D;

internal class Program
{
	static void Main(string[] args)
	{
		var keepRunning = true;
		while (keepRunning)
		{
			Console.Write("Enter the number of rows of the array: ");
			int rows = Convert.ToInt32(Console.ReadLine());
			Console.Write("Enter the number of columns of the array: ");
			int cols = Convert.ToInt32(Console.ReadLine());
			int[,] arr = new int[rows, cols];
			// fill array with random numbers
			var random = new Random();
			for (int i = 0; i < rows; i++)
			{
				for (int j = 0; j < cols; j++)
				{
					arr[i, j] = random.Next(1, 100);
				}
			}
			Console.WriteLine("Printing the 2D array:");
			for (int i = 0; i < rows; i++)
			{
				for (int j = 0; j < cols; j++)
				{
					Console.Write($"{arr[i, j]} ");
					//if (j == cols - 1)
					//{
					//	Console.Write($"{Environment.NewLine}");
					//}
				}
				Console.WriteLine();
			}
			Console.Write($"{Environment.NewLine}Do you wish to continue? (y to continue, anything else to stop): ");
			var response = Console.ReadLine().ToLower();
			if (response != "y" && response != "yes")
			{
				keepRunning = false;
			}
		}
	}
}
