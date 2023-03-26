//Console.OutputEncoding = System.Text.Encoding.Unicode;

//Console.SetWindowSize(80, 20);
//Console.SetBufferSize(80, 20);

/*Верхние строки закомментированы из-за соображений поддержки UNIX-платформ (на MAC нельзя оперировать размерами
 окна консоли и её буфера). Помимо этого, на маке данная кодировка работает некорректно. Если не подключать кодировку
 командой, то всё работает корректно.*/


decimal balance = GetStartBalance();

Console.Clear();

while (balance > 0)
{
    decimal bet = GetBet(balance);
    char[] currentSpin = spinValues();

    PrintSlots(currentSpin);

    decimal win = WinScore(currentSpin, bet);
    balance += win;
    PrintGameStats(win, balance);
    Console.ReadKey();
    Console.Clear();
}

Console.ReadKey();
//---------------------------------------------------------------------------------------

static decimal GetStartBalance()
{
    Console.Write("Введите стартовый баланс: ");
    string inputBalance = Console.ReadLine();
    return Convert.ToDecimal(inputBalance);
}

static decimal GetBet(decimal balance)
{
    while (true)
    {
        Console.Clear();
        Console.Write($"Введите ставку (ставка должна быть не больше баланса: {balance}): ");
        string inputBet = Console.ReadLine();
        decimal bet = Convert.ToDecimal(inputBet);
        if (bet <= balance && bet > 0)
        {
            return bet;
        }
    }
}

static char[] spinValues()
{
    Random rnd = new Random();
    char[] spinValues = new char[3];
    for (int i = 0; i < spinValues.Length; i++)
    {
        int chance = rnd.Next(1, 101);
        spinValues[i] = chance <= 35 ? (char)SuitsCode.Hearts :
            (chance > 35) && (chance <= 50) ? (char)SuitsCode.Diamonds :
            (chance > 50) && (chance <= 90) ? (char)SuitsCode.Clubs : (char)SuitsCode.Spades;
    }
    return spinValues;
}

static void PrintSlots(char[] currentSpinValues)
{

    string slotMachine = @"
_____________
|   |   |   |
|   |   |   |
|___|___|___|
";
    Console.WriteLine(slotMachine);

    ConsoleColor[] slotColors = _getColorOfSlots(currentSpinValues);

    int leftMargin = 2;
    int topMargin = 4;

    for (int i = 0; i < currentSpinValues.Length; i++)
    {
        Console.SetCursorPosition(leftMargin, topMargin);

        Console.ForegroundColor = slotColors[i];

        Console.Write(currentSpinValues[i]);

        Console.ResetColor();

        leftMargin += 4;
    }
    leftMargin = 0;
    topMargin = 8;

    Console.SetCursorPosition(leftMargin, topMargin);
}

static decimal WinScore(char[] currentSpinValues, decimal bet)
{
    bool[] conditions = _winningConditdons(currentSpinValues);
    decimal win = conditions[0] ? bet :
        conditions[1] || conditions[2] ? bet * 0.5m :
        conditions[3] ? bet * 0.7m : bet * -1;
    return Math.Round(win, 2, MidpointRounding.AwayFromZero);
}

static void PrintGameStats(decimal win, decimal balance)
{
    string winStatus = win >= 0 ? "выиграли" : "проиграли";
    Console.WriteLine($"Вы {winStatus}: {Math.Abs(win)} руб.");
    Console.WriteLine($"Баланс: {balance} руб.");
}

static ConsoleColor[] _getColorOfSlots(char[] currentSpin)
{
    bool[] conditions = _winningConditdons(currentSpin);
    if (conditions[0])
    {
        return new ConsoleColor[] { ConsoleColor.Magenta, ConsoleColor.Magenta, ConsoleColor.Magenta };
    }
    if (conditions[1])
    {
        return new ConsoleColor[] { ConsoleColor.Blue, ConsoleColor.Blue, ConsoleColor.White };
    }
    if (conditions[2])
    {
        return new ConsoleColor[] { ConsoleColor.White, ConsoleColor.Blue, ConsoleColor.Blue };
    }
    if (conditions[3])
    {
        return new ConsoleColor[] { ConsoleColor.Yellow, ConsoleColor.White, ConsoleColor.Yellow };
    }

    return new ConsoleColor[] { ConsoleColor.Red, ConsoleColor.Red, ConsoleColor.Red };
}

static bool[] _winningConditdons(char[] currentSpin)
{
    bool[] winningConditdons = new bool[4];
    winningConditdons[0] = (currentSpin[0] == currentSpin[1] && currentSpin[1] == currentSpin[2]);
    winningConditdons[1] = (currentSpin[0] == currentSpin[1] && currentSpin[1] != currentSpin[2]);
    winningConditdons[2] = (currentSpin[0] != currentSpin[1] && currentSpin[1] == currentSpin[2]);
    winningConditdons[3] = (currentSpin[0] == currentSpin[2] && currentSpin[0] != currentSpin[1]);
    return winningConditdons;
}
enum SuitsCode
{
    Hearts = 9829,
    Diamonds = 9830,
    Clubs = 9827,
    Spades = 9824
}