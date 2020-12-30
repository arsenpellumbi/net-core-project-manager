using MediatR;
using Microsoft.AspNetCore.Mvc;
using System;

namespace ProjectManager.API.Controllers
{
    /// <summary>
    /// Api Controller Base
    /// </summary>
    public class ApiControllerBase : ControllerBase
    {
        /// <summary>
        /// return status code 200 and no content
        /// </summary>
        /// <returns></returns>
        [ProducesResponseType(typeof(Guid), 201)]
        [ProducesResponseType(400)]
        [ProducesResponseType(401)]
        [ProducesResponseType(403)]
        [ProducesResponseType(404)]
        protected IActionResult Created(Guid id)
        {
            if (Uri.TryCreate($"{Request.Host}{Request.Path}/{id}", UriKind.RelativeOrAbsolute, out var uri))
            {
                return base.Created(uri, id);
            }
            return base.Created("", id);
        }

        /// <summary>
        /// return status code 200 and no content
        /// </summary>
        /// <returns></returns>
        [ProducesResponseType(typeof(Unit), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(401)]
        [ProducesResponseType(403)]
        [ProducesResponseType(404)]
        protected IActionResult Updated()
        {
            return Ok();
        }

        /// <summary>
        /// return status code 204 (NoContent)
        /// </summary>
        /// <returns></returns>
        [ProducesResponseType(typeof(Unit), 204)]
        [ProducesResponseType(400)]
        [ProducesResponseType(401)]
        [ProducesResponseType(403)]
        [ProducesResponseType(404)]
        protected IActionResult Deleted()
        {
            return NoContent();
        }
    }
}