using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ProjectLocator.Areas.Identity.Accounts.Register;

namespace ProjectLocator.Areas.Identity.Accounts
{
    [Route("api/[controller]/[action]")]
    public class AccountController : Controller
    {
        private IMediator _mediator;

        public AccountController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost]
        [AllowAnonymous]
        public IActionResult Register([FromBody] RegisterCommand model)
        {
            var a = _mediator.Send(model);

            return Ok(true);
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult Test()
        {
            return Ok(true);
        }
    }
}