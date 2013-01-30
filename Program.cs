using System;
using System.Collections.Generic;
using System.Reflection;
using System.Windows.Forms;

namespace CustomGenericCollection
{
  #region Automobile definitions
    public enum State
    { Alive, Dead }
    public enum CarParts
    { Engine, Tires }

    #region Engines
    public abstract class Engine
    {
        public State engState;
        protected int maxSpeed;
        public Engine(int max)
        {
            maxSpeed = max;
            engState = State.Alive;
        }
        public int MaxSpeed
        { get { return maxSpeed; } } 
    }

    public class SportsEngine : Engine
    {
        const int MAXSPEED = 240;
        public SportsEngine()
            : base(MAXSPEED) { }
    }
    public class MiniVanEngine : Engine
    {
        const int MAXSPEED = 180;
        public MiniVanEngine()
            : base(MAXSPEED) { }
    }
    #endregion

    public class Tires
    {
        public State tiresState;
        protected int maxMileage;
        public Tires(int max)
        {
            tiresState = State.Alive;
            maxMileage = max;
        }
        public int MaxMileage
        { get { return maxMileage; } }
    }


    public abstract class Car
    {
        public State commonState = State.Alive;
        protected string petName;
        protected int currSpeed;
        public Engine engine;
        public Tires tires;
        public event EventHandler<BrokeEventArgs> BrokeEvent;
        //public abstract void TurboBoost();
        public Car() { }
        public Car(string name)
        {
            petName = name;
        }
        public void IncSpeed()
        {
            currSpeed += 10;
            Console.WriteLine("Current speed: {0}", CurrSpeed);
            if (currSpeed > engine.MaxSpeed)
            {
                OnBrokeEvent(CarParts.Engine);
                engine.engState = State.Dead;
                commonState = State.Dead;
                Console.WriteLine("Car is broken");
            }
        }
        public void DecSpeed()
        {
            if (currSpeed > 0)
            {
                currSpeed -= 10;
                Console.WriteLine("Current speed: {0}", CurrSpeed);
            }
            else
            {
                Console.WriteLine("You do not move");
            }
        }
        public void Stop()
        {
            CurrSpeed = 0;
            Console.WriteLine("You stopped");
        }
        void OnBrokeEvent(CarParts part)
        {
            BrokeEventArgs b = new BrokeEventArgs();
            if (BrokeEvent != null)
            {
                b._brokenPart = part;
                BrokeEvent(this, b);
            }
        }
        public string PetName
        {
            get { return petName; }
            set { petName = value; }
        }
        public int CurrSpeed
        {
            get { return currSpeed; }
            set { currSpeed = value; }
        }
        //public EngineState EngineState
        //{ get { return egnState; } }
    }


    public class SportsCar : Car
    {
        public SportsCar() { }
        public SportsCar(string name) : base(name) {
            engine = new SportsEngine();
            tires = new Tires(200);
        }
    }
    public class MiniVan : Car
    {
        public MiniVan() { }
        public MiniVan (string name) : base(name) {
            engine = new MiniVanEngine();
            tires = new Tires(150);
        }
    }

    public class BrokeEventArgs : EventArgs
    {
        public CarParts _brokenPart;
    }
    public class Driver
    {
        Car _myCar;
        public Driver(Car myCar)
        {
            _myCar = myCar;
            //_speedLimit = myCar.engine.MaxSpeed;
            _myCar.BrokeEvent += new EventHandler<BrokeEventArgs>(BrokeHandler);
        }
        public void BrokeHandler(object source, BrokeEventArgs args) { }

        public void Control()
        {
            if (_myCar.commonState == State.Alive)
            {
                ConsoleKeyInfo key;
                do
                {
                    key = Console.ReadKey();
                    switch (key.Key)
                    {
                        case ConsoleKey.UpArrow:
                            _myCar.IncSpeed();
                            break;
                        case ConsoleKey.DownArrow:
                            _myCar.DecSpeed();
                            break;
                        case ConsoleKey.S:
                            _myCar.Stop();
                            Console.WriteLine("Exit");
                            break;
                        default:
                            Console.WriteLine("Wrong key");
                            break;
                    }
                } while (key.Key != ConsoleKey.S && _myCar.commonState == State.Alive);
            }
            else
            {
                Console.WriteLine("Your car needs a diagnostics");
            }
        }
        
    }
    public class Diagnostics
    {
        Car _car;
        public Diagnostics(Car car)
        {
            _car = car;
        }
        public void Diagnose()
        {
            if (_car.commonState == State.Alive)
            {
                Console.WriteLine("Your car is good");
                return;
            }
            ConsoleKeyInfo key;
            if (_car.engine.engState == State.Dead)
            {
                Console.WriteLine("Engine is dead \nDo you want to repair? \nType 'y' or 'n':");
                key = Console.ReadKey();
                switch (key.Key)
                {
                    case ConsoleKey.Y:
                        Type t = _car.engine.GetType();
                        ConstructorInfo[] ci = t.GetConstructors();
                        int x;
                        for (x = 0; x < ci.Length; x++)
                        {
                            ParameterInfo[] pi = ci[x].GetParameters();
                            if (pi.Length == 0) break;
                        }
                        _car.engine = (Engine)ci[x].Invoke(null);
                        Console.WriteLine("You have a new engine");
                        break;
                    case ConsoleKey.N:
                        break;
                }
            }
            if (_car.tires.tiresState == State.Dead)
            {
                Console.WriteLine("Tires is dead \nDo you want to repair? \nType 'y' or 'n':");
                key = Console.ReadKey();
                switch (key.Key)
                {
                    case ConsoleKey.Y:
                        Type t = _car.tires.GetType();
                        ConstructorInfo[] ci = t.GetConstructors();
                        int x;
                        for (x = 0; x < ci.Length; x++)
                        {
                            ParameterInfo[] pi = ci[x].GetParameters();
                            if (pi.Length == 0) break;
                        }
                        _car.tires = (Tires)ci[x].Invoke(null);
                        Console.WriteLine("You have a new engine");
                        break;
                    case ConsoleKey.N:
                        break;
                }
            }
        }
    }

  #endregion

  #region Car Collection
  public class CarCollection<T> : IEnumerable<T> where T : Car
  {
    private List<T> arCars = new List<T>();

    public T GetCar(int pos)
    { return arCars[pos]; }

    public void AddCar(T c)
    { arCars.Add(c); }

    public void ClearCars()
    { arCars.Clear(); }

    public int Count
    { get { return arCars.Count; } }

    // IEnumerable<T> extends IEnumerable, therefore
    // we need to implement both versions of GetEnumerator().
    IEnumerator<T> IEnumerable<T>.GetEnumerator()
    { return arCars.GetEnumerator(); }
    System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
    { return arCars.GetEnumerator(); }

    // This function will only work because
    // of our applied constraint. 
    public void PrintPetName(int pos)
    {
      Console.WriteLine(arCars[pos].PetName);
    }
  }
  #endregion
  
  class Program
  {
    static void Main(string[] args)
    {
      Console.WriteLine("***** Custom Generic Collection *****\n");


      // CarCollection<Car> can hold any type deriving from Car.
      CarCollection<Car> myAutos = new CarCollection<Car>();
      myAutos.AddCar(new MiniVan("Family Truckster"));
      myAutos.AddCar(new SportsCar("Crusher"));
      int i = 1;
      foreach (Car c in myAutos)
      {
          Console.WriteLine("{0}. Type: {1}, PetName: {2}, MaxSpeed: {3}, MaxMileage: {4}",
            i, c.GetType().Name, c.PetName, c.engine.MaxSpeed, c.tires.MaxMileage);
          i++;
      }
      Console.Write("Your choose: ");
      i = int.Parse(Console.ReadLine());
      Car car = myAutos.GetCar(--i);
      Driver driver = new Driver(car);
      Console.WriteLine("Let's go!");
      driver.Control();
      Console.WriteLine("Let's diagnosis!");
      Diagnostics diagnostics = new Diagnostics(car);
      diagnostics.Diagnose();
      Console.ReadLine();
    }
  }
}
