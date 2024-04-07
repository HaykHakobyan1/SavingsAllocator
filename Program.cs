using System;
using System.Collections.Generic;
using System.IO;

namespace AutomaticSavingsAllocator
{
    class SavingsAccount
    {
        public string AccountName { get; }
        public decimal Balance { get; private set; }

        public SavingsAccount(string accountName)
        {
            AccountName = accountName;
            Balance = 0;
        }

        public void Deposit(decimal amount)
        {
            Balance += amount;
            Console.WriteLine($"Deposited {amount:C} into {AccountName} account.");
        }

        public void Withdraw(decimal amount)
        {
            if (amount > Balance)
            {
                Console.WriteLine($"Insufficient funds in {AccountName} account.");
                return;
            }

            Balance -= amount;
            Console.WriteLine($"Withdrawn {amount:C} from {AccountName} account.");
        }
    }

    class SavingsGoal
    {
        public string Name { get; }
        public decimal TargetAmount { get; }
        public decimal AllocationPercentage { get; }

        public SavingsGoal(string name, decimal targetAmount, decimal allocationPercentage)
        {
            Name = name;
            TargetAmount = targetAmount;
            AllocationPercentage = allocationPercentage;
        }
    }

    class SavingsAllocator
    {
        private SavingsAccount _incomeAccount;
        private SavingsAccount _savingsAccount;
        private List<SavingsGoal> _savingsGoals;

        private const string DataFilePath = "userdata.txt";

        public SavingsAllocator(decimal initialIncome)
        {
            _incomeAccount = new SavingsAccount("Income");
            _incomeAccount.Deposit(initialIncome);

            _savingsAccount = new SavingsAccount("Savings");

            _savingsGoals = new List<SavingsGoal>();

            LoadUserData();
        }

        public void AddSavingsGoal(string name, decimal targetAmount, decimal allocationPercentage)
        {
            _savingsGoals.Add(new SavingsGoal(name, targetAmount, allocationPercentage));
            Console.WriteLine($"Added savings goal '{name}' with a target amount of {targetAmount:C} and allocation percentage of {allocationPercentage}%.");
            SaveUserData();
        }

        public void AllocateSavings()
        {
            foreach (var goal in _savingsGoals)
            {
                decimal amountToAllocate = (_incomeAccount.Balance * goal.AllocationPercentage) / 100;
                _savingsAccount.Deposit(amountToAllocate);
                _incomeAccount.Withdraw(amountToAllocate);

                Console.WriteLine($"Allocated {goal.AllocationPercentage}% of income to '{goal.Name}' savings goal.");
            }
            SaveUserData();
        }

        public void DisplaySavingsGoals()
        {
            foreach (var goal in _savingsGoals)
            {
                Console.WriteLine($"Savings Goal: {goal.Name}, Target Amount: {goal.TargetAmount:C}, Allocation Percentage: {goal.AllocationPercentage}%");
            }
        }

        public void DisplayBalances()
        {
            Console.WriteLine($"Income Account Balance: {_incomeAccount.Balance:C}");
            Console.WriteLine($"Savings Account Balance: {_savingsAccount.Balance:C}");
        }

        public void DisplayGoalProgress()
        {
            foreach (var goal in _savingsGoals)
            {
                decimal progressPercentage = (_savingsAccount.Balance / goal.TargetAmount) * 100;
                Console.WriteLine($"Progress towards '{goal.Name}' savings goal: {progressPercentage:F2}%");
            }
        }

        private void LoadUserData()
        {
            if (File.Exists(DataFilePath))
            {
                try
                {
                    using (StreamReader reader = new StreamReader(DataFilePath))
                    {
                        string line;
                        while ((line = reader.ReadLine()) != null)
                        {
                            string[] parts = line.Split(',');
                            if (parts.Length == 3)
                            {
                                string name = parts[0];
                                decimal targetAmount = Convert.ToDecimal(parts[1]);
                                decimal allocationPercentage = Convert.ToDecimal(parts[2]);
                                _savingsGoals.Add(new SavingsGoal(name, targetAmount, allocationPercentage));
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error loading user data: {ex.Message}");
                }
            }
        }

        private void SaveUserData()
        {
            try
            {
                using (StreamWriter writer = new StreamWriter(DataFilePath))
                {
                    foreach (var goal in _savingsGoals)
                    {
                        writer.WriteLine($"{goal.Name},{goal.TargetAmount},{goal.AllocationPercentage}");
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error saving user data: {ex.Message}");
            }
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Welcome to Automatic Savings Allocator!");

            Console.Write("Enter your initial income: ");
            decimal initialIncome;
            while (!decimal.TryParse(Console.ReadLine(), out initialIncome) || initialIncome <= 0)
            {
                Console.WriteLine("Invalid input. Please enter a valid positive number for initial income.");
                Console.Write("Enter your initial income: ");
            }

            var allocator = new SavingsAllocator(initialIncome);

            // Allow users to add multiple savings goals
            Console.WriteLine("Add your savings goals:");
            while (true)
            {
                Console.Write("Enter goal name (or type 'done' to finish adding goals): ");
                string goalName = Console.ReadLine().Trim();
                if (goalName.ToLower() == "done")
                    break;

                decimal targetAmount;
                Console.Write("Enter target amount: ");
                while (!decimal.TryParse(Console.ReadLine(), out targetAmount) || targetAmount <= 0)
                {
                    Console.WriteLine("Invalid input. Please enter a valid positive number for target amount.");
                    Console.Write("Enter target amount: ");
                }

                decimal allocationPercentage;
                Console.Write("Enter allocation percentage: ");
                while (!decimal.TryParse(Console.ReadLine(), out allocationPercentage) || allocationPercentage <= 0 || allocationPercentage > 100)
                {
                    Console.WriteLine("Invalid input. Please enter a valid positive number between 0 and 100 for allocation percentage.");
                    Console.Write("Enter allocation percentage: ");
                }

                allocator.AddSavingsGoal(goalName, targetAmount, allocationPercentage);
            }

            Console.WriteLine("Initial Account Balances:");
            allocator.DisplayBalances();
            allocator.DisplaySavingsGoals();

            // Allocate savings
            allocator.AllocateSavings();

            Console.WriteLine("\nUpdated Account Balances:");
            allocator.DisplayBalances();
            allocator.DisplayGoalProgress();
        }
    }
}
