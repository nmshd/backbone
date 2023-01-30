namespace Devices.AdminCli;

public class ConsoleMenu
{
    private readonly Dictionary<int, MenuItem> _items;

    public ConsoleMenu(IEnumerable<MenuItem> items)
    {
        _items = items.ToDictionary(i => i.Order, i => i);
    }

    public MenuItem AskForItemChoice()
    {
        Console.WriteLine("##################################################");
        foreach (var item in _items.Values)
        {
            Console.WriteLine($"{item.Order}) {item.Description}");
        }

        var input = ConsoleHelpers.ReadRequiredNumber("Choose an action", _items.Keys.Min(), _items.Keys.Max());

        return _items[input];
    }
}

public class MenuItem
{
    public MenuItem(int order, string description, Action action)
    {
        Order = order;
        Action = action;
        Description = description;
    }

    public int Order { get; set; }
    public Action Action { get; set; }
    public string Description { get; set; }
}
