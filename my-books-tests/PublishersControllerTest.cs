using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using my_books.Controllers;
using my_books.Data;
using my_books.Data.Models;
using my_books.Data.Services;
using my_books.Data.ViewModels;
using my_books.Exceptions;

namespace my_books_tests
{
    public class PublishersControllerTest
    {
        private static DbContextOptions<AppDbContext> options = 
            new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(databaseName: "BookDbControllerTest")
                .Options;

        AppDbContext context;
        PublishersService publishersService;
        PublisherController publisherController;

        [OneTimeSetUp]
        public void Setup()
        {
            context = new AppDbContext(options);
            context.Database.EnsureCreated();

            SeedDatabase();

            publishersService = new PublishersService(context);

            publisherController = new PublisherController(publishersService, new NullLogger<PublisherController>());
        }

        [OneTimeTearDown]
        public void CleanUp()
        {
            context.Database.EnsureDeleted();
        }

        private void SeedDatabase()
        {
            var publishers = new List<Publisher>()
            {
                new Publisher()
                {
                    Id = 1,
                    Name = "Publisher 1"
                },
                new Publisher()
                {
                    Id = 2,
                    Name = "Publisher 2"
                },
                new Publisher()
                {
                    Id = 3,
                    Name = "Publisher 3"
                },
                new Publisher()
                {
                    Id = 4,
                    Name = "Publisher 4"
                },
                new Publisher()
                {
                    Id = 5,
                    Name = "Publisher 5"
                },
                new Publisher()
                {
                    Id = 6,
                    Name = "Publisher 6"
                },
                new Publisher()
                {
                    Id = 7,
                    Name = "Publisher 7"
                }
            };

            context.Publishers.AddRange(publishers);

            context.SaveChanges();
        }

        [Test, Order(1)]
        public void HTTPGET_GetAllPublishers_SortBy_SearchStringPageNr_ReturnOk_Test()
        {
            #region First Page
            var result = publisherController.GetAllPublishers("name_desc", "Publisher", 1);

            Assert.That(result, Is.TypeOf<OkObjectResult>());

            var resultData = (result as OkObjectResult).Value as List<Publisher>;

            Assert.That(resultData.FirstOrDefault().Name, Is.EqualTo("Publisher 7"));
            Assert.That(resultData.FirstOrDefault().Id, Is.EqualTo(7));

            Assert.That(resultData.Count, Is.EqualTo(3));
            #endregion First Page

            #region Third Page
            var resultThirdPage = publisherController.GetAllPublishers("name_desc", "Publisher", 3);

            Assert.That(resultThirdPage, Is.TypeOf<OkObjectResult>());

            var resultDataThirdPage = (resultThirdPage as OkObjectResult).Value as List<Publisher>;

            Assert.That(resultDataThirdPage.FirstOrDefault().Name, Is.EqualTo("Publisher 1"));
            Assert.That(resultDataThirdPage.FirstOrDefault().Id, Is.EqualTo(1));

            Assert.That(resultDataThirdPage.Count, Is.EqualTo(1));

            #endregion Third Page
        }

        [Test, Order(2)]
        public void HTTPGET_GetPublisherById_ReturnNotFound_Test()
        {
            var result = publisherController.GetPublisherById(0);

            Assert.That(result, Is.TypeOf<NotFoundResult>());
        }

        [Test, Order(3)]
        public void HTTPGET_GetPublisherById_ReturnOk_Test()
        {
            var result = publisherController.GetPublisherById(2);

            Assert.That(result, Is.TypeOf<OkObjectResult>());

            var resultData = (result as OkObjectResult).Value as Publisher;

            Assert.That(resultData.Name, Is.EqualTo("Publisher 2"));
            Assert.That(resultData.Id, Is.EqualTo(2));

        }

        [Test, Order(4)]
        public void HTTPPOST_AddPublisher_ReturnsCreated_Test()
        {
            var newPublisher = new PublisherVM()
            {
                Name = "New Publisher"
            };

            var result = publisherController.AddPublisher(newPublisher);

            Assert.That(result, Is.TypeOf<CreatedResult>());
            //Assert.That(result.Name, Does.StartWith("Without"));
            //Assert.That(result.Id, Is.Not.Null);
        }

        [Test, Order(5)]
        public void HTTPPOST_AddPublisher_ReturnsBadRequest_Test()
        {
            var newPublisher = new PublisherVM()
            {
                Name = "123 New Publisher"
            };

            Assert.That(() => publisherController.AddPublisher(newPublisher), Is.TypeOf<BadRequestObjectResult>());
        }

        [Test, Order(6)]
        public void HTTPDELETE_DeletePublisherById_ReturnsOk_Test()
        {
            var result = publisherController.DeletePublisherById(6);

            Assert.That(result, Is.TypeOf<OkResult>());
            //Assert.That(result.Name, Does.StartWith("Without"));
            //Assert.That(result.Id, Is.Not.Null);
        }

        [Test, Order(7)]
        public void HTTPDELETE_DeletePublisherById_ReturnsBadRequest_Test()
        {
            Assert.That(() => publisherController.DeletePublisherById(6), Is.TypeOf<BadRequestObjectResult>());
        }

    }
}