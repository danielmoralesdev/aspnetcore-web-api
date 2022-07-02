using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using my_books.ActionResults;
using my_books.Data.Models;
using my_books.Data.Services;
using my_books.Data.ViewModels;
using my_books.Exceptions;

namespace my_books.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PublisherController : ControllerBase
    {
        private PublishersService _publishersService;
        private readonly ILogger<PublisherController> _logger;

        public PublisherController(PublishersService publishersService, ILogger<PublisherController> logger)
        {
            _publishersService = publishersService;
            _logger = logger;
        }

        [HttpGet("get-all-publishers")]
        public IActionResult GetAllPublishers(string? sortBy, string? searchString, int pageNumber)
        {
            //throw new Exception("This is an exception thrown from GetAllPublishers");

            try
            {
                _logger.LogInformation("This is just a log in GetAllPublishers");

                var _response = _publishersService.GetAllPublishers(sortBy, searchString, pageNumber);

                if (_response == null)
                {
                    return NotFound();
                }
                else
                {
                    return Ok(_response);
                }
            }
            catch (Exception ex)
            {
                return BadRequest("Sorry, we could not load the publishers");
            }
        }

        [HttpPost("add-publisher")]
        public IActionResult AddPublisher([FromBody] PublisherVM publisher)
        {
            try
            {
                var newPub = _publishersService.AddPublisher(publisher);

                return Created(nameof(AddPublisher), newPub);
            }
            catch (PublisherNameException ex)
            {
                return BadRequest($"{ex.Message}, Publisher name: {ex.PublisherName}");
            }
            catch (Exception ex)
            {

                return BadRequest(ex);
            }
        }

        [HttpGet("get-publisher-by-id/{id}")]
        public IActionResult GetPublisherById(int id)
        {
            //throw new Exception("This is an exception that will be handled by middleware");

            var _response = _publishersService.GetPublisherById(id);

            if (_response == null)
            {
                return NotFound();
                //return null;

                //var _responseObj = new CustomActionResultVM()
                //{
                //    Exception = new Exception("This coming from publisher controller.")
                //};

                //return new CustomActionResult(_responseObj);
            }
            else
            {
                return Ok(_response);
                //return _response;
                //var _responseObj = new CustomActionResultVM()
                //{
                //    Publisher = _response
                //};

                //return new CustomActionResult(_responseObj);
            }
        }

        [HttpGet("get-publisher-books-with-authors/{id}")]
        public IActionResult GetPublisherData(int id)
        {
            var _response = _publishersService.GetPublisherData(id);

            return Ok(_response);
        }

        [HttpDelete("delete-publisher-by-id/{id}")]
        public IActionResult DeletePublisherById(int id)
        {
            try
            {
                _publishersService.DeletePublisherById(id);

                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
