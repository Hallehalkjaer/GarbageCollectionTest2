using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading;

namespace GarbageCollectionTest2
{
    class Program
    {
        static bool checkForNotify = true;
        static bool bAllocate = false;
        static bool finalExit = false;
        static List<byte[]> load = new List<byte[]>();


        static void Main(string[] args)
        {
            TraceSource ts = new TraceSource("GC");
            ts.Switch = new SourceSwitch("GCTest", "all");
            TraceListener txtListener = new TextWriterTraceListener(new StreamWriter("GCTest.txt") { AutoFlush = true });
            ts.Listeners.Add(txtListener);
            Animal testanimal = new Animal("testFirst", 0, false);

            void TestAnimalInfo()
            {
                ts.TraceEvent(TraceEventType.Information, 2, "student info  " + GC.GetGeneration(testanimal));
            }
            TestAnimalInfo();
            Animal testanimal2 = testanimal;


            void GCInfo()
            {
                ts.TraceEvent(TraceEventType.Information, 0, "Hvor mange collection i Gen0 " + GC.CollectionCount(0));
                ts.TraceEvent(TraceEventType.Information, 0, "Hvor mange collection i Gen1 " + GC.CollectionCount(1));
                ts.TraceEvent(TraceEventType.Information, 0, "Hvor mange collection i Gen2 " + GC.CollectionCount(2));
                ts.TraceEvent(TraceEventType.Information, 1, "Generation collected " + GC.GetGCMemoryInfo().Generation);
                ts.TraceEvent(TraceEventType.Information, 2, "Heapsize Excluding Fragmentation " + GC.GetTotalMemory(false));
                ts.TraceEvent(TraceEventType.Information, 3, "committed bytes " + GC.GetGCMemoryInfo().TotalCommittedBytes);
                ts.TraceEvent(TraceEventType.Information, 4, "size of heap " + GC.GetGCMemoryInfo().HeapSizeBytes);
                ts.TraceEvent(TraceEventType.Information, 5, "pause cause by collection of 0&1 " + GC.GetGCMemoryInfo().PauseDurations[0].TotalSeconds);
                ts.TraceEvent(TraceEventType.Information, 6, "pause% of total runtime of app " + GC.GetGCMemoryInfo().PauseTimePercentage);
                ts.TraceEvent(TraceEventType.Information, 7, "promoted bytes " + GC.GetGCMemoryInfo().PromotedBytes);
            }




            //GCInfo();
            //GC.Collect();
            //GCInfo();
            ts.TraceEvent(TraceEventType.Information, 0, "info efter objekter er oprettet men før collection er kaldt ");
            Console.WriteLine("Hello World!");
            List<Animal> animals = new List<Animal>();
            GarbageCreator gc = new GarbageCreator();
            gc.MakeGarbage(animals);
            //GC.Collect();
            GCInfo();
            TestAnimalInfo();



            //test af hastighed af collecction af gen0 vs full collection
            ts.TraceEvent(TraceEventType.Information, 0, "\nNæste to er efter collection første collection af gen0, og så info efter full collection");
            GC.Collect(0);
            GCInfo();
            GC.Collect(2);
            GCInfo();

            gc.DeleteGarbage(animals);
            Console.WriteLine("collectiong");
            ts.TraceEvent(TraceEventType.Information, 0, "\ninfo efter sletning af 20% af listen");
            GC.Collect();
            GCInfo();
            TestAnimalInfo();

            //collecting without deleting anything to see if promote is 0
            ts.TraceEvent(TraceEventType.Information, 0, "\ncollection uden noget andet er sket for at tjekke om promote forbliver det samme alså der ikke er flere at promovere");
            GC.Collect();
            GCInfo();
            Console.WriteLine("collected");

            //clear list 
            ts.TraceEvent(TraceEventType.Information, 0, "\ninfo efte alle animals er slettet ");
            gc.DeleteAll(animals);
            GC.Collect();
            GCInfo();


            //delete list object
            ts.TraceEvent(TraceEventType.Information, 0, "\ninfor efter listen er gjort null");
            animals = null;
            testanimal = null;
            GC.Collect();
            GCInfo();

            GC.RegisterForFullGCNotification(10, 10);
            List<Animal> animals2 = new List<Animal>();
            bool finalExit = false;
            Thread thWaitForFullGC = new Thread(new ThreadStart(WaitForFullGCProc));
            thWaitForFullGC.Start();
            int lastCollCount = 0;
            int newCollCount = 0;
            checkForNotify = true;
            bAllocate = true;
            while (true)
            {
                if (bAllocate)
                {
                    gc.MakeGarbage(animals2);
                    newCollCount = GC.CollectionCount(2);
                    if (newCollCount != lastCollCount)
                    {
                        // Show collection count when it increases:
                        Console.WriteLine("Gen 2 collection count: {0}", GC.CollectionCount(2).ToString());
                        ts.TraceEvent(TraceEventType.Information, 98, "heap size " + GC.GetGCMemoryInfo().HeapSizeBytes);
                        lastCollCount = newCollCount;
                    }
                    if (newCollCount == 10)
                    {
                        finalExit = true;
                        checkForNotify = false;
                        break;
                    }
                }
            }
            finalExit = true;
            checkForNotify = false;
            GC.CancelFullGCNotification();

            //try {
            //    Console.WriteLine("new shit happening");
            //    GC.RegisterForFullGCNotification(10, 10);
            //    Console.WriteLine("registered for notification");
            //    bool checkForNotif = true;
            //    bool bAllocate = true;
            //    bool finalExit = false;

            //    Thread thWaitForFullGC = new Thread(new ThreadStart(WaitForFullGCProc));
            //    thWaitForFullGC.Start();
            //    try
            //    {

            //        int lastCollCount = 0;
            //        int newCollCount = 0;

            //        while (true)
            //        {
            //            if (bAllocate)
            //            {
            //                load.Add(new byte[10]);
            //                newCollCount = GC.CollectionCount(2);
            //                if (newCollCount != lastCollCount)
            //                {
            //                    // Show collection count when it increases:
            //                    Console.WriteLine("Gen 2 collection count: {0}", GC.CollectionCount(2).ToString());
            //                    ts.TraceEvent(TraceEventType.Information, 98, "heap size " + GC.GetGCMemoryInfo().HeapSizeBytes);
            //                    lastCollCount = newCollCount;
            //                }

            //                // For ending the example (arbitrary).
            //                if (newCollCount == 10)
            //                {
            //                    finalExit = true;
            //                    checkForNotif = false;
            //                    break;
            //                }
            //            }
            //        }
            //    }

            //    catch (OutOfMemoryException)
            //    {
            //        Console.WriteLine("out of memory");
            //    }

            //}
            //catch (InvalidOperationException invalidOp)
            //{

            //    Console.WriteLine("GC Notifications are not supported while concurrent GC is enabled.\n"
            //        + invalidOp.Message);
            //}

            void OnFullGCApproachNotif()
            {
                RedirectRequests();
                GC.Collect();
                Console.WriteLine("collection called");
            }
            void OnFullGCCompleteEndNotify()
            {
                AcceptRequests();
            }
            void WaitForFullGCProc()
            {
                while (true)
                {
                    while (checkForNotify)
                    {
                        GCNotificationStatus s = GC.WaitForFullGCApproach();
                        if (s == GCNotificationStatus.Succeeded)
                        {
                            Console.WriteLine("GC notif send");
                            OnFullGCApproachNotif();
                        }
                        else if (s == GCNotificationStatus.Canceled)
                        {
                            Console.WriteLine("GC notif cancelled");
                            break;
                        }
                        else
                        {
                            break;
                        }
                        GCNotificationStatus status = GC.WaitForFullGCComplete();
                        if (status == GCNotificationStatus.Succeeded)
                        {
                            Console.WriteLine("GC notif raised");
                            OnFullGCCompleteEndNotify();
                        }
                        else if (status == GCNotificationStatus.Canceled)
                        {
                            Console.WriteLine("GC notif Cnacelled");
                            break;
                        }
                        else
                            break;


                    }
                    Thread.Sleep(500);
                    if (finalExit)
                    {
                        break;
                    }
                }
            }
            void RedirectRequests()
            {
                bAllocate = false;
            }
            void AcceptRequests()
            {
                bAllocate = true;
            }
        }
        //public static void OnFullGCApproachNotif()
        //{
        //    RedirectRequests();
        //    FinishingExistingRequests();
        //    GC.Collect();
        //    Console.WriteLine("collection called");
        //}
        //public static void OnFullGCCompleteEndNotify()
        //{
        //    AcceptRequests();
        //}
        //public static void WaitForFullGCProc()
        //{
        //    while (true)
        //    {
        //        while (checkForNotify)
        //        {
        //            GCNotificationStatus s = GC.WaitForFullGCApproach();
        //            if (s == GCNotificationStatus.Succeeded)
        //            {
        //                Console.WriteLine("GC notif send");
        //                OnFullGCApproachNotif();
        //            }
        //            else if (s == GCNotificationStatus.Canceled)
        //            {
        //                Console.WriteLine("GC notif cancelled");
        //                break;
        //            }
        //            else
        //            {
        //                break;
        //            }
        //            GCNotificationStatus status = GC.WaitForFullGCComplete();
        //            if (status == GCNotificationStatus.Succeeded)
        //            {
        //                Console.WriteLine("GC notif raised");
        //                OnFullGCCompleteEndNotify();
        //            }
        //            else if (status == GCNotificationStatus.Canceled)
        //            {
        //                Console.WriteLine("GC notif Cnacelled");
        //                break;
        //            }
        //            else
        //                break;


        //        }
        //        Thread.Sleep(500);
        //        if (finalExit)
        //        {
        //            break;
        //        }
        //    }
        //}
        //private static void RedirectRequests()
        //{
        //    bAllocate = false;
        //}
        //private static void FinishingExistingRequests()
        //{
        //    load.Clear();
        //}
        //private static void AcceptRequests()
        //{
        //    bAllocate = true;
        //}
    }
}
