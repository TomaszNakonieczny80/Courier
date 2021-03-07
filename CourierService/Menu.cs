using System;
using System.Collections.Generic;
using System.Text;

namespace Courier
{
    public interface IMenu
    {
        void AddOption(MenuItem item);
        void ClearOption();
        void ExecuteOption(int optionKey);
        void PrintAvailableOptions();
    }
    public class Menu : IMenu
    {
        private Dictionary<int, MenuItem> _options = new Dictionary<int, MenuItem>();

        public void AddOption(MenuItem item)
        {
            if (_options.ContainsKey(item.Key))
            {
                return;
            }

            _options.Add(item.Key, item);
        }

        public void ClearOption()
        {
            _options.Clear();
        }

        public void ExecuteOption(int optionKey)
        {
            if (!_options.ContainsKey(optionKey))
            {

                Console.WriteLine("\nWrong choice, try again...");
                return;
            }

            var item = _options[optionKey];
            item.Action();
        }

        public void PrintAvailableOptions()
        {
            foreach (var option in _options)
            {
                Console.WriteLine($"{option.Key}. {option.Value.Description}");
            }
        }
    }
}
