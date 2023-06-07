using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GarbageCollectionTest2
{
    class Animal
    {
        
            private static int internalid;
            private int id;
            public string Name { get; set; }
            public int Legs { get; set; }
            public bool Tail { get; set; }
            public int ID
            {
                get { return id; }
                set { id = value; }
            }


            public Animal(string name, int legs, bool tail)
            {
                Name = name;
                Legs = legs;
                Tail = tail;
                ID = internalid;
                internalid++;
            }

        
    }
}
