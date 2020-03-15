using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using PersonFinder.Models;

namespace PersonFinder.Controllers
{
	[ApiController]
	[Route("[controller]")]
	public class PeopleController : ControllerBase
	{
		private readonly ILogger<PeopleController> _logger;

		public PeopleController(ILogger<PeopleController> logger)
		{
			_logger = logger;
		}

		[HttpGet]
		public IEnumerable<Person> Get()
		{
			return Enumerable.Empty<Person>();
		}
	}
}
