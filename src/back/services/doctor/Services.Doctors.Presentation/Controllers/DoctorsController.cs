using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Services.Doctors.Presentation.Controllers.Base;

namespace Services.Doctors.Presentation.Controllers;

[Authorize]
[ApiController]
[Route("auth")]
[Produces("application/json")]
internal class DoctorsController : BaseController
{
    protected DoctorsController(ISender sender) : base(sender)
    {
    }
}
