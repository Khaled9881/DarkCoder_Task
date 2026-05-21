using CarAPI.Entities;
using CarAPI.Repositories_DAL;
using CarAPI.Services_BLL;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebApp.Controllers;

namespace CarFactoryAPI.Tests
{
    public class CarsServiceTests
    {

        [Fact]
        public void GetCarById_IdOfExistingCar_Car()
        {
            //Arrange
            Car car = new Car() { Id = 1, Price = 10000, Type = CarType.Audi, OwnerId = 1 };

            var repo = new Mock<ICarsRepository>();
            repo.Setup(r => r.GetCarById(1)).Returns(car);

            var carServ = new CarsService(repo.Object);
            //Act
            var res = carServ.GetCarById(1);

            //Assert
            Assert.Equal(car.Id, res.Id);
            Assert.Equal(car.Price, res.Price);
            Assert.Equal(car.Type, res.Type);
            Assert.Equal(car.OwnerId, res.OwnerId);
        }

        [Fact]
        public void GetCarById_IdOfNonExistingCar_null()
        {
            //Arrange
            Car car = null;

            var repo = new Mock<ICarsRepository>();
            repo.Setup(r => r.GetCarById(1)).Returns(car);

            var carServ = new CarsService(repo.Object);
            //Act
            var res = carServ.GetCarById(1);

            //assert
            Assert.Null(res);
        }



    }
}
