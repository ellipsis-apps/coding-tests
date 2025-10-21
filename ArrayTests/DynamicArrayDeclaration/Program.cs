namespace DynamicArrayDeclaration;

internal class Program
{
	static void Main(string[] args)
	{
		Console.Write("Enter the size of the array: ");
		int size = int.Parse(Console.ReadLine());
		Guid[] dynamicArray = new Guid[size];
		for (int i = 0; i < dynamicArray.Length; i++)
		{
			dynamicArray[i] = Guid.NewGuid();
		}
		for (int i = 0; i < size; i++)
		{
			Console.WriteLine($"Element - {i}: {dynamicArray[i].ToString()}");
		}
		Console.WriteLine($"{Environment.NewLine}Press any key to exit.");
		Console.ReadKey();
	}
}
