namespace SwapArrayElements;

internal class Program
{
	static void Main(string[] args)
	{
		int[] arr = new int[5];
		int[] arrCopy = new int[arr.Length];
		for (int i = 0; i < arr.Length; i++)
		{
			Console.Write($"Element - {i}: ");
			arr[i] = Convert.ToInt32(Console.ReadLine());
			arrCopy[i] = arr[i];
		}
		Console.WriteLine($"Printing the array in the order entered");
		for (int i = 0; i < arr.Length; i++)
		{
			Console.WriteLine($"You entered for {i}: {arr[i]}");
		}
		int left = 0;
		int right = arr.Length - 1;		
		int temp;
		while (left < right)
		{
			temp = arr[left];
			arr[left] = arr[right];
			arr[right] = temp;
			left++;
			right--;
		}
		Console.WriteLine($"Printing the array in the swapped order");
		for (int i = 0; i < arr.Length; i++)
		{
			Console.WriteLine($"You entered for {i}: {arr[i]}");
		}
		Console.WriteLine($"{Environment.NewLine}Press any key to exit.");
		Console.ReadKey();
	}
}
