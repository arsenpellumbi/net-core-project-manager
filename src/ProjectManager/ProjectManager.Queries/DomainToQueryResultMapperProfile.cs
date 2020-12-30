using ProjectManager.Queries.ProjectManagement;
using AutoMapper;
using System.IO;
using System.Linq;
using ProjectManager.Core.Domain;
using ProjectManager.Core.SeedWork.Interfaces;

namespace ProjectManager.Queries
{
    public class DomainToQueryResultMapperProfile : Profile, IMapperProfile
    {
        public int Order => 1;

        public DomainToQueryResultMapperProfile()
        {
            CreateMap<Project, GetProjectByIdResult>();
            CreateMap<Project, GetProjectsItemResult>();
            CreateMap<Project, SearchProjectsItemResult>();

            CreateMap<ProjectTask, GetTaskByIdResult>();
            CreateMap<ProjectTask, GetTasksByProjectIdItemResult>();
            CreateMap<ProjectTask, SearchTasksInProjectItemResult>();
        }
    }
}