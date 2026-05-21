using CarAPI.Entities;
using CarAPI.Models;
using CarAPI.Payment;
using CarAPI.Repositories_DAL;
using CarAPI.Services_BLL;
using Microsoft.AspNetCore.Builder;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CarFactoryAPI.Tests
{
    public class OwnersServiceTests
    {

        Mock<ICarsRepository> CarRepo;
        Mock<IOwnersRepository> ownersRepository;
        ICashService cashService;
        OwnersService ownersService;
        public OwnersServiceTests()
        {
            CarRepo = new Mock<ICarsRepository>();

            ownersRepository = new Mock<IOwnersRepository>();

            cashService = new CashService();

            ownersService = new OwnersService(CarRepo.Object, ownersRepository.Object, cashService);

        }



        [Fact]
        public void BuyCar_CarsIsNull_CarDoesnotExist()
        {
            //Arrange

            BuyCarInput buyCarInput = new BuyCarInput()
            {
                CarId = 22,
                OwnerId = 1,
                Amount = 1,
            };

            CarRepo.Setup(r => r.GetCarById(buyCarInput.CarId)).Returns((Car)null);

            //Act
            var res = ownersService.BuyCar(buyCarInput);

            //Assert
            Assert.Equal(res, "Car doesn't exist");
        }

        [Fact]
        public void BuyCar_CarHasOwner_ReturnsCarSold()
        {
            //Arrange

            BuyCarInput buyCarInput = new BuyCarInput()
            {
                CarId = 1,
                OwnerId = 1,
                Amount = 1,
            };

            Car car = new Car() { Id = 1, Price = 500000, Type = CarType.Audi };

            car.Owner = new Owner();
            car.Owner.Id = 1;

            CarRepo.Setup(r => r.GetCarById(buyCarInput.CarId)).Returns(car);

            //Act
            var res = ownersService.BuyCar(buyCarInput);

            //Assert
            Assert.Equal(res, "Already sold");
        }

        [Fact]
        public void BuyCar_CarWithNoOwnerandOwnerNotExist_ReturnsOwnerNotExist()
        {
            //Arrange

            BuyCarInput buyCarInput = new BuyCarInput()
            {
                CarId = 1,
                OwnerId = 22,
                Amount = 1,
            };

            Car car = new Car() { Id = 1, Price = 500000, Type = CarType.Audi };

            car.Owner = null;

            CarRepo.Setup(r => r.GetCarById(buyCarInput.CarId)).Returns(car);
            ownersRepository.Setup(s => s.GetOwnerById(buyCarInput.OwnerId)).Returns((Owner)null);

            //Act
            var res = ownersService.BuyCar(buyCarInput);

            //Assert
            Assert.Equal(res, "Owner doesn't exist");
        }


        [Fact]
        //public void BuyCar_CarWithnoOwnerAndOwnerWithNoCar_ReturnsOwnerNotExist()
        public void BuyCar_OwnerWithCar_ReturnsOwnerHasCar()
        {
            //Arrange

            BuyCarInput buyCarInput = new BuyCarInput()
            {
                CarId = 1,
                OwnerId = 22,
                Amount = 1,
            };

            Car car = new Car() { Id = 1, Price = 500000, Type = CarType.Audi };

            car.Owner = null;

            Owner owner = new Owner();
            owner.Car = new Car() { Id = 8, Price = 500000, Type = CarType.Audi };

            CarRepo.Setup(r => r.GetCarById(buyCarInput.CarId)).Returns(car);
            ownersRepository.Setup(s => s.GetOwnerById(buyCarInput.OwnerId)).Returns(owner);

            //Act
            var res = ownersService.BuyCar(buyCarInput);

            //Assert
            Assert.Equal(res, "Already have car");
        }

    }
}
