using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;

namespace BayesianNetwork
{
    internal class Parser
    {
        //Fields
        protected List<string> fileContents = new List<string>();//All unique words of a single document 
        protected List<string> stopWords = new List<string>();//List of Stop words
        protected int totalChanged = 0;//Count of total changed words  
        protected static void RemoveCharacters(StreamReader sr, List<string> str)
        {
            List<int> count = new List<int>();
            for (int i = 0; i < str.Count; i++)
            {
                str[i] = str[i].ToLower();
            }
            for (int i = 0; i < str.Count; i++)
            {

                str[i] = str[i].Replace("\r", String.Empty);
                str[i] = str[i].Replace("\"", String.Empty);
                str[i] = str[i].Replace(".", String.Empty);
                str[i] = str[i].Replace(",", String.Empty);
                str[i] = str[i].Replace("?", String.Empty);
                str[i] = str[i].Replace("'s", String.Empty);
                str[i] = str[i].Replace("s'", String.Empty);
                str[i] = str[i].Replace(";", String.Empty);
                str[i] = str[i].Replace(":", String.Empty);
                str[i] = str[i].Replace("�s", String.Empty);
                str[i] = str[i].Replace("s�", String.Empty);
                if (str[i] == String.Empty || str[i] == "")
                {
                    count.Add(i);
                }
            }
            for (int i = 0; i < count.Count; i++)
            {
                str.RemoveAt(count[i] - i);
            }
            sr.Close();
        }
        protected string srcDirectory; //Source directory
        protected void ParseTextDocument(string FileDirectory, string stopWordDoc, string lemmatizationDoc)
        {
            try
            {
                using (StreamReader sr = new StreamReader(FileDirectory))
                {
                    string[] localFileContent;
                    localFileContent = sr.ReadToEnd().Split(' ', '\n', '\t');
                    fileContents = new List<string>(localFileContent);

                    RemoveCharacters(sr, fileContents);

                    if (fileContents.Count == 1 && fileContents[0] == "")
                    {
                        Console.WriteLine("Empty Document");
                        fileContents[0].Remove(0, 1);
                    }
                    sr.Close();
                }
                using (StreamReader sr = new StreamReader(srcDirectory + stopWordDoc))
                {
                    string[] localStopWords;
                    localStopWords = sr.ReadToEnd().Split(' ', '\n', '\t');
                    stopWords = new List<string>(localStopWords);
                    RemoveCharacters(sr, stopWords);
                    sr.Close();
                }
                using (StreamReader sr = new StreamReader(srcDirectory + lemmatizationDoc))
                {
                    string lemmatizationLine;
                    string[] lemmatizationLineArray;


                    while ((lemmatizationLine = sr.ReadLine()) != null)
                    {
                        lemmatizationLineArray = lemmatizationLine.Split(' ', '\n', '\t');

                        for (int i = 1; i < lemmatizationLineArray.Length; i++)
                        {
                            for (int x = 0; x < fileContents.Count; x++)
                            {
                                if (lemmatizationLineArray[i] == fileContents[x])
                                {

                                    fileContents[x] = lemmatizationLineArray[0];
                                    totalChanged++;
                                }
                            }
                        }
                    }
                    sr.Close();

                }

            }
            catch (Exception e)
            {
                Console.WriteLine("\n" + e.Message);
                Console.WriteLine("Press any button to exit.");
                Console.ReadKey();
            }
            if (fileContents.Count == 0)
            {
                Console.WriteLine("Read File as empty!");
                Console.ReadKey();
                Environment.Exit(0);
            }
            int totalRemoved = 0;
            List<int> count = new List<int>();
            for (int i = 0; i < fileContents.Count; i++)
            {

                for (int x = 0; x < stopWords.Count; x++)
                {

                    if (stopWords[x] == fileContents[i])
                    {

                        count.Add(i);

                        totalRemoved++;
                        break;
                    }
                }
            }
            for (int i = 0; i < count.Count; i++)
            {
                fileContents.RemoveAt(count[i] - i);
            }

            Console.WriteLine("\nTotal Lemmatizations: " + totalChanged);
            Console.WriteLine("Total Words Ignored: " + totalRemoved);
            Console.WriteLine("Total Words to be  Added to network: " + fileContents.Count);


        }//Parse document
    }
    internal class Training : Parser
    {
        //This class parses and trains a document to a selected category

        //Fields        
        private List<string> allUniqueWords = new List<string>();//All unique words         
        private List<string> uniqueContents = new List<string>();//All unique words of a single document
        private List<int> frequency = new List<int>();//Frequency of indexed unique words 

        public bool Success { get; private set; } //Loop check in for Main        

        //Private Functions
        //Removing of unwanthed characters
        
        //Removing stopwords and combining lemmatizations     

        //Public Funcitons
        //Instance for clearing data
        public Training()
        {
            DirectoryInfo parentDirectory = Directory.GetParent(Directory.GetCurrentDirectory());
            srcDirectory = parentDirectory + "\\" + "res" + "\\";
        }
        //Use for resetting data to the default set
        public Training(string filename, string stopWordDoc, string lemmatizationDoc)
        {
            DirectoryInfo parentDirectory = Directory.GetParent(Directory.GetCurrentDirectory());
            srcDirectory = parentDirectory + "\\" + "res" + "\\";
            string FileDirectory = srcDirectory + filename + ".txt";

            ParseTextDocument(FileDirectory, stopWordDoc, lemmatizationDoc);
            Success = true;
        }
        //Use for creating data manually   
        public Training(string stopWordDoc, string lemmatizationDoc)
        {
            Console.Clear();
            Console.WriteLine("Please enter the name of the desired training text file.");



            DirectoryInfo parentDirectory = Directory.GetParent(Directory.GetCurrentDirectory());
            srcDirectory = parentDirectory + "\\" + "res" + "\\";
            string FileDirectory = srcDirectory + Console.ReadLine() + ".txt"; ;



            if (File.Exists(FileDirectory))
            {
                ParseTextDocument(FileDirectory, stopWordDoc, lemmatizationDoc);
                Success = true;
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
        //Obtain unique words and its frequencies
        public void ObtainTeachingData(string catMasterDoc)
        {
            try
            {
                using (StreamReader sr = new StreamReader(srcDirectory + catMasterDoc))
                {
                    if (sr.ReadLine() == null)
                    {
                        sr.Close();
                        
                        uniqueContents = fileContents.Distinct().ToList();

                        for (int i = 0; i < uniqueContents.Count; i++)
                        {
                            int count = 0;
                            for (int j = 0; j < fileContents.Count; j++)
                            {
                                if (uniqueContents[i] == fileContents[j])
                                {
                                    count++;
                                }
                            }
                            frequency.Add(count);                                               
                        }                        

                        StreamWriter sw = new StreamWriter(srcDirectory + catMasterDoc);
                        for (int i = 0; i < uniqueContents.Count; i++)
                        {
                            sw.WriteLine(uniqueContents[i] + " " + frequency[i]);
                           
                        }
                        sw.Close();

                    }
                    else
                    {
                        sr.Close();
                        StreamReader sr1 = new StreamReader(srcDirectory + catMasterDoc);
                        string line;
                        string[] lineArray;


                        while ((line = sr1.ReadLine()) != null)
                        {
                            lineArray = line.Split(' ');

                            uniqueContents.Add(lineArray[0]);
                            frequency.Add(Int32.Parse(lineArray[1]));

                        }
                        sr1.Close();


                        for (int i = 0; i < fileContents.Count; i++)
                        {
                            bool newWord = true;
                            for (int j = 0; j < uniqueContents.Count(); j++)
                            {
                                if (fileContents[i] == uniqueContents[j])
                                {
                                    frequency[j]++;

                                    newWord = false;
                                }
                            }
                            if (newWord == true)
                            {
                                uniqueContents.Add(fileContents[i]);
                                frequency.Add(1);
                            }
                        }                        

                        System.IO.File.WriteAllText(srcDirectory + catMasterDoc, string.Empty);
                        StreamWriter sr2 = new StreamWriter(srcDirectory + catMasterDoc);

                        for (int i = 0; i < uniqueContents.Count(); i++)
                        {
                            sr2.WriteLine(uniqueContents[i] + " " + frequency[i]);
                        }
                        sr2.Close();
                    }


                    
                    Console.WriteLine("\nDocument Learned!: "+ catMasterDoc);


                }
            }
            catch (Exception e)
            {
                Console.WriteLine("\n" + e.Message);
                Console.WriteLine("Press any button to exit.");
                Console.ReadKey();
                Environment.Exit(0);
            }
        }
        //Use for resetting data to the default set
        public void Reset(string c, string l, string d, string m)
        {
            try
            {
                File.WriteAllText(srcDirectory + c, String.Empty);
                File.WriteAllText(srcDirectory + l, String.Empty);
                File.WriteAllText(srcDirectory + d, String.Empty);
                File.WriteAllText(srcDirectory + m, String.Empty);
                StreamWriter sw = new StreamWriter(srcDirectory + m);

                sw.WriteLine("Conservative 0");
                sw.WriteLine("Labor 0");
                sw.WriteLine("LibDemCon 0");
                sw.WriteLine("Count 0");
                sw.Close();
            }
            catch (Exception e)
            {
                Console.WriteLine("\n"+e.Message);
                Console.WriteLine("Press any button to exit.");
                Console.ReadKey();
                Environment.Exit(0);
            }

        }
        //Adding dictionary to a selected category master document
        public void SetMaster(int type, string master, string conservativeMaster, string laborMaster, string libDemConMaster)
        {


            string line;
            string[] lineArray;
            List<string> lineList = new List<string>();


            try
            {
                using (StreamReader sr = new StreamReader(srcDirectory + master))
                {

                    while ((line = sr.ReadLine()) != null)
                    {
                        lineArray = line.Split(' ');
                        lineList.Add(lineArray[0]);
                        lineList.Add(lineArray[1]);


                    }
                    sr.Close();
                    line = null;
                    lineArray = null;

                    StreamWriter sw = new StreamWriter(srcDirectory + master);

                    StreamReader conservativeDoc = new StreamReader(srcDirectory + conservativeMaster);
                    StreamReader laborDoc = new StreamReader(srcDirectory + laborMaster);
                    StreamReader LibDemConCoalitionDoc = new StreamReader(srcDirectory + libDemConMaster);



                    while ((line = conservativeDoc.ReadLine()) != null)
                    {
                        lineArray = line.Split(' ');
                        allUniqueWords.Add(lineArray[0]);


                    }
                    while ((line = laborDoc.ReadLine()) != null)
                    {
                        lineArray = line.Split(' ');
                        allUniqueWords.Add(lineArray[0]);


                    }
                    while ((line = LibDemConCoalitionDoc.ReadLine()) != null)
                    {
                        lineArray = line.Split(' ');
                        allUniqueWords.Add(lineArray[0]);


                    }
                    allUniqueWords = allUniqueWords.Distinct().ToList();
                    conservativeDoc.Close();
                    laborDoc.Close();
                    LibDemConCoalitionDoc.Close();


                    switch (type)
                    {

                        case 0:

                            for (int i = 0; i < lineList.Count; i++)
                            {
                                if (lineList[i] == "Conservative")
                                {
                                    lineList[i + 1] = (Int32.Parse(lineList[i + 1]) + 1).ToString();
                                    break;
                                }
                            }
                            for (int i = 0; i < lineList.Count; i++)
                            {
                                if (lineList[i] == "Count")
                                {
                                    lineList[i + 1] = allUniqueWords.Count.ToString();
                                    break;
                                }
                            }

                            for (int i = 0; i < lineList.Count; i = i + 2)
                            {
                                line = lineList[i] + " " + lineList[i + 1];
                                sw.WriteLine(line);
                            }
                            sw.Close();

                            break;
                        case 1:
                            for (int i = 0; i < lineList.Count; i++)
                            {
                                if (lineList[i] == "Labor")
                                {
                                    lineList[i + 1] = (Int32.Parse(lineList[i + 1]) + 1).ToString();
                                    break;
                                }
                            }
                            for (int i = 0; i < lineList.Count; i++)
                            {
                                if (lineList[i] == "Count")
                                {
                                    lineList[i + 1] = allUniqueWords.Count.ToString();
                                    break;
                                }
                            }

                            for (int i = 0; i < lineList.Count; i = i + 2)
                            {
                                line = lineList[i] + " " + lineList[i + 1];
                                sw.WriteLine(line);
                            }
                            sw.Close();

                            break;
                        case 2:
                            for (int i = 0; i < lineList.Count; i++)
                            {
                                if (lineList[i] == "LibDemCon")
                                {
                                    lineList[i + 1] = (Int32.Parse(lineList[i + 1]) + 1).ToString();
                                    break;
                                }
                            }
                            for (int i = 0; i < lineList.Count; i++)
                            {
                                if (lineList[i] == "Count")
                                {
                                    lineList[i + 1] = allUniqueWords.Count.ToString();
                                    break;
                                }
                            }

                            for (int i = 0; i < lineList.Count; i = i + 2)
                            {
                                line = lineList[i] + " " + lineList[i + 1];
                                sw.WriteLine(line);
                            }
                            sw.Close();

                            break;
                    }







                }
            }
            catch (Exception e)
            {
                Console.WriteLine("\n" + e.Message);
                Console.WriteLine("Press any button to exit.");
                Console.ReadKey();
                Environment.Exit(0);
            }


        }        
    }      
    
       
    
}
