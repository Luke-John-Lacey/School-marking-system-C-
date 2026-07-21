using System;
using System.Collections.Generic;

namespace StudentExamEligibility
{
    public class Program
    {
        private const double PassMark = 50.0;

        private static readonly Dictionary<char, (double Min, double Max)> GradeBoundaries = new()
        {
            { 'A', (80.0, 100.0) },
            { 'B', (70.0, 79.99) },
            { 'C', (60.0, 69.99) },
            { 'D', (50.0, 59.99) },
            { 'F', (0.0, 49.99) }
        };

        private static readonly (string Name, double Weight)[] Assessments =
        {
            ("Test 1", 0.30),
            ("Test 2", 0.50),
            ("Assignment 1", 0.10),
            ("Project", 0.10),
        };

        static void Main(string[] args)
        {
            AssertWeightsSumToOne();
            PrintHeader();

            bool continueCalculating = true;
            while (continueCalculating)
            {
                string studentName = GetStudentName();

                Console.WriteLine();
                Console.WriteLine("INPUT PHASE");
                Console.WriteLine("─────────────────────────────────────────");
                double[] marks = new double[Assessments.Length];
                for (int i = 0; i < Assessments.Length; i++)
                    marks[i] = ReadMark(Assessments[i].Name, Assessments[i].Weight);

                Console.WriteLine("─────────────────────────────────────────");
                Console.WriteLine("CALCULATING RESULTS");
                Console.WriteLine("─────────────────────────────────────────");
                double total = CalculateWeightedAverage(marks);

                PrintResult(studentName, total);

                continueCalculating = PromptAnotherStudent();
                Console.WriteLine();
            }

            Console.WriteLine("Thank you for using the Exam Eligibility Calculator. Goodbye!");
        }

        private static string GetStudentName()
        {
            while (true)
            {
                Console.Write(">> Enter student name: ");
                string input = Console.ReadLine();

                if (string.IsNullOrWhiteSpace(input))
                {
                    WriteLineColored("   ! Student name is required.", ConsoleColor.Red);
                    continue;
                }

                return input.Trim();
            }
        }

        private static double ReadMark(string assessmentName, double weight)
        {
            while (true)
            {
                Console.Write($">> Enter {assessmentName} mark (0-100): ");
                string input = Console.ReadLine();

                if (string.IsNullOrWhiteSpace(input))
                {
                    WriteLineColored("   ! Mark required. Please enter a number.", ConsoleColor.Red);
                    continue;
                }

                if (!double.TryParse(input.Trim(), out double mark))
                {
                    WriteLineColored($"   ! '{input}' is not a valid number. Try again.", ConsoleColor.Red);
                    continue;
                }

                if (mark < 0 || mark > 100)
                {
                    WriteLineColored($"   ! Mark must be between 0 and 100 (got {mark}).", ConsoleColor.Red);
                    continue;
                }

                return mark;
            }
        }

        private static double CalculateWeightedAverage(double[] marks)
        {
            double total = 0.0;
            for (int i = 0; i < Assessments.Length; i++)
            {
                double contribution = marks[i] * Assessments[i].Weight;
                total += contribution;
                Console.WriteLine($"{Assessments[i].Name}: {marks[i]} × {Assessments[i].Weight:F2} = {contribution:F2}");
            }
            Console.WriteLine($"TOTAL WEIGHTED AVERAGE: {total:F2}%");
            return total;
        }

        private static char GetGradeLetters(double percentage)
        {
            foreach (var grade in GradeBoundaries)
            {
                if (percentage >= grade.Value.Min && percentage <= grade.Value.Max)
                    return grade.Key;
            }
            return 'F';
        }

        private static void AssertWeightsSumToOne()
        {
            double sum = 0.0;
            foreach (var assessment in Assessments)
            {
                sum += assessment.Weight;
            }

            if (Math.Abs(sum - 1.0) > 0.0001)
                throw new InvalidOperationException($"Assessment weights must sum to 1.0 (got {sum}).");
        }

        private static void PrintHeader()
        {
            WriteLineColored("╔════════════════════════════════════════════╗", ConsoleColor.Cyan);
            WriteLineColored("     STUDENT EXAM ELIGIBILITY CALCULATOR",     ConsoleColor.Cyan);
            WriteLineColored("╚════════════════════════════════════════════╝", ConsoleColor.Cyan);
            Console.WriteLine("Cloud Native Programming - CNA271");
            Console.WriteLine();
            Console.WriteLine("Enter marks for 4 assessments (0-100):");
            foreach (var assessment in Assessments)
            {
                Console.WriteLine($"  ─ {assessment.Name} ({assessment.Weight:P0} weight)");
            }
            Console.WriteLine($"Minimum: {PassMark} to qualify for exam");
            Console.WriteLine("─────────────────────────────────────────");
        }

        private static void PrintResult(string studentName, double total)
        {
            char grade = GetGradeLetters(total);
            bool borderline = Math.Abs(total - PassMark) < 0.005;

            Console.WriteLine("─────────────────────────────────────────");
            Console.WriteLine($"Student: {studentName}");
            Console.WriteLine($"Weighted Average: {total:F2}%");
            Console.WriteLine($"Grade: {grade}");
            Console.WriteLine("─────────────────────────────────────────");

            if (total >= PassMark)
            {
                var color = borderline ? ConsoleColor.Yellow : ConsoleColor.Green;
                WriteLineColored("✓ QUALIFIED FOR EXAM", color);
                string suffix = borderline ? " — Exactly at minimum threshold" : "";
                WriteLineColored($"Status: Pass ({total:F2} ≥ {PassMark:F0}){suffix}", color);
            }
            else
            {
                double shortfall = PassMark - total;
                WriteLineColored("✗ NOT QUALIFIED", ConsoleColor.Red);
                WriteLineColored($"Status: Fail ({total:F2} < {PassMark:F0}) — Need {shortfall:F2} more points", ConsoleColor.Red);
            }
        }

        private static bool PromptAnotherStudent()
        {
            while (true)
            {
                Console.Write("Calculate another student? (yes/no): ");
                string input = Console.ReadLine();

                if (string.IsNullOrWhiteSpace(input))
                    continue;

                string answer = input.Trim().ToLower();
                if (answer == "yes" || answer == "y")
                    return true;
                if (answer == "no" || answer == "n")
                    return false;

                WriteLineColored("   ! Please enter 'yes' or 'no'.", ConsoleColor.Red);
            }
        }

        private static void WriteLineColored(string text, ConsoleColor color)
        {
            Console.ForegroundColor = color;
            Console.WriteLine(text);
            Console.ResetColor();
        }
    }
}