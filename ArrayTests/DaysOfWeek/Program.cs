namespace DaysOfWeek;

internal class Program
{
	static void Main(string[] args)
	{
		string[] dayArr = { "Sunday", "Monday", "Tuesday", "Wednesday", "Thursday", "Friday", "Saturday" };
		var days = dayArr.ToList();
		foreach (var day in days)
		{
			Console.WriteLine($"Day: {day}");
		}
		Console.WriteLine($"{Environment.NewLine}Press any key to exit.");
		Console.ReadKey();
	}
}
