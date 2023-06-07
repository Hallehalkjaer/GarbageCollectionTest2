using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GarbageCollectionTest2
{
    class GarbageCreator
    {
        public void MakeGarbage(List<Animal> animals)
        {
            for (int i = 0; i < 2500; i++)
            {
                string name = "test" + i.ToString();
                bool tail = i % 2 == 0;
                Animal animal = new Animal(name, i, tail);
                animals.Add(animal);
            }
        }

        public void DeleteGarbage(List<Animal> animals)
        {
            List<Animal> animalsToDelete = new List<Animal>();
            foreach (Animal animal in animals)
            {
                if (animal.ID % 5 == 0)
                {
                    animalsToDelete.Add(animal);
                }
            }
            foreach (Animal animal1 in animalsToDelete)
            {
                animals.Remove(animal1);
            }
        }
        public void DeleteAll(List<Animal> animals)
        {
            for (int i = 0; i < animals.Count; i++)
            {
                animals[i] = null;
            }
            animals.Clear();
        }
    }
}
