using ProjectManager.Commands.ProjectManagement;
using ProjectManager.Queries.ProjectManagement;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;

namespace ProjectManager.API.Controllers
{
    /// <summary>
    /// Projects controller
    /// </summary>
    [Route("api/v1/projects")]
    [AllowAnonymous]
    [ApiController]
    public class ProjectsController : ApiControllerBase
    {
        private readonly IMediator _mediator;

        /// <summary>
        /// Projects controller ctor
        /// </summary>
        /// <param name="mediator"></param>
        public ProjectsController(IMediator mediator)
        {
            _mediator = mediator;
        }

        /// <summary>
        /// Create data set
        /// </summary>
        /// <param name="command">Command</param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> CreateProject(CreateProject command)
        {
            var result = await _mediator.Send(command);

            return Created(result);
        }

        /// <summary>
        /// Update data set
        /// </summary>
        /// <param name="command">Command</param>
        /// <returns></returns>
        [HttpPut]
        public async Task<IActionResult> UpdateProject(UpdateProject command)
        {
            await _mediator.Send(command);

            return Updated();
        }

        /// <summary>
        /// Delete data set
        /// </summary>
        /// <param name="command">Command</param>
        /// <returns></returns>
        [Route("{id:Guid}")]
        [HttpDelete]
        public async Task<IActionResult> DeleteProject([FromRoute] DeleteProject command)
        {
            await _mediator.Send(command);

            return Deleted();
        }

        /// <summary>
        /// Get data set by id
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        [Route("{id:Guid}")]
        [HttpGet]
        [ProducesResponseType(typeof(GetProjectByIdResult), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        public async Task<IActionResult> GetProjectById([FromRoute] GetProjectById query)
        {
            var result = await _mediator.Send(query);

            return Ok(result);
        }

        /// <summary>
        /// Get data sets
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<GetProjectsItemResult>), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        public async Task<IActionResult> GetProjects([FromQuery] GetProjects query)
        {
            var result = await _mediator.Send(query);

            return Ok(result);
        }

        /// <summary>
        /// Search data sets
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        [Route("search")]
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<SearchProjectsItemResult>), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        public async Task<IActionResult> SearchProjects([FromQuery] SearchProjects query)
        {
            var result = await _mediator.Send(query);

            return Ok(result);
        }

        /// <summary>
        /// Create tasks
        /// </summary>
        /// <param name="command"></param>
        /// <returns></returns>
        [Route("tasks")]
        [HttpPost]
        public async Task<IActionResult> CreateTask(CreateTask command)
        {
            var result = await _mediator.Send(command);

            return Created(result);
        }

        /// <summary>
        /// Update tasks
        /// </summary>
        /// <param name="command"></param>
        /// <returns></returns>
        [Route("tasks")]
        [HttpPut]
        public async Task<IActionResult> UpdateTask(UpdateTask command)
        {
            await _mediator.Send(command);

            return Updated();
        }

        /// <summary>
        /// Delete tasks
        /// </summary>
        /// <param name="command"></param>
        /// <returns></returns>
        [Route("tasks/{id:Guid}")]
        [HttpDelete]
        public async Task<IActionResult> DeleteTask([FromRoute] DeleteTask command)
        {
            await _mediator.Send(command);

            return Deleted();
        }

        /// <summary>
        /// Get tasks by id
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        [Route("tasks/{id:Guid}")]
        [HttpGet]
        [ProducesResponseType(typeof(GetTaskByIdResult), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        public async Task<IActionResult> GetTaskById([FromRoute] GetTaskById query)
        {
            var result = await _mediator.Send(query);

            return Ok(result);
        }

        /// <summary>
        /// Get tasks by data set id
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        [Route("tasks")]
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<GetTasksByProjectIdItemResult>), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        public async Task<IActionResult> GetTasksByProjectId([FromQuery] GetTasksByProjectId query)
        {
            var result = await _mediator.Send(query);

            return Ok(result);
        }

        /// <summary>
        /// Get tasks by data set id
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        [Route("tasks/search")]
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<SearchTasksInProjectItemResult>), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        public async Task<IActionResult> SearchTasksInProject([FromQuery] SearchTasksInProject query)
        {
            var result = await _mediator.Send(query);

            return Ok(result);
        }
    }
}