using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;

namespace BayesianNetwork
{
    internal class CategoryTable : ExeptionMethods
    {
        //This class contains a document's word probabilities to a selected category
        //Which is used in classification class

        //Fields
        private List<string> word = new List<string>();
        private List<int> FCatWord = new List<int>();
        private List<double> LogPWordCat = new List<double>();
        private readonly int totalWordCount = 0;

        //Public Functions
        //Get selected word probability
        public double GetPWordCat(string desireWord, double totalUniqueWords)
        {
            for (int i = 0; i < word.Count; i++)
            {
                if (desireWord == word[i])
                {
                    return LogPWordCat[i];
                }
            }
            return (Math.Log(((0 + 1) / (totalWordCount + totalUniqueWords))));
        }

        public CategoryTable(string fileLocation, double totalUniqueWords)
        {
            try
            {
                StreamReader sr = new StreamReader(fileLocation);

                    string line;
                    string[] lineArray;
                    while ((line = sr.ReadLine()) != null)
                    {
                        lineArray = line.Split(' ');
                        word.Add(lineArray[0]);
                        totalWordCount += Convert.ToInt32(lineArray[1]);
                        FCatWord.Add(Convert.ToInt32(lineArray[1]));
                    }
                sr.Close();  

                for (int i = 0; i < word.Count; i++)
                {
                    double temp = Math.Log(Convert.ToDouble(FCatWord[i] + 1) / (totalWordCount + totalUniqueWords));

                    LogPWordCat.Add(temp);
                }
            }
            catch (Exception e)
            {
                ExeptionMethod(e);
            }
        }
    }
    internal class Classification : Parser
    {
        //This class calculates the highest probability of an unknown document        
       
        //Tables of each category
        private readonly CategoryTable tableConservative;
        private readonly CategoryTable tableLabor;
        private readonly CategoryTable tableLibDem;

        //List of word probabilities of unknown document for each category
        private readonly List<double> PCatConDoc = new List<double>();
        private readonly List<double> PCatLaborDoc = new List<double>();
        private readonly List<double> PCatConservativeConseravtiveCoalitionDoc = new List<double>();


        //Private Methods
        //Removing of unwanthed characters

        //Calculating final probability document belonging to category
#pragma warning disable RECS0082 // Parameter has the same name as a member and hides it
        private double CalculatePCatDoc(CategoryTable tableConservative, double totalUniqueWords, double PCatParty, List<double> PCatPartyDoc)
#pragma warning restore RECS0082 // Parameter has the same name as a member and hides it
        {
            double CatLogTotal=0;
            for (int i = 0; i < fileContents.Count; i++)
            {
                PCatPartyDoc.Add(tableConservative.GetPWordCat(fileContents[i], totalUniqueWords));
            }
            if (PCatPartyDoc.Any())
            {
                PCatPartyDoc.Add(PCatParty);
            }
            else
            {
                PCatPartyDoc.Add(0);
                PCatPartyDoc.Add(PCatParty);
            }

            for (int i = 0; i < PCatPartyDoc.Count() - 1; i++)
            {
                CatLogTotal += PCatPartyDoc[i];
            }
            return CatLogTotal;

        }
        //Removing stopwords and combining lemmatizations
        
        //Public Methods
        public Classification(string conservativeDoc, string laborDoc, string libDemConDoc,string master,string stopWordDoc,string lemmatizationDoc)
        {
            try
            {
                SetSourceDirectory();
                Console.WriteLine("Please enter the name of the .txt document you wish to predict.");
                Console.WriteLine("Name of included test documents: 'test1', 'test2', 'test3'");
                string FileDirectory = srcDirectory + Console.ReadLine() + ".txt";
                if (File.Exists(FileDirectory))
                {
                    ParseTextDocument(FileDirectory, stopWordDoc, lemmatizationDoc);

                    StreamReader sr = new StreamReader(srcDirectory + master);
                   
                    double totalConservative = 0;
                    double totalLabour = 0;
                    double totalLibDemCon = 0;
                    double totalUniqueWords = 0;

                    string[] tempMaster = sr.ReadToEnd().Split(' ', '\n', '\t');
                    for (int i = 0; i < tempMaster.Length; i++)
                    {
                        if (tempMaster[i] == "Conservative")
                        {
                            totalConservative = double.Parse(tempMaster[i + 1]);
                        }
                        if (tempMaster[i] == "Labor")
                        {
                            totalLabour = double.Parse(tempMaster[i + 1]);
                        }
                        if (tempMaster[i] == "LibDemCon")
                        {
                            totalLibDemCon = double.Parse(tempMaster[i + 1]);
                        }
                        if (tempMaster[i] == "Count")
                        {
                            totalUniqueWords = int.Parse(tempMaster[i + 1]);
                        }

                        sr.Close();
                    }
                    if (Math.Abs((totalConservative + totalLabour + totalLibDemCon)) < 1e-11)
                    {
                        Console.WriteLine("\nNo words were found in the network");
                        Console.ReadKey();
                        return;
                    }
                    double PCatConservative = Math.Log((totalConservative / (totalConservative + totalLabour + totalLibDemCon)));

                    double PCatLabor = Math.Log((totalLabour / (totalConservative + totalLabour + totalLibDemCon)));

                    double PCatConservativeLibDemCoalition = Math.Log((totalLibDemCon / (totalConservative + totalLabour + totalLibDemCon)));

                    tableConservative = new CategoryTable(srcDirectory + conservativeDoc, totalUniqueWords);
                    tableLabor = new CategoryTable(srcDirectory + laborDoc, totalUniqueWords);
                    tableLibDem = new CategoryTable(srcDirectory + libDemConDoc, totalUniqueWords);

                    double consLogTotal = CalculatePCatDoc(tableConservative, totalUniqueWords, PCatConservative, PCatConDoc);
                    double laborLogTotal = CalculatePCatDoc(tableLabor, totalUniqueWords, PCatLabor, PCatLaborDoc);
                    double libDemConLogTotal = CalculatePCatDoc(tableLibDem, totalUniqueWords, PCatConservativeLibDemCoalition, PCatConservativeConseravtiveCoalitionDoc);

                    if (consLogTotal > laborLogTotal && consLogTotal > libDemConLogTotal)
                    {
                        Console.WriteLine("\nMost Likely: Conservative");
                    }
                    else if (laborLogTotal > consLogTotal && laborLogTotal > libDemConLogTotal)
                    {
                        Console.WriteLine("\nMost Likely: Labor");
                    }
                    else if (libDemConLogTotal > consLogTotal && libDemConLogTotal > laborLogTotal)
                    {
                        Console.WriteLine("\nMost Likely: Liberal Democrats / Conservative Coalition");
                    }
                    else
                    {
                        Console.WriteLine("\nCannot Determine Party!");
                    }

                    Console.WriteLine("\nConservative                              LogE(Probability) = " + consLogTotal);
                    Console.WriteLine("Labor                                     LogE(Probability) = " + laborLogTotal);
                    Console.WriteLine("Conservative/Liberal Democrats Coalition  LogE(Probability) = " + libDemConLogTotal);
                    Console.WriteLine("\nNB: The unknown document is classified to the most posative LogE(Probability) category.");
                    Console.WriteLine("\nPress any KEY to continue.");

                    Console.ReadKey();
                }
                else
                {
                    Console.Clear();
                    Console.WriteLine(FileDirectory + " Doesn't exist!");
                    Console.WriteLine("Please enter the name of a valid .txt file.");
                    Console.WriteLine("Press any key to restart.");
                    Console.WriteLine("Or Press ESC to exit.");

                    if (Console.ReadKey().Key == ConsoleKey.Escape)
                    {
                        Environment.Exit(0);
                    }
                }
            }
            catch(Exception e)
            {
                ExeptionMethod(e);
            }
        }
    }
}
