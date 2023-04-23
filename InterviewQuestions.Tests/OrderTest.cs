using System;
using Moq;
using NUnit.Framework;

namespace InterviewQuestions.Tests
{
    [TestFixture]
    public class OrderTests
    {
        private Mock<IOrderService> orderServiceMock;
        private Order order;

        [SetUp]
        public void SetUp()
        {
            orderServiceMock = new Mock<IOrderService>();
            order = new Order(orderServiceMock.Object, 100);
        }

        [Test]
        public void RespondToTick_PriceLessThanThreshold_BuysAndRaisesPlacedEvent()
        {
            // Arrange
            var code = "KMSLTD";
            var price = 99;

            // Act
            order.RespondToTick(code, price);

            // Assert
            orderServiceMock.Verify(x => x.Buy(code, 1, price), Times.Once);
            Assert.That(() => order.Placed += Order_Placed, Throws.Nothing);
        }

        private void Order_Placed(PlacedEventArgs e)
        {
            throw new NotImplementedException();
        }

        [Test]
        public void RespondToTick_PriceEqualToThreshold_DoesNothing()
        {
            // Arrange
            var code = "KMSLTD";
            var price = 100;

            // Act
            order.RespondToTick(code, price);

            // Assert
            orderServiceMock.Verify(x => x.Buy(It.IsAny<string>(), It.IsAny<int>(), It.IsAny<decimal>()), Times.Never);
            Assert.That(() => order.Placed += Order_Placed, Throws.Nothing);
            //Assert.That(() => order.Errored += Order_Errored, Throws.Nothing);
        }

        private void Order_Errored(ErroredEventArgs e)
        {
            throw new NotImplementedException();
        }

        [Test]
        public void RespondToTick_PriceGreaterThanThreshold_DoesNothing()
        {
            // Arrange
            var code = "KMSLTD";
            var price = 101;

            // Act
            order.RespondToTick(code, price);

            // Assert
            orderServiceMock.Verify(x => x.Buy(It.IsAny<string>(), It.IsAny<int>(), It.IsAny<decimal>()), Times.Never);
            Assert.That(() => order.Placed += Order_Placed, Throws.Nothing);
            Assert.That(() => order.Errored += Order_Errored, Throws.Nothing);
        }

        private void Order_Errored1(ErroredEventArgs e)
        {
            throw new NotImplementedException();
        }

        private void Order_Placed1(PlacedEventArgs e)
        {
            throw new NotImplementedException();
        }

        [Test]
        public void RespondToTick_BuyThrowsException_RaisesErroredEvent()
        {
            // Arrange
            var code = "KMSLTD";
            var price = 99;
            var exception = new Exception("Buy failed");
            orderServiceMock.Setup(x => x.Buy(code, 1, price)).Throws(exception);

            // Act
            order.RespondToTick(code, price);

            // Assert
            orderServiceMock.Verify(x => x.Buy(code, 1, price), Times.Once);
            Assert.That(() => order.Errored += Order_Errored, Throws.Nothing);
            Assert.That(() => order.Placed += Order_Placed, Throws.Nothing);
        }

        [Test]
        public void RespondToTick_OrderAlreadyPlaced_DoesNothing()
        {
            // Arrange
            var code = "KMSLTD";
            var price1 = 99;
            var price2 = 98;
            order.RespondToTick(code, price1);

            // Act
            order.RespondToTick(code, price2);

            // Assert
            orderServiceMock.Verify(x => x.Buy(code, 1, price2), Times.Never);
            Assert.That(() => order.Placed += Order_Placed, Throws.Nothing);
        }

        [Test]
        public void RespondToTick_ErrorAlreadyOccurred_DoesNothing()
        {
            // Arrange
            var code = "KMSLTD";
            var price = 99;
            var exception = new Exception("Buy failed");
            orderServiceMock.Setup(x => x.Buy(code, 1, price)).Throws(exception);
            order.RespondToTick(code, price);

            // Act
            order.RespondToTick(code, price);

            // Assert
            orderServiceMock.Verify(x => x.Buy(code, 1, price), Times.Once);
            Assert.That(() => order.Errored += Order_Errored, Throws.Nothing);
            Assert.That(() => order.Placed += Order_Placed, Throws.Nothing);
        }
    }
}
