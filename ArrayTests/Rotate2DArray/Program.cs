namespace Rotate2DClockwise90;

internal class Program
{
	static void Main(string[] args)
	{
		var keepRunning = true;
		while (keepRunning)
		{
			Console.Write("Enter the number of mm of the array: ");
			int mm = Convert.ToInt32(Console.ReadLine());
			Console.Write("Enter the number of columns of the array: ");
			int nn = Convert.ToInt32(Console.ReadLine());
			int[,] source = new int[mm, nn];
			int[,] dest = new int[nn, mm];
			// fill array with random numbers
			var random = new Random();
			for (int i = 0; i < mm; i++)
			{
				for (int j = 0; j < nn; j++)
				{
					source[i, j] = random.Next(1, 100);
				}
			}
			Console.WriteLine("Printing the source array:");
			for (int i = 0; i < mm; i++)
			{
				for (int j = 0; j < nn; j++)
				{
					Console.Write($"{source[i, j]} ");
				}
				Console.WriteLine();
			}
			for (int i = 0; i < mm; i++)
			{
				for (int j = 0; j < nn; j++)
				{
					dest[j, mm - 1 - i] = source[i, j];
				}
			}
			Console.WriteLine("Printing the dest array:");
			for (int i = 0; i < nn; i++)
			{
				for (int j = 0; j < mm; j++)
				{
					Console.Write($"{dest[i, j]} ");
					//if (j == mm - 1)
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
