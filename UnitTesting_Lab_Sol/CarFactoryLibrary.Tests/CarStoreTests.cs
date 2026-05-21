using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CarFactoryLibrary.Tests
{
    public class CarStoreTests
    {

        [Fact]
        public void AddCar_newCarAdded_CarsListCountIncreased()
        {

            // Arrange
            CarStore carStore = new CarStore();
            Toyota car = new Toyota()
            {
                velocity = 1,
                drivingMode = DrivingMode.Backward
            };

            // Act
            carStore.AddCar(car);

            // Assert
            //Assert.Equal(1, carStore.cars?.Count());
            Assert.Contains(car, carStore.cars);

        }

        //[Fact]
        //public void AddCar_NullCarAdded_Excption()
        //{

        //    // Arrange
        //    CarStore carStore = new CarStore();

        //    // Act
        //    carStore.AddCar(null);

        //    // Assert
        //    Assert.True(carStore.cars.Count > 0);

        //}

        [Fact]
        public void AddCars_WhenMultipleCarsAdded_AllCarsAreStored()
        {
            // Arrange
            CarStore carStore = new CarStore();

            Toyota car1 = new Toyota { velocity = 1, drivingMode = DrivingMode.Backward };
            Toyota car2 = new Toyota { velocity = 2, drivingMode = DrivingMode.Forward };
            Toyota car3 = new Toyota { velocity = 3, drivingMode = DrivingMode.Stopped };

            List<Car> cars = new List<Car> { car1, car2, car3 };

            // Act
            carStore.AddCars(cars);

            // Assert
            Assert.Equal(3, carStore.cars.Count);
            Assert.Contains(car1, carStore.cars);
            Assert.Contains(car2, carStore.cars);
            Assert.Contains(car3, carStore.cars);
        }


    }
}


// Arrange

// Act

// Assert