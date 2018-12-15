//Rommulluss Caraiman

using System;
using System.Collections.Generic;
using System.IO;
namespace BayesianNetwork
{
    static internal class Program
    {
        //file location constants
        private const string stopWordDoc = "library//stopwords.txt";
        private const string lemmatizationDoc = "library//lemmatization.txt";
        private const string conservativeMaster = "master//ConservativesMaster.txt";
        private const string LaborMaster = "master//LaborMaster.txt";
        private const string LibDemConMaster = "master//LibDemConMaster.txt";
        private const string Master = "master//Master.txt";

        //While loop and handles main menu input
        static void Main()
        {                     
            while (true)
            {                                         
                switch(WelcomeMessage())
                {
                    case 0:
                        break;
                    case 1:
                        Training trainObj = new Training(stopWordDoc, lemmatizationDoc);                        


                        if (trainObj.Success != true)
                        {
                            
                            Main();
                           
                        }
                        else
                        {
                            bool done=false;
                            do
                            {
                                
                                Console.WriteLine("\nSelect which party was in power for the speech.");
                                Console.WriteLine("C = Conservatives\nL = Labor\nD = Liberal Democrats / Conservative Coalition");
                                Console.WriteLine("\nOr press Esc to exit.");
                                var input = Console.ReadKey();
                                switch (input.Key)
                                {
                                    case ConsoleKey.C:
                                        trainObj.ObtainTeachingData(conservativeMaster);
                                        trainObj.SetMaster(0, Master,conservativeMaster, LaborMaster, LibDemConMaster);
                                        done = true;
                                        break;
                                    case ConsoleKey.L:
                                        trainObj.ObtainTeachingData(LaborMaster);
                                        trainObj.SetMaster(1, Master, conservativeMaster, LaborMaster, LibDemConMaster);
                                        done = true;
                                        break;

                                    case ConsoleKey.D:
                                        trainObj.ObtainTeachingData(LibDemConMaster);
                                        trainObj.SetMaster(2, Master, conservativeMaster, LaborMaster, LibDemConMaster);
                                        done = true;
                                        break;
                                    case ConsoleKey.Escape:
                                        Environment.Exit(0);
                                        break;
                                    default:
                                        Console.Clear();
                                        Console.WriteLine("Please enter valid input!");
                                        break;
                                }
                            }
                            while (done == false);

                        }
                        break;
                    case 2:
                        Classification class1 = new Classification(conservativeMaster,LaborMaster,LibDemConMaster,Master,stopWordDoc,lemmatizationDoc);


                        Console.Clear();
                        
                        break;
                    case 3:
                        Training eraseTraining = new Training();
                        eraseTraining.Reset(conservativeMaster,LaborMaster,LibDemConMaster,Master);
                        Console.WriteLine("Data erased!\nPress any key to continue.");
                        Console.ReadKey();
                        break;
                    case 4:

                        Training resetTraining = new Training();
                        resetTraining.Reset(conservativeMaster, LaborMaster, LibDemConMaster, Master);

                        resetTraining = new Training("originalTeachingData/Coalition9thMay2012",  stopWordDoc,  lemmatizationDoc);
                        resetTraining.ObtainTeachingData(LibDemConMaster);
                        resetTraining.SetMaster(2, Master, conservativeMaster, LaborMaster, LibDemConMaster);

                        resetTraining = new Training("originalTeachingData/Conservative16thNov1994", stopWordDoc, lemmatizationDoc);
                        resetTraining.ObtainTeachingData(conservativeMaster);
                        resetTraining.SetMaster(0, Master, conservativeMaster, LaborMaster, LibDemConMaster);

                        resetTraining = new Training("originalTeachingData/Conservative27thMay2015", stopWordDoc, lemmatizationDoc);
                        resetTraining.ObtainTeachingData(conservativeMaster);
                        resetTraining.SetMaster(0, Master, conservativeMaster, LaborMaster, LibDemConMaster);

                        resetTraining = new Training("originalTeachingData/Labour6thNov2007", stopWordDoc, lemmatizationDoc);
                        resetTraining.ObtainTeachingData(LaborMaster);
                        resetTraining.SetMaster(1, Master, conservativeMaster, LaborMaster, LibDemConMaster);

                        resetTraining = new Training("originalTeachingData/Labour26thNov2003", stopWordDoc, lemmatizationDoc);
                        resetTraining.ObtainTeachingData(LaborMaster);
                        resetTraining.SetMaster(1, Master, conservativeMaster, LaborMaster, LibDemConMaster);



                        Console.WriteLine("Press any KEY to continue.");

                        Console.ReadKey();
                        break;
                }
               
                


            }                       
        }
        //Displays Welcome Message
        static int WelcomeMessage()
        {
            Console.Clear();
            Console.WriteLine("Welcome to a Text Classification Program Using Naive Bayes.\n\nThis program predicts political party currently in power from Queen's speeches acquired from the State Openings of Parliament.");
            
            Console.WriteLine("\nWithin the '/res' file, there are all the Queen's speeches from 1996 that you can use to teach the network or to test classification.");
            Console.WriteLine("\nIf this is first time run, please teach the netowrk by (R)eset or (T)eaching.");
            Console.WriteLine("\nSelect the following command.");
            Console.WriteLine("\nPress T to select and set TRAINING data.\nPress C for CLASSIFICATION.\nPress E to ERASE learned data.\nPress R to RESET (Using the TRAINING data provided by assesment)\n\nOr press Esc to exit.");
            var input = Console.ReadKey();
            Console.Clear();
            switch (input.Key) //Switch on Key enum
            {
                case ConsoleKey.T:       
                    //Teach
                    return 1;
                    
                case ConsoleKey.C:     
                    //Classify
                    return 2;

                case ConsoleKey.E:
                    //Erase
                    Console.WriteLine("Are you want to DELETE the data set?");
                    Console.WriteLine("Y/N");
                    var input1 = Console.ReadKey();
                    switch (input1.Key)
                    {
                        case ConsoleKey.Y:
                            Console.WriteLine();
                            return 3;
                        default:
                            return 0;
                    }

                case ConsoleKey.R:
                    //Erase
                    Console.WriteLine("Are you sure want to RESET the data set?");
                    Console.WriteLine("Y/N");
                    var input2 = Console.ReadKey();
                    switch (input2.Key)
                    {
                        case ConsoleKey.Y:
                            Console.WriteLine();
                            return 4;
                        default:
                            return 0;
                    }


                case ConsoleKey.Escape:
                    //exit
                   
                    Environment.Exit(0);
                    return 0;
                    
                default:
                    //restart

                  
                    
                    return 0;
            }
        }
        
    }
    
}
