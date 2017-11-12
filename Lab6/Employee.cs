using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lab6
{
    internal class Employee
    {
        public string LastName { get; }
        public string SecondName { get; }
        public string FirstName { get; }
        public Gender Gender { get; }
        public int Age { get; }
        public double Salary { get; }

        private Employee(string lastName, string secondName, string firstName, Gender gender, int age, double salary)
        {
            LastName = lastName;
            SecondName = secondName;
            FirstName = firstName;
            Gender = gender;
            Age = age;
            Salary = salary;
        }

        public static Employee Parse(string str)
        {
            var strs = str.Split(';');
            try
            {
                return new Employee(strs[0], strs[1], strs[2], (Gender)Enum.Parse(typeof(Gender), strs[3], true), int.Parse(strs[4]),
                    double.Parse(strs[5]));
            }
            catch
            {
                throw new FormatException("Входная строка имела неверный формат");
            }
        }

        public override string ToString()
        {
            return string.Format("| {0,-15} | {1,-10} | {2,-15} | Пол: {5,3} | Возраст: {3,3} | з/п: {4,5}р. |", LastName, SecondName, FirstName, Age, Salary, Gender == Gender.Male ? "муж" : (Gender == Gender.Female ? "жен" : "нет"));
        }
    }
}
