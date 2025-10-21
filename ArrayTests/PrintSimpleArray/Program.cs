namespace ArrayTests;

internal class Program
{
	static void Main(string[] args)
	{
		Console.Write("Enter the size of the array: ");
		int size = Convert.ToInt32(Console.ReadLine());
		int[] arr = new int[size];
		// fill array with random numbers
		for (int i = 0; i < arr.Length; i++)
		{
			var random = new Random();
			arr[i] = random.Next(1, 100);
		}
		Console.WriteLine($"Printing the array in the order entered");
		for (int i = 0; i < arr.Length; i++)
		{
			Console.WriteLine($"Element {i}: {arr[i]}");
		}
		Console.WriteLine($"{Environment.NewLine}Printing the array in reverse order");
		for (int i = arr.Length - 1; i >= 0; i--)
		{
			Console.WriteLine($"You entered for {i}: {arr[i]}");
		}
		var sum = 0;
		for (int i = 0; i < arr.Length; i++)
		{
			sum += arr[i];
		}
		Console.WriteLine($"{Environment.NewLine}The sum is: {sum}");
		Console.WriteLine($"{Environment.NewLine}Press any key to exit.");
		Console.ReadKey();
	}
}
