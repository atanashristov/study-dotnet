using Microsoft.AspNetCore.Mvc;
using MediatR;

namespace WebApi.Controllers
{
    [ApiController]
    public class BaseController<T> : ControllerBase
    {
        private ISender _sender;

        public ISender MediatorSender => _sender ??= HttpContext.RequestServices.GetService<ISender>();
    }
}
