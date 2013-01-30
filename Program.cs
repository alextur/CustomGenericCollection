using System;
using System.Collections.Generic;
using System.Text;
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
        //public abstract void TurboBoost();
        public Car() { }
        public Car(string name)
        {
            petName = name;
        }
        public abstract void BrokeHandler(object source, BrokeEventArgs args);
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
        public override void BrokeHandler(object source, BrokeEventArgs args)
        {
            switch (args._brokenPart)
            {
                case CarParts.Engine:
                    engine.engState = State.Dead;
                    break;
                case CarParts.Tires:
                    tires.tiresState = State.Dead;
                    break;
            }
            commonState = State.Dead;
        }
        //public void TurboBoost()
        //{
        //    MessageBox.Show("Ramming speed!", "Faster is better...");
        //}
    }
    public class MiniVan : Car
    {
        public MiniVan() { }
        public MiniVan (string name) : base(name) {
            engine = new MiniVanEngine();
            tires = new Tires(150);
        }
        public override void BrokeHandler(object source, BrokeEventArgs args)
        {
            switch (args._brokenPart)
            {
                case CarParts.Engine:
                    engine.engState = State.Dead;
                    break;
                case CarParts.Tires:
                    tires.tiresState = State.Dead;
                    break;
            }
            commonState = State.Dead;
        }
    //    public override void TurboBoost()
    //    {
    //        // У минивэнов возможности ускорения всегда плохие! 
    //        egnState = EngineState.engineDead;
    //        MessageBox.Show("Time to call AAA", "Your car is dead");
    //    }
    }

    public class BrokeEventArgs : EventArgs
    {
        public CarParts _brokenPart;
    }
    public class Driver
    {
        Car _myCar;
        public event EventHandler<BrokeEventArgs> BrokeEvent;
        int _speedLimit;
        int _currSpeed = 0;
        public Driver(Car myCar)
        {
            _myCar = myCar;
            _speedLimit = myCar.engine.MaxSpeed;
            BrokeEvent += new EventHandler<BrokeEventArgs>(_myCar.BrokeHandler);
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
        public void Control()
        {

        }
        
    }
    public class Diagnostics
    {
        Car _car;
        public Diagnostics(Car car)
        {
            _car = car;
        }

    }

  #endregion

  #region Custom Generic Collection
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
      Console.ReadLine();
    }
  }
}
