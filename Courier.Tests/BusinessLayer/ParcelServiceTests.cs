using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Courier.BusinessLayer;
using Courier.DataLayer;
using Courier.DataLayer.Models;
using Moq;
using NUnit.Framework;

namespace Courier.Tests.BusinessLayer
{
    class ParcelServiceTests
    {
        private readonly Func<IParcelsDbContext> _contextFactoryMethod = () => new ParcelInMemoryDbContext();
        private Mock<IParcelsService> _parcelServiceMock;

        [SetUp]
        public void SetUp()
        {

            using (var context = _contextFactoryMethod())
            {
                var carParcels = context.CarParcels
                    .ToList();

                context.CarParcels.RemoveRange(carParcels);

                var carParcel1 = new CarParcel
                {
                    Id = 1,
                    ParcelSize = 1,
                    CarId = 2
                };

                var carParcel2 = new CarParcel
                {
                    Id = 2,
                    ParcelSize = 2,
                    CarId = 3
                };

                context.CarParcels.Add(carParcel1);
                context.CarParcels.Add(carParcel2);
                context.SaveChanges();

                //_parcelServiceMock = new Mock<IParcelsService>();
                //_parcelServiceMock
                //    .Setup(x => x.GetPostedParcels())
                //    .Returns(n)
            }
        }

        [Test]
        public void GetAllCarParcels_CorrectList()
        {
            
            var sut = new ParcelsService(_contextFactoryMethod);

            var result = sut.GetAllCarParcel();

            Assert.AreEqual(2, result.Count);
        }

        [Test]
        public void GetCarId_CorrectCarId()
        {
            var carParcelId = 1;
            var sut = new ParcelsService(_contextFactoryMethod);

            var result = sut.GetCarParcelId(carParcelId);

            Assert.AreEqual(2, result.CarId);
        }
    }
}
