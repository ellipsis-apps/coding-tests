namespace FindSecondMax;

internal class Program
{
	static void Main(string[] args)
	{
		int[] arr = new int[5];
		for (int i = 0; i < arr.Length; i++)
		{
			Console.Write($"Element - {i}: ");
			arr[i] = Convert.ToInt32(Console.ReadLine());
		}
		Console.WriteLine($"Printing the array in the order entered");
		for (int i = 0; i < arr.Length; i++)
		{
			Console.WriteLine($"You entered for {i}: {arr[i]}");
		}
		int max = arr[0];
		int secondMax = arr[0];
		for (int i = 1; i < arr.Length; i++)
		{
			if (arr[i] > max)
			{
				secondMax = max;
				max = arr[i];
			}
			else if (arr[i] > secondMax && arr[i] != max)
			{
				secondMax = arr[i];
			}
		}
		Console.WriteLine($"Max: {max}");
		Console.WriteLine($"Second Max: {secondMax}");
		Console.WriteLine($"{Environment.NewLine}Press any key to exit.");
		Console.ReadKey();
	}
}
