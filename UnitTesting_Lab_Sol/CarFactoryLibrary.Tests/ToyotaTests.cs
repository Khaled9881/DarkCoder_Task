using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CarFactoryLibrary.Tests
{
    public class ToyotaTests
    {
        [Fact]
        public void IsStopped_Velocity0_true()
        {
            // Arrange
            var toyota = new Toyota();
            // Act
            Boolean res = toyota.IsStopped();

            // Assert
            Assert.True(res);
        }

        [Fact]
        public void IsStopped_VelocityNotZero_false()
        {
            // Arrange
            var toyota = new Toyota();
            toyota.velocity = 5;
            // Act
            Boolean res = toyota.IsStopped();

            // Assert
            Assert.False(res);
        }

        [Fact]
        public void Stop_CarStopped_Velocity0andDrivingModeIsStopped()
        {
            // Arrange
            Toyota toyota = new Toyota();

            // Act
            toyota.Stop();

            // Assert
            Assert.True(toyota.velocity == 0);
            Assert.True(toyota.drivingMode == DrivingMode.Stopped);

        }

        [Fact]
        public void IncreaseVelocity_VelocityIncreaseBy6_VelocityIncreased()
        {

            // Arrange
            Toyota toyota = new Toyota();

            // Act
            toyota.IncreaseVelocity(6);

            // Assert
            Assert.Equal(6, toyota.velocity);
        }

        [Fact]
        public void Accelerate_whenAccelerated_VelocityIncreasedBy5()
        {

            // Arrange
            Toyota toyota = new Toyota();

            // Act
            toyota.Accelerate();

            // Assert
            Assert.Equal(5, toyota.velocity);
        }


        [Fact]
        public void GetDirection_WhenBackwardDirectionNeeded_BackwardDirectionReturned()
        {

            // Arrange
            Toyota toyota = new Toyota();
            toyota.drivingMode = DrivingMode.Backward;

            // Act
            string direction = toyota.GetDirection();

            // Assert
            Assert.Equal(DrivingMode.Backward.ToString(), direction);
        }

        [Fact]
        public void GetDirection_WhenStoppedDirectionNeeded_StoppedDirectionReturned()
        {

            // Arrange
            Toyota toyota = new Toyota();
            toyota.drivingMode = DrivingMode.Stopped;

            // Act
            string direction = toyota.GetDirection();

            // Assert
            Assert.Equal(DrivingMode.Stopped.ToString(), direction);
        }

        [Fact]
        public void GetDirection_WhenForwardDirectionNeeded_ForwardDirectionReturned()
        {

            // Arrange
            Toyota toyota = new Toyota();
            toyota.drivingMode = DrivingMode.Forward;

            // Act
            string direction = toyota.GetDirection();

            // Assert
            Assert.Equal(DrivingMode.Forward.ToString(), direction);
        }


        //[Theory]
        //[InlineData(DrivingMode.Stopped)]
        //[InlineData(DrivingMode.Backward)]
        //[InlineData(DrivingMode.Forward)]
        //public void GetDirection_DirectionNeeded_DirectionReturned(DrivingMode drivingMode)
        //{

        //    // Arrange
        //    Toyota toyota = new Toyota();
        //    toyota.drivingMode = drivingMode;

        //    // Act
        //    string direction = toyota.GetDirection();

        //    // Assert
        //    Assert.Equal(drivingMode.ToString(), direction);
        //}


        [Theory]
        [InlineData(DrivingMode.Stopped, "Stopped")]
        [InlineData(DrivingMode.Backward, "Backward")]
        [InlineData(DrivingMode.Forward, "Forward")]
        public void GetDirection_ReturnsCorrectDirection(DrivingMode drivingMode, string expected)
        {
            // Arrange
            Toyota toyota = new Toyota();
            toyota.drivingMode = drivingMode;

            // Act
            string direction = toyota.GetDirection();

            // Assert
            Assert.Equal(expected, direction);
        }


        [Fact]
        public void GetMyCar_RetuenSameCarInstance()
        {
            // Arrange
            Toyota toyota = new Toyota();

            // Act
            Toyota? t2 = toyota.GetMyCar() as Toyota;

            // Assert
            Assert.Same(t2, toyota);

        }

        //public double TimeToCoverDistance(double distance)
        //{
        //    return distance / velocity;
        //}

        [Fact]
        public void TimeToCoverDistance_Velocity0_ThrowExcption()
        {
            // Arrange
            Toyota toyota = new Toyota();
            toyota.velocity = 0;

            // Assert
            Assert.Throws<InvalidOperationException>(() => toyota.TimeToCoverDistance(10));

        }

        [Fact]
        public void TimeToCoverDistance_Velocity10andDsitance20_Time2()
        {
            // Arrange
            Toyota toyota = new Toyota();
            toyota.velocity = 10;

            //Act
            var expeted = toyota.TimeToCoverDistance(20);

            // Assert
            Assert.Equal(2, expeted);

        }

        [Fact]
        public void TimeToCoverDistance_Velocity10andDsitance0_Time2()
        {
            // Arrange
            Toyota toyota = new Toyota();
            toyota.velocity = 10;

            //Act
            var expeted = toyota.TimeToCoverDistance(0);

            // Assert
            Assert.Equal(0, expeted);

        }


    }
}

// Arrange

// Act

// Assert
