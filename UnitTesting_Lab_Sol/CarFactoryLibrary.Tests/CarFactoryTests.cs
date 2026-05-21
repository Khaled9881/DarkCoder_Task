using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CarFactoryLibrary.Tests
{
    public class CarFactoryTests
    {
        [Fact]
        public void NewCar_ManufactureHonda_Exception()
        {
            // Arrange
            CarTypes carTypes = CarTypes.Honda;

            // Act
            // Assert

            Assert.Throws<NotImplementedException>(() => CarFactory.NewCar(carTypes));
        }


        //[Theory]
        //[InlineData(CarTypes.BMW, typeof(BMW))]
        //[InlineData(CarTypes.Toyota, typeof(Toyota))]
        //[InlineData(CarTypes.Audi, null)]
        //public void NewCar_ManufactureCar<T>(CarTypes carTypes, T car)
        //{
        //    // Arrange
        //    //CarTypes carTypes = CarTypes.Honda;

        //    // Act
        //    var res = CarFactory.NewCar(carTypes);

        //    // Assert

        //    Assert.Same(car, res);

        //}




    }
}
